using System;
using System.Collections.Generic;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;
using System.Text;

namespace TableauAPI.RESTRequests
{
    public class Schedule : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private List<SiteSchedule> _schedules;
        private SiteSchedule _siteSchedule;

        public Schedule(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
        }

        public void QuerySchedules() {
            var statusLog = OnlineSession.StatusLog;

            //Create a web request, in including the users logged-in auth information in the request headers
            var urlRequest = _onlineUrls.Url__QuerySchedules();
            var webRequest = CreateLoggedInWebRequest(urlRequest);
            webRequest.Method = "GET";

            //Request the data from server
            OnlineSession.StatusLog.AddStatus("Web request: " + urlRequest, -10);
            var response = GetWebResponseLogErrors(webRequest, "get schedule");
            var xmlDoc = GetWebResponseAsXml(response);

            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
            var collections = xmlDoc.SelectNodes("//iwsOnline:schedules/iwsOnline:schedule", nsManager);

            var schedules = new List<SiteSchedule>();

            foreach (XmlNode itemXml in collections) {
                try
                {
                    var s = new SiteSchedule(itemXml);
                    schedules.Add(s);
                }
                catch {
                    OnlineSession.StatusLog.AddError("Error parsing schedule: " + itemXml.InnerXml);
                }
            }
            _schedules = schedules;
        }

        public void GetSchedule(string scheduleId) {
            var statusLog = OnlineSession.StatusLog;

            //Create a web request, in including the users logged-in auth information in the request headers
            var urlRequest = _onlineUrls.Url_GetSchedule(scheduleId);
            var webRequest = CreateLoggedInWebRequest(urlRequest);
            webRequest.Method = "GET";

            //Request the data from server
            OnlineSession.StatusLog.AddStatus("Web request: " + urlRequest, -10);
            var response = GetWebResponseLogErrors(webRequest, "get schedule");

            var xmlDoc = GetWebResponseAsXml(response); 

            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
            var collection = xmlDoc.SelectSingleNode("//iwsOnline:schedule", nsManager);

            //Get schedule
            try
            {
                var s = new SiteSchedule(collection);
                _siteSchedule = s;
            }
            catch
            {
                OnlineSession.StatusLog.AddError("Error parsing schedule: " + collection.InnerXml);
            }
        }
    }
}
