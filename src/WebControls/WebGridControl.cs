using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Collections.ObjectModel;
using System.Threading;
using WebControls.Diagnostics;
using LogLevel = WebControls.Diagnostics.LogLevel;

namespace WebControls
{
    public class WebGridControl : WebUIElement, IGridControl
    {

        public WebGridControl(string identifier, string identifiertype) : base(identifier, identifiertype)
        { 

        }

        public string SelectContextMenuOptionOnGridRow(string rowIndex, string contextMenuOption, string comboDivToken, string comboListToken, int attempt = 2)
        {
            try
            {
                if (Element == null)
                {
                    Logger.LogMessage(LogLevel.Debug, "Cannot find grid");
                    return Global.FAILURE;
                }


                ReadOnlyCollection<IWebElement> rowElements = Element.FindElements(By.TagName("td"));

                GeneralHelper.MoveToElement(rowElements[int.Parse(rowIndex)], WebDriver);
                Actions actions = new Actions(WebDriver);

                actions.ContextClick(rowElements[int.Parse(rowIndex)]).Perform();

                IComboListControl comboList = ControlFactory.Instance.getControl(comboDivToken) as IComboListControl;
                comboList.WebDriver = WebDriver;

                return comboList.SelectElementInComboList(comboListToken, contextMenuOption);
            }
            catch (StaleElementReferenceException ex)
            {

                Logger.LogMessage(LogLevel.Debug, "Stale element exception occured: {0}", ex.Message);
                attempt--;
                if (attempt >= 1)
                {
                    return SelectContextMenuOptionOnGridRow(rowIndex, contextMenuOption, comboDivToken, comboListToken, 2);
                }
                return Global.FAILURE;

            }
        }

        public int GetIndexOfHeaderColumn(string token, string columnName)
        {
            int index = 0;
            IHeaderGridControl headerGrid = ControlFactory.Instance.getControl(token) as IHeaderGridControl;
            if (headerGrid == null)
            {
                Logger.LogMessage("Control not found");
            }

            headerGrid.WebDriver = WebDriver;

            index = headerGrid.GetHeaderColumnIndex(columnName);
            if (index == -1)
            {
                return -1;
            }
            return index;
        }

        public string ClickOnGridCell(string rowIndex, string columnName, string token, string buttonName, int attempt = 2)
        {
            try
            {
                if (Element == null)
                {
                    Logger.LogMessage(LogLevel.Debug, "Cannot find grid");
                    return Global.FAILURE;
                }

                int columnIndex = GetIndexOfHeaderColumn(token, columnName);

                if (columnIndex == -1)
                {
                    Logger.LogMessage("Could not find column with the name {0}", columnName);
                    return Global.FAILURE;
                }


                ReadOnlyCollection<IWebElement> allRows = Element.FindElements(By.TagName("tr"));

                ReadOnlyCollection<IWebElement> columnElement = allRows[int.Parse(rowIndex)].FindElements(By.TagName("td"));


                GeneralHelper.MoveToElement(columnElement[columnIndex], WebDriver);               

                ReadOnlyCollection<IWebElement> buttons = columnElement[columnIndex].FindElements(By.TagName("button"));

                if(buttons == null)
                {
                    Logger.LogMessage("Buttons not found!");
                    return Global.FAILURE;
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
                    return ClickOnGridCell(rowIndex, columnName, token, buttonName, 2);
                }
                return Global.FAILURE;

            }
        }

        public string ClickOnGridCellCheckBox(string rowIndex, string columnName, string token, string elementIdentifier, int attempt = 2)
        {
            try
            {
                if (Element == null)
                {
                    Logger.LogMessage(LogLevel.Debug, "Cannot find grid");
                    return Global.FAILURE;
                }

                int columnIndex = GetIndexOfHeaderColumn(token, columnName);
                if (columnIndex == -1)
                {
                    Logger.LogMessage("Could not find column with the name {0}", columnName);
                    return Global.FAILURE;
                }



                ReadOnlyCollection<IWebElement> allRows = Element.FindElements(By.TagName("tr"));

                ReadOnlyCollection<IWebElement> columnElement = allRows[int.Parse(rowIndex)].FindElements(By.TagName("td"));

                GeneralHelper.MoveToElement(columnElement[columnIndex], WebDriver);

                IWebElement checkBox = columnElement[columnIndex].FindElement(By.ClassName(elementIdentifier));

                string aria = checkBox.GetAttribute("aria-checked");

                if (checkBox != null && !String.IsNullOrEmpty(aria))
                {
                    if (aria.CompareTo("false") == 0)
                    {
                        checkBox.Click();
                        Logger.LogMessage("Checkbox clicked successfully!");
                    }
                    else 
                        return Global.FAILURE;
                }

                aria = checkBox.GetAttribute("aria-checked");

                if (!String.IsNullOrEmpty(aria))
                {
                  if( GeneralHelper.CompareStringAndBoolean(true, aria)==true)
                        return Global.SUCCESS;
                }

                Logger.LogMessage("Checkbox not clicked");
                return Global.FAILURE;

            }
            catch (StaleElementReferenceException ex)
            {

                Logger.LogMessage(LogLevel.Debug, "Stale element exception occured: {0}", ex.Message);
                attempt--;
                if (attempt >= 1)
                {
                    return ClickOnGridCellCheckBox(rowIndex, columnName, token, elementIdentifier, 2);
                }
                return Global.FAILURE;

            }
        }

        public string UnCheckOnGridCellCheckBox(string rowIndex, string columnName, string token, string elementIdentifier, int attempt = 2)
        {
            try
            {
                if (Element == null)
                {
                    Logger.LogMessage(LogLevel.Debug, "Cannot find grid");
                    return Global.FAILURE;
                }

                int columnIndex = GetIndexOfHeaderColumn(token, columnName);
                if (columnIndex == -1)
                {
                    Logger.LogMessage("Could not find column with the name {0}", columnName);
                    return Global.FAILURE;
                }



                ReadOnlyCollection<IWebElement> allRows = Element.FindElements(By.TagName("tr"));

                ReadOnlyCollection<IWebElement> columnElement = allRows[int.Parse(rowIndex)].FindElements(By.TagName("td"));

                GeneralHelper.MoveToElement(columnElement[columnIndex], WebDriver);
                IWebElement checkBox = Element.FindElement(By.ClassName(elementIdentifier));
                string aria = checkBox.GetAttribute("aria-checked");

                if (checkBox != null && !String.IsNullOrEmpty(aria))
                {
                    if (aria.CompareTo("true") == 0)
                    {
                        checkBox.Click();
                        Logger.LogMessage("Checkbox clicked successfully");
                    }
                    else
                        return Global.FAILURE;
                }
                aria = checkBox.GetAttribute("aria-checked");

                if (!String.IsNullOrEmpty(aria))
                {
                    if (GeneralHelper.CompareStringAndBoolean(false, aria) == true)
                        return Global.SUCCESS;
                }

                Logger.LogMessage("Checkbox is not clicked");
                return Global.FAILURE;

            }
            catch (StaleElementReferenceException ex)
            {

                Logger.LogMessage(LogLevel.Debug, "Stale element exception occured: {0}", ex.Message);
                attempt--;
                if (attempt >= 1)
                {
                    return ClickOnGridCellCheckBox(rowIndex, columnName, token, elementIdentifier, 2);
                }
                return Global.FAILURE;

            }
        }


        public string GetGridRowCount()
        {
            Logger.LogMessage("Element {0}", Element);

            if (Element == null)
            {
                Logger.LogMessage(LogLevel.Debug, "Cannot find grid");
                return Global.FAILURE;
            }

            ReadOnlyCollection<IWebElement> rows = Element.FindElements(By.TagName("tr"));
            return rows.Count.ToString();
        }

        public string GetColumnCount()
        {
            Logger.LogMessage("Element {0}", Element);

            if (Element == null)
            {
                Logger.LogMessage(LogLevel.Debug, "Cannot find grid");
                return Global.FAILURE;
            }

            ReadOnlyCollection<IWebElement> cols = Element.FindElements(By.TagName("td"));
            return cols.Count.ToString();
        }


        public string GetColumnValue(string rowIndex, string columnName, string token, int attempt = 2)
        {
            try
            {
                if (Element == null)
                {
                    Logger.LogMessage(LogLevel.Debug, "Cannot find grid");
                    return Global.FAILURE;
                }

                int columnIndex = GetIndexOfHeaderColumn(token, columnName);
                if (columnIndex == -1)
                {
                    Logger.LogMessage("Could not find column with the name {0}", columnName);
                    return Global.FAILURE;
                }

                ReadOnlyCollection<IWebElement> allRows = Element.FindElements(By.TagName("tr"));

                ReadOnlyCollection<IWebElement> columnElement = allRows[int.Parse(rowIndex)].FindElements(By.TagName("td"));

                GeneralHelper.MoveToElement(columnElement[columnIndex], WebDriver);

                IWebElement cellText = columnElement[columnIndex];

                return cellText.Text;

            }
            catch (StaleElementReferenceException ex)
            {

                Logger.LogMessage(LogLevel.Debug, "Stale element exception occured: {0}", ex.Message);
                attempt--;
                if (attempt >= 1)
                {
                    return GetColumnValue(rowIndex, columnName, token, 2);
                }
                return Global.FAILURE;

            }
        }


        public List<string> GetAllColumnValues(string columnName, string token, int attempt = 2)
        {
            List<string> allColumns = new List<string>();
            
            string count = GetGridRowCount();

            Logger.LogMessage("GridRowCount() is {0}", count);

            int rowcount = 0;

            if (!Int32.TryParse(count, out rowcount))
                return allColumns;
            int i = 0;
            while (i < rowcount-1)
            {
                allColumns.Add(GetColumnValue(i.ToString(), columnName, token));
                i++;
            }         
            return allColumns;
        }


        public string ValidateGridRow(string rowIndex, string token, string[] keyvaluePair)
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            if (GeneralHelper.ParseAndCreateKeywordArgs(ref keyValues, keyvaluePair))
            {
                Dictionary<string, string>.Enumerator item = keyValues.GetEnumerator();
                while (item.MoveNext())
                {

                    string colText = GetColumnValue(rowIndex, item.Current.Key, token);
                    if (colText == item.Current.Value)
                        continue;
                    else
                    {
                        Logger.LogMessage(LogLevel.Debug, "Value found: {0} while value given: {1}", colText, item.Current.Value);
                        return Global.FAILURE;
                    }
                }
                Logger.LogMessage(LogLevel.Debug, "Row validated successfully at index: {0}", rowIndex);
                return Global.SUCCESS;
            }
            Logger.LogMessage(LogLevel.Debug, "Invalid arguments. Unable to parse keywords");
            return Global.FAILURE;
        }


        public string SetColumnValue(string rowIndex, string columnName, string token, string QuantityBox, string text, int attempt = 2)

        {
            try
            {
                int columnIndex = GetIndexOfHeaderColumn(token, columnName);
                

                if (columnIndex == -1)
                {
                    Logger.LogMessage("Could not find column with the name {0}", columnName);
                    return Global.FAILURE;
                }

                Logger.LogMessage("Colindex {0}", columnIndex);
                ReadOnlyCollection<IWebElement> allRows = Element.FindElements(By.TagName("tr"));

                ReadOnlyCollection<IWebElement> columnElement = allRows[int.Parse(rowIndex)].FindElements(By.TagName("td"));
                //This code is commented, as this is used only for debugging 
                //foreach (var item in columnElement)
                //{
                //    Logger.LogMessage("Name : " + item.Text + "");
                //}
                Logger.LogMessage("Numbers of columns we got: {0}", columnElement.Count);



                GeneralHelper.MoveToElement(columnElement[columnIndex], WebDriver);

                IWebElement textElement = columnElement[columnIndex].FindElement(By.Id(JsonHelper.Instance.getElement(QuantityBox).Identifier));

                GeneralHelper.MoveToElement(textElement, WebDriver);
                

                //this can produce an error when there is need to move into textbox.
                textElement.Click();
                textElement.Clear();
                textElement.SendKeys(text);
                return Global.SUCCESS;
            }
            catch (Exception ex)
            {

                Logger.LogMessage(LogLevel.Debug, "Exception occured: {0}", ex.Message);
                attempt--;
                if (attempt >= 1)
                {
                    return SetColumnValue(rowIndex, columnName, token, text, QuantityBox, attempt);
                }
                return Global.FAILURE;

            }
        }

        public string SetDifferentColumnValue(string rowIndex, string columnName, string token, string QuantityBox, string text, int attempt = 2)

        {
            try
            {
                int columnIndex = GetIndexOfHeaderColumn(token, columnName);

                if (columnIndex == -1)
                {
                    Logger.LogMessage("Could not find column with the name {0}", columnName);
                    return Global.FAILURE;
                }

                Logger.LogMessage("Colindex {0}", columnIndex);
                ReadOnlyCollection<IWebElement> allRows = Element.FindElements(By.TagName("tr"));

                ReadOnlyCollection<IWebElement> columnElement = allRows[int.Parse(rowIndex)].FindElements(By.TagName("td"));
                //This code is commented, as this is used only for debugging 
                //foreach (var item in columnElement)
                //{
                //    Logger.LogMessage("Name : " + item.Text + "");
                //}
                Logger.LogMessage("Numbers of columns we got: {0}", columnElement.Count);

                GeneralHelper.MoveToElement(columnElement[columnIndex], WebDriver);

                if (columnName == "Account Number")
                {
                    IWebElement textElement = columnElement[columnIndex].FindElement(By.ClassName(JsonHelper.Instance.getElement("AccountTextBox").Identifier));
                    GeneralHelper.MoveToElement(textElement, WebDriver);

                    Actions actions = new Actions(WebDriver);
                    textElement.Click();
                    textElement.Clear();
                    string[] texts = text.Split("-");
                    foreach (var item in texts)
                    {
                        textElement.SendKeys(item);
                    }
                    
                }
                else
                {
                    IWebElement text_Element = columnElement[columnIndex].FindElement(By.Id(JsonHelper.Instance.getElement(QuantityBox).Identifier));
                    GeneralHelper.MoveToElement(text_Element, WebDriver);

                    //this can produce an error when there is need to move into textbox.
                    text_Element.Click();
                    //Thread.Sleep(3000);
                    //text_Element.Clear();
                    Actions actions = new Actions(WebDriver);
                    //actions.Click(text_Element).KeyDown(Keys.Control).SendKeys("a").KeyUp(Keys.Control).SendKeys(Keys.Backspace).Build().Perform();
                    Thread.Sleep(3000);
                    text_Element.SendKeys(text);
                    text_Element.SendKeys(Keys.Tab);
                    //Thread.Sleep(3000);
                }

                return Global.SUCCESS;
            }
            catch (Exception ex)
            {
                Logger.LogMessage(LogLevel.Debug, "Exception occured: {0}", ex.Message);
                attempt--;
                if (attempt >= 1)
                {
                    return SetColumnValue(rowIndex, columnName, token, text, QuantityBox, attempt);
                }
                return Global.FAILURE;
            }
        }


        public string GetColumnValueRow(string rowIndex, string columnName, string token, int attempt = 2)
        {
            try
            {
                if (Element == null)
                {
                    Logger.LogMessage(LogLevel.Debug, "Cannot find grid");
                    return Global.FAILURE;
                }

                int columnIndex = GetIndexOfHeaderColumn(token, columnName);
                if (columnIndex == -1)
                {
                    Logger.LogMessage("Could not find column with the name {0}", columnName);
                    return Global.FAILURE;
                }

                ReadOnlyCollection<IWebElement> allRows = Element.FindElements(By.TagName("tr"));

                ReadOnlyCollection<IWebElement> columnElement = allRows[int.Parse(rowIndex)].FindElements(By.TagName("td"));

                GeneralHelper.MoveToElement(columnElement[columnIndex], WebDriver);

                IWebElement cellText = columnElement[columnIndex];
                return cellText.Text;

            }
            catch (StaleElementReferenceException ex)
            {

                Logger.LogMessage(LogLevel.Debug, "Stale element exception occured: {0}", ex.Message);
                attempt--;
                if (attempt >= 1)
                {
                    return GetColumnValueRow(rowIndex, columnName, token, attempt);
                }
                return Global.FAILURE;

            }
        }

        public string ValidateRow(string GridHeaderToken, params string[] keyValuePair)
        {

            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            if (GeneralHelper.ParseAndCreateKeywordArgs(ref keyValues, keyValuePair))
            {
                bool flag = false;

                string rowCount = GetGridRowCount();

                if (rowCount == "-1")
                {
                    Logger.LogMessage("Row count is found to be: '{0}'", rowCount);
                    return Global.FAILURE;
                }

                int count = Int32.Parse(rowCount);

                for (int index = 0; index < count; index++)
                {
                    Dictionary<string, string>.Enumerator item = keyValues.GetEnumerator();
                    while (item.MoveNext())
                    {

                        string colText = GetColumnValue(index.ToString(), item.Current.Key, GridHeaderToken);
                        if (colText == item.Current.Value)
                        {
                            flag = true;
                            Logger.LogMessage("Value found as: {0}, in column {1}", item.Current.Value, item.Current.Key);
                            continue;
                        }

                        else
                        {
                            Logger.LogMessage(LogLevel.Debug, "Value found: '{0}' while expected: '{1}'. In Validate Grid Row", colText, item.Current.Value);
                            flag = false;
                            break;
                        }

                    }

                    if (flag == true)
                    {
                        Logger.LogMessage("Row validated successfully at index: '{0}'", index);
                        return Global.SUCCESS;
                    }

                }

            }
            else
            {
                Logger.LogMessage(LogLevel.Debug, "Invalid arguments. Unable to parse keywords");
            }

            Logger.LogMessage("Row could not be validated in grid.");
            return Global.FAILURE;

        }


       
    }
}
