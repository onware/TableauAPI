using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    public class DownloadViewsForSiteList : TableauServerSignedInRequestBase
    {

        /// <summary>
        /// URL manager
        /// </summary>
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _userId;

        private List<SiteView> _views;
        public ICollection<SiteView> Views
        {
            get
            {
                var views = _views;
                if (views == null) return null;
                return views.AsReadOnly();
            }
        }

        public DownloadViewsForSiteList(TableauServerUrls onlineUrls, TableauServerSignIn login) : base(login)
        {
            _onlineUrls = onlineUrls;
            _userId = login.UserId;
        }

        public DownloadViewsForSiteList(TableauServerUrls onlineUrls, TableauServerSignIn login, string userId) : base(login)
        {
            _onlineUrls = onlineUrls;
            _userId = userId;
        }

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
    }
}
