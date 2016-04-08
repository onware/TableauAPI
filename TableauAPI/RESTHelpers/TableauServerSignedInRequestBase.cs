using System;
using System.Net;
using TableauAPI.FilesLogging;
using TableauAPI.RESTRequests;

namespace TableauAPI.RESTHelpers
{
    /// <summary>
    /// Abstract class for making requests AFTER having logged into the server
    /// </summary>
    public abstract class TableauServerSignedInRequestBase : TableauServerRequestBase
    {
        /// <summary>
        /// Current Tableau Server Authentication
        /// </summary>
        protected readonly TableauServerSignIn OnlineSession;

        /// <summary>
        /// Returns the Status Log for this request.
        /// </summary>
        public TaskStatusLogs StatusLog => OnlineSession.StatusLog;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="login"></param>
        protected TableauServerSignedInRequestBase(TableauServerSignIn login)
        {
            OnlineSession = login;
        }


        /// <summary>
        /// Download a file
        /// </summary>
        /// <param name="urlDownload"></param>
        /// <param name="downloadToDirectory"></param>
        /// <param name="baseFilename"></param>
        /// <param name="downloadTypeMapper"></param>
        /// <returns>The path to the downloaded file</returns>
        internal string DownloadFile(string urlDownload, string downloadToDirectory, string baseFilename, DownloadPayloadTypeHelper downloadTypeMapper)
        {
            //Lets keep track of how long it took
            var startDownload = DateTime.Now;
            string outputPath;
            try
            {
                outputPath =  DownloadFile_inner(urlDownload, downloadToDirectory, baseFilename, downloadTypeMapper);
            }
            catch (Exception)
            {
                StatusLog.AddError("Download failed after " + (DateTime.Now - startDownload).TotalSeconds.ToString("#.#") + " seconds. " + urlDownload);
                throw;
            }

            var finishDownload = DateTime.Now;
            StatusLog.AddStatus("Download success duration " + (finishDownload - startDownload).TotalSeconds.ToString("#.#") + " seconds. " + urlDownload, -10);
            return outputPath;
        }

        /// <summary>
        /// Downloads a file
        /// </summary>
        /// <param name="urlDownload"></param>
        /// <param name="downloadToDirectory"></param>
        /// <param name="baseFilename">Filename without extension</param>
        /// <param name="downloadTypeMapper"></param>
        /// <returns>The path to the downloaded file</returns>
        private string DownloadFile_inner(string urlDownload, string downloadToDirectory, string baseFilename, DownloadPayloadTypeHelper downloadTypeMapper)
        {

            //Strip off an extension if its there
            baseFilename =  FileIOHelper.GenerateWindowsSafeFilename(System.IO.Path.GetFileNameWithoutExtension(baseFilename));

            var webClient = CreateLoggedInWebClient();
            using(webClient)
            { 
                //Choose a temp file name to download to
                var starterName = System.IO.Path.Combine(downloadToDirectory, baseFilename + ".tmp");
                OnlineSession.StatusLog.AddStatus("Attempting file download: " + urlDownload, -10);
                webClient.DownloadFile(urlDownload, starterName); //Download the file

                //Look up the correct file extension based on the content type downloaded
                var contentType = webClient.ResponseHeaders["Content-Type"];
                var fileExtension = downloadTypeMapper.GetFileExtension(contentType);
                var finishName = System.IO.Path.Combine(downloadToDirectory, baseFilename + fileExtension);

                //Rename the downloaded file
                System.IO.File.Move(starterName, finishName);
                return finishName;
            }
        }

        /// <summary>
        /// Web client class used for downloads from Tableau Server
        /// </summary>
        /// <returns></returns>
        protected WebClient CreateLoggedInWebClient()
        {
            OnlineSession.StatusLog.AddStatus("Web client being created", -10);

            var webClient = new TableauServerWebClient(); //Create a WebClient object with a large TimeOut value so that larger content can be downloaded
            AppendLoggedInHeadersForRequest(webClient.Headers);
            return webClient;
        }

        /// <summary>
        /// Creates a web request and appends the user credential tokens necessary
        /// </summary>
        /// <param name="url"></param>
        /// <param name="protocol"></param>
        /// <param name="requestTimeout">Useful for specifying timeouts for operations that can take a long time</param>
        /// <returns></returns>
        protected WebRequest CreateLoggedInWebRequest(string url, string protocol = "GET", Nullable<int> requestTimeout = null)
        {
            OnlineSession.StatusLog.AddStatus("Attempt web request: " + url, -10);

            var webRequest = WebRequest.Create(url);
            webRequest.Method = protocol;

            //If an explicit timeout was passed in then use it
            if(requestTimeout != null)
            {
                webRequest.Timeout = requestTimeout.Value;
            }

            AppendLoggedInHeadersForRequest(webRequest.Headers);
            return webRequest;
        }


        /// <summary>
        /// Creates a web request with a MIME payload and send it to server
        /// </summary>
        /// <param name="url"></param>
        /// <param name="protocol">e.g. "PUT" "POST" </param>
        /// <param name="mimeToSend">Mime data we are goign to send</param>
        /// <param name="requestTimeout">timeout in milliseconds</param>
        /// <returns></returns>
        internal WebRequest CreateAndSendMimeLoggedInRequest(string url, string protocol, MimeWriterBase mimeToSend, int? requestTimeout = null)
        {
            var webRequest = CreateLoggedInWebRequest(url, protocol, requestTimeout); 

            //var uploadChunkAsMime = new OnlineMimeUploadChunk(uploadDataBuffer, numBytes);
            var uploadMimeChunk = mimeToSend.GenerateMimeEncodedChunk();

            webRequest.ContentLength = uploadMimeChunk.Length;
            webRequest.ContentType = "multipart/mixed; boundary=" + mimeToSend.MimeBoundaryMarker;

            //Write out the request
            var requestStream = webRequest.GetRequestStream();
            requestStream.Write(uploadMimeChunk, 0, uploadMimeChunk.Length);

            return webRequest;
        }


        /// <summary>
        /// Adds header information that authenticates the request to Tableau Online
        /// </summary>
        /// <param name="webHeaders"></param>
        private void AppendLoggedInHeadersForRequest(WebHeaderCollection webHeaders)
        {
            webHeaders.Add("X-Tableau-Auth", OnlineSession.LogInAuthToken);
            OnlineSession.StatusLog.AddStatus("Append header X-Tableau-Auth: " + OnlineSession.LogInAuthToken, -20);
        }

        /// <summary>
        /// Get the web response; log any error codes that occur and rethrow the exception.
        /// This allows us to get error log data with detailed information
        /// </summary>
        /// <param name="webRequest"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        protected WebResponse GetWebResponseLogErrors(WebRequest webRequest, string description)
        {
            string requestUri = webRequest.RequestUri.ToString();
            try
            {
                return webRequest.GetResponse();
            }
            catch (WebException webException)
            {
                AttemptToLogWebException(webException, description + " (" + requestUri + ") ", StatusLog);
                throw;
            }
        }


        /// <summary>
        /// Attempt to log any detailed information we find about the failed web request
        /// </summary>
        /// <param name="webException"></param>
        /// <param name="description"></param>
        /// <param name="onlineStatusLog"></param>
        private static void AttemptToLogWebException(WebException webException, string description, TaskStatusLogs onlineStatusLog)
        {
            if(onlineStatusLog == null) return; //No logger? nothing to do

            try
            {
                if(string.IsNullOrWhiteSpace(description))
                {
                    description = "web request failed";
                }
                var response = webException.Response;
                var responseText = GetWebResponseAsText(response);
                response.Close();
                if(responseText == null) responseText = "";

                onlineStatusLog.AddError(description +  ": " + webException.Message + "\r\n" + responseText + "\r\n");
            }
            catch (Exception ex)
            {
                onlineStatusLog.AddError("Error in web request exception: " + ex.Message);
            }
        }

    }
}
