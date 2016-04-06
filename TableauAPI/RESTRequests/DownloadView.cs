using System;
using System.Net;
using TableauAPI.RESTHelpers;

namespace TableauAPI.RESTRequests
{
    public class DownloadView : TableauServerSignedInRequestBase
    {

        /// <summary>
        /// URL manager
        /// </summary>
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _userId;

        public DownloadView(TableauServerUrls onlineUrls, TableauServerSignIn login) : base(login)
        {
            _onlineUrls = onlineUrls;
            _userId = login.UserId;
        }

        public string Ticket(string userName)
        {
            var value = string.Empty;
            using (var webClient = new WebClient())
            {

                // Get the Trusted Ticket
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded;charset=UTF-8");

                value = webClient.UploadString(
                    string.Format("{0}/trusted", _onlineUrls.ServerUrlWithProtocol),
                    string.Format("username={0}&target_site={1}", userName, _onlineUrls.SiteUrlSegement));
            }
            return value == "-1" ? string.Empty : value;
        }

        
    }
}
