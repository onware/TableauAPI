using System.Text;
using TableauAPI.RESTHelpers;

namespace TableauAPI.RESTRequests
{
    public class QueryGraphqlMetadata : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private string _returnJson;

        public string Result
        {
            get 
            {
                return _returnJson;
            }
        }

        public QueryGraphqlMetadata(TableauServerUrls onlineUrls, TableauServerSignIn login) : base(login)
        {
            _onlineUrls = onlineUrls;
        }

        /// <summary>
        /// take serialized json object
        /// {
        ///     "query": "...",
        ///     "operationName": "...",
        ///     "variables": { "myVariable": "someValue", ... }
        /// }
        /// operationName/variables are optional
        /// referemce https://graphql.org/learn/serving-over-http/#post-request
        /// </summary>
        /// <param name="queryString"></param>
        public void Execute(string queryString)
        {
            var urlQuery = _onlineUrls.Url_QueryGraphqlMetadata();
            var webRequest = CreateLoggedInWebRequest(urlQuery, "POST");
            OnlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            webRequest.ContentType = "application/json";
            
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] byte1 = encoding.GetBytes(queryString);
            webRequest.ContentLength = byte1.Length;
            var newStream = webRequest.GetRequestStream();
            newStream.Write(byte1,0,byte1.Length);
            newStream.Close();
            var response = GetWebResponseLogErrors(webRequest, "query meata data");

            _returnJson = GetWebResponseAsText(response);

        }
    }
}
