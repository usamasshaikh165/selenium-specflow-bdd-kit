using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebControls
{
    public class WebCheckboxControl : WebButtonControl, ICheckboxControl
    {

        public WebCheckboxControl(string identifier, string identifiertype) : base(identifier, identifiertype)
        {

        }

        public string ClickCheckbox()
        {
            return base.ClickButton();
        }

        public string ValidateCheckboxChecked()
        {
            throw new NotImplementedException();
        }

        public string ValidateCheckBoxText(string text)
        {
            return base.ValidateButtonText(text);
        }
    }
}
