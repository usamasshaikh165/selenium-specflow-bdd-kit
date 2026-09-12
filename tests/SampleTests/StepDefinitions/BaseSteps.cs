using NUnit.Framework;
using SampleTests.Support;
using WebControls;

namespace SampleTests.StepDefinitions
{
    /// <summary>
    /// Shared plumbing for step classes: the per-scenario browser and the
    /// login flow that most scenarios start from.
    /// </summary>
    public abstract class BaseSteps
    {
        protected readonly Browser Browser;

        protected BaseSteps(Browser browser)
        {
            Browser = browser;
        }

        protected void OpenLoginPage()
        {
            Assert.AreEqual(Global.SUCCESS, Browser.Start());
            Browser.GoTo(TestSettings.BaseUrl);
            Browser.WaitUntilVisible("LoginUsername");
        }

        protected void SubmitCredentials(string username, string password)
        {
            Assert.AreEqual(Global.SUCCESS, Browser.Control<ITextControl>("LoginUsername").FillText(username));
            Assert.AreEqual(Global.SUCCESS, Browser.Control<ITextControl>("LoginPassword").FillText(password));
            Assert.AreEqual(Global.SUCCESS, Browser.Control<IButtonControl>("LoginButton").ClickButton());
        }

        protected void LoginAsDefaultUser()
        {
            OpenLoginPage();
            SubmitCredentials(TestSettings.DefaultUser, TestSettings.DefaultPassword);
            Browser.WaitUntilVisible("ProductsTitle");
        }
    }
}
