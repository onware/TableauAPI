using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Obtain specific workbook
    /// </summary>
    public class ObtainWorkbook : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _workbookId;

        private SiteWorkbook _workbook;
        /// <summary>
        /// Workbooks we've parsed from server results
        /// </summary>
        public SiteWorkbook Workbook
        {
            get
            {
                return _workbook;
            }
        }

        /// <summary>
        /// Constructor: Call when we want to query the Workbooks on behalf of an explicitly specified user
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="workbookId"></param>
        public ObtainWorkbook(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, string workbookId)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _workbookId = workbookId;
        }

        /// <summary>
        /// Get a workbook
        /// </summary>
        public void ExecuteRequest()
        {
            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = _onlineUrls.Url_Workbook(OnlineSession, _workbookId);
            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            OnlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebResponseLogErrors(webRequest, "get workbooks list");
            var xmlDoc = GetWebResponseAsXml(response);

            //Get all the workbook nodes
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
            var workbooks = xmlDoc.SelectNodes("//iwsOnline:workbook", nsManager);

            //Get information for each of the data sources
            foreach (XmlNode itemXml in workbooks)
            {
                try
                {
                    _workbook = new SiteWorkbook(itemXml);
                }
                catch
                {
                    AppDiagnostics.Assert(false, "Workbook parse error");
                    OnlineSession.StatusLog.AddError("Error parsing workbook: " + itemXml.InnerXml);
                }
            } //end: foreach
        }
    }
}