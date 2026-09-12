using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using WebControls.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using LogLevel = WebControls.Diagnostics.LogLevel;

namespace WebControls
{
    class WebCalendarControl : WebUIElement, IWebCalendarControl
    {

        public WebCalendarControl(string identifier, string identifiertype) : base(identifier, identifiertype)
        { }

        public int GetHeaderColumnIndex(string token, string columnName)
        {
            int index = 0;
            IHeaderGridControl headerGrid = ControlFactory.Instance.getControl(token) as IHeaderGridControl;
            headerGrid.WebDriver = WebDriver;
            //index = headerGrid.GetHeaderColumnIndex(columnName);
            index = headerGrid.GetHeaderColumnIndexIndividual(columnName);

            var ele = ControlFactory.Instance.getControl(token);
            if(ele == null)
            {
                Logger.LogMessage("Is null");
            }
            IHeaderGridControl el1 = ele as IHeaderGridControl;
            el1.WebDriver = WebDriver;
            //index = el1.GetHeaderColumnIndex(columnName);
            index = el1.GetHeaderColumnIndexIndividual(columnName);
            if (index == -1)
            {
                return -1;
            }
            return index;
        }
        public string ClickOnCalenderCell(string rowIndex, string columnName, string token, string buttonName, int attempt = 2)
        {
            try
            {
                if (Element == null)
                {
                    Logger.LogMessage(LogLevel.Debug, "Cannot find grid");
                    return Global.FAILURE;
                }

                int columnIndex = GetHeaderColumnIndex(token, columnName);
                if (columnIndex == -1)
                {
                    Logger.LogMessage("Could not find column with the name {0}", columnName);
                    return Global.FAILURE;
                }

                Logger.LogMessage("Column Index is :{0}", columnIndex);

                ReadOnlyCollection<IWebElement> allRows = Element.FindElements(By.TagName("tr"));

                ReadOnlyCollection<IWebElement> columnElement = allRows[int.Parse(rowIndex)].FindElements(By.TagName("td"));
                Logger.LogMessage("Numbers of columns we got: {0}", columnElement.Count);



                GeneralHelper.MoveToElement(columnElement[columnIndex], WebDriver);

                ReadOnlyCollection<IWebElement> buttons = columnElement[columnIndex].FindElements(By.TagName("span"));

                if (buttons == null)
                {
                    buttons = columnElement[columnIndex].FindElements(By.TagName("button"));
                }
                foreach (IWebElement button in buttons)
                {

                    if (button.Text == buttonName || button.GetAttribute("title") == buttonName)
                    {
                        button.Click();
                        Logger.LogMessage("Button with name: {0}  clicked successfully", buttonName);
                        return Global.SUCCESS;
                    }
                }
                Logger.LogMessage("Could not find button with the name: {0}", buttonName);
                return Global.FAILURE;
            }
            catch (StaleElementReferenceException ex)
            {

                Logger.LogMessage(LogLevel.Debug, "Stale element exception occured: {0}", ex.Message);
                attempt--;
                if (attempt >= 1)
                {
                    return ClickOnCalenderCell(rowIndex, columnName, token, buttonName, 2);
                }
                return Global.FAILURE;

            }
        }
        public string ClickMonthBtn(string btnname)
        {
            if (Element == null)
            {
                Logger.LogMessage(LogLevel.Debug, "Cannot find grid");
                return Global.FAILURE;
            }

            ReadOnlyCollection<IWebElement> allRows = Element.FindElements(By.TagName("td"));

            foreach (IWebElement button in allRows)
            {
                GeneralHelper.MoveToElement(button, WebDriver);

                if (button.Text == btnname)
                {
                    OpenQA.Selenium.Interactions.Actions actions = new OpenQA.Selenium.Interactions.Actions(WebDriver);
                    GeneralHelper.MoveToElement(button, WebDriver);
                    Thread.Sleep(600);
                    button.Click();
                    Logger.LogMessage("Button with name: {0}  clicked successfully", btnname);
                    return Global.SUCCESS;
                }
            }

            return Global.SUCCESS;
        }
        public string ClickYearBtn(string btnname)
        {
            if (Element == null)
            {
                Logger.LogMessage(LogLevel.Debug, "Cannot find grid");
                return Global.FAILURE;
            }

            ReadOnlyCollection<IWebElement> allRows = Element.FindElements(By.TagName("td"));

            foreach (IWebElement button in allRows)
            {
                GeneralHelper.MoveToElement(button, WebDriver);

                if (button.Text == btnname)
                {
                    OpenQA.Selenium.Interactions.Actions actions = new OpenQA.Selenium.Interactions.Actions(WebDriver);
                    GeneralHelper.MoveToElement(button, WebDriver);
                    Thread.Sleep(500);
                    button.Click();
                    Logger.LogMessage("Button with name: {0}  clicked successfully", btnname);
                    return Global.SUCCESS;
                }
            }

            return Global.SUCCESS;
        }

        public string ClickDateBtn(string btnname)
        {
            if (Element == null)
            {
                Logger.LogMessage(LogLevel.Debug, "Cannot find grid");
                return Global.FAILURE;
            }

            ReadOnlyCollection<IWebElement> allRows = Element.FindElements(By.TagName("td"));

            foreach (IWebElement button in allRows)
            {
                GeneralHelper.MoveToElement(button, WebDriver);

                if (button.Text == btnname)
                {
                    OpenQA.Selenium.Interactions.Actions actions = new OpenQA.Selenium.Interactions.Actions(WebDriver);
                    GeneralHelper.MoveToElement(button, WebDriver);
                    Thread.Sleep(500);
                    button.Click();
                    Logger.LogMessage("Button with name: {0}  clicked successfully", btnname);
                    return Global.SUCCESS;
                }
            }

            return Global.SUCCESS;
        }

    }
}
