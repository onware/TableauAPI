using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    public class ExtractRefreshTask : TableauServerSignedInRequestBase
    {

        private readonly TableauServerUrls _onlineUrls;
        private ServerData.ExtractRefreshTask _extractRefreshTask;

        public ExtractRefreshTask(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo) : base (logInInfo)
        {
            _onlineUrls = onlineUrls;
        }

        public ServerData.ExtractRefreshTask extractRefreshTask
        {
            get 
            {
                return _extractRefreshTask;
            }
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
            try
            {
                var refreshTask = new ServerData.ExtractRefreshTask(nsManager,xmlDoc,OnlineSession.SiteId);
                _extractRefreshTask = refreshTask;
                statusLog.AddStatus("Extract Refresh Task: " + refreshTask.taskId + "/" + refreshTask.nextRunAt + "/" + refreshTask.scheduleId);
            }
            catch
            {
                AppDiagnostics.Assert(false, "Site parse error");
                statusLog.AddError("Error parsing site: " + xmlDoc.InnerXml);
            }
            
        }
    }
}
