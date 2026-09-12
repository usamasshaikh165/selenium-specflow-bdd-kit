using NUnit.Framework;
using SampleTests.Support;
using TechTalk.SpecFlow;
using WebControls;

namespace SampleTests.StepDefinitions
{
    [Binding]
    public sealed class LoginSteps : BaseSteps
    {
        public LoginSteps(Browser browser) : base(browser) { }

        // ── Given ────────────────────────────────────────────────────────────

        [Given(@"the user is on the login page")]
        public void GivenTheUserIsOnTheLoginPage()
        {
            OpenLoginPage();
        }

        // ── When ─────────────────────────────────────────────────────────────

        [When(@"the user enters a valid username and password")]
        public void WhenTheUserEntersValidCredentials()
        {
            SubmitCredentials(TestSettings.DefaultUser, TestSettings.DefaultPassword);
        }

        [When(@"the user enters a username that is not registered")]
        public void WhenTheUserEntersAnUnregisteredUsername()
        {
            SubmitCredentials("no_such_user", "wrong-password");
        }

        [When(@"the user submits a password only")]
        public void WhenTheUserSubmitsAPasswordOnly()
        {
            SubmitCredentials(string.Empty, TestSettings.DefaultPassword);
        }

        [When(@"the user submits a username only")]
        public void WhenTheUserSubmitsAUsernameOnly()
        {
            SubmitCredentials(TestSettings.DefaultUser, string.Empty);
        }

        [When(@"the user enters the locked account's credentials")]
        public void WhenTheUserEntersTheLockedAccountsCredentials()
        {
            SubmitCredentials(TestSettings.LockedUser, TestSettings.LockedPassword);
        }

        [When(@"the user enters the valid username in upper case with the valid password")]
        public void WhenTheUserEntersTheValidUsernameInUpperCase()
        {
            SubmitCredentials(TestSettings.DefaultUser.ToUpperInvariant(), TestSettings.DefaultPassword);
        }

        [When(@"the user enters ""(.*)"" as the username and ""(.*)"" as the password")]
        public void WhenTheUserEntersUsernameAndPassword(string username, string password)
        {
            SubmitCredentials(username, password);
        }

        // ── Then ─────────────────────────────────────────────────────────────

        [Then(@"the product list page should be displayed")]
        public void ThenTheProductListPageShouldBeDisplayed()
        {
            Browser.WaitUntilVisible("ProductsTitle");
            StringAssert.Contains("inventory", Browser.Driver.Url);
            Assert.IsTrue(Browser.IsVisible("ProductList"), "Product list is not visible");
        }

        [Then(@"the page heading should read ""(.*)""")]
        public void ThenThePageHeadingShouldRead(string heading)
        {
            Assert.AreEqual(heading, Browser.Control<ILabelControl>("ProductsTitle").GetLabelText());
        }

        [Then(@"an error message should state ""(.*)""")]
        public void ThenAnErrorMessageShouldState(string expected)
        {
            Browser.WaitUntilVisible("LoginError");
            StringAssert.Contains(expected, Browser.Control<ILabelControl>("LoginError").GetLabelText());
        }

        [Then(@"the user should remain on the login page")]
        public void ThenTheUserShouldRemainOnTheLoginPage()
        {
            Assert.IsTrue(Browser.IsVisible("LoginButton"), "Login button is not visible");
            StringAssert.DoesNotContain("inventory", Browser.Driver.Url);
        }
    }
}
