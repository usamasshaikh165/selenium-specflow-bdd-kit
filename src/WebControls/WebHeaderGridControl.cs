using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Threading;
using WebControls.Diagnostics;
using LogLevel = WebControls.Diagnostics.LogLevel;

namespace WebControls
{
    public class WebHeaderGridControl : WebUIElement, IHeaderGridControl
    {
        public WebHeaderGridControl(string identifier, string identifiertype) : base(identifier, identifiertype)
        { }

        public int GetHeaderColumnIndex(string headerText)
        {
            int index = 0;

            if (Element == null)
            {
                Logger.LogMessage("Header element not found");
            }

            IWebElement row = Element.FindElement(By.TagName("tr"));

            IReadOnlyCollection<IWebElement> columns = row.FindElements(By.TagName("td"));
            foreach (IWebElement col in columns)
            {
                Actions action = new Actions(WebDriver);
                action.MoveToElement(col);
                action.Perform();

                if (col.Text == headerText)
                {
                    Logger.LogMessage("Column header name: {0} found", col.Text);
                    return index;

                }
                index++;
            }

            return -1;
        }


        public List<string> GetAllColumnValues()
        {
            List<string> columnNames = new List<string>();

            IWebElement headerRow = Element.FindElement(By.TagName("tr"));
            IReadOnlyCollection<IWebElement> listColumn = headerRow.FindElements(By.TagName("td"));

            foreach(var item in listColumn)
            {
                GeneralHelper.MoveToElement(item, WebDriver);
                columnNames.Add(item.Text);
            }

            return columnNames;
        }

        public string SelectContextMenuOptionOnHeaderCell(string headerText, string contextMenuOption, string comboDivToken, string comboListToken)
        {
            IWebElement row = Element.FindElement(By.TagName("tr"));

            IReadOnlyCollection<IWebElement> columns = row.FindElements(By.TagName("td"));
            foreach (IWebElement col in columns)
            {
                GeneralHelper.MoveToElement(col, WebDriver);

                if (col.Text == headerText)
                {
                   
                    Actions actions = new Actions(WebDriver);
                    GeneralHelper.MoveToElement(col, WebDriver);
                    Thread.Sleep(500);
                    actions.ContextClick(col).Perform();
                    break;
                }
            }
            IComboListControl comboList = ControlFactory.Instance.getControl(comboDivToken) as IComboListControl;
            comboList.WebDriver = WebDriver;
            Thread.Sleep(1000);
            return comboList.SelectElementInComboList(comboListToken, contextMenuOption);
        }

        public void ClickOnHeaderCell(string headerText)
        {
            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    IWebElement row = Element.FindElement(By.TagName("tr"));
                    IReadOnlyCollection<IWebElement> columns = row.FindElements(By.TagName("td"));
                    bool found = false;
                    foreach (IWebElement col in columns)
                    {
                        GeneralHelper.MoveToElement(col, WebDriver);
                        if (col.Text == headerText)
                        {
                            Actions actions = new Actions(WebDriver);
                            GeneralHelper.MoveToElement(col, WebDriver);
                            Thread.Sleep(500);
                            actions.Click(col).Perform();
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                    // Column not found in iteration — use direct XPath as fallback
                    // Build XPath-safe string: use concat() when the text contains apostrophes
                    string safeText = headerText.Contains("'")
                        ? "concat('" + headerText.Replace("'", "', \"'\", '") + "')"
                        : $"'{headerText}'";
                    var directCol = WebDriver.FindElements(By.XPath($"//div[contains(@class,'dx-datagrid-headers')]//td[normalize-space()={safeText}]"));
                    if (directCol.Count > 0) { directCol[0].Click(); }
                    break;
                }
                catch (StaleElementReferenceException)
                {
                    Thread.Sleep(500);
                }
            }
        }

        public string ClickGridHeaderCheckBox(string elementidentifier, int attempt = 2) 
        {
            try
            {
                if (Element == null)
                {
                    Logger.LogMessage(LogLevel.Debug, "Cannot find grid");
                }
                IWebElement checkBox = Element.FindElement(By.ClassName(elementidentifier));
                string aria= checkBox.GetAttribute("aria-checked");

                if (checkBox != null && !String.IsNullOrEmpty(aria))
                {
                    if (aria.CompareTo("false") == 0)
                    {
                        checkBox.Click();
                        Logger.LogMessage("CheckBox clicked successfully");
                    }
                    else
                        return Global.FAILURE;
                }
                aria = checkBox.GetAttribute("aria-checked");

                if (!String.IsNullOrEmpty(aria))
                {
                    if (GeneralHelper.CompareStringAndBoolean(true, aria) == true)
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
                    return ClickGridHeaderCheckBox(elementidentifier, 2);
                }
                return Global.FAILURE;
            }
        }

        public string ClickGridHeaderColumn(string elementidentifier, int attempt = 2)
        {
            try
            {
                if (Element == null)
                {
                    Logger.LogMessage(LogLevel.Debug, "Cannot find grid");
                }
                IWebElement gridcol = Element.FindElement(By.Id(elementidentifier));
              
                if (gridcol != null)
                {
                    gridcol.Click();
                    Logger.LogMessage("Button with name: {0}  clicked successfully", elementidentifier);
                    return Global.SUCCESS;
                }
                Logger.LogMessage("Could not find button with the name: {0}", elementidentifier);
                return Global.FAILURE;
            }
            catch (StaleElementReferenceException ex)
            {
                Logger.LogMessage(LogLevel.Debug, "Stale element exception occured: {0}", ex.Message);
                attempt--;
                if (attempt >= 1)
                {
                    return ClickGridHeaderCheckBox(elementidentifier, 2);
                }
                return Global.FAILURE;
            }
            return Global.SUCCESS;
        }

        public string UnCheckGridHeaderCheckBox(string elementidentifier, int attempt = 2)
        {
            try
            {
                if (Element == null)
                {
                    Logger.LogMessage(LogLevel.Debug, "Cannot find grid");
                }
                IWebElement checkBox = Element.FindElement(By.ClassName(elementidentifier));
                string aria = checkBox.GetAttribute("aria-checked");

                if (checkBox != null && !String.IsNullOrEmpty(aria))
                {
                    if (aria.CompareTo("true") == 0)
                    {
                        checkBox.Click();
                        Logger.LogMessage("Checkbox Checked successfully");
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
                    return ClickGridHeaderCheckBox(elementidentifier, 2);
                }
                return Global.FAILURE;
            }
            return Global.SUCCESS;
        }

        public string ValidateColumnNames(List<string> columnNames, int attempt = 2)
        {
            try
            {              
                if (Element == null)
                {
                    Logger.LogMessage("Cannot find grid");
                    return Global.FAILURE;
                }
                IWebElement row = Element.FindElement(By.TagName("tr"));
                IReadOnlyCollection<IWebElement> columns = row.FindElements(By.TagName("td"));
                if(columns == null)
                {
                    Logger.LogMessage("No columns found!");
                    return Global.FAILURE;
                }

                for (int columnName = 0; columnName <= columns.Count - 1; columnName++)
                {
                    IWebElement ele = columns.ElementAt(columnName);
                    GeneralHelper.MoveToElement(ele, WebDriver);
                    Logger.LogMessage("Column: {0} found!", ele.Text);
                    Thread.Sleep(5000);
                    if (!columnNames.Contains(ele.Text))
                    {
                        Logger.LogMessage("Column: {0} not found!", ele.Text);
                        return Global.FAILURE;
                    }
                }
                Logger.LogMessage("All columns verified!");
                return Global.SUCCESS;
            }

            catch (Exception ex)
            {
                Logger.LogMessage("Exception occured: {0}", ex.Message);
                attempt--;
                if (attempt >= 1)
                {
                    return ValidateColumnNames(columnNames, 2);
                }
                return Global.FAILURE;
            }

        }

        public string ClickOnHeaderFilter(string ColumnText)
        {
            IWebElement row = Element.FindElement(By.TagName("tr"));

            IReadOnlyCollection<IWebElement> columns = row.FindElements(By.TagName("td"));
            foreach (IWebElement col in columns)
            {
                GeneralHelper.MoveToElement(col, WebDriver);

                if (col.Text == ColumnText)
                {

                    Actions actions = new Actions(WebDriver);
                    GeneralHelper.MoveToElement(col, WebDriver);

                    
                    //we can get it from json helper in future.
                    IWebElement filterButton = col.FindElement(By.ClassName("dx-column-indicators"));
                    filterButton.Click();
                    return Global.SUCCESS;
                    break;
                }
            }

            return Global.FAILURE;

        }

        public Dictionary<string, string> GetAllColumnValuesUsingDict()
        {
            Dictionary<string, string> columnNames = new Dictionary<string, string>();
            IWebElement headerRow = Element.FindElement(By.TagName("tr"));
            IReadOnlyCollection<IWebElement> listColumn = headerRow.FindElements(By.TagName("div"));
            
            foreach (var item in listColumn)
            {
                //string value = listColumn.ElementAt(0);
                GeneralHelper.MoveToElement(item, WebDriver);
                columnNames.Add(item.Text, listColumn.ElementAt(1).Text);
            }
            return columnNames;
        }

        public Dictionary<string, int> GetAllIndexesOfHeader(string token)
        {
            Dictionary<string, int> HeaderIndexMap = new Dictionary<string, int>();
            var ele1 = ControlFactory.Instance.getControl(token);
            if (ele1 == null)
            {
                Logger.LogMessage("Control not found!");
                return null;
            }
            IHeaderGridControl ele = ele1 as IHeaderGridControl;
            ele.WebDriver = WebDriver;
            List<string> colNames = ele.GetAllColumnValues();
            for (int i = 0; i < colNames.Count; i++)
            {
                //string s2 = Regex.Replace(colNames[i], @"\s", "");
                HeaderIndexMap.Add(colNames[i],i);

            }
            return HeaderIndexMap;

        }
        public Dictionary<string, string> GetAllIndexesOfHeaderForKeyValueAsString(string token)
        {
            Dictionary<string, string> HeaderIndexMap = new Dictionary<string, string>();
            var ele1 = ControlFactory.Instance.getControl(token);
            if (ele1 == null)
            {
                Logger.LogMessage("Control not found!");
                return null;
            }
            IHeaderGridControl ele = ele1 as IHeaderGridControl;
            ele.WebDriver = WebDriver;
            Dictionary<string, string> colNames = ele.GetAllColumnValuesUsingDict();
            //for (int i = 0; i < colNames.Count; i++)
            //{
            //    //string s2 = Regex.Replace(colNames[i], @"\s", "");
            //    HeaderIndexMap.Add(colNames[i], i);

            //}
            return HeaderIndexMap;

        }

        public int GetHeaderColumnIndexIndividual(string headerText)
        {
            int index = 0;

            if (Element == null)
            {
                Logger.LogMessage("Header element not found");
            }

            IWebElement row = Element.FindElement(By.TagName("tr"));

            IReadOnlyCollection<IWebElement> columns = row.FindElements(By.TagName("th"));
            foreach (IWebElement col in columns)
            {
                Actions action = new Actions(WebDriver);
                action.MoveToElement(col);
                action.Perform();

                if (col.Text == headerText)
                {
                    Logger.LogMessage("Column header name: {0} found", col.Text);
                    return index;
                }
                index++;
            }

            return -1;
        }
    }
}
