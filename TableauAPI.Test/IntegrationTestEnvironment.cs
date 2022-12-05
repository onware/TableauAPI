using System.Xml;
using TableauAPI.RESTHelpers;
using TableauAPI.RESTRequests;

namespace TableauAPI.Test
{
    public class IntegrationTestEnvironment
    {
        private readonly string _tableauServer;
        private readonly string _tableauSite;
        private readonly string _tableauAuthTokenName;
        private readonly string _tableauAuthTokenValue;

        public IntegrationTestEnvironment(string tableauServer, string tableauSite, string tableauAuthTokenName, string tableauAuthTokenValue)
        {
            _tableauServer = tableauServer;
            _tableauSite = tableauSite;
            _tableauAuthTokenName = tableauAuthTokenName;
            _tableauAuthTokenValue = tableauAuthTokenValue;
        }

        public TableauServerUrls Urls()
        {
            return new TableauServerUrls(ServerProtocol.Https, _tableauServer, _tableauSite, serverVersion: ServerVersion.Server2021_2);
        }

        public TableauServerSignInToken Login()
        {
            var signIn = new TableauServerSignInToken(Urls(), _tableauAuthTokenName, _tableauAuthTokenValue, new FilesLogging.TaskStatusLogs());
            signIn.ExecuteRequest();
            return signIn;
        }

        public static IntegrationTestEnvironment Load()
        {
            var doc = new XmlDocument();
            doc.Load("integration-test-config.xml");
            var config = doc.FirstChild;
            return new IntegrationTestEnvironment(
                config.Attributes["tableauServer"].Value,
                config.Attributes["tableauSite"].Value,
                config.Attributes["tableauAuthTokenName"].Value,
                config.Attributes["tableauAuthTokenValue"].Value
            );
        }
    }
}
