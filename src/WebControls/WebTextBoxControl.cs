using WebControls.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogLevel = WebControls.Diagnostics.LogLevel;

namespace WebControls
{
    public class WebTextBoxControl : WebUIElement, ITextControl
    {


        public WebTextBoxControl()
        {

        }

        public WebTextBoxControl(string identifier, string identifiertype) : base(identifier, identifiertype)
        {

        }
        
       
        public string FillText(string text)
        {
            WebDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            if (Element == null)
            {
                Logger.LogMessage(LogLevel.Debug, "Cannot find textbox element");
                return Global.FAILURE;
            }

            Actions action = new Actions(WebDriver);

            Element.Click();
            action.MoveToElement(Element);
            action.Perform();

            Element.Clear();
            Element.SendKeys(text);
            Logger.LogMessage(LogLevel.Debug, "Textbox element filled with {0}", text);
            return Global.SUCCESS;
        }
        

        public string SendKeys(string text)
        {
            WebDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            if (Element == null)
            {
                Logger.LogMessage(LogLevel.Debug, "Cannot find textbox element");
                return Global.FAILURE;
            }

            Actions action = new Actions(WebDriver);

            Element.Clear();
            Element.Click();
            //action.MoveToElement(Element);
            //action.Perform();

           
            Element.SendKeys(text);
            Logger.LogMessage(LogLevel.Debug, "Textbox element filled with {0}", text);
            return Global.SUCCESS;
        }


        public string GetText()
        {
            if (Element == null)
            {
                Logger.LogMessage(LogLevel.Debug, "Cannot find textbox element");
                return Global.FAILURE;
            }

            Actions action = new Actions(WebDriver);
            action.MoveToElement(Element);
            action.Perform();

            return Element.Text;
        }

        public string ValidateFieldEnabled()
        {
            throw new NotImplementedException();
        }

        public string ValidateText(string text)
        {
            if (Element == null)
            {
                Logger.LogMessage(LogLevel.Debug, "Cannot find textbox element");
                return Global.FAILURE;
            }

            Actions action = new Actions(WebDriver);
            action.MoveToElement(Element);
            action.Perform();

            string colText = Element.Text;
            if (colText == text)
            {
                Logger.LogMessage(LogLevel.Debug, "Textbox value: {0} validated successfully", text);
                return Global.SUCCESS;
            }
            Logger.LogMessage(LogLevel.Debug, "Textbox value found:{0} while expected: {1}", colText, text);
            return Global.FAILURE;
        }
       
    }
}
