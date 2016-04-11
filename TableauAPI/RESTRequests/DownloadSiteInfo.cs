using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Request to download Site Info from the Tableau REST API
    /// </summary>
    public class DownloadSiteInfo : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private SiteInfoSite _onlineSiteInfoSite;

        /// <summary>
        /// Workbooks we've parsed from server results
        /// </summary>
        public SiteInfoSite SiteInfoSite
        {
            get
            {
                return _onlineSiteInfoSite;
            }
        }

        /// <summary>
        /// Create an instance of a request for Site information from the Tableau REST API
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        public DownloadSiteInfo(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
        }

        /// <summary>
        /// Execute the request for Site Information
        /// </summary>
        public void ExecuteRequest()
        {
            var statusLog = OnlineSession.StatusLog;

            //Create a web request, in including the users logged-in auth information in the request headers
            var urlRequest = _onlineUrls.Url_SiteInfo(OnlineSession);
            var webRequest = CreateLoggedInWebRequest(urlRequest);
            webRequest.Method = "GET";

            //Request the data from server
            OnlineSession.StatusLog.AddStatus("Web request: " + urlRequest, -10);
            var response = GetWebResponseLogErrors(webRequest, "get site info");
        
            var xmlDoc = GetWebResponseAsXml(response);

            //Get all the workbook nodes
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
            var sites = xmlDoc.SelectNodes("//iwsOnline:site", nsManager);

            int numberSites = 0;
            foreach(XmlNode contentXml in sites)
            {
                try
                {
                    numberSites++;
                    var site = new SiteInfoSite(contentXml);
                    _onlineSiteInfoSite = site;

                    statusLog.AddStatus("Site info: " + site.Name + "/" + site.Id + "/" + site.State);
                }
                catch
                {
                    AppDiagnostics.Assert(false, "Site parse error");
                    statusLog.AddError("Error parsing site: " + contentXml.InnerXml);
                }
            }

            //Sanity check
            if(numberSites > 1)
            {
                statusLog.AddError("Error - how did we get more than 1 site? " + numberSites.ToString() + " sites");
            }
        }
    }
}
