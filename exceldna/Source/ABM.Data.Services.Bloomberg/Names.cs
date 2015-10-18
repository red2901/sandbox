namespace ABM.Data.Services.Bloomberg
{
    using Bloomberglp.Blpapi;

    public class Names
    {
        /// <summary>
        ///     The category.
        /// </summary>
        public static readonly Name CATEGORY = new Name("category");

        /// <summary>
        ///     The erro r_ info.
        /// </summary>
        public static readonly Name ERROR_INFO = new Name("errorInfo");

        /// <summary>
        ///     The fiel d_ exceptions.
        /// </summary>
        public static readonly Name FIELD_EXCEPTIONS = new Name("fieldExceptions");

        /// <summary>
        ///     The fiel d_ id.
        /// </summary>
        public static readonly Name FIELD_ID = new Name("fieldId");

        /// <summary>
        ///     The message.
        /// </summary>
        public static readonly Name MESSAGE = new Name("message");

        /// <summary>
        ///     The respons e_ error.
        /// </summary>
        public static readonly Name RESPONSE_ERROR = new Name("responseError");

        /// <summary>
        ///     The securit y_ data.
        /// </summary>
        public static readonly Name SECURITY_DATA = new Name("securityData");

        /// <summary>
        ///     The securit y_ error.
        /// </summary>
        public static readonly Name SECURITY_ERROR = new Name("securityError");

        /// <summary>
        ///     The fiel d_ data.
        /// </summary>
        public static readonly Name FIELD_DATA = new Name("fieldData");

        /// <summary>
        ///     The security.
        /// </summary>
        public static readonly Name SECURITY = new Name("security");
    }
}