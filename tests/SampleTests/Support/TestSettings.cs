using System;

namespace SampleTests.Support
{
    /// <summary>
    /// Environment-driven settings. Defaults point at the public Sauce Demo site
    /// so the kit runs out of the box; set the variables (or a .env loaded by
    /// your shell / CI) to target your own application. Never commit real
    /// credentials to source.
    /// </summary>
    public static class TestSettings
    {
        public static string BaseUrl => Get("BASE_URL", "https://www.saucedemo.com/");

        public static string DefaultUser => Get("DEFAULT_USER_EMAIL", "standard_user");
        public static string DefaultPassword => Get("DEFAULT_USER_PASSWORD", "secret_sauce");

        public static string LockedUser => Get("LOCKED_USER_EMAIL", "locked_out_user");
        public static string LockedPassword => Get("LOCKED_USER_PASSWORD", "secret_sauce");

        /// <summary>Run Chrome without a window. Defaults to on under CI.</summary>
        public static bool Headless =>
            Get("HEADLESS", Environment.GetEnvironmentVariable("CI") != null ? "1" : "0") == "1";

        /// <summary>Open the Extent HTML report when the run finishes (local convenience).</summary>
        public static bool OpenReport => Get("OPEN_REPORT", "0") == "1";

        private static string Get(string name, string fallback)
        {
            var value = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }
    }
}
