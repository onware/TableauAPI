using System;
using System.Collections.Generic;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;
using System.Text;

namespace TableauAPI.RESTRequests
{
    public class FavoriteViews : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _userId;

        private List<SiteView> _views;

        private List<string> _favoriteViewIds;
        private List<string> _favoriteViewWorkbookIds;

        /// <summary>
        /// Favorite views we've parsed from server results
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
        /// Constructor: Call when we want to query favorite views on behalf of the currently logged in user
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        public FavoriteViews(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _userId = logInInfo.UserId;
        }

        /// <summary>
        /// Constructor: Call when we want to query favorite views on behalf of an explicitly specified user
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="userId">Tableau User Id</param>
        public FavoriteViews(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, string userId)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _userId = userId;
        }

        /// <summary>
        /// Adds the specified view to a user's favorites.
        /// </summary>
        /// <param name="favoriteLabel">A label to assign to the favorite. This value is displayed when you search for favorites on the server. If the label is already in use for another view, an error is returned.</param>
        /// <param name="viewId">The ID (not name) of the view to add as a favorite.</param>
        /// <returns></returns>
        public void AddViewToFavorites(string favoriteLabel, string viewId)
        {
            var url = _onlineUrls.Url_AddToFavorites(_userId, OnlineSession);
            var sb = new StringBuilder();
            var xmlSettings = new XmlWriterSettings();
            xmlSettings.OmitXmlDeclaration = true;
            var xmlWriter = XmlWriter.Create(sb, xmlSettings);
            xmlWriter.WriteStartElement("tsRequest");
            xmlWriter.WriteStartElement("favorite");
            xmlWriter.WriteAttributeString("label", favoriteLabel);
            xmlWriter.WriteStartElement("view");
            xmlWriter.WriteAttributeString("id", viewId);
            xmlWriter.WriteEndElement(); // end view element
            xmlWriter.WriteEndElement(); // end favorite element
            xmlWriter.WriteEndElement(); // end tsRequest element
            xmlWriter.Close();

            var xmlText = sb.ToString();

            var webRequest = CreateLoggedInWebRequest(url, "PUT");

            SendRequestContents(webRequest, xmlText, "PUT");

            var response = GetWebResponseLogErrors(webRequest, "add view to favorites");
        }

        /// <summary>
        /// Deletes a view from a user's favorites. If the specified view is not a favorite of the specified user, this call has no effect.
        /// </summary>
        /// <param name="viewId">The ID of the view to remove from the user's favorites.</param>
        /// <returns></returns>
        public void DeleteViewFromFavorites(string viewId)
        {
            var url = _onlineUrls.Url_DeleteViewFromFavorites(viewId, _userId, OnlineSession);

            var webRequest = CreateLoggedInWebRequest(url, "DELETE");

            var response = GetWebResponseLogErrors(webRequest, "delete view from favorites");
        }

        /// <summary>
        /// Get a listing of a user's views that have been favorited.
        /// </summary>
        public List<string> GetFavoriteViewIds()
        {
            if (_views == null || _views.Count <= 0)
            {
                ExecuteRequest();
            }
            return _favoriteViewIds;
        }

        public List<string> GetWorkbookIdsFromFavoriteViews()
        {
            if (_views == null || _views.Count <= 0)
            {
                ExecuteRequest();
            }
            return _favoriteViewWorkbookIds;
        }


        /// <summary>
        /// Execute request for favorite view listings
        /// </summary>
        public void ExecuteRequest()
        {
            //Sanity check
            if (string.IsNullOrWhiteSpace(_userId))
            {
                OnlineSession.StatusLog.AddError("User ID required to query favorite views");
            }

            _views = new List<SiteView>();
            _favoriteViewIds = new List<string>();
            _favoriteViewWorkbookIds = new List<string>();

            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = _onlineUrls.Url_GetFavoritesForUser(_userId, OnlineSession);
            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            OnlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebResponseLogErrors(webRequest, "get favorite views list");
            var xmlDoc = GetWebResponseAsXml(response);

            //Get all the favorite view nodes
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
            var views = xmlDoc.SelectNodes("//iwsOnline:favorite/iwsOnline:view", nsManager);

            //Get information for each of the data sources
            foreach (XmlNode itemXml in views)
            {
                try
                {
                    var ds = new SiteView(itemXml);
                    _favoriteViewIds.Add(ds.Id);
                    _favoriteViewWorkbookIds.Add(ds.WorkbookId);
                    _views.Add(ds);
                }
                catch
                {
                    AppDiagnostics.Assert(false, "Favorite view parse error");
                    OnlineSession.StatusLog.AddError("Error parsing favorite view: " + itemXml.InnerXml);
                }
            } //end: foreach

        }


    }
}

