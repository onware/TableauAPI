using System;
using System.Collections.Generic;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Request to download views for a workbook
    /// </summary>
    public class DownloadViewsForWorkbookList : TableauServerSignedInRequestBase
    {

        private readonly TableauServerUrls _onlineUrls;
        private readonly string _userId;
        private readonly string _workbookId;
        private List<SiteView> _views;

        /// <summary>
        /// Returns list of Views for the workbook
        /// </summary>
        public ICollection<SiteView> Views
        {
            get
            {
                var views = _views;
                return views?.AsReadOnly();
            }
        }

        /// <summary>
        /// Create a request to retrieve Views for a Workbook from the Tableau REST API on behalf of the current user
        /// </summary>
        /// <param name="workbookId">Workbook ID</param>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        public DownloadViewsForWorkbookList(string workbookId, TableauServerUrls onlineUrls, TableauServerSignIn logInInfo) : base(logInInfo)
        {
            _workbookId = workbookId;
            _onlineUrls = onlineUrls;
            _userId = logInInfo.UserId;
        }

        /// <summary>
        /// Create a request to retrieve Views for a Workbook from the Tableau REST API on behalf of a given user
        /// </summary>
        /// <param name="workbookId">Workbook ID</param>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="userId">User ID of user whom we should get Views for</param>
        public DownloadViewsForWorkbookList(string workbookId, TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, string userId) : base(logInInfo)
        {
            _workbookId = workbookId;
            _onlineUrls = onlineUrls;
            _userId = userId;
        }

        /// <summary>
        /// Execute request for Workbook Views
        /// </summary>
        public void ExecuteRequest()
        {
            if (string.IsNullOrWhiteSpace(_userId))
            {
                OnlineSession.StatusLog.AddError("User ID required to query workbooks");
            }

            try
            {
                var urlQuery = _onlineUrls.Url_ViewsListForWorkbook(_workbookId, OnlineSession);
                var webRequest = CreateLoggedInWebRequest(urlQuery);
                webRequest.Method = "GET";
                OnlineSession.StatusLog.AddStatus($"Web request: {urlQuery}", -10);
                var response = GetWebResponseLogErrors(webRequest, "get views list");
                var xmlDoc = GetWebResponseAsXml(response);

                var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
                var workbooks = xmlDoc.SelectNodes("//iwsOnline:view", nsManager);
                _views = new List<SiteView>();
                foreach (XmlNode itemXml in workbooks)
                {
                    try
                    {
                        var ds = new SiteView(itemXml);
                        _views.Add(ds);
                    }
                    catch
                    {
                        AppDiagnostics.Assert(false, "View parse error");
                        OnlineSession.StatusLog.AddError("Error parsing view: " + itemXml.InnerXml);
                    }
                }
            }
            catch (Exception exPageRequest)
            {
                StatusLog.AddError($"Workbooks error during page request: {exPageRequest.Message}");
            }
        }

    }
}
