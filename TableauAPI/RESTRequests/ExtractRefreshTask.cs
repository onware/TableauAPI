using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;
using System.Collections.Generic;

namespace TableauAPI.RESTRequests
{
    public class ExtractRefreshTask : TableauServerSignedInRequestBase
    {

        private readonly TableauServerUrls _onlineUrls;
        private List<ServerData.ExtractRefreshTask> _extractRefreshTasks;

        public ICollection<ServerData.ExtractRefreshTask> extractRefreshTasks
        {
            get {
                var e = _extractRefreshTasks;
                return e?.AsReadOnly();
            }
        }
        public ExtractRefreshTask(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo) : base (logInInfo)
        {
            _onlineUrls = onlineUrls;
        }


        public void GetExtractRefreshTasks() {
            var statusLog = OnlineSession.StatusLog;

            //Create a web request, in including the users logged-in auth information in the request headers
            var urlRequest = _onlineUrls.Url_ExtractRefreshTaskBySiteId(OnlineSession);
            var webRequest = CreateLoggedInWebRequest(urlRequest);
            webRequest.Method = "GET";

            //Request the data from server
            OnlineSession.StatusLog.AddStatus("Web request: " + urlRequest, -10);
            var response = GetWebResponseLogErrors(webRequest, "Get Extract Refresh Tasks");

            var xmlDoc = GetWebResponseAsXml(response);

            //Get extract refresh task by site
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");

            var tasks = xmlDoc.SelectNodes("//iwsOnline:task", nsManager);

            var sources = new List<ServerData.ExtractRefreshTask>();

            //Get information for each of the tasks
            foreach (XmlNode itemXml in tasks) {
                try {
                    var task = new ServerData.ExtractRefreshTask(nsManager, itemXml, OnlineSession.SiteId);
                    sources.Add(task);
                }
                catch
                {
                    statusLog.AddError("Error parsing task: " + xmlDoc.InnerXml);
                }
            }
            _extractRefreshTasks = sources;
        }
    }
}
