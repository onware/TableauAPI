using TableauAPI.RESTHelpers;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// A arbitrary HTTP GET request we can perform after login into the REST API session
    /// </summary>
    public class SendPostLogInCommand : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _postLoginCommandUrl;

        private string _commandResult;
        /// <summary>
        /// The result from running the command
        /// </summary>
        public string CommandResult => _commandResult;

        /// <summary>
        /// Connect to Tableau and execute a command
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="logInInfo"></param>
        /// <param name="commandUrl"></param>
        public SendPostLogInCommand(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, string commandUrl)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _postLoginCommandUrl = commandUrl;
        }

        /// <summary>
        /// Execute a custom command on the Tableau server.
        /// </summary>
        /// <returns>XML response from server as string.</returns>
        public string ExecuteRequest()
        {
            string url = _postLoginCommandUrl;
            var webRequest = CreateLoggedInWebRequest(url);
            webRequest.Method = "GET";

            //Request the data from server
            OnlineSession.StatusLog.AddStatus("Custom web request: " + url, -10);
            var response = GetWebResponseLogErrors(webRequest, "custom request");
        
            var responseText = GetWebResponseAsText(response);
            _commandResult = responseText;
            return responseText;
        }
    }
}
