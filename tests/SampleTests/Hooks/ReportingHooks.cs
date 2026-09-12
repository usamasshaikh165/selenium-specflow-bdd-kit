using System;
using System.IO;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;
using OpenQA.Selenium;
using SampleTests.Support;
using TechTalk.SpecFlow;

namespace SampleTests.Hooks
{
    /// <summary>
    /// Builds an Extent HTML report: one node per feature, one child per scenario,
    /// one grandchild per step with a screenshot. Output lands in
    /// bin/.../Reports/ExtentReport.html. Set OPEN_REPORT=1 to launch it after the run.
    /// </summary>
    [Binding]
    public sealed class ReportingHooks
    {
        private static readonly object Sync = new object();
        private static ExtentReports _extent;
        private static bool _flushed;

        private static readonly string ReportDir = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
        private static readonly string ReportPath = Path.Combine(ReportDir, "ExtentReport.html");
        private static readonly string ScreenshotDir = Path.Combine(ReportDir, "Screenshots");

        [ThreadStatic] private static ExtentTest _featureNode;
        private ExtentTest _scenarioNode;

        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;
        private readonly Browser _browser;

        public ReportingHooks(ScenarioContext scenarioContext, FeatureContext featureContext, Browser browser)
        {
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
            _browser = browser;
        }

        [BeforeTestRun(Order = 0)]
        public static void CreateReport()
        {
            Directory.CreateDirectory(ReportDir);
            Directory.CreateDirectory(ScreenshotDir);

            var html = new ExtentHtmlReporter(ReportPath);
            html.Config.Theme = Theme.Standard;
            html.Config.DocumentTitle = "Test Execution Report";
            html.Config.ReportName = WebControls.Global.APPLICATION_NAME;

            _extent = new ExtentReports();
            _extent.AttachReporter(html);
        }

        [BeforeScenario(Order = 0)]
        public void CreateScenarioNode()
        {
            lock (Sync)
            {
                var featureTitle = _featureContext?.FeatureInfo?.Title ?? "UnknownFeature";
                if (_featureNode == null || _featureNode.Model.Name != featureTitle)
                {
                    _featureNode = _extent.CreateTest<Feature>(featureTitle);
                }
                _scenarioNode = _featureNode.CreateNode<Scenario>(_scenarioContext.ScenarioInfo.Title);
            }
        }

        [AfterStep]
        public void LogStep()
        {
            if (_scenarioNode == null) return;

            var step = _scenarioContext.StepContext.StepInfo;
            var node = _scenarioNode.CreateNode(new GherkinKeyword(step.StepDefinitionType.ToString()), step.Text);

            if (_scenarioContext.TestError == null)
            {
                node.Pass("Passed");
            }
            else
            {
                node.Fail(_scenarioContext.TestError.Message);
            }

            var screenshot = TakeScreenshot(step.Text);
            if (screenshot != null)
            {
                node.AddScreenCaptureFromPath(screenshot);
            }
        }

        [AfterTestRun]
        public static void FlushReport()
        {
            lock (Sync)
            {
                if (_extent == null || _flushed) return;
                _extent.Flush();
                _flushed = true;
                Console.WriteLine("Extent report: " + ReportPath);

                if (TestSettings.OpenReport && File.Exists(ReportPath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = ReportPath,
                        UseShellExecute = true,
                    });
                }
            }
        }

        private string TakeScreenshot(string stepText)
        {
            try
            {
                if (!(_browser.Driver is ITakesScreenshot camera)) return null;
                var name = $"{DateTime.Now:yyyyMMdd_HHmmss_fff}_{Sanitize(_scenarioContext.ScenarioInfo.Title)}_{Sanitize(stepText)}.png";
                var path = Path.Combine(ScreenshotDir, name);
                camera.GetScreenshot().SaveAsFile(path);
                return path;
            }
            catch
            {
                return null;
            }
        }

        private static string Sanitize(string input)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                input = input.Replace(c, '_');
            }
            return input.Length > 60 ? input.Substring(0, 60) : input;
        }
    }
}
