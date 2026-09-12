using WebControls.Diagnostics;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebControls;
using OpenQA.Selenium.Interactions;
namespace WebControls
{
    
    
    public class WebComboListControl : WebUIElement, IComboListControl
    {

        public WebComboListControl(string identifier, string identifiertype) : base(identifier, identifiertype)
        {
        }

        public List<string> GetComboList(string comboListToken)
        {
            
            List<string> listItems = new List<string>();

            if (Element == null)
            {
                Logger.LogMessage("Cannot find combo popup element");
                return null;
            }

            ReadOnlyCollection<IWebElement> listWebElement = Element.FindElements(By.ClassName(JsonHelper.Instance.getElement(comboListToken).Identifier));
            if (listWebElement == null)
            {
                Logger.LogMessage("Cannot find combo list elements");
                return null;
            }
            foreach (IWebElement ele in listWebElement)
            {
                GeneralHelper.MoveToElement(ele, WebDriver);
                listItems.Add(ele.Text);
                Thread.Sleep(250);
            }
            return listItems;
        }

        public string SelectElementInComboList(string comboListToken, string elementName)
        {
            try
            {

                if (Element == null)
                {
                    Logger.LogMessage("Cannot find combo popup element");
                    return Global.FAILURE;
                }

                ReadOnlyCollection<IWebElement> listWebElement = Element.FindElements(By.ClassName(JsonHelper.Instance.getElement(comboListToken).Identifier));

                if (listWebElement == null)
                {
                    Logger.LogMessage("Cannot find control: from list elements");
                    return Global.FAILURE;
                }
                foreach (IWebElement ele in listWebElement)
                {
                    GeneralHelper.MoveToElement(ele, WebDriver);
                    if (ele.Text.Trim() == elementName.Trim())
                    {
                        if (!ele.Enabled)
                            return Global.FAILURE;
                        ele.Click();
                        Logger.LogMessage("Element: {0} selected", elementName);
                        return Global.SUCCESS;
                    }
                }

                Logger.LogMessage("Cannot find: {0} element in combo", elementName);
                return Global.FAILURE;
            }
            catch (Exception e)
            {
                Logger.LogMessage("Error occured :{0}", e);
                return Global.FAILURE;
            }
        }
       /// <summary>
       /// /duplicate method with new param
       /// </summary>
       /// <param name="comboListToken"></param>
       /// <param name="elementName"></param>
       /// <param name="comboListTokenType"></param>
       /// <returns></returns>
        public string SelectElementInComboListWithElementType(string comboListToken, string elementName, string comboListTokenType)
        {
            ReadOnlyCollection<IWebElement> listWebElement = null;

            try
            {

                if (Element == null)
                {
                    Logger.LogMessage("Cannot find combo popup element");
                    return Global.FAILURE;
                }
               
                if (comboListTokenType.Equals("XPath",StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                       listWebElement = Element.FindElements(By.XPath(JsonHelper.Instance.getElement(comboListToken).Identifier));
                        if (listWebElement == null)
                        {
                            Logger.LogMessage("Cannot find control: from list elements");
                            return Global.FAILURE;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogMessage("Error occured :{0}", e);
                        return Global.FAILURE;
                    }
                }
                else if(comboListTokenType.Equals("ClassName", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        listWebElement = Element.FindElements(By.ClassName(JsonHelper.Instance.getElement(comboListToken).Identifier));
                        if (listWebElement == null)
                        {
                            Logger.LogMessage("Cannot find control: from list elements");
                            return Global.FAILURE;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogMessage("Error occured :{0}", e);
                        return Global.FAILURE;
                    }
                }
                else if (comboListTokenType.Equals("CssSelector", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        listWebElement = Element.FindElements(By.CssSelector(JsonHelper.Instance.getElement(comboListToken).Identifier));
                        if (listWebElement == null)
                        {
                            Logger.LogMessage("Cannot find control: from list elements");
                            return Global.FAILURE;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogMessage("Error occured :{0}", e);
                        return Global.FAILURE;
                    }
                }

                foreach (IWebElement ele in listWebElement)
                {
                    GeneralHelper.MoveToElement(ele, WebDriver);
                    if (ele.Text.Trim() == elementName.Trim())
                    {
                        if (!ele.Enabled)
                            return Global.FAILURE;
                        ele.Click();
                        Logger.LogMessage("Element: {0} selected", elementName);
                        return Global.SUCCESS;
                    }
                }

                Logger.LogMessage("Cannot find: {0} element in combo", elementName);
                return Global.FAILURE;
            }
            catch (Exception e)
            {
                Logger.LogMessage("Error occured :{0}", e);
                return Global.FAILURE;
            }
        }


        public string ClickElementInComboListWithElementType(string comboListToken, string comboListTokenType)
        {
            ReadOnlyCollection<IWebElement> listWebElement = null;

            try
            {

                if (Element == null)
                {
                    Logger.LogMessage("Cannot find combo popup element");
                    return Global.FAILURE;
                }

                if (comboListTokenType.Equals("XPath", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        listWebElement = Element.FindElements(By.XPath(JsonHelper.Instance.getElement(comboListToken).Identifier));
                        if (listWebElement == null)
                        {
                            Logger.LogMessage("Cannot find control: from list elements");
                            return Global.FAILURE;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogMessage("Error occured :{0}", e);
                        return Global.FAILURE;
                    }
                }
                else if (comboListTokenType.Equals("ClassName", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        listWebElement = Element.FindElements(By.ClassName(JsonHelper.Instance.getElement(comboListToken).Identifier));
                        if (listWebElement == null)
                        {
                            Logger.LogMessage("Cannot find control: from list elements");
                            return Global.FAILURE;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogMessage("Error occured :{0}", e);
                        return Global.FAILURE;
                    }
                }
                else if (comboListTokenType.Equals("CssSelector", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        listWebElement = Element.FindElements(By.CssSelector(JsonHelper.Instance.getElement(comboListToken).Identifier));
                        if (listWebElement == null)
                        {
                            Logger.LogMessage("Cannot find control: from list elements");
                            return Global.FAILURE;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogMessage("Error occured :{0}", e);
                        return Global.FAILURE;
                    }
                }

                foreach (IWebElement ele in listWebElement)
                {
                    GeneralHelper.MoveToElement(ele, WebDriver);
                   
                    if (!ele.Enabled)
                    {
                        return Global.FAILURE;
                    }
                    else
                    {
                        ele.Click();
                        Logger.LogMessage("Element: {0} selected", listWebElement);
                        return Global.SUCCESS;
                    }
                              
                    
                }

                Logger.LogMessage("Cannot find: {0} element in combo", listWebElement);
                return Global.FAILURE;
            }
            catch (Exception e)
            {
                Logger.LogMessage("Error occured :{0}", e);
                return Global.FAILURE;
            }
        }



        // Generic: usable in any project
        public IList<IWebElement> GetAllDropDownElements(string comboListToken, string elementName)
        {
            IList<IWebElement> listWebElement = null;
            List<IWebElement> OriginalListWebElement = new List<IWebElement>();
            int attempt = 3;

            do
            {
                listWebElement = GetElements(comboListToken, elementName);



                if (listWebElement != null)
                {
                    if (listWebElement.Last().GetAttribute("style").Contains("yellow"))
                    {
                        attempt--;
                    }



                    else
                    {
                        foreach (IWebElement ele in listWebElement)
                        {
                            if (!ele.GetAttribute("style").Contains("yellow"))
                            {
                                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)WebDriver;
                                jsExecutor.ExecuteScript("arguments[0].style.background='yellow'", ele);
                                OriginalListWebElement.Add(ele);
                                GeneralHelper.MoveToElement(ele, WebDriver);
                            }
                        }
                        Thread.Sleep(200);
                        ((IJavaScriptExecutor)WebDriver).ExecuteScript("window.scrollBy(0, 400);");



                    }



                }

            }
            while (attempt != 0);



            return OriginalListWebElement;
        }



 public IList<IWebElement> GetElements(string comboListToken, string elementName)

        {

            if (Element == null)

            {

                Logger.LogMessage("Cannot find combo popup element");

                return null;

            }



            IList<IWebElement> listWebElement = Element.FindElements(By.ClassName(JsonHelper.Instance.getElement(comboListToken).Identifier));

            if (listWebElement == null)

            {

                Logger.LogMessage("Cannot find control: from list elements");

                return null;

            }



            return listWebElement;

        }

        public string SelectElementInGridCombo(string elementName, string comboDivToken, string comboListToken)
        {
            IList<IWebElement> listWebElement = GetAllDropDownElements(comboListToken, elementName);



            foreach (IWebElement ele in listWebElement)
            {
                GeneralHelper.MoveToElement(ele, WebDriver);
                if (ele.Text.Trim() == elementName.Trim())
                {
                    Logger.LogMessage("Value is getting. {0}", ele.Text);
                    if (!ele.Enabled)
                        return Global.FAILURE;
                    ele.Click();

                    Logger.LogMessage("Element: {0} selected", elementName);
                    return Global.SUCCESS;
                }
            }



            Logger.LogMessage("Cannot find: {0} element in combo", elementName);
            return Global.FAILURE;



        }
    }
}
