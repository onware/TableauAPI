using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    public class Favorites : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;

        /// <summary>
        /// Work with a user's favorites
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        public Favorites(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
        }

        /// <summary>
        /// Adds the specified workbook to a user's favorites.
        /// </summary>
        /// <param name="favoriteLabel">A label to assign to the favorite. This value is displayed when you search for favorites on the server. If the label is already in use for another workbook, an error is returned.</param>
        /// <param name="workbookId">The ID (not name) of the workbook to add as a favorite.</param>
        /// <returns></returns>
        public void AddWorkbookToFavorites(string favoriteLabel, string workbookId)
        {
            var url = _onlineUrls.Url_AddWorkbookToFavorites(OnlineSession);

            var webRequest = CreateLoggedInWebRequest(url);

            webRequest.Method = "PUT";

            var response = GetWebResponseLogErrors(webRequest, "add workbook to favorites");            
        }

        /// <summary>
        /// Deletes a workbook from a user's favorites. If the specified workbook is not a favorite of the specified user, this call has no effect.
        /// </summary>
        /// <param name="workbookId">The ID of the workbook to remove from the user's favorites.</param>
        /// <returns></returns>
        public void DeleteWorkbookFromFavorites(string workbookId)
        {
            var url = _onlineUrls.Url_DeleteWorkbookFromFavorites(workbookId, OnlineSession);

            var webRequest = CreateLoggedInWebRequest(url);

            webRequest.Method = "DELETE";

            var response = GetWebResponseLogErrors(webRequest, "delete workbook from favorites");
        }

        /// <summary>
        /// Returns a list of favorite projects, data sources, views, workbooks, and flows for a user.
        /// </summary>
        /// <param name="workbookId">The ID of the workbook to remove from the user's favorites.</param>
        /// <returns></returns>
        public void GetFavoritesForUser(string workbookId)
        {
            var url = _onlineUrls.Url_GetFavoritesForUser(OnlineSession);

            var webRequest = CreateLoggedInWebRequest(url);

            webRequest.Method = "GET";

            var response = GetWebResponseLogErrors(webRequest, "get favorites for user");
        }


    }
}
