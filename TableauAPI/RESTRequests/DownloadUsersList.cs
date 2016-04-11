using TableauAPI.RESTHelpers;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Get's the list of users in a Tableau Server site.
    /// </summary>
    /// <remark>
    /// This derives from a base class because "Getting the set of users on the site" and "Getting the set of users in a group" are very similar 
    /// and can share most of the code
    /// </remark>
    public class DownloadUsersList : DownloadUsersListBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="login"></param>
        public DownloadUsersList(TableauServerUrls onlineUrls, TableauServerSignIn login) 
            : base(onlineUrls, login)
        {
        }
        
        /// <summary>
        /// Generate the URL we use to request the list of users in the site
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        protected override string  UrlForUsersListRequest(int pageSize, int pageNumber)
        {
            //The URL to get us the data
            return _onlineUrls.Url_UsersList(OnlineSession, pageSize, pageNumber);
        }

    }
}
