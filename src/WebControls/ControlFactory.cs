using System;
using WebControls.Diagnostics;
using OpenQA.Selenium;
using LogLevel = WebControls.Diagnostics.LogLevel;

namespace WebControls
{
    public class ControlFactory
    {
        private static ControlFactory m_Instance = null;

        public static ControlFactory Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new ControlFactory();


                }

                return m_Instance;
            }
        }

        public By ByWhat(string ElementIdentifierType, string ElementIdentifier)
        {
            try
            {
                switch (ElementIdentifierType.ToLower())
                {
                    case "name": return (By.Name(ElementIdentifier));
                    case "id": return (By.Id(ElementIdentifier));
                    case "xpath": return (By.XPath(ElementIdentifier));
                    case "tagname": return (By.TagName(ElementIdentifier));
                    case "linktext": return (By.LinkText(ElementIdentifier));
                    case "cssselector": return (By.CssSelector(ElementIdentifier));
                    case "classname": return (By.ClassName(ElementIdentifier));
                    default:
                        {
                            Logger.LogMessage("Cannot find the By type for Identifier Type in Cases . . .");
                            return null;
                        }
                }

            }
            catch (Exception ex)
            {
                Logger.LogMessage(WebControls.Diagnostics.LogLevel.Error, "Cannot find By type for: '{0}'.\nException: {1}", ElementIdentifierType, ex.Message);
                return null;
            }
        }
        public virtual IControl getControl(string token)
        {

            IControl newControl = null;

            JsonElement controlData = JsonHelper.Instance.getElement(token);
            if (controlData == null)
            {

                return null;
            }

            try
            {
                switch (controlData.ControlType)
                {
                    case "Button":
                        {
                            newControl = new WebButtonControl(controlData.Identifier, controlData.IdentifierType);
                            Logger.LogMessage("Control object has been created with token: {0}", token);
                            break;
                        }
                    case "Textbox":
                        {
                            newControl = new WebTextBoxControl(controlData.Identifier, controlData.IdentifierType);
                            Logger.LogMessage("Control object has been created with token: {0}", token);
                            break;
                        }

                    case "Grid":
                        {
                            newControl = new WebGridControl(controlData.Identifier, controlData.IdentifierType);
                            Logger.LogMessage("Control object has been created with token: {0}", token);
                            break;
                        }
                    case "GridHeader":
                        {
                            newControl = new WebHeaderGridControl(controlData.Identifier, controlData.IdentifierType);
                            Logger.LogMessage("Control object has been created with token: {0}", token);
                            break;
                        }
                    case "ComboList":
                        {
                            newControl = new WebComboListControl(controlData.Identifier, controlData.IdentifierType);
                            Logger.LogMessage("Control object has been created with token: {0}", token);
                            break;

                        }
                    case "Combobox":
                        {
                            newControl = new WebComboBoxControl(controlData.Identifier, controlData.IdentifierType);
                            Logger.LogMessage("Control object has been created with token: {0}", token);
                            break;
                        }
                    case "CheckBox":
                        {
                            newControl = new WebCheckboxControl(controlData.Identifier, controlData.IdentifierType);
                            Logger.LogMessage("Control object has been created with token: {0}", token);
                            break;
                        }
                    case "Label":
                        {
                            newControl = new WebLabelControl(controlData.Identifier, controlData.IdentifierType);
                            Logger.LogMessage("Label object has been created with token: {0}", token);
                            break;
                        }
                    case "Calendar":
                        {
                            newControl = new WebCalendarControl(controlData.Identifier, controlData.IdentifierType);
                            Logger.LogMessage("Calendar object has been created with token: {0}", token);
                            break;
                        }

                    default:
                        {
                            Logger.LogMessage("Control type :{0} does not exists.", controlData.ControlType);
                            break;
                        }
                }

            }
            catch (Exception e)
            {
                Logger.LogMessage("Could not create control object with token: {0}, Thrown an Excetion :{1}", token, e);
                return null;
            }


            return newControl;

        }
    }
}