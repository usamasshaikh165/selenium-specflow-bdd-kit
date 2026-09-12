using WebControls.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogLevel = WebControls.Diagnostics.LogLevel;


namespace WebControls
{
    public class WebComboBoxControl : WebUIElement, IComboboxControl
    {
        private string listControlXpath;

        public WebComboBoxControl(string identifier, string identifiertype) : base(identifier, identifiertype)
        {

        }


        public WebComboBoxControl(IWebDriver driver, string listcontrolXpath)
        {
            WebDriver = driver;
            listControlXpath = listcontrolXpath;
        }

        public List<string> GetComboListWebElement(string comboDivToken, string comboListToken)
        {
            if (!ClickOnCombo())
            {
                Logger.LogMessage(LogLevel.Error, "Clicking on combo box failed");
                return null;
            }

            IComboListControl comboList = ControlFactory.Instance.getControl(comboDivToken) as IComboListControl;
            comboList.WebDriver = WebDriver;
            return comboList.GetComboList(comboListToken);
        }



        public string SelectComboListWebElement(string elementName, string comboDivToken, string comboListToken)
        {

            if (!ClickOnCombo())
            {
                Logger.LogMessage(LogLevel.Error, "Clicking on combo box failed");
                return Global.FAILURE;
            }

            IComboListControl comboList = ControlFactory.Instance.getControl(comboDivToken) as IComboListControl;

            comboList.WebDriver = WebDriver;
            if (comboList.SelectElementInComboList(comboListToken, elementName) == Global.SUCCESS)
                return Global.SUCCESS;


            return Global.FAILURE;
        }

        //duplicate method
        public string SelectComboListWebElementWithListElementType(string elementName, string comboDivToken, string comboListToken, string comboListTokenType)
        {

            if (!ClickOnCombo())
            {
                Logger.LogMessage(LogLevel.Error, "Clicking on combo box failed");
                return Global.FAILURE;
            }

            IComboListControl comboList = ControlFactory.Instance.getControl(comboDivToken) as IComboListControl;

            comboList.WebDriver = WebDriver;
            if (comboList.SelectElementInComboListWithElementType(comboListToken, elementName, comboListTokenType) == Global.SUCCESS)
                return Global.SUCCESS;


            return Global.FAILURE;
        }


        public string ClickComboListWebElementWithListElementType(string comboDivToken, string comboListToken, string comboListTokenType)
        {

            if (!ClickOnCombo())
            {
                Logger.LogMessage(LogLevel.Error, "Clicking on combo box failed");
                return Global.FAILURE;
            }
            IComboListControl comboList = ControlFactory.Instance.getControl(comboDivToken) as IComboListControl;

            comboList.WebDriver = WebDriver;
            if (comboList.ClickElementInComboListWithElementType(comboListToken, comboListTokenType) == Global.SUCCESS)
                return Global.SUCCESS;


            return Global.FAILURE;
        }


        public string SelectComboListWebElements(string comboDivToken, string comboListToken)
        {

            if (!ClickOnCombo())
            {
                Logger.LogMessage(LogLevel.Error, "Clicking on combo box failed");
                return Global.FAILURE;
            }

            IComboListControl comboList = ControlFactory.Instance.getControl(comboDivToken) as IComboListControl;

            if (comboList == null)
            {
                Logger.LogMessage(LogLevel.Error, "Control doesnot exist", comboList);
                return Global.FAILURE;
            }

            comboList.WebDriver = WebDriver;
            if (comboList.SelectElementInComboList(comboDivToken, comboListToken) == Global.SUCCESS)
                return Global.SUCCESS;


            return Global.FAILURE;
        }
        public string SelectComboListWebElementWithIndexwithHTMLPath(string elementName, string comboDivToken, string comboListToken, int index, string HTMLPath)
        {
            WebDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            if (!ClickOnCombo())
            {
                Logger.LogMessage(LogLevel.Error, "Clicking on combo box failed");
                return Global.FAILURE;
            }

            IReadOnlyCollection<IWebElement> comboListParent = WebDriver.FindElements(By.ClassName(JsonHelper.Instance.getElement(comboDivToken).Identifier));
            if (comboListParent == null)
            {
                Logger.LogMessage("Cannot find parent combo list");
                return Global.FAILURE;
            }
            IWebElement element = comboListParent.ElementAt(index);
            ReadOnlyCollection<IWebElement> listWebElement = element.FindElements(By.ClassName(JsonHelper.Instance.getElement(comboListToken).Identifier));

            if (listWebElement == null)
            {
                Logger.LogMessage("Cannot find list in parent of combo");
                return Global.FAILURE;
            }
            foreach (IWebElement ele in listWebElement)
            {
                GeneralHelper.MoveToElement(ele, WebDriver);
                if (ele.Text == elementName)
                {
                    if (!ele.Enabled)
                        return Global.FAILURE;
                    ele.Click();
                    Logger.LogMessage("Element: {0} selected", elementName);
                    WebDriver.FindElement(By.XPath(JsonHelper.Instance.getElement(HTMLPath).Identifier)).Click();
                    return Global.SUCCESS;
                }
            }

            Logger.LogMessage("Cannot find: {0} element in combo", elementName);
            return Global.FAILURE;

        }
        public string SelectComboListWebElementWithIndex(string elementName, string comboDivToken, string comboListToken, int index)
        {
            WebDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            if (!ClickOnCombo())
            {
                Logger.LogMessage(LogLevel.Error, "Clicking on combo box failed");
                return Global.FAILURE;
            }

            IReadOnlyCollection<IWebElement> comboListParent = WebDriver.FindElements(By.ClassName(JsonHelper.Instance.getElement(comboDivToken).Identifier));
            if (comboListParent == null)
            {
                Logger.LogMessage("Cannot find parent combo list");
                return Global.FAILURE;
            }


            IWebElement element = comboListParent.ElementAt(index);


            ReadOnlyCollection<IWebElement> listWebElement = element.FindElements(By.ClassName(JsonHelper.Instance.getElement(comboListToken).Identifier));


            if (listWebElement == null)
            {
                Logger.LogMessage("Cannot find list in parent of combo");
                return Global.FAILURE;
            }
            var iteration = 1;
            foreach (IWebElement ele in listWebElement)
            {
                Thread.Sleep(200);
                Logger.LogMessage("Iteration number: ", iteration);
                GeneralHelper.MoveToElement(ele, WebDriver);
                string eleText;
                try
                {
                    eleText = ele.Text;
                }
                catch (StaleElementReferenceException)
                {
                    var freshList = element.FindElements(By.ClassName(JsonHelper.Instance.getElement(comboListToken).Identifier));
                    var freshMatch = freshList.FirstOrDefault(e => { try { return e.Text == elementName; } catch { return false; } });
                    if (freshMatch == null) { iteration++; continue; }
                    try { freshMatch.Click(); } catch { return Global.FAILURE; }
                    Logger.LogMessage("Element: {0} selected (after stale re-fetch)", elementName);
                    return Global.SUCCESS;
                }

                if (eleText == elementName)
                {
                    try
                    {
                        if (!ele.Enabled)
                            return Global.FAILURE;
                        ele.Click();
                    }
                    catch (StaleElementReferenceException)
                    {
                        var freshList = element.FindElements(By.ClassName(JsonHelper.Instance.getElement(comboListToken).Identifier));
                        var freshEle = freshList.FirstOrDefault(e => { try { return e.Text == elementName; } catch { return false; } });
                        if (freshEle == null || !freshEle.Enabled)
                            return Global.FAILURE;
                        freshEle.Click();
                    }

                    Logger.LogMessage("Element: {0} selected", elementName);
                    return Global.SUCCESS;
                }
                iteration++;
            }

            Logger.LogMessage("Cannot find: {0} element in combo", elementName);
            return Global.FAILURE;



        }

        public string SelectComboListWebElementInHierarchy(string elementName, string comboDivToken, string comboListToken)
        {
            WebDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            if (!ClickOnCombo())
            {
                Logger.LogMessage(LogLevel.Error, "Clicking on combo box failed");
                return Global.FAILURE;
            }

            IWebElement divElement = Element.FindElement(By.XPath(JsonHelper.Instance.getElement(comboDivToken).Identifier));

            if (divElement == null)
            {
                Logger.LogMessage("Cannot find Div element");
                return Global.FAILURE;
            }

            ReadOnlyCollection<IWebElement> listWebElement = divElement.FindElements(By.XPath(JsonHelper.Instance.getElement(comboListToken).Identifier));
            if (listWebElement == null)
            {
                Logger.LogMessage("Cannot find control: from list elements");
                return Global.FAILURE;
            }
            var iteration = 1;
            foreach (IWebElement ele in listWebElement)
            {

                Logger.LogMessage("Iteration number: ", iteration);
                GeneralHelper.MoveToElement(ele, WebDriver);
                if (ele.Text == elementName)
                {
                    Thread.Sleep(5000);
                    if (!ele.Enabled)
                        return Global.FAILURE;

                    ele.Click();

                    Logger.LogMessage("Element: {0} selected", elementName);


                    return Global.SUCCESS;
                }
                iteration++;
            }

            Logger.LogMessage("Cannot find: {0} element in combo", elementName);
            return Global.FAILURE;

        }


        public string SelectComboListWebElementByTyping(string elementName)
        {
            if (!ClickOnCombo())
            {
                Logger.LogMessage(LogLevel.Error, "Clicking on combo box failed");
                return Global.FAILURE;
            }
            Element.SendKeys(elementName);
            Thread.Sleep(500);
            Element.Click();
            return Global.SUCCESS;

        }

        public bool ClickOnCombo()
        {

            if (Element == null)
            {
                Logger.LogMessage(LogLevel.Debug, "Cannot find combobox control");
                return false;
            }

            Actions action = new Actions(WebDriver);
            action.MoveToElement(Element);
            action.Perform();
            Element.Click();
            Thread.Sleep(1000);
            return true;
        }

    }
}


