using System;
using System.Collections.Generic;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Downloads a set of workbooks from server
    /// </summary>
    public class DownloadWorkbook : TableauServerSignedInRequestBase
    {
        /// <summary>
        /// URL manager
        /// </summary>
        private readonly TableauServerUrls _onlineUrls;

        /// <summary>
        /// Workbooks we've parsed from server results
        /// </summary>
        private readonly SiteWorkbook _workbook;

        /// <summary>
        /// Local path where we are going to save downloaded workbooks to
        /// </summary>
        private readonly string _localSavePath;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="workbook">IEnumerable of SiteWorkbook objects</param>
        /// <param name="localSavePath">Local path where the workbooks should be saved</param>
        public DownloadWorkbook(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, SiteWorkbook workbook, string localSavePath)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _workbook = workbook;
            _localSavePath = localSavePath;
        }

        /// <summary>
        /// Execute request for Workbooks
        /// </summary>
        public ICollection<SiteWorkbook> ExecuteRequest()
        {
            var statusLog = OnlineSession.StatusLog;
            var downloadedContent = new List<SiteWorkbook>();

            if (_workbook == null)
            {
                statusLog.AddError("NULL workbook. Aborting download.");
                return null;
            }

            //Depending on the HTTP download file type we want different file extensions
            var typeMapper = new DownloadPayloadTypeHelper("twbx", "twb");

            //Local path save the workbook
            string urlDownload = _onlineUrls.Url_WorkbookDownload(OnlineSession, _workbook);
            statusLog.AddStatus("Starting Workbook download " + _workbook.Name + " " + _workbook.ToString());
            try
            {
                //Generate the directory name we want to download into
                var pathToSaveTo = FileIOHelper.EnsureProjectBasedPath(_localSavePath, _workbook, StatusLog);

                var fileDownloaded = DownloadFile(urlDownload, pathToSaveTo, _workbook.Name, typeMapper);
                var fileDownloadedNoPath = System.IO.Path.GetFileName(fileDownloaded);
                statusLog.AddStatus("Finished Workbook download " + fileDownloadedNoPath);

                //Add to the list of our downloaded data sources
                if (!string.IsNullOrWhiteSpace(fileDownloaded))
                {
                    downloadedContent.Add(_workbook);
                }
                else
                {
                    //We should never hit this code; just being defensive
                    statusLog.AddError("Download error, no local file path for downloaded content");
                }
            }
            catch (Exception ex)
            {
                statusLog.AddError("Error during Workbook download " + _workbook.Name + "\r\n  " + urlDownload + "\r\n  " + ex.ToString());
            }

            return downloadedContent;
        }

    }
}
