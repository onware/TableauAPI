using System.IO;
using TableauAPI.RESTHelpers;
using TableauAPI.RESTRequests;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Download a Tableau View and associated artifacts such as Preview Images.
    /// </summary>
    public class DownloadView : TableauServerSignedInRequestBase
    {

        private readonly TableauServerUrls _onlineUrls;

        /// <summary>
        /// Create a View request for the Tableau REST API
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        public DownloadView(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo) : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
        }

        /// <summary>
        /// Return a Preview Image for a View
        /// </summary>
        /// <param name="workbookId"></param>
        /// <param name="viewId"></param>
        /// <returns></returns>
        public byte[] GetPreviewImage(string workbookId, string viewId)
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

        /// <summary>
        /// Return an Image for a View
        /// </summary>
        /// <param name="workbookId"></param>
        /// <param name="viewId"></param>
        /// <returns></returns>
        public byte[] GetImage(string workbookId, string viewId, string filterName="", string filterValue="")
        {
            var url = _onlineUrls.Url_ViewImage(workbookId, viewId, filterName, filterValue, OnlineSession);
            var webRequest = CreateLoggedInWebRequest(url);
            webRequest.Method = "GET";
            var response = GetWebResponseLogErrors(webRequest, "get view image");
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

        /// <summary>
        /// Return data for a View (CSV format)
        /// </summary>
        /// <param name="viewId"></param>
        /// <returns></returns>
        public string GetData(string viewId, string filterName = "", string filterValue="")
        {
            var url = _onlineUrls.Url_ViewData(viewId, filterName, filterValue, OnlineSession);
            var webRequest = CreateLoggedInWebRequest(url);
            webRequest.Method = "GET";
            var response = GetWebResponseLogErrors(webRequest, "get view data");
            byte[] data;
            using (var stream = response.GetResponseStream())
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    data = ms.ToArray();
                }
            }
            return System.Text.Encoding.UTF8.GetString(data);
        }


        /// <summary>
        /// Return PDF for a View 
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="pageType"></param>
        /// <param name="pageOrientation"></param>
        /// <returns></returns>
        public byte[] GetPDF(string viewId, PageType pageType = PageType.Letter, PageOrientation pageOrientation = PageOrientation.Portrait)
        {
            var url = _onlineUrls.Url_DownloadViewPDF(OnlineSession, viewId, pageType, pageOrientation);
            var webRequest = CreateLoggedInWebRequest(url);
            webRequest.Method = "GET";
            var response = GetWebResponseLogErrors(webRequest, "get view pdf");
            byte[] pdf;
            using (var stream = response.GetResponseStream())
            {

                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    pdf = ms.ToArray();
                }
            }
            return pdf;
        }
    }
}
