using NUnit.Framework;
using SampleTests.Support;
using TechTalk.SpecFlow;
using WebControls;

namespace SampleTests.StepDefinitions
{
    [Binding]
    public sealed class CartSteps : BaseSteps
    {
        public CartSteps(Browser browser) : base(browser) { }

        // ── Given ────────────────────────────────────────────────────────────

        [Given(@"the user is signed in and viewing the product list")]
        public void GivenTheUserIsSignedInAndViewingTheProductList()
        {
            LoginAsDefaultUser();
        }

        [Given(@"the cart is empty")]
        public void GivenTheCartIsEmpty()
        {
            Assert.IsFalse(Browser.IsVisible("CartBadge"), "Cart badge is showing but the cart should be empty");
        }

        [Given(@"the cart contains ""([^""]*)""$")]
        public void GivenTheCartContains(string product)
        {
            WhenTheUserAddsToTheCart(product);
        }

        [Given(@"the cart contains ""([^""]*)"" and ""([^""]*)""$")]
        public void GivenTheCartContainsTwoProducts(string first, string second)
        {
            WhenTheUserAddsToTheCart(first);
            WhenTheUserAddsToTheCart(second);
        }

        // ── When ─────────────────────────────────────────────────────────────

        [When(@"the user adds ""(.*)"" to the cart")]
        public void WhenTheUserAddsToTheCart(string product)
        {
            Assert.AreEqual(Global.SUCCESS, Browser.Control<IButtonControl>("AddToCartButton", product).ClickButton());
        }

        [When(@"the user removes ""(.*)"" from the cart")]
        public void WhenTheUserRemovesFromTheCart(string product)
        {
            Assert.AreEqual(Global.SUCCESS, Browser.Control<IButtonControl>("RemoveButton", product).ClickButton());
        }

        [When(@"the user opens the cart")]
        public void WhenTheUserOpensTheCart()
        {
            Assert.AreEqual(Global.SUCCESS, Browser.Control<IButtonControl>("CartLink").ClickButton());
            StringAssert.Contains("cart", Browser.Driver.Url);
        }

        // ── Then ─────────────────────────────────────────────────────────────

        [Then(@"the cart badge should show (\d+)")]
        public void ThenTheCartBadgeShouldShow(int count)
        {
            Browser.WaitUntilVisible("CartBadge");
            Assert.AreEqual(count.ToString(), Browser.Control<ILabelControl>("CartBadge").GetLabelText());
        }

        [Then(@"the cart badge should not be displayed")]
        public void ThenTheCartBadgeShouldNotBeDisplayed()
        {
            Assert.IsFalse(Browser.IsVisible("CartBadge"), "Cart badge is still visible");
        }

        [Then(@"the ""(.*)"" button should read ""(.*)""")]
        public void ThenTheProductButtonShouldRead(string product, string label)
        {
            var token = label == "Remove" ? "RemoveButton" : "AddToCartButton";
            Assert.IsTrue(Browser.IsVisible(token, product), $"Expected the '{product}' button to read '{label}'");
        }

        [Then(@"the cart page should list (\d+) items?")]
        public void ThenTheCartPageShouldListItems(int count)
        {
            Assert.AreEqual(count, Browser.Count("CartItem"));
        }

        [Then(@"""(.*)"" should be listed in the cart")]
        public void ThenProductShouldBeListedInTheCart(string product)
        {
            Assert.IsTrue(Browser.IsVisible("CartItemByName", product), $"'{product}' is not in the cart");
        }
    }
}
