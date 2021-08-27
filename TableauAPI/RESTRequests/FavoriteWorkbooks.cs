using System;
using System.Collections.Generic;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;
using System.Text;

namespace TableauAPI.RESTRequests
{
    public class FavoriteWorkbooks : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _userId;

        private List<string> _favoriteWorkbookIds;

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
        /// Constructor: Call when we want to query favorite workbooks on behalf of the currently logged in user
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        public FavoriteWorkbooks(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _userId = logInInfo.UserId;
        }

        /// <summary>
        /// Constructor: Call when we want to query favorite workbooks on behalf of an explicitly specified user
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="userId">Tableau User Id</param>
        public FavoriteWorkbooks(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, string userId)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _userId = userId;
        }

        /// <summary>
        /// Adds the specified workbook to a user's favorites.
        /// </summary>
        /// <param name="favoriteLabel">A label to assign to the favorite. This value is displayed when you search for favorites on the server. If the label is already in use for another workbook, an error is returned.</param>
        /// <param name="workbookId">The ID (not name) of the workbook to add as a favorite.</param>
        /// <returns></returns>
        public void AddWorkbookToFavorites(string favoriteLabel, string workbookId)
        {
            var url = _onlineUrls.Url_AddToFavorites(_userId, OnlineSession);
            var sb = new StringBuilder();
            var xmlSettings = new XmlWriterSettings();
            xmlSettings.OmitXmlDeclaration = true;
            var xmlWriter = XmlWriter.Create(sb, xmlSettings);
            xmlWriter.WriteStartElement("tsRequest");
            xmlWriter.WriteStartElement("favorite");
            xmlWriter.WriteAttributeString("label", favoriteLabel);
            xmlWriter.WriteStartElement("workbook");
            xmlWriter.WriteAttributeString("id", workbookId);
            xmlWriter.WriteEndElement(); // end workbook element
            xmlWriter.WriteEndElement(); // end favorite element
            xmlWriter.WriteEndElement(); // end tsRequest element
            xmlWriter.Close();

            var xmlText = sb.ToString();

            var webRequest = CreateLoggedInWebRequest(url, "PUT");

            SendRequestContents(webRequest, xmlText, "PUT");

            var response = GetWebResponseLogErrors(webRequest, "add workbook to favorites");
        }

        /// <summary>
        /// Deletes a workbook from a user's favorites. If the specified workbook is not a favorite of the specified user, this call has no effect.
        /// </summary>
        /// <param name="workbookId">The ID of the workbook to remove from the user's favorites.</param>
        /// <returns></returns>
        public void DeleteWorkbookFromFavorites(string workbookId)
        {
            var url = _onlineUrls.Url_DeleteWorkbookFromFavorites(workbookId, _userId, OnlineSession);

            var webRequest = CreateLoggedInWebRequest(url, "DELETE");

            var response = GetWebResponseLogErrors(webRequest, "delete workbook from favorites");
        }

        /// <summary>
        /// Get a listing of a user's workbooks that have been favorited.
        /// </summary>
        public List<string> GetFavoriteWorkbookIds()
        {
            if (_workbooks == null || _workbooks.Count <= 0)
            {
                ExecuteRequest();
            }
            return _favoriteWorkbookIds;
        }

        /// <summary>
        /// Execute request for Favorite workbook listings
        /// </summary>
        public void ExecuteRequest()
        {
            //Sanity check
            if (string.IsNullOrWhiteSpace(_userId))
            {
                OnlineSession.StatusLog.AddError("User ID required to query favorite workbooks");
            }

            _workbooks = new List<SiteWorkbook>();
            _favoriteWorkbookIds = new List<string>();

            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = _onlineUrls.Url_GetFavoritesForUser(_userId, OnlineSession);
            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            OnlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebResponseLogErrors(webRequest, "get Favorites list");
            var xmlDoc = GetWebResponseAsXml(response);

            //Get all the favorite workbook nodes
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
            var workbooks = xmlDoc.SelectNodes("//iwsOnline:favorite/iwsOnline:workbook", nsManager);

            //Get information for each of the data sources
            foreach (XmlNode itemXml in workbooks)
            {
                try
                {
                    var ds = new SiteWorkbook(itemXml);
                    _favoriteWorkbookIds.Add(ds.Id);
                    _workbooks.Add(ds);
                }
                catch
                {
                    AppDiagnostics.Assert(false, "Favorite workbook parse error");
                    OnlineSession.StatusLog.AddError("Error parsing favorite workbook: " + itemXml.InnerXml);
                }
            } //end: foreach
        }

        /// <summary>
        /// Reorders a workbook from a user's favorites. If the specified workbook is not a favorite of the specified user, this call has no effect.
        /// Tableau API Call params
        /// </summary>
        /// <param name="favoriteId">The ID of the workbook to move from the user's favorites.</param>
        /// <param name="favoriteAfterId">The ID of the workbook to which our FavoriteId will be placed after from the user's favorites. If not specified</param>
        /// <param name="contentType">The content type of the favorite. To specify the type, use one of the following values: datasource, workbook, view, project, flow</param>
        /// <returns></returns>
        public void ReorderWorkbookFavorites(string favoriteId, string favoriteAfterId, string contentType)
        {
            var url = _onlineUrls.Url_OrderFavoritesForUser(_userId, OnlineSession);
            var sb = new StringBuilder();
            var xmlSettings = new XmlWriterSettings();
            xmlSettings.OmitXmlDeclaration = true;
            var xmlWriter = XmlWriter.Create(sb, xmlSettings);
            xmlWriter.WriteStartElement("tsRequest");
            xmlWriter.WriteStartElement("favoriteOrderings");
            xmlWriter.WriteStartElement("favoriteOrdering");
            xmlWriter.WriteAttributeString("favoriteId", favoriteId);
            xmlWriter.WriteAttributeString("favoriteType", contentType);
            xmlWriter.WriteAttributeString("favoriteIdMoveAfter", favoriteAfterId);
            xmlWriter.WriteAttributeString("favoriteTypeMoveAfter", contentType);
            xmlWriter.WriteEndElement(); // end favorite ordering element
            xmlWriter.WriteEndElement(); // end favorite orderings element
            xmlWriter.WriteEndElement(); // end tsRequest element
            xmlWriter.Close();

            var xmlText = sb.ToString();

            var webRequest = CreateLoggedInWebRequest(url, "PUT");

            SendRequestContents(webRequest, xmlText, "PUT");

            var response = GetWebResponseLogErrors(webRequest, "reorder workbook favorites");            
        }
    }
}
