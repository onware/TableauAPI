using System;
using System.Collections.Generic;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Manages the download of a flow from a Tableau REST API and saves it to disk
    /// </summary>
    public class DownloadFlow : TableauServerSignedInRequestBase
    {

        private readonly TableauServerUrls _onlineUrls;
        private readonly SiteFlow _flow;
        private readonly string _localSavePath;
        private readonly SiteProject _downloadToProjectDirectory;

        /// <summary>
        /// Create the request to download a flow from the Tableau REST API
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="flow">List of Tableau Flows to save to disk</param>
        /// <param name="localSavePath">File system location where data sources should be saved</param>
        /// <param name="project"></param>
        public DownloadFlow(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, SiteFlow flow, string localSavePath, SiteProject project)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _flow = flow;
            _localSavePath = localSavePath;
            _downloadToProjectDirectory = project;
        }

        /// <summary>
        /// Execute the REST API call for an individual flow
        /// </summary>
        public List<SiteFlow> ExecuteRequest()
        {
            var statusLog = OnlineSession.StatusLog;
            var downloadedContent = new List<SiteFlow>();

            //Depending on the HTTP download file type we want different file extensions
            var typeMapper = new DownloadPayloadTypeHelper("tfl", "tflx");

            if (_flow == null)
            {
                statusLog.AddError("NULL flow. Aborting download.");
                return null;
            }

            //Local path save the workbook
            string urlDownload = _onlineUrls.Url_FlowDownload(OnlineSession, _flow);
            statusLog.AddStatus("Starting Flow download " + _flow.Name);
            try
            {
                //Generate the directory name we want to download into
                var pathToSaveTo = FileIOHelper.EnsureProjectBasedPath(_localSavePath, _downloadToProjectDirectory, this.StatusLog);

                var fileDownloaded = this.DownloadFile(urlDownload, pathToSaveTo, _flow.Name, typeMapper);
                var fileDownloadedNoPath = System.IO.Path.GetFileName(fileDownloaded);
                statusLog.AddStatus("Finished Flow download " + fileDownloadedNoPath);

                //Add to the list of our downloaded flows
                if (!string.IsNullOrEmpty(fileDownloaded))
                {
                    downloadedContent.Add(_flow);
                }
                else
                {
                    //We should never hit this code; just being defensive
                    statusLog.AddError("Download error, no local file path for downloaded content");
                }
            }
            catch (Exception ex)
            {
                statusLog.AddError($"Error during Flow download '{_flow.Name}': {urlDownload}", ex);
            }

            //Return the set of successfully downloaded content
            return downloadedContent;
        }
    }
}