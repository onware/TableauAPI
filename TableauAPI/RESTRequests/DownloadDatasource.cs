using System;
using System.Collections.Generic;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// 
    /// </summary>
    public class DownloadDatasource : TableauServerSignedInRequestBase
    {

        private readonly TableauServerUrls _onlineUrls;
        private readonly SiteDatasource _datasource;
        private readonly string _localSavePath;
        private readonly SiteProject _downloadToProjectDirectory;

        /// <summary>
        /// Create the request for to download Data sources from the Tableau REST API
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="datasource">List of Tableau Data sources to save to disk</param>
        /// <param name="localSavePath">File system location where data sources should be saved</param>
        /// <param name="project"></param>
        public DownloadDatasource(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, SiteDatasource datasource, string localSavePath, SiteProject project)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _datasource = datasource;
            _localSavePath = localSavePath;
            _downloadToProjectDirectory = project;
        }

        /// <summary>
        /// Execute the REST API call for a list of data sources
        /// </summary>
        public List<SiteDatasource> ExecuteRequest()
        {
            var statusLog = OnlineSession.StatusLog;
            var downloadedContent = new List<SiteDatasource>();

            //Depending on the HTTP download file type we want different file extensions
            var typeMapper = new DownloadPayloadTypeHelper("tdsx", "tds");

            if (_datasource == null)
            {
                statusLog.AddError("NULL datasource. Aborting download.");
                return null;
            }

            //Local path save the workbook
            string urlDownload = _onlineUrls.Url_DatasourceDownload(OnlineSession, _datasource);
            statusLog.AddStatus("Starting Datasource download " + _datasource.Name);
            try
            {
                //Generate the directory name we want to download into
                var pathToSaveTo = FileIOHelper.EnsureProjectBasedPath(_localSavePath, _downloadToProjectDirectory, this.StatusLog);

                var fileDownloaded = this.DownloadFile(urlDownload, pathToSaveTo, _datasource.Name, typeMapper);
                var fileDownloadedNoPath = System.IO.Path.GetFileName(fileDownloaded);
                statusLog.AddStatus("Finished Datasource download " + fileDownloadedNoPath);

                //Add to the list of our downloaded data sources
                if (!string.IsNullOrEmpty(fileDownloaded))
                {
                    downloadedContent.Add(_datasource);
                }
                else
                {
                    //We should never hit this code; just being defensive
                    statusLog.AddError("Download error, no local file path for downloaded content");
                }
            }
            catch (Exception ex)
            {
                statusLog.AddError("Error during Datasource download " + _datasource.Name + "\r\n  " + urlDownload + "\r\n  " + ex.ToString());
            }

            //Return the set of successfully downloaded content
            return downloadedContent;
        }
    }
}