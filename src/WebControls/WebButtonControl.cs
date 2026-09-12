using WebControls.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogLevel = WebControls.Diagnostics.LogLevel;
namespace WebControls
{
   public class WebButtonControl : WebUIElement, IButtonControl
    {

        //private IWebDriver webDriver;

        public WebButtonControl(string identifier, string identifiertype) : base(identifier, identifiertype)
        {

        }

        public string ClickButton()
        {
            for (int attempt = 0; attempt < 3; attempt++)
            {
                var element = Element;
                if (element == null)
                {
                    Logger.LogMessage(LogLevel.Debug, "Cannot find button element");
                    return Global.FAILURE;
                }
                try
                {
                    ((IJavaScriptExecutor)WebDriver).ExecuteScript("arguments[0].scrollIntoView({block:'center',inline:'nearest'});", element);
                    ((IJavaScriptExecutor)WebDriver).ExecuteScript("arguments[0].click();", element);
                    return Global.SUCCESS;
                }
                catch (StaleElementReferenceException)
                {
                    if (attempt == 2) return Global.FAILURE;
                    System.Threading.Thread.Sleep(500);
                }
            }
            return Global.FAILURE;
        }


        public string Click()
        {
            if (Element == null)
            {
                Logger.LogMessage(LogLevel.Debug, "Cannot find button element");
                return Global.FAILURE;
            }

            Element.Click();
            return Global.SUCCESS;
        }


        public string ValidateButtonText(string text)
        {
            if (Element == null)
            {
                Logger.LogMessage(LogLevel.Debug, "Cannot find button element");
                return Global.FAILURE;
            }

            Actions action = new Actions(WebDriver);
            action.MoveToElement(Element);
            action.Perform();

            string colText = Element.Text;
            if (colText == text)
            {
                Logger.LogMessage(LogLevel.Debug, "Button text value: {0} validated successfully", text);
                return Global.SUCCESS;
            }
            Logger.LogMessage(LogLevel.Debug, "Button text value found:{0} while expected: {1}", colText, text);
            return Global.FAILURE;
        }

        public string ValidateButtonState(Boolean flag)
        {
            if (Element == null)
            {
                Logger.LogMessage(LogLevel.Debug, "Cannot find button element");
                return Global.FAILURE;
            }

            if (Element.Enabled==flag)
            {
                Logger.LogMessage(LogLevel.Info, "Verified button state as {0}",flag);
                return Global.SUCCESS;
            }

            
            return Global.FAILURE;
        }

        public string DoubleClickButton()
        {
            if (Element == null)
            {
                Logger.LogMessage(LogLevel.Debug, "Cannot find button element");
                return Global.FAILURE;
            }

            Actions action = new Actions(WebDriver);
            action.DoubleClick(Element).Perform();
            return Global.SUCCESS;
        }

        
    }
}
