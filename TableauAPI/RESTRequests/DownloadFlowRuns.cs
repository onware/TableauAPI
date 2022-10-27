using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    public class DownloadFlowRuns : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private List<SiteFlowRun> _flowRuns;

        /// <summary>
        /// Flow Runs we've parsed from server results
        /// </summary>
        public ICollection<SiteFlowRun> FlowRuns
        {
            get
            {
                var ds = _flowRuns;
                return ds?.AsReadOnly();
            }
        }
        /// <summary>
        /// Create a request to get a list of Flow Runs from the Tableau REST API
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="login"></param>
        public DownloadFlowRuns(TableauServerUrls onlineUrls, TableauServerSignIn login) : base(login)
        {
            _onlineUrls = onlineUrls;
        }

        public void ExecuteRequest(DateTime? startedAt = null)
        {
            if (startedAt == null) { startedAt = DateTime.UtcNow.AddDays(-7); }
            var onlineFlowRuns = new List<SiteFlowRun>();
            bool canContinue = true;
            while (canContinue) {
                ExecuteRequestByDate(onlineFlowRuns, (DateTime)startedAt, out canContinue);
                string strStartedAt = onlineFlowRuns.OrderByDescending(x => x.startedAt).FirstOrDefault().startedAt;
                startedAt = DateTimeOffset.Parse(strStartedAt).UtcDateTime;
            }
            _flowRuns = onlineFlowRuns.GroupBy(x => x.Id).Select(x => x.First()).ToList();  
        }
        public void ExecuteRequestByDate(List<SiteFlowRun> onlineFlowRuns, DateTime startedAt, out bool canContinue) 
        {
            var urlQuery = _onlineUrls.Url_FlowRunsList(OnlineSession,startedAt);
            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            OnlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebResponseLogErrors(webRequest, "get flows list");
            var xmlDoc = GetWebResponseAsXml(response);

            //Get all the flow run nodes
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
            var flowRuns = xmlDoc.SelectNodes("//iwsOnline:flowRuns", nsManager);

            //Get information for each of the flow
            foreach (XmlNode itemXml in flowRuns)
            {
                try
                {
                    if (!itemXml.InnerXml.Contains("flowRuns")) { 
                        var ds = new SiteFlowRun(itemXml);
                        onlineFlowRuns.Add(ds);
                    }
                }
                catch
                {
                    AppDiagnostics.Assert(false, "Flow run parse error");
                    OnlineSession.StatusLog.AddError("Error parsing flow run: " + itemXml.InnerXml);
                }
            }
            if (flowRuns.Count < 100) { canContinue = false; } else { canContinue = true; }
        }

    }
}
