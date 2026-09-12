using WebControls.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebControls
{
    public class WebLabelControl : WebButtonControl, ILabelControl
    {
        public WebLabelControl(string identifier, string identifiertype) : base(identifier, identifiertype)
        {

        }


        public string ClickLabel()
        {
            return base.ClickButton();
        }

        public string ValidateLabelText(string text)
        {
            return base.ValidateButtonText(text);
        }

        public string GetLabelText()
        {
            GeneralHelper.MoveToElement(Element, WebDriver);
            return Element.Text;
        }
    }
}
