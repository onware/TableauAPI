using System;
using System.Collections.Generic;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Downloads the list of Workbooks from the server
    /// </summary>
    public class DownloadWorkbooksList : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _userId;

        private List<SiteWorkbook> _workbooks;
        /// <summary>
        /// Workbooks we've parsed from server results
        /// </summary>
        public ICollection<SiteWorkbook> Workbooks
        {
            get
            {
                var wb = _workbooks;
                return wb?.AsReadOnly();
            }
        }

        /// <summary>
        /// Constructor: Call when we want to query the workbooks on behalf of the currently logged in user
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        public DownloadWorkbooksList(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _userId = logInInfo.UserId;
        }

        /// <summary>
        /// Constructor: Call when we want to query the Workbooks on behalf of an explicitly specified user
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="userId">User ID of person we are downloading on behalf of</param>
        public DownloadWorkbooksList(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, string userId) : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _userId = userId;
        }

        /// <summary>
        /// Execute request for Workbook listings
        /// </summary>
        public void ExecuteRequest()
        {
            //Sanity check
            if(string.IsNullOrWhiteSpace(_userId))
            {
                OnlineSession.StatusLog.AddError("User ID required to query workbooks");            
            }

            var onlineWorkbooks = new List<SiteWorkbook>();
            int numberPages = 1; //Start with 1 page (we will get an updated value from server)
            //Get subsequent pages
            for (int thisPage = 1; thisPage <= numberPages; thisPage++)
            {
                try
                {
                    _ExecuteRequest_ForPage(onlineWorkbooks, thisPage, out numberPages);
                }
                catch (Exception exPageRequest)
                {
                    StatusLog.AddError("Workbooks error during page request: " + exPageRequest.Message);
                }
            }

            _workbooks = onlineWorkbooks;
        }

        #region Private methods

        /// <summary>
        /// Get a page's worth of Workbook listings
        /// </summary>
        /// <param name="onlineWorkbooks"></param>
        /// <param name="pageToRequest">Page # we are requesting (1 based)</param>
        /// <param name="totalNumberPages">Total # of pages of data that Server can return us</param>
        private void _ExecuteRequest_ForPage(List<SiteWorkbook> onlineWorkbooks, int pageToRequest, out int totalNumberPages)
        {
            int pageSize = _onlineUrls.PageSize;
            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = _onlineUrls.Url_WorkbooksListForUser(OnlineSession, _userId, pageSize, pageToRequest);
            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            OnlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebResponseLogErrors(webRequest, "get workbooks list");
            var xmlDoc = GetWebResponseAsXml(response);

            //Get all the workbook nodes
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
            var workbooks = xmlDoc.SelectNodes("//iwsOnline:workbook", nsManager);

            //Get information for each of the data sources
            foreach (XmlNode itemXml in workbooks)
            {
                try
                {
                    var ds = new SiteWorkbook(itemXml);
                    onlineWorkbooks.Add(ds);
                }
                catch
                {
                    AppDiagnostics.Assert(false, "Workbook parse error");
                    OnlineSession.StatusLog.AddError("Error parsing workbook: " + itemXml.InnerXml);
                }
            } //end: foreach

            //-------------------------------------------------------------------
            //Get the updated page-count
            //-------------------------------------------------------------------
            totalNumberPages = DownloadPaginationHelper.GetNumberOfPagesFromPagination(
                xmlDoc.SelectSingleNode("//iwsOnline:pagination", nsManager),
                pageSize);
        }

        #endregion

    }
}
