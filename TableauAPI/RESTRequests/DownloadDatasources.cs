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
    public class DownloadDatasources : TableauServerSignedInRequestBase
    {

        private readonly TableauServerUrls _onlineUrls;
        private readonly IEnumerable<SiteDatasource> _datasources;
        private readonly string _localSavePath;
        private readonly IProjectsList _downloadToProjectDirectories;

        /// <summary>
        /// Create the request for to download Data sources from the Tableau REST API
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="datasources">List of Tableau Data sources to save to disk</param>
        /// <param name="localSavePath">File system location where data sources should be saved</param>
        /// <param name="projectsList">List of projects for which we should pull data sources from</param>
        public DownloadDatasources(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, IEnumerable<SiteDatasource> datasources, string localSavePath, IProjectsList projectsList)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _datasources = datasources;
            _localSavePath = localSavePath;
            _downloadToProjectDirectories = projectsList;
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

            var datasources = _datasources;
            if (datasources == null)
            {
                statusLog.AddError("NULL datasources. Aborting download.");
                return null;
            }

            //For each datasource, download it and save it to the local file system
            foreach (var dsInfo in datasources)
            {
                //Local path save the workbook
                string urlDownload = _onlineUrls.Url_DatasourceDownload(OnlineSession, dsInfo);
                statusLog.AddStatus("Starting Datasource download " + dsInfo.Name);
                try
                {
                    //Generate the directory name we want to download into
                    var pathToSaveTo = FileIOHelper.EnsureProjectBasedPath(
                        _localSavePath,
                        _downloadToProjectDirectories,
                        dsInfo,
                        this.StatusLog);

                    var fileDownloaded = this.DownloadFile(urlDownload, pathToSaveTo, dsInfo.Name, typeMapper);
                    var fileDownloadedNoPath = System.IO.Path.GetFileName(fileDownloaded);
                    statusLog.AddStatus("Finished Datasource download " + fileDownloadedNoPath);

                    //Add to the list of our downloaded data sources
                    if (!string.IsNullOrEmpty(fileDownloaded))
                    {
                        downloadedContent.Add(dsInfo);
                    }
                    else
                    {
                        //We should never hit this code; just being defensive
                        statusLog.AddError("Download error, no local file path for downloaded content");
                    }
                }
                catch (Exception ex)
                {
                    statusLog.AddError("Error during Datasource download " + dsInfo.Name + "\r\n  " + urlDownload + "\r\n  " + ex.ToString());
                }
            } //foreach


            //Return the set of successfully downloaded content
            return downloadedContent;
        }
    }
}
