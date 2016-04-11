using System;
using System.Collections.Generic;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Request for Views on a Site from the Tableau REST API
    /// </summary>
    public class DownloadViewsForSiteList : TableauServerSignedInRequestBase
    {

        private readonly TableauServerUrls _onlineUrls;
        private readonly string _userId;
        private List<SiteView> _views;

        /// <summary>
        /// List of Views from a Tableau site
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
        /// Create a request to download Views for a site.
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        public DownloadViewsForSiteList(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo) : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _userId = logInInfo.UserId;
        }

        /// <summary>
        /// Create a request to download Views for a site on behalf of a given user.
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="userId">User ID whom we should retrieve site Views for</param>
        public DownloadViewsForSiteList(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, string userId) : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _userId = userId;
        }

        /// <summary>
        /// Execute the request for Site Views
        /// </summary>
        public void ExecuteRequest()
        {
            if (string.IsNullOrWhiteSpace(_userId))
            {
                OnlineSession.StatusLog.AddError("User ID required to query workbooks");
            }

            int numberPages = 1; //Start with 1 page (we will get an updated value from server)
            //Get subsequent pages
            _views = new List<SiteView>();
            for (int thisPage = 1; thisPage <= numberPages; thisPage++)
            {
                try
                {
                    _ExecuteRequest_ForPage(_views, thisPage, out numberPages);
                }
                catch (Exception exPageRequest)
                {
                    StatusLog.AddError($"Workbooks error during page request: {exPageRequest.Message}");
                }
            }
        }

        #region Private Methods

        private void _ExecuteRequest_ForPage(List<SiteView> onlineViews, int pageToRequest, out int totalNumberPages)
        {
            var pageSize = _onlineUrls.PageSize;
            var urlQuery = _onlineUrls.Url_ViewsListForSite(OnlineSession, pageSize, 1);
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
            totalNumberPages = DownloadPaginationHelper.GetNumberOfPagesFromPagination(
                xmlDoc.SelectSingleNode("//iwsOnline:pagination", nsManager),
                pageSize);
        }

        #endregion

    }
}
