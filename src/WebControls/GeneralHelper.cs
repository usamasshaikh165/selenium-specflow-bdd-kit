using WebControls.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogLevel = WebControls.Diagnostics.LogLevel;
using SeleniumExtras.WaitHelpers;

namespace WebControls
{
    public class GeneralHelper
    {
        public static bool ParseAndCreateKeywordArgs(ref Dictionary<string, string> keywordDict, params string[] values)
        {
            //if not even
            if (values.Length < 2 || values.Length % 2 > 0)
                return false;

            int count = 0;
            while (count < values.Length)
            {
                keywordDict.Add(values[count], values[count + 1]);
                count += 2;
            }

            return true;
        }

        public static void MoveToElement(IWebElement element, IWebDriver driver)
        {
            try
            {
                ((OpenQA.Selenium.IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block:'nearest'});", element);
            }
            catch (Exception) { }
            try
            {
                Actions action = new Actions(driver);
                action.MoveToElement(element);
                action.Perform();
            }
            catch (Exception) { }
        }


        public static string VerifyMessageInCollection(IWebDriver driver, ReadOnlyCollection<IWebElement> eleCollection, string message)
        {
           
            foreach (IWebElement ele in eleCollection)
            {
                MoveToElement(ele, driver);
               
                string elementText = ele.Text?.Trim();
                if (string.IsNullOrEmpty(elementText))
                    elementText = ele.GetAttribute("textContent")?.Trim();
                if (elementText == message)
                {
                    Logger.LogMessage(LogLevel.Debug, "Error mesage: {0} verified", message);
                    return Global.SUCCESS;                    
                }
            }

            Logger.LogMessage(LogLevel.Error, "No error message: {0} displayed", message);
            return Global.FAILURE;
        }

        public static string SelectComboListWebElement(string elementName, string xpath, IWebDriver driver)
        {
           
            IWebElement popupElement = driver.FindElement(By.XPath(xpath));
            if (popupElement == null)
            {
                Logger.LogMessage(LogLevel.Error, "Cannot find combo popup element");
                return Global.FAILURE;
            }
         
             ReadOnlyCollection<IWebElement> listWebElement = popupElement.FindElements(By.ClassName("dx-list-item-content"));
            if (listWebElement == null)
            {
                Logger.LogMessage(LogLevel.Error, "Cannot find control: firm list elements");
                return Global.FAILURE;
            }
            foreach (IWebElement ele in listWebElement)
            {
                MoveToElement(ele, driver);
                if (ele.Text == elementName)
                {
                    ele.Click();
                    Logger.LogMessage(LogLevel.Debug, "Element: {0} selected", elementName);
                    return Global.SUCCESS;
                }
            }

            Logger.LogMessage(LogLevel.Error, "Cannot find: {0} element in combo", elementName);
            return Global.FAILURE;
        }


        public static List<string> GetComboListWebElement(string xpath, IWebDriver driver)
        {
            List<string> listItems = new List<string>();
            IWebElement popupElement = driver.FindElement(By.XPath(xpath));
            if (popupElement == null)
            {
                Logger.LogMessage(LogLevel.Error, "Cannot find combo popup element");
                return null;
            }

            ReadOnlyCollection<IWebElement> listWebElement = popupElement.FindElements(By.ClassName("dx-list-item-content"));
            if (listWebElement == null)
            {
                Logger.LogMessage(LogLevel.Error, "Cannot find control: firm list elements");
                return null;
            }
            foreach (IWebElement ele in listWebElement)
            {
                MoveToElement(ele, driver);
                listItems.Add(ele.Text);
                Thread.Sleep(250);
            }
            return listItems;
        }

        public static bool CompareStringAndBoolean(bool toggle, string ariaText)
        {
            
            if(toggle == false)
            {
                if (ariaText.Equals("false") && ariaText != null)
                    return true;
                else return false;

            }
            else
            {
                if (ariaText.Equals("true") && ariaText != null)
                    return true;
                else return false;
            }
        }
        public static string GetJSONIdentifierType(string JSONEntry)
        {
            return JsonHelper.Instance.getElement(JSONEntry).IdentifierType;
        }
        public static string GetJSONIdentifier(string JSONEntry)
        {
            return JsonHelper.Instance.getElement(JSONEntry).Identifier;
        }
        public static bool ThreadWait(IWebDriver webdriver, int time, string key)
        {
            WebDriverWait wait = new WebDriverWait(webdriver, TimeSpan.FromSeconds(time));

            string eleLocater = JsonHelper.Instance.getElement(key).IdentifierType;
            try
            {
                switch (eleLocater) 
                {
                    case "Id": 
                    {
                            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(JsonHelper.Instance.getElement(key).Identifier)));
                            break;
                    }
                    case "XPath": 
                    {
                            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath(JsonHelper.Instance.getElement(key).Identifier)));
                            break; 
                    }
                    case "ClassName":
                    {
                            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName(JsonHelper.Instance.getElement(key).Identifier)));
                            break;
                    }
                    case "TagName":
                    {
                            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.TagName(JsonHelper.Instance.getElement(key).Identifier)));
                            break;
                    }
                    case "Name":
                    {
                            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Name(JsonHelper.Instance.getElement(key).Identifier)));
                            break;
                    }
                    case "CssSelector":
                    {
                            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(JsonHelper.Instance.getElement(key).Identifier)));
                            break;
                    }
                    default:
                    {
                            Logger.LogMessage("Control type :{0} does not exists.", eleLocater);
                            break;
                    }


                }
            }
            catch (Exception e)
            {
                Logger.LogMessage("Could not create control object with token: Thrown an Excetion :{1}", e);
                return false;
            }

            return true;          
        }
        public static void ExplicitWaitTillElementIsGone(IWebDriver driver, int time, string key)
        {//timeout exception
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(time));
            try
            {
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated
                    (ControlFactory.Instance.ByWhat(GetJSONIdentifierType(key), GetJSONIdentifier(key))));
            }

            catch (TimeoutException e)
            {
                Logger.LogMessage("{0} has reached. Exception {1}", time, e.ToString());
            }
        }

        /// <summary>
        /// FluentWait / smart wait helpers
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="time"></param>
        /// <param name="poll"></param>
        /// <param name="key"></param>
        public static void FluentWaitTillElementIsVisible(IWebDriver driver, int time, int poll, string key)
        {


            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(driver);
            fluentWait.Timeout = TimeSpan.FromSeconds(time);
            fluentWait.PollingInterval = TimeSpan.FromSeconds(poll);
            /* Ignore the exception - NoSuchElementException that indicates that the element is not present */
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));

            try
            {

                fluentWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(ControlFactory.Instance.ByWhat(GetJSONIdentifierType(key), GetJSONIdentifier(key))));
                Console.Write("eleemnt reached");

            }
            catch (TimeoutException e)
            {
                Logger.LogMessage("{0} has reached. Exception {1}", time, e.ToString());

            }
            catch (Exception e)
            {
                Logger.LogMessage("Unexpected exception: {0}", e.ToString());
            }

        }


        public static void FluentWaitForElementPresence(IWebDriver driver, int time, int poll, string key)
        {


            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(driver);
            fluentWait.Timeout = TimeSpan.FromSeconds(time);
            fluentWait.PollingInterval = TimeSpan.FromSeconds(poll);
            /* Ignore the exception - NoSuchElementException that indicates that the element is not present */
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));

            try
            {

                fluentWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(ControlFactory.Instance.ByWhat(GetJSONIdentifierType(key), GetJSONIdentifier(key))));
                Logger.LogMessage("Element Exists!", key);

            }
            catch (TimeoutException e)
            {
                Logger.LogMessage("{0} has reached. Exception {1}", time, e.ToString());

            }
            catch (Exception e)
            {
                Logger.LogMessage("Unexpected exception: {0}", e.ToString());
            }

        }




        /// <summary>
        /// Wait for element to be clickable with interval and frequency
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="time"></param>
        /// <param name="poll"></param>
        /// <param name="key"></param>
        public static void FluentWaitTillElementIsInVisible(IWebDriver driver, int time, int poll, string key,string text)
        {

            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(driver);
            fluentWait.Timeout = TimeSpan.FromSeconds(time);
            fluentWait.PollingInterval = TimeSpan.FromSeconds(poll);
            /* Ignore the exception - NoSuchElementException that indicates that the element is not present */
            //fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));

            try
            {
                fluentWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementWithText(ControlFactory.Instance.ByWhat(GetJSONIdentifierType(key), GetJSONIdentifier(key)),text));
            }
            catch (TimeoutException e)
            {
                Logger.LogMessage("{0} has reached. Exception {1}", time, e.ToString());

            }


        }

        public static void FluentWaitTillElementToBeClickable(IWebDriver driver, int time, int poll, string key)
        {

            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(driver);
            fluentWait.Timeout = TimeSpan.FromSeconds(time);
            fluentWait.PollingInterval = TimeSpan.FromSeconds(poll);
            /* Ignore the exception - NoSuchElementException that indicates that the element is not present */
            //fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));

            try
            {
                fluentWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(ControlFactory.Instance.ByWhat(GetJSONIdentifierType(key), GetJSONIdentifier(key))));
               
            }
            catch (TimeoutException e)
            {
                Logger.LogMessage("{0} has reached. Exception {1}", time, e.ToString());

            }


        }



        /// <summary>
        /// Explicit wait 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="time"></param>
        /// <param name="key"></param>
        public static void ExplicitWaitTillElementIsVisible(IWebDriver driver, int time, string key)
        {//timeout exception
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(time));
            try
            {
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy
                    (ControlFactory.Instance.ByWhat(GetJSONIdentifierType(key), GetJSONIdentifier(key))));
            }

            catch (TimeoutException e)
            {
                Logger.LogMessage("{0} has reached. Exception {1}", time, e.ToString());
            }
        }

        public static bool WaitUntilElementIsInvisible(IWebDriver webdriver, double timeInSeconds, string jsonKey, int attempts = 2)
        {
            WebDriverWait wait = new WebDriverWait(webdriver, TimeSpan.FromSeconds(timeInSeconds));

            string eleIdentifierType = JsonHelper.Instance.getElement(jsonKey).IdentifierType;
            string eleIdentifier = JsonHelper.Instance.getElement(jsonKey).Identifier;
            try
            {
                if (attempts >= 0)
                {
                    Logger.LogMessage("Explicit wait 'Until Element Is Invisible' started for JSON Key: '{0}' for {1} seconds . . .", jsonKey, timeInSeconds);
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(ControlFactory.Instance.ByWhat(eleIdentifierType, eleIdentifier)));

                }
                else
                {
                    Logger.LogMessage("Attempted for 3 times to wait for the Element to be Invisible . . .");
                    return false;
                }


            }
            catch (Exception ex)
            {
                attempts--;
                Logger.LogMessage("Error in Explicit Wait Until Element is Invisible for: '{0}'.\nException: {1}", jsonKey, ex);
                // Attempting to click on the Element only if the Element is a Label (Toast) . . .

                if (JsonHelper.Instance.getElement(jsonKey).ControlType == "Label")
                {
                    Logger.LogMessage("Attempting to click on the Element: '{0}' . . .", jsonKey);
                    ILabelControl label = ControlFactory.Instance.getControl(jsonKey) as ILabelControl;
                    label.WebDriver = webdriver;
                    label.ClickLabel();
                }

                WaitUntilElementIsInvisible(webdriver, timeInSeconds, jsonKey, attempts);
            }

            return true;

        }

        public static void WaitForElement(IWebDriver driver, int time, string key)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(time));
            wait.Until(ExpectedConditions.ElementIsVisible(ControlFactory.Instance.ByWhat(GetJSONIdentifierType(key), GetJSONIdentifier(key))));
        }

        //Sprint 180
        public static string ModifyXPath(string XPathToken, string value)
        {
            string[] BreakXPath = JsonHelper.Instance.getElement(XPathToken).Identifier.Split("&&");
            string finalXpath = BreakXPath[0] + value + BreakXPath[1];
            return finalXpath;
        }
    }
}
