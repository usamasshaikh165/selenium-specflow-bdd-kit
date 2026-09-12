using WebControls.Diagnostics;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogLevel = WebControls.Diagnostics.LogLevel;
namespace WebControls
{
    public class WebUIElement : IControl
    {
        private string iElementIdentifier;
        private string ielementidentifiertype;
        private IWebDriver webDriver;

        public WebUIElement()
        {

        }
        public WebUIElement(IWebDriver driver)
        {
            webDriver = driver;
        }

        public WebUIElement(string identifier, string identifiertype)
        {
            iElementIdentifier = identifier;
            ielementidentifiertype = identifiertype;
        }

        public IWebElement Element
        {
            get { return GetWebElement(webDriver); }
        }

        public string Identifier
        {
            get { return iElementIdentifier; }
        }
        public IWebDriver WebDriver
        {
            get { return webDriver; }
            set { webDriver = value; }
        }

        public string ElementIdentifier
        {
            get { return iElementIdentifier; }
        }

        public string elementidentifiertype
        {
            get { return ielementidentifiertype; }
        }


        public IWebElement GetWebElement(IWebDriver driver)
        {
            try
            {
                switch (ielementidentifiertype)
                {
                    case "Name":
                        return driver.FindElement(By.Name(iElementIdentifier));

                    case "Id":
                        return driver.FindElement(By.Id(iElementIdentifier));

                    case "XPath":
                        return driver.FindElement(By.XPath(iElementIdentifier));
                    case "TagName":
                        return driver.FindElement(By.TagName(iElementIdentifier));

                    case "LinkText":
                        return driver.FindElement(By.LinkText(iElementIdentifier));

                    case "CssSelector":
                        return driver.FindElement(By.CssSelector(iElementIdentifier));

                    case "ClassName":
                        return driver.FindElement(By.ClassName(iElementIdentifier));

                    default:
                        return driver.FindElement(By.Name(iElementIdentifier));
                }
            }
            catch (Exception ex)
            {
                Logger.LogMessage(LogLevel.Error, "Cannot find element due to an exception:{0}", ex.Message);
                return null;
            }
        }
    }
}
