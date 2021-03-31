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
            var url = _onlineUrls.Url_AddWorkbookToFavorites(_userId, OnlineSession);
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
            //Sanity check
            if (string.IsNullOrWhiteSpace(_userId))
            {
                OnlineSession.StatusLog.AddError("User ID required to query favorite workbooks");
            }

            string urlQuery = _onlineUrls.Url_GetFavoritesForUser(_userId, OnlineSession);

            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            OnlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebResponseLogErrors(webRequest, "get Favorites list");
            var xmlDoc = GetWebResponseAsXml(response);

            //Get all the workbook nodes
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
            var favorites = xmlDoc.SelectNodes("//iwsOnline:favorite/iwsOnline:workbook", nsManager);

            List<string> onlineFavoriteWorkbookIds = new List<string>();

            //Get information for each of the data sources
            foreach (XmlNode itemXml in favorites)
            {
                try
                {
                    var ds = itemXml.Attributes["id"]?.Value;
                    onlineFavoriteWorkbookIds.Add(ds);
                }
                catch
                {
                    AppDiagnostics.Assert(false, "Favorite parse error");
                    OnlineSession.StatusLog.AddError("Error parsing favorite: " + itemXml.InnerXml);
                }
            }

            _favoriteWorkbookIds = onlineFavoriteWorkbookIds;

            return _favoriteWorkbookIds;
        }
    }
}
