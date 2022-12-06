using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TableauAPI.RESTHelpers;
using TableauAPI.RESTRequests;
using Newtonsoft.Json;

namespace TableauAPI.Test
{
    [TestClass]
    public class DatasourceQueryTest
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
        public void TestDatasourcesPaging()
        {
            var unpagedRequest = new DownloadDatasourcesList(_urls, _session);
            unpagedRequest.QueryDataSources();
            var unpagedResults = unpagedRequest.Datasources;

            var pagedRequest = new DownloadDatasourcesList(_urls, _session);
            pagedRequest.ExecuteRequest();
            var pagedResults = pagedRequest.Datasources;

            Assert.IsTrue(pagedResults.Count >= unpagedResults.Count, "paged returns more or same (if unpaged)");
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

        [TestMethod]
        public void TestDatasourceIndividual()
        {
            var listRequest = new DownloadDatasourcesList(_urls, _session);
            listRequest.QueryDataSources();
            var listResults = listRequest.Datasources;

            foreach (var listItem in listResults)
            {
                var byId = new DownloadDatasourcesList(_urls, _session).QueryDataSource(listItem.Id);
                Assert.IsNotNull(byId);
                Assert.AreEqual(listItem.Id, byId.Id);
                Assert.AreEqual(listItem.Name, byId.Name);
                Assert.AreEqual(listItem.ProjectId, byId.ProjectId);
                Assert.AreEqual(listItem.ProjectName, byId.ProjectName);
                Assert.AreEqual(listItem.OwnerId, byId.OwnerId);

                Assert.AreEqual(listItem.TagsSet.Tags.Count(), byId.TagsSet.Tags.Count());
                foreach (var pair in listItem.TagsSet.Tags.Zip(byId.TagsSet.Tags, (list, id) => (list, id))) 
                {
                    Assert.AreEqual(pair.list.Label, pair.id.Label);
                }

                // type doesn't always match

                Assert.AreEqual(listItem.ContentUrl, byId.ContentUrl);
                Assert.AreEqual(listItem.Description, byId.Description);
                Assert.AreEqual(listItem.CreatedAt, byId.CreatedAt);
                Assert.AreEqual(listItem.UpdatedAt, byId.UpdatedAt);
                Assert.AreEqual(listItem.IsCertified, byId.IsCertified);
                Assert.IsNotNull(byId.WebpageUrl);
            }
        }
    }
}
