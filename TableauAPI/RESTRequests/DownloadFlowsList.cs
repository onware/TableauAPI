using System;
using System.Collections.Generic;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Downloads the list of flows from the Tableau REST API
    /// </summary>
    public class DownloadFlowsList : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private List<SiteFlow> _flows;

        /// <summary>
        /// Flows we've parsed from server results
        /// </summary>
        public ICollection<SiteFlow> Flows
        {
            get
            {
                var ds = _flows;
                return ds?.AsReadOnly();
            }
        }

        /// <summary>
        /// Create a request to get a list of Flows from the Tableau REST API
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="login"></param>
        public DownloadFlowsList(TableauServerUrls onlineUrls, TableauServerSignIn login)
            : base(login)
        {
            _onlineUrls = onlineUrls;
        }

        /// <summary>
        /// Request the data from Online
        /// </summary>
        public void ExecuteRequest()
        {

            var onlineFlows = new List<SiteFlow>();
            int numberPages = 1; //Start with 1 page (we will get an updated value from server)
            //Get subsequent pages
            for (int thisPage = 1; thisPage <= numberPages; thisPage++)
            {
                try
                {
                    _ExecuteRequest_ForPage(onlineFlows, thisPage, out numberPages);
                }
                catch (Exception exPageRequest)
                {
                    StatusLog.AddError("Flows error during page request", exPageRequest);
                }
            }
            _flows = onlineFlows;
        }

        #region Private Methods

        /// <summary>
        /// Get a page's worth of Flows
        /// </summary>
        /// <param name="onlineFlows"></param>
        /// <param name="pageToRequest">Page # we are requesting (1 based)</param>
        /// <param name="totalNumberPages">Total # of pages of data that Server can return us</param>
        private void _ExecuteRequest_ForPage(List<SiteFlow> onlineFlows, int pageToRequest, out int totalNumberPages)
        {
            int pageSize = _onlineUrls.PageSize;
            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = _onlineUrls.Url_FlowsList(OnlineSession, pageSize, pageToRequest);
            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            OnlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebResponseLogErrors(webRequest, "get flows list");
            var xmlDoc = GetWebResponseAsXml(response);

            //Get all the flow nodes
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
            var flows = xmlDoc.SelectNodes("//iwsOnline:flow", nsManager);

            //Get information for each of the flow
            foreach (XmlNode itemXml in flows)
            {
                try
                {
                    var ds = new SiteFlow(itemXml);
                    onlineFlows.Add(ds);
                }
                catch
                {
                    OnlineSession.StatusLog.AddError("Error parsing flow: " + itemXml.InnerXml);
                }
            } //end: foreach

            //-------------------------------------------------------------------
            //Get the updated page-count
            //-------------------------------------------------------------------
            totalNumberPages = DownloadPaginationHelper.GetNumberOfPagesFromPagination(
                xmlDoc.SelectSingleNode("//iwsOnline:pagination", nsManager),
                pageSize);
        }

        #endregion

    }
}
