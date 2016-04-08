using System.Net;
using TableauAPI.RESTHelpers;

namespace TableauAPI.RESTRequests
{
    public class TableauServerTicket : TableauServerSignedInRequestBase
    {
        /// <summary>
        /// URL manager
        /// </summary>
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _userName;



        public TableauServerTicket(TableauServerUrls onlineUrls, TableauServerSignIn login) : base(login)
        {
            _onlineUrls = onlineUrls;
            _userName = login.UserName;
        }

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
