using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;


namespace TableauAPI.RESTRequests
{
    public class Schedule : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;

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

            //Get schedule
            
        }
    }
}
