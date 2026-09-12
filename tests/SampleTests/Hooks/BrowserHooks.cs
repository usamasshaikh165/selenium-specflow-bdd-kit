using SampleTests.Support;
using TechTalk.SpecFlow;
using WebControls.Diagnostics;

namespace SampleTests.Hooks
{
    [Binding]
    public sealed class BrowserHooks
    {
        private readonly Browser _browser;

        public BrowserHooks(Browser browser)
        {
            _browser = browser;
        }

        [BeforeTestRun]
        public static void StartLogging()
        {
            Logger.DefaultSessionName = WebControls.Global.APPLICATION_NAME;
            Logger.StartLogging(true, LogLevel.All);
        }

        // Runs after every scenario regardless of outcome, so no chromedriver
        // processes are ever left behind by a failing test.
        [AfterScenario(Order = 100)]
        public void QuitBrowser()
        {
            _browser.Dispose();
        }
    }
}
