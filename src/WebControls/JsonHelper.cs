using WebControls.Diagnostics;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WebControls
{
    public class JsonHelper
    {

        private static JsonHelper m_Instance = null;
        private JObject allControls;

        public static JsonHelper Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new JsonHelper();
                    m_Instance.loadJson();

                }

                return m_Instance;
            }
        }

        /// <summary>
        /// Path of the locator repository. Defaults to Locators.json next to the
        /// test assembly; override with the LOCATOR_FILE environment variable.
        /// </summary>
        public static string LocatorFilePath =>
            Environment.GetEnvironmentVariable("LOCATOR_FILE")
            ?? Path.Combine(AppContext.BaseDirectory, "Locators.json");

        private JObject loadJson()
        {
            string jsonData = File.ReadAllText(LocatorFilePath);
            return allControls = JObject.Parse(jsonData);
        }

        public JsonElement getElement(string token)
        {
            JsonElement element = null;

            try
            {
                element = allControls[token].ToObject<JsonElement>();
                return element;
            }
            catch (Exception e)
            {
                Logger.LogMessage(LogLevel.Error, "cannot find control with token {0} in {1}", token, LocatorFilePath);
                return null;
            }
        }
    }
    public class JsonElement
    {
        public string Identifier { get; set; }
        public string IdentifierType { get; set; }
        public string ControlType { get; set; }

    }
}
