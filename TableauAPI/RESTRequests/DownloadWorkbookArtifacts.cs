using System.IO;
using TableauAPI.RESTHelpers;


namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Download a Workbook and associated artifacts such as its PDF.
    /// </summary>
    public class DownloadWorkbookArtifacts : TableauServerSignedInRequestBase
    {

        private readonly TableauServerUrls _onlineUrls;

        /// <summary>
        /// Create a Workbook request for the Tableau REST API
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        public DownloadWorkbookArtifacts(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo) : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
        }

        /// <summary>
        /// Returns a PDF for a workbook
        /// </summary>
        /// <param name="workbookId"></param>
        /// <param name="pageType"></param>
        /// <param name="pageOrientation"></param>
        /// <returns></returns>
        public byte[] GetPDF(string workbookId, PageType pageType = PageType.Letter, PageOrientation pageOrientation = PageOrientation.Portrait)
        {
            var url = _onlineUrls.Url_DownloadWorkbookPDF(OnlineSession, workbookId, pageType, pageOrientation);
            var webRequest = CreateLoggedInWebRequest(url);
            webRequest.Method = "GET";
            var response = GetWebResponseLogErrors(webRequest, "get workbook pdf");
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