using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Initiates the upload process for a file
    /// </summary>
    public class InitiateFileUpload : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;

        /// <summary>
        /// Create the request to begin uploading a file to the Tableau REST API
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        public InitiateFileUpload(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
        }

        /// <summary>
        /// Execute the REST API call to initiate a file upload - returns the file upload session
        /// </summary>
        public string ExecuteRequest()
        {
            var statusLog = OnlineSession.StatusLog;
            string fileUploadSession = "";

            string urlInitiate = _onlineUrls.Url_InitiateFileUpload(OnlineSession);
            statusLog.AddStatus("Initiating File Upload");
            try
            {
                var webRequest = CreateLoggedInWebRequest(urlInitiate, "POST");
                var response = GetWebResponseLogErrors(webRequest, "initiate file upload");
                var xmlDoc = GetWebResponseAsXml(response);

                var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
                var result = xmlDoc.SelectSingleNode("//iwsOnline:tsResponse/iwsOnline:fileUpload", nsManager);
                fileUploadSession = result.Attributes?["uploadSessionId"]?.Value ?? "";
            }
            catch (Exception ex)
            {
                statusLog.AddError("Error during file upload initiate\r\n  " + urlInitiate + "\r\n  " + ex.ToString());
            }

            //Return the set of successfully downloaded content
            return fileUploadSession;
        }
    }
}