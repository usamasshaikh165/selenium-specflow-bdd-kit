using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace WebControls
{
    public interface IControl
    {
        IWebDriver WebDriver { get; set; }
    }
    public interface ITextControl : IControl
    {
        string GetText();
        string FillText(string textToSet);
        string SendKeys(string textToSet);
        string ValidateText(string textToSet);
        string ValidateFieldEnabled();
    }

    public interface IButtonControl : IControl
    {
        string ClickButton();

        string Click();
        string ValidateButtonText(string text);
        string ValidateButtonState(Boolean flag);
        public string DoubleClickButton();
    }

    public interface ICheckboxControl : IControl
    {
        string ClickCheckbox();
        string ValidateCheckBoxText(string text);
        string ValidateCheckboxChecked();
    }

    public interface IRadioButtonControl : IControl
    {
        string ClickRadioButton();
        string ValidateRadioButtonText(string text);
        string ValidateRadioButtonChecked();
    }

    public interface IGridControl : IControl
    {
        string SelectContextMenuOptionOnGridRow(string rowIndex, string contextMenuOption, string comboDivToken, string comboListToken, int attempt = 2);
        string ClickOnGridCell(string rowIndex, string columnName, string token, string buttonName, int attempt = 2);
        string ClickOnGridCellCheckBox(string rowIndex, string columnName, string token, string buttonName, int attempt = 2);
        string GetGridRowCount();
        string GetColumnCount();
        public string GetColumnValueRow(string rowIndex, string columnName, string token, int attempt = 2);
        string GetColumnValue(string rowIndex, string columnName, string token, int attempt = 2);
        List<string> GetAllColumnValues(string columnName, string token, int attempt = 2);
        string SetColumnValue(string rowIndex, string columnName, string token, string QuantityBox, string text, int attempt = 2);
        int GetIndexOfHeaderColumn(string token, string columnName);
        string UnCheckOnGridCellCheckBox(string rowIndex, string columnName, string token, string elementIdentifier, int attempt = 2);
        string SetDifferentColumnValue(string rowIndex, string columnName, string token, string QuantityBox, string text, int attempt = 2);
        string ValidateRow(string GridHeaderToken, params string[] keyvaluePair);
    }
    public interface IWebCalendarControl : IControl
    {
        int GetHeaderColumnIndex(string token, string columnName);
        string ClickOnCalenderCell(string rowIndex, string columnName, string token, string buttonName, int attempt = 2);

        string ClickMonthBtn(string btnname);
        string ClickYearBtn(string btnname);

        public string ClickDateBtn(string btnname);


    }

    public interface ILabelControl : IControl
    {
        string ClickLabel();
        string ValidateLabelText(string text);
        string GetLabelText();
    }

    public interface IComboboxControl : IControl
    {
        List<string> GetComboListWebElement(string comboDivToken, string comboListToken);
        string SelectComboListWebElement(string elementName, string comboDivToken, string comboListToken);
        string SelectComboListWebElementWithListElementType(string elementName, string comboDivToken, string comboListToken, string comboListTokenType);
        string SelectComboListWebElementByTyping(string elementName);
        string SelectComboListWebElements(string maindiv, string elementlist);
        string SelectComboListWebElementWithIndexwithHTMLPath(string elementName, string comboDivToken, string comboListToken, int index,string HTMLPath);
        string SelectComboListWebElementInHierarchy(string elementName, string comboDivToken, string comboListToken);
        string SelectComboListWebElementWithIndex(string elementName, string comboDivToken, string comboListToken, int index);
        string ClickComboListWebElementWithListElementType(string comboDivToken, string comboListToken, string comboListTokenType);       
        bool ClickOnCombo();
    }

    public interface IHeaderGridControl : IControl
    {
        int GetHeaderColumnIndex(string headerText);
        string SelectContextMenuOptionOnHeaderCell(string headerText, string contextMenuOption, string comboDivToken, string comboListToken);
        string ValidateColumnNames(List<string> columnNames, int attempt = 2);
        string ClickGridHeaderCheckBox(string elementidentifier, int attempt = 2);
        string UnCheckGridHeaderCheckBox(string elementidentifier, int attempt = 2);
        string ClickGridHeaderColumn(string elementidentifier, int attempt = 2);
        void ClickOnHeaderCell(string headerText);
        string ClickOnHeaderFilter(string ColumnText);
        List<string> GetAllColumnValues();
        Dictionary<string, int> GetAllIndexesOfHeader(string token);
        Dictionary<string, string> GetAllColumnValuesUsingDict();
        int GetHeaderColumnIndexIndividual(string headerText);
    }

    public interface IComboListControl : IControl
    {
       string SelectElementInComboList(string comboListToken, string elementName);
       string SelectElementInComboListWithElementType(string comboListToken, string elementName,string comboListTokenType);
        string ClickElementInComboListWithElementType(string comboListToken,string comboListTokenType);   
       List<string> GetComboList(string comboListToken);
       string SelectElementInGridCombo(string elementName, string comboDivToken, string comboListToken);



    }
}
