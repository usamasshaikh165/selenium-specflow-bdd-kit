namespace WebControls
{
    /// <summary>
    /// Framework-wide constants. Application data (URLs, accounts, expected
    /// texts) does not belong here: keep it in the test project's settings,
    /// sourced from environment variables or a gitignored config file.
    /// </summary>
    public static class Global
    {
        public const string SUCCESS = "SUCCESS";
        public const string FAILURE = "FAILURE";
        public const string SUCCESS_AND_STOP = "SUCCESS_AND_STOP";

        /// <summary>Name used for log sessions and report titles.</summary>
        public const string APPLICATION_NAME = "SeleniumBddKit";
    }
}
