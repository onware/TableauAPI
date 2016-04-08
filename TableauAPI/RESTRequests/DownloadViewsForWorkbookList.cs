using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    public class DownloadViewsForWorkbookList : TableauServerSignedInRequestBase
    {

        /// <summary>
        /// URL manager
        /// </summary>
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _userId;
        private readonly string _workbookId;

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

        public DownloadViewsForWorkbookList(string workbookId, TableauServerUrls onlineUrls, TableauServerSignIn login) : base(login)
        {
            _workbookId = workbookId;
            _onlineUrls = onlineUrls;
            _userId = login.UserId;
        }

        public DownloadViewsForWorkbookList(string workbookId, TableauServerUrls onlineUrls, TableauServerSignIn login, string userId) : base(login)
        {
            _workbookId = workbookId;
            _onlineUrls = onlineUrls;
            _userId = userId;
        }

        public void ExecuteRequest()
        {
            if (string.IsNullOrWhiteSpace(_userId))
            {
                OnlineSession.StatusLog.AddError("User ID required to query workbooks");
            }

            try
            {
                var pageSize = _onlineUrls.PageSize;
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
