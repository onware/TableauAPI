using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableauAPI.RESTHelpers;

namespace TableauAPI.RESTRequests
{
    public class DownloadView : TableauServerSignedInRequestBase
    {

        private readonly TableauServerUrls _onlineUrls;
        public DownloadView(TableauServerUrls onlineUrls, TableauServerSignIn login) : base(login)
        {
            _onlineUrls = onlineUrls;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workbookId"></param>
        /// <param name="viewId"></param>
        /// <returns></returns>
        public byte[] GetThumbnail(string workbookId, string viewId)
        {
            var url = _onlineUrls.Url_ViewThumbnail(workbookId, viewId, OnlineSession);
            var webRequest = CreateLoggedInWebRequest(url);
            webRequest.Method = "GET";
            var response = GetWebResponseLogErrors(webRequest, "get view thumbnail");
            byte[] thumbnail;
            using (var stream = response.GetResponseStream())
            {

                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    thumbnail = ms.ToArray();
                }
            }
            return thumbnail;
        }
    }
}
