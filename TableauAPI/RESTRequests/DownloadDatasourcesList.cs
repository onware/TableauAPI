using System;
using System.Collections.Generic;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Downloads the list of data sources from the Tableau REST API
    /// </summary>
    public class DownloadDatasourcesList : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private List<SiteDatasource> _datasources;
        
        /// <summary>
        /// Workbooks we've parsed from server results
        /// </summary>
        public ICollection<SiteDatasource> Datasources
        {
            get
            {
                var ds = _datasources;
                return ds?.AsReadOnly();
            }
        }

        /// <summary>
        /// Create a request to get a list of Datasources from the Tableau REST API
        /// </summary>
        /// <param name="onlineUrls"></param>
        /// <param name="login"></param>
        public DownloadDatasourcesList(TableauServerUrls onlineUrls, TableauServerSignIn login)
            : base(login)
        {
            _onlineUrls = onlineUrls;
        }

        /// <summary>
        /// Request the data from Online
        /// </summary>
        public void ExecuteRequest()
        {

            var onlineDatasources = new List<SiteDatasource>();
            int numberPages = 1; //Start with 1 page (we will get an updated value from server)
            //Get subsequent pages
            for (int thisPage = 1; thisPage <= numberPages; thisPage++)
            {
                try
                {
                    _ExecuteRequest_ForPage(onlineDatasources, thisPage, out numberPages);
                }
                catch(Exception exPageRequest)
                {
                    StatusLog.AddError("Datasources error during page request: " + exPageRequest.Message);
                }
            }
            _datasources = onlineDatasources;
        }

        #region Private Methods

        /// <summary>
        /// Get a page's worth of Data Sources
        /// </summary>
        /// <param name="onlineDatasources"></param>
        /// <param name="pageToRequest">Page # we are requesting (1 based)</param>
        /// <param name="totalNumberPages">Total # of pages of data that Server can return us</param>
        private void _ExecuteRequest_ForPage(List<SiteDatasource> onlineDatasources, int pageToRequest, out int totalNumberPages)
        {
            int pageSize =_onlineUrls.PageSize; 
            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = _onlineUrls.Url_DatasourcesList(OnlineSession, pageSize, pageToRequest);
            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            OnlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebResponseLogErrors(webRequest, "get datasources list");
            var xmlDoc = GetWebResponseAsXml(response);

            //Get all the workbook nodes
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
            var datasources = xmlDoc.SelectNodes("//iwsOnline:datasource", nsManager);

            //Get information for each of the data sources
            foreach (XmlNode itemXml in datasources)
            {
                try
                {
                    var ds = new SiteDatasource(itemXml);
                    onlineDatasources.Add(ds);
                }
                catch
                {
                    AppDiagnostics.Assert(false, "Datasource parse error");
                    OnlineSession.StatusLog.AddError("Error parsing datasource: " + itemXml.InnerXml);
                }
            } //end: foreach

            //-------------------------------------------------------------------
            //Get the updated page-count
            //-------------------------------------------------------------------
            totalNumberPages  =DownloadPaginationHelper.GetNumberOfPagesFromPagination(
                xmlDoc.SelectSingleNode("//iwsOnline:pagination", nsManager),
                pageSize);
        }

        #endregion

    }
}
