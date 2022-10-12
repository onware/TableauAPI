using System;
using System.Collections.Generic;
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

        public void ExecuteRequest() 
        {
            var onlineFlowRuns = new List<SiteFlowRun>();
            var urlQuery = _onlineUrls.Url_FlowRunsList(OnlineSession);
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

            _flowRuns = onlineFlowRuns;

        }

    }
}
