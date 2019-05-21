using System;
using System.Net;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Manages the signed in session for a Tableau Server site's sign in
    /// </summary>
    public class TableauServerSignIn : TableauServerRequestBase
    {
        const string xmlLogIn = "<tsRequest>  <credentials name=\"{{iwsUserName}}\"    password=\"{{iwsPassword}}\" >  <site contentUrl=\"{{iwsSiteUrl}}\" /> </credentials></tsRequest>";

        private readonly TableauServerUrls _onlineUrls;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _siteUrlSegment;
        private string _logInCookies;
        private string _logInToken;
        private string _logInSiteId;
        private string _logInUserId;

        /// <summary>
        /// Status Log for the current Sign In Session
        /// </summary>
        public readonly TaskStatusLogs StatusLog;
        
        /// <summary>
        /// Synchronous call to test and make sure sign in works
        /// </summary>
        /// <param name="url">Tableau site url</param>
        /// <param name="username">Tableau username</param>
        /// <param name="userPassword">Tableau user's password</param>
        /// <param name="statusLog">Status log</param>
        public static void VerifySignInPossible(string url, string username, string userPassword, TaskStatusLogs statusLog)
        {
            var urlManager = TableauServerUrls.FromContentUrl(url, 1000);
            var signIn = new TableauServerSignIn(urlManager, username, userPassword, statusLog);
            var success = signIn.ExecuteRequest();

            if(!success)
            {
                throw new Exception("Failed sign in");
            }
        }

        /// <summary>
        /// Create a sign in manager for the given user
        /// </summary>
        /// <param name="url">Tableau site url</param>
        /// <param name="username">Tableau username</param>
        /// <param name="password">Tableau user's password</param>
        /// <param name="statusLog">Status log</param>
        public TableauServerSignIn(TableauServerUrls url, string username, string password, TaskStatusLogs statusLog)
        {
            if (statusLog == null) { statusLog = new TaskStatusLogs(); }
            StatusLog = statusLog;

            _onlineUrls = url;
            _userName = username;
            _password = password;
            _siteUrlSegment = url.SiteUrlSegement;
        }

        /// <summary>
        /// Returns the current user's login cookies.
        /// </summary>
        public string LogInCookies => _logInCookies;

        /// <summary>
        /// Returns the current user's login token.
        /// </summary>
        public string LogInAuthToken => _logInToken;

        /// <summary>
        /// Returns the current site the user is authenticated against.
        /// </summary>
        public string SiteId => _logInSiteId;

        /// <summary>
        /// Returns the user's ID.
        /// </summary>
        public string UserId => _logInUserId;

        /// <summary>
        /// Return the current user's username.
        /// </summary>
        public string UserName => _userName;

        /// <summary>
        /// Executes the authentication request against the Tableau server
        /// </summary>
        public bool ExecuteRequest()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            var webRequest = WebRequest.Create(_onlineUrls.UrlLogin);
            string bodyText = xmlLogIn;
            bodyText = bodyText.Replace("{{iwsUserName}}", _userName);
            bodyText = bodyText.Replace("{{iwsPassword}}", _password);
            bodyText = bodyText.Replace("{{iwsSiteUrl}}", _siteUrlSegment);
            AssertTemplateFullyReplaced(bodyText);

            //===============================================================================================
            //Make the sign in request, trap and note, and rethrow any errors
            //===============================================================================================
            try
            {
                SendRequestContents(webRequest, bodyText);
            }
            catch (Exception exSendRequest)
            {
                StatusLog.AddError("Error sending sign in request: " + exSendRequest);
                throw;
            }


            //===============================================================================================
            //Get the web response, trap and note, and rethrow any errors
            //===============================================================================================
            WebResponse response;
            try
            {
                response = webRequest.GetResponse();
            }
            catch(Exception exResponse)
            {
                StatusLog.AddError("Error returned from sign in response: " + exResponse);
                throw;
            }

            var allHeaders = response.Headers;
            var cookies = allHeaders["Set-Cookie"];
            _logInCookies = cookies; //Keep any cookies

            //===============================================================================================
            //Get the web response's XML payload, trap and note, and rethrow any errors
            //===============================================================================================
            XmlDocument xmlDoc;
            try
            {
                xmlDoc = GetWebResponseAsXml(response);
            }
            catch (Exception exSignInResponse)
            {
                StatusLog.AddError("Error returned from sign in xml response: " + exSignInResponse);
                throw;
            }

            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
            var credentialNode = xmlDoc.SelectSingleNode("//iwsOnline:credentials", nsManager);
            var siteNode = xmlDoc.SelectSingleNode("//iwsOnline:site", nsManager);
            _logInSiteId = siteNode.Attributes["id"].Value;
            _logInToken = credentialNode.Attributes["token"].Value;

            //Adding the UserId to the log-in return was a feature that was added late in the product cycle.
            //For this reason this code is going to defensively look to see if hte attribute is there
            var userNode = xmlDoc.SelectSingleNode("//iwsOnline:user", nsManager);
            string userId = null;
            if(userNode != null)
            {
                var userIdAttribute =  userNode.Attributes?["id"];
                if(userIdAttribute != null)
                {
                    userId = userIdAttribute.Value;
                }
                _logInUserId = userId;
            }

            //Output some status text...
            if(!string.IsNullOrWhiteSpace(userId))
            {
                StatusLog.AddStatus("Log-in returned user id: '" + userId + "'", -10);
            }
            else
            {
                StatusLog.AddStatus("No User Id returned from log-in request");
                return false;  //Failed sign in
            }

            return true; //Success
        }
    }
}
