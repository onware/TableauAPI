using System.Collections.Generic;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Downloads the definitions of the data sources embedded inside a workbook
    /// </summary>
    public class DownloadWorkbookConnections : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _workbookId;
        private List<SiteConnection> _connections;
        
        /// <summary>
        /// Workbooks we've parsed from server results
        /// </summary>
        public ICollection<SiteConnection> Connections
        {
            get
            {
                var connections = _connections;
                return connections?.AsReadOnly();
            }
        }

        /// <summary>
        /// Constructor: Call when we want to query the workbooks on behalf of the currently logged in user
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="workbookId">Workbook ID</param>
        public DownloadWorkbookConnections(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, string workbookId)
            : base(logInInfo)
        {
            _workbookId = workbookId;
            _onlineUrls = onlineUrls;
        }


        /// <summary>
        /// Execute request for Workbook connections.
        /// </summary>
        public void ExecuteRequest()
        {
            var wbConnections = new List<SiteConnection>();

            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = _onlineUrls.Url_WorkbookConnectionsList(OnlineSession, _workbookId);
            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            OnlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebResponseLogErrors(webRequest, "get workbook's connections list");
            var xmlDoc = GetWebResponseAsXml(response);

            //Get all the workbook nodes
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
            var connections = xmlDoc.SelectNodes("//iwsOnline:connection", nsManager);

            //Get information for each of the data sources
            foreach (XmlNode itemXml in connections)
            {
                try
                {
                    var connection = new SiteConnection(itemXml);
                    wbConnections.Add(connection);
                }
                catch
                {
                    AppDiagnostics.Assert(false, "Workbook  connections parse error");
                    OnlineSession.StatusLog.AddError("Error parsing workbook: " + itemXml.InnerXml);
                }
            } //end: foreach

            _connections = wbConnections;
        }
    }
}
