using System;
using System.Collections.Generic;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Manages the download of a set of data sources from a Tableau REST API and saves it to disk
    /// </summary>
    public class DownloadFlows : TableauServerSignedInRequestBase
    {

        private readonly TableauServerUrls _onlineUrls;
        private readonly IEnumerable<SiteFlow> _flows;
        private readonly string _localSavePath;
        private readonly IProjectsList _downloadToProjectDirectories;

        /// <summary>
        /// Create the request for to download Data sources from the Tableau REST API
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="flows">List of Tableau Flows to save to disk</param>
        /// <param name="localSavePath">File system location where data sources should be saved</param>
        /// <param name="projectsList">List of projects for which we should pull data sources from</param>
        public DownloadFlows(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, IEnumerable<SiteFlow> flows, string localSavePath, IProjectsList projectsList)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _flows = flows;
            _localSavePath = localSavePath;
            _downloadToProjectDirectories = projectsList;
        }

        /// <summary>
        /// Execute the REST API call for a list of data sources
        /// </summary>
        public List<SiteFlow> ExecuteRequest()
        {
            var statusLog = OnlineSession.StatusLog;
            var downloadedContent = new List<SiteFlow>();

            //Depending on the HTTP download file type we want different file extensions
            var typeMapper = new DownloadPayloadTypeHelper("tfl", "tflx");

            var flows = _flows;
            if (flows == null)
            {
                statusLog.AddError("NULL flows. Aborting download.");
                return null;
            }

            //For each flow, download it and save it to the local file system
            foreach (var fInfo in flows)
            {
                //Local path save the flow
                string urlDownload = _onlineUrls.Url_FlowDownload(OnlineSession, fInfo);
                statusLog.AddStatus("Starting Flow download " + fInfo.Name);
                try
                {
                    //Generate the directory name we want to download into
                    var pathToSaveTo = FileIOHelper.EnsureProjectBasedPath(
                        _localSavePath,
                        _downloadToProjectDirectories,
                        fInfo,
                        this.StatusLog);

                    var fileDownloaded = this.DownloadFile(urlDownload, pathToSaveTo, fInfo.Name, typeMapper);
                    var fileDownloadedNoPath = System.IO.Path.GetFileName(fileDownloaded);
                    statusLog.AddStatus("Finished Datasource download " + fileDownloadedNoPath);

                    //Add to the list of our downloaded flows
                    if (!string.IsNullOrEmpty(fileDownloaded))
                    {
                        downloadedContent.Add(fInfo);
                    }
                    else
                    {
                        //We should never hit this code; just being defensive
                        statusLog.AddError("Download error, no local file path for downloaded content");
                    }
                }
                catch (Exception ex)
                {
                    statusLog.AddError("Error during Datasource download " + fInfo.Name + "\r\n  " + urlDownload + "\r\n  " + ex.ToString());
                }
            } //foreach


            //Return the set of successfully downloaded content
            return downloadedContent;
        }
    }
}
