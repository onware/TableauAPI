using System;
using System.Collections.Generic;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Get the list of a user's groups in a Tableau Server site.
    /// </summary>
    public class DownloadGroupsForUserList : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _userId;

        private List<SiteGroup> _groups;

        /// <summary>
        /// Groups we've parsed from server results
        /// </summary>
        public IEnumerable<SiteGroup> Groups
        {
            get
            {
                var ds = _groups;
                return ds?.AsReadOnly();
            }
        }

        /// <summary>
        /// Constructor: Call when we want to query the groups on behalf of the currently logged in user
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="login"></param>
        public DownloadGroupsForUserList(TableauServerUrls onlineUrls, TableauServerSignIn login)
            : base(login)
        {
            _onlineUrls = onlineUrls;
            _userId = login.UserId;
        }

        /// <summary>
        /// Constructor: Call when we want to query the groups on behalf of an explicitly specified user
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="login"></param>
        /// <param name="userId"></param>
        public DownloadGroupsForUserList(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, string userId) : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _userId = userId;
        }

        /// <summary>
        /// Generate the URL we use to request the list of a user's groups in the site
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        /// <summary>
        /// Execute request for User's groups listing
        /// </summary>
        public void ExecuteRequest()
        {
            //Sanity check
            if (string.IsNullOrWhiteSpace(_userId))
            {
                OnlineSession.StatusLog.AddError("User ID required to query a user's groups");
            }

            var onlineGroups = new List<SiteGroup>();
            int numberPages = 1; //Start with 1 page (we will get an updated value from server)
            //Get subsequent pages
            for (int thisPage = 1; thisPage <= numberPages; thisPage++)
            {
                try
                {
                    _ExecuteRequest_ForPage(onlineGroups, thisPage, out numberPages);
                }
                catch (Exception exPageRequest)
                {
                    StatusLog.AddError("User groups error during page request: " + exPageRequest.Message);
                }
            }

            _groups = onlineGroups;
        }

        #region Private methods

        /// <summary>
        /// Get a page's worth of a User's group listings
        /// </summary>
        /// <param name="onlineGroups"></param>
        /// <param name="pageToRequest">Page # we are requesting (1 based)</param>
        /// <param name="totalNumberPages">Total # of pages of data that Server can return us</param>
        private void _ExecuteRequest_ForPage(List<SiteGroup> onlineGroups, int pageToRequest, out int totalNumberPages)
        {
            int pageSize = _onlineUrls.PageSize;
            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = _onlineUrls.Url_GroupsListForUser(OnlineSession, _userId, pageSize, pageToRequest);
            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            OnlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebResponseLogErrors(webRequest, "get groups list");
            var xmlDoc = GetWebResponseAsXml(response);

            //Get all the group nodes
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
            var groups = xmlDoc.SelectNodes("//iwsOnline:group", nsManager);

            //Get information for each of the data sources
            foreach (XmlNode itemXml in groups)
            {
                try
                {
                    var ds = new SiteGroup(itemXml, new List<SiteUser>());
                    onlineGroups.Add(ds);
                }
                catch
                {
                    AppDiagnostics.Assert(false, "Group parse error");
                    OnlineSession.StatusLog.AddError("Error parsing Group: " + itemXml.InnerXml);
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

