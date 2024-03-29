﻿using System;
using System.Collections.Generic;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Get's the list of users in a Tableau Server site
    /// </summary>
    public abstract class DownloadUsersListBase : TableauServerSignedInRequestBase
    {
        private List<SiteUser> _users;
        
        /// <summary>
        /// Tableau server information
        /// </summary>
        protected readonly TableauServerUrls _onlineUrls;
        
        /// <summary>
        /// Assist with the build out of an implementing object.
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="login"></param>
        public DownloadUsersListBase(TableauServerUrls onlineUrls, TableauServerSignIn login)
            : base(login)
        {
            _onlineUrls = onlineUrls;
        }
        
        /// <summary>
        /// Derrived classes must implement.  This will be the URL we call to request the list of users. 
        /// </summary>
        /// <param name="pageSize">The maximum number of results we want the server to return in a single request</param>
        /// <param name="pageNumber">The current page number of results we want the server to return us data for</param>
        /// <returns></returns>
        protected abstract string  UrlForUsersListRequest(int pageSize, int pageNumber);
        
        /// <summary>
        /// Users we've parsed from server results
        /// </summary>
        public IEnumerable<SiteUser> Users => _users.AsReadOnly();


        /// <summary>
        /// Look for the user with the matching email address
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public SiteUser FindUserByEmail(string email)
        {
            email = email.ToLower();
            foreach(var user in _users)
            {
                if(user.Name.ToLower() == email)
                {
                    return user;
                }
            }

            return null;
        }


        /// <summary>
        /// Execute request for Users
        /// </summary>
        public void ExecuteRequest()
        {
            var onlineUser = new List<SiteUser>();
            int numberPages = 1; //Start with 1 page (we will get an updated value from server)
            //Get subsequent pages
            for (int thisPage = 1; thisPage <= numberPages; thisPage++)
            {
                try
                {
                    _ExecuteRequest_ForPage(onlineUser, thisPage, out numberPages);
                }
                catch (Exception exPageRequest)
                {
                    StatusLog.AddError("Users list error during page request", exPageRequest);
                }
            }

            _users = onlineUser;
        }

        #region Private Methods

        /// <summary>
        /// Get a page's worth of Users listings
        /// </summary>
        /// <param name="onlineUsers"></param>
        /// <param name="pageToRequest">Page # we are requesting (1 based)</param>
        /// <param name="totalNumberPages">Total # of pages of data that Server can return us</param>
        private void _ExecuteRequest_ForPage(List<SiteUser> onlineUsers, int pageToRequest, out int totalNumberPages)
        {
            int pageSize = _onlineUrls.PageSize;
            var urlQuery = UrlForUsersListRequest(pageSize, pageToRequest);

            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            OnlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebResponseLogErrors(webRequest, "get users list");
            var xmlDoc = GetWebResponseAsXml(response);

            //Get all the user nodes
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
            var users = xmlDoc.SelectNodes("//iwsOnline:user", nsManager);

            //Get information for each of the data sources
            foreach (XmlNode itemXml in users)
            {
                try
                {
                    var ds = new SiteUser(itemXml);
                    onlineUsers.Add(ds);
                }
                catch
                {
                    OnlineSession.StatusLog.AddError("Error parsing user " + itemXml.InnerXml);
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
