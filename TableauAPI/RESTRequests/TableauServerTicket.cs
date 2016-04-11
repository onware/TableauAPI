using System.Net;
using TableauAPI.RESTHelpers;

namespace TableauAPI.RESTRequests
{

    /// <summary>
    /// Retrieves a Tableau Server Trusted Authentication Ticket.
    /// </summary>
    public class TableauServerTicket : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _userName;

        /// <summary>
        /// Creates an instance of the Tableau Server Ticket API helper.
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="login"></param>
        public TableauServerTicket(TableauServerUrls onlineUrls, TableauServerSignIn login) : base(login)
        {
            _onlineUrls = onlineUrls;
            _userName = login.UserName;
        }

        /// <summary>
        /// Return a one-time use Ticket to access Tableau server without the end user needing to authenticate.
        /// </summary>
        /// <returns></returns>
        public string Ticket()
        {
            var value = string.Empty;
            using (var webClient = new WebClient())
            {

                // Get the Trusted Ticket
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded;charset=UTF-8");

                value = webClient.UploadString(
                    string.Format("{0}/trusted", _onlineUrls.ServerUrlWithProtocol),
                    string.Format("username={0}&target_site={1}", _userName, _onlineUrls.SiteUrlSegement));
            }
            return value;
        }
    }
}
