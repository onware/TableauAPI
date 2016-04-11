using TableauAPI.RESTHelpers;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Get's the list of users in a Tableau Server site.
    /// </summary>
    /// <remarks>
    /// This derives from a base class because "Getting the set of users on the site" and 
    /// "Getting the set of users in a group" are very similar and can share most of the code.
    /// </remarks>
    public class DownloadUsersListInGroup : DownloadUsersListBase
    {
        private readonly string _groupId;
        
        /// <summary>
        /// Create a request to get a list of Tableau server site Users
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="logInInfo"></param>
        /// <param name="groupId"></param>
        public DownloadUsersListInGroup(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, string groupId) 
            : base(onlineUrls, logInInfo)
        {
            _groupId = groupId;
        }
        
        /// <summary>
        /// Generate the URL we use to request the list of users in the site
        /// </summary>
        /// <param name="pageSize">Size of the page of data to retrieve from the Tableau server</param>
        /// <param name="pageNumber">Page of data to retrieve from the Tableau server</param>
        /// <returns></returns>
        protected override string UrlForUsersListRequest(int pageSize, int pageNumber)
        {
            //The URL to get us the data
            return _onlineUrls.Url_UsersListInGroup(OnlineSession, _groupId, pageSize, pageNumber);
        }

    }
}
