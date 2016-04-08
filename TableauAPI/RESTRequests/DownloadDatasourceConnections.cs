using System.Collections.Generic;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Downloads the data connection in a published data source
    /// </summary>
    public class DownloadDatasourceConnections : TableauServerSignedInRequestBase
    {
        /// <summary>
        /// URL manager
        /// </summary>
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _datasourceId;

        /// <summary>
        /// Workbooks we've parsed from server results
        /// </summary>
        private List<SiteConnection> _connections;
        public ICollection<SiteConnection> Connections
        {
            get
            {
                var connections = _connections;
                return connections?.AsReadOnly();
            }
        }

        /// <summary>
        /// Constructor: Call when we want to query the datasource on behalf of the currently logged in user
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="login"></param>
        /// <param name="datasourceId"></param>
        public DownloadDatasourceConnections(TableauServerUrls onlineUrls, TableauServerSignIn login, string datasourceId)
            : base(login)
        {
            _datasourceId = datasourceId;
            _onlineUrls = onlineUrls;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        public void ExecuteRequest()
        {
            var dsConnections = new List<SiteConnection>();

            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = _onlineUrls.Url_DatasourceConnectionsList(OnlineSession, _datasourceId);
            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            OnlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebResponseLogErrors(webRequest, "get datasources's connections list");
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
                    dsConnections.Add(connection);
                }
                catch
                {
                    AppDiagnostics.Assert(false, "Workbook  connections parse error");
                    OnlineSession.StatusLog.AddError("Error parsing workbook: " + itemXml.InnerXml);
                }
            } //end: foreach

            _connections = dsConnections;
        }
    }
}
