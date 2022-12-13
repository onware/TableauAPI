using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TableauAPI.RESTHelpers;
using TableauAPI.RESTRequests;
using Newtonsoft.Json;

namespace TableauAPI.Test
{
    [TestClass]
    public class GraphqlMetadataTest
    {
        private TableauServerUrls _urls;
        private TableauServerSignIn _session;

        public class graphqlRequest
        {
            public string query { get; set; }
            //public string operationName { get; set; } optional property
            //public string variables { get; set; } optional property
        }

        [TestInitialize]
        public void Login()
        {
            var env = IntegrationTestEnvironment.Load();
            _urls = env.Urls();
            _session = env.Login();
        }

        [TestMethod]
        public void TestMetadata()
        {

            var meta = new QueryGraphqlMetadata(_urls, _session);

            var q = new graphqlRequest()
            {
                query = "query example{\r\n    datasources\r\n    {\r\n      id\r\n    }\r\n}"
            };
            string query = JsonConvert.SerializeObject(q);
            meta.Execute(query);
            var result = meta.Result;
        }
    }
}
