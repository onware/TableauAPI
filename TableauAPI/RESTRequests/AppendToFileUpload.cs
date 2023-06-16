using System;
using TableauAPI.RESTHelpers;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Uploads data to an initiated upload session
    /// </summary>
    public class AppendToFileUpload : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _uploadSession;

        /// <summary>
        /// Create the request to upload a portion of a file to the Tableau REST API
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        public AppendToFileUpload(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, string uploadSession)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _uploadSession = uploadSession;
        }

        /// <summary>
        /// Execute the REST API call to initiate a file upload
        /// </summary>
        public void ExecuteRequest(byte[] chunkContent, int length = -1)
        {
            if (length == -1)
            {
                length = chunkContent.Length;
            }

            var statusLog = OnlineSession.StatusLog;

            string urlUpload = _onlineUrls.Url_AppendFileUploadChunk(OnlineSession, _uploadSession);
            statusLog.AddStatus("Initiating File Upload Append");
            try
            {
                var webRequest = CreateAndSendMimeLoggedInRequest(urlUpload, "PUT", new MimeWriterFileUploadChunk(chunkContent, length));

                var response = GetWebResponseLogErrors(webRequest, "file upload append");
            }
            catch (Exception ex)
            {
                statusLog.AddError("Error during file upload append: " + urlUpload, ex);
            }
        }
    }
}