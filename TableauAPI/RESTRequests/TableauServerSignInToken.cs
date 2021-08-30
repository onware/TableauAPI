using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;

namespace TableauAPI.RESTRequests
{
   

    /// <summary>
    /// Manages the signed in session for a Tableau Server site's sign in using a Personal Access Token
    /// </summary>
    public class TableauServerSignInToken : TableauServerSignIn
    {
        const string xmlLogIn = "<tsRequest>  <credentials personalAccessTokenName=\"{{iwsPersonalAccessTokenName}}\"    personalAccessTokenSecret=\"{{iwsPersonalAccessTokenSecret}}\" >  <site contentUrl=\"{{iwsSiteUrl}}\" /> </credentials></tsRequest>";

        private readonly string _tokenName;
        private readonly string _tokenSecret;

        /// <summary>
        /// Synchronous call to test and make sure sign in works
        /// </summary>
        /// <param name="url">Tableau site url</param>
        /// <param name="tokenName">Personal Access Token name</param>
        /// <param name="tokenSecret">Personal Access Token secret</param>
        /// <param name="statusLog">Status log</param>
        public new static void VerifySignInPossible(string url, string tokenName, string tokenSecret, TaskStatusLogs statusLog)
        {
            var urlManager = TableauServerUrls.FromContentUrl(url, 1000);
            var signIn = new TableauServerSignInToken(urlManager, tokenName, tokenSecret, statusLog);
            var success = signIn.ExecuteRequest();

            if (!success)
            {
                throw new Exception("Failed sign in");
            }
        }

        /// <summary>
        /// Create a sign in manager for the given user
        /// </summary>
        /// <param name="url">Tableau site url</param>
        /// <param name="tokenName">Personal Access Token name</param>
        /// <param name="tokenSecret">Personal Access Token secret</param>
        /// <param name="statusLog">Status log</param>
        public TableauServerSignInToken(TableauServerUrls url, string tokenName, string tokenSecret, TaskStatusLogs statusLog): base(url, tokenName, tokenSecret, statusLog)
        {
            _tokenName = tokenName;
            _tokenSecret = tokenSecret;
        }

        /// <summary>
        /// Executes the authentication request against the Tableau server
        /// </summary>
        public new bool ExecuteRequest()
        {
            string bodyText = xmlLogIn;
            bodyText = bodyText.Replace("{{iwsPersonalAccessTokenName}}", _tokenName);
            bodyText = bodyText.Replace("{{iwsPersonalAccessTokenSecret}}", _tokenSecret);
            bodyText = bodyText.Replace("{{iwsSiteUrl}}", _siteUrlSegment);
            AssertTemplateFullyReplaced(bodyText);

            return base.ExecuteRequestForBodyText(bodyText);
        }


    }
}
