using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebControls;
using WebControls.Diagnostics;
using LogLevel = WebControls.Diagnostics.LogLevel;

namespace SampleTests.Support
{
    /// <summary>
    /// Browser lifecycle and locator-driven waits for one scenario.
    ///
    /// One instance per scenario is created by SpecFlow's context injection, so
    /// scenarios can run in parallel without sharing a driver. Element lookups go
    /// through the WebControls factory using the token names in Locators.json.
    /// </summary>
    public sealed class Browser : IDisposable
    {
        private IWebDriver _driver;

        public IWebDriver Driver => _driver ?? throw new InvalidOperationException("Browser not started. Call Start() first.");

        public string Start()
        {
            if (_driver != null)
            {
                Logger.LogMessage(LogLevel.Error, "Browser already started for this scenario");
                return Global.FAILURE;
            }

            var options = new ChromeOptions();
            // Fixed window size rather than Maximize(): responsive layouts collapse controls
            // below ~1470px, and Maximize() is bounded by whatever monitor is primary.
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-gpu");
            if (TestSettings.Headless)
            {
                options.AddArgument("--headless=new");
            }

            // Selenium Manager (bundled with Selenium 4.6+) downloads a matching
            // chromedriver automatically, so no driver package is needed.
            _driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromSeconds(120));
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            Logger.LogMessage(LogLevel.Debug, "Browser started (headless={0})", TestSettings.Headless);
            return Global.SUCCESS;
        }

        public void GoTo(string url)
        {
            Driver.Navigate().GoToUrl(url);
        }

        // ── Locator-token helpers ────────────────────────────────────────────────

        /// <summary>Resolve a Locators.json token to a control bound to this driver.</summary>
        public T Control<T>(string token) where T : class, IControl
        {
            var control = ControlFactory.Instance.getControl(token) as T;
            if (control == null)
            {
                throw new InvalidOperationException($"Locator token '{token}' is missing or not a {typeof(T).Name}. Check Locators.json.");
            }
            control.WebDriver = Driver;
            return control;
        }

        /// <summary>
        /// Resolve a token whose Identifier contains a {0} placeholder, e.g. a row
        /// or card matched by its visible name.
        /// </summary>
        public T Control<T>(string token, string value) where T : class, IControl
        {
            var data = JsonHelper.Instance.getElement(token)
                       ?? throw new InvalidOperationException($"Locator token '{token}' not found in Locators.json.");
            var identifier = string.Format(data.Identifier, value);
            IControl control = data.ControlType switch
            {
                "Button" => new WebButtonControl(identifier, data.IdentifierType),
                "Textbox" => new WebTextBoxControl(identifier, data.IdentifierType),
                "Label" => new WebLabelControl(identifier, data.IdentifierType),
                _ => throw new NotSupportedException($"Parameterised control type '{data.ControlType}' is not supported."),
            };
            control.WebDriver = Driver;
            return (T)control;
        }

        public By ByToken(string token, string value = null)
        {
            var data = JsonHelper.Instance.getElement(token)
                       ?? throw new InvalidOperationException($"Locator token '{token}' not found in Locators.json.");
            var identifier = value == null ? data.Identifier : string.Format(data.Identifier, value);
            return ControlFactory.Instance.ByWhat(data.IdentifierType, identifier);
        }

        public bool IsVisible(string token, string value = null)
        {
            try
            {
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
                foreach (var element in Driver.FindElements(ByToken(token, value)))
                {
                    if (element.Displayed) return true;
                }
                return false;
            }
            finally
            {
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            }
        }

        public int Count(string token)
        {
            try
            {
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
                return Driver.FindElements(ByToken(token)).Count;
            }
            finally
            {
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            }
        }

        public void WaitUntilVisible(string token, int seconds = 15) =>
            GeneralHelper.FluentWaitTillElementIsVisible(Driver, seconds, 1, token);

        public void WaitUntilClickable(string token, int seconds = 15) =>
            GeneralHelper.FluentWaitTillElementToBeClickable(Driver, seconds, 1, token);

        public void WaitUntilGone(string token, int seconds = 15) =>
            GeneralHelper.ExplicitWaitTillElementIsGone(Driver, seconds, token);

        // ── Cleanup ──────────────────────────────────────────────────────────────

        public void Dispose()
        {
            if (_driver == null) return;
            try
            {
                _driver.Quit();
                Logger.LogMessage(LogLevel.Debug, "Browser quit");
            }
            catch (Exception ex)
            {
                Logger.LogMessage(LogLevel.Error, "Browser quit failed: {0}", ex.Message);
            }
            finally
            {
                _driver.Dispose();
                _driver = null;
            }
        }
    }
}
