using System;
using System.Text;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;

namespace TableauAPI.ServerData
{
    /// <summary>
    /// Information about a Data connection that is embedded in a Workbook or Data Source
    /// </summary>
    public class SiteConnection : IHasSiteItemId
    {
        /// <summary>
        /// Connection ID
        /// </summary>
        public readonly string Id;
        /// <summary>
        /// The underlying source of the data (e.g. SQL Server? MySQL? Excel? CSV?)
        /// </summary>
        public readonly string ConnectionType;
        /// <summary>
        /// Server IP, hostname or FQDN
        /// </summary>
        public readonly string ServerAddress;
        /// <summary>
        /// Server Port
        /// </summary>
        public readonly string ServerPort;
        /// <summary>
        /// User name of authorized User.
        /// </summary>
        public readonly string UserName;

        /// <summary>
        /// Any developer/diagnostic notes we want to indicate
        /// </summary>
        public readonly string DeveloperNotes;

        /// <summary>
        /// Password embed
        /// </summary>
        public readonly string EmbedPassword;

        /// <summary>
        /// Create an instance of a SiteConnection from XML returned by the Tableau server
        /// </summary>
        /// <param name="projectNode"></param>
        public SiteConnection(XmlNode projectNode)
        {
            var sbDevNotes = new StringBuilder();

            if (projectNode.Name.ToLower() != "connection")
            {
                AppDiagnostics.Assert(false, "Not a connection");
                throw new Exception("Unexpected content - not connection");
            }

            Id = projectNode.Attributes?["id"].Value;
            ConnectionType = projectNode.Attributes?["type"].Value;

            ServerAddress = XmlHelper.SafeParseXmlAttribute(projectNode, "serverAddress", "");
            ServerPort = XmlHelper.SafeParseXmlAttribute(projectNode, "serverPort", "");
            UserName = XmlHelper.SafeParseXmlAttribute(projectNode, "userName", "");
            EmbedPassword = XmlHelper.SafeParseXmlAttribute(projectNode, "embedPassword", "");

            DeveloperNotes = sbDevNotes.ToString();
        }

        /// <summary>
        /// Returns a string representing the current Tableau Server Connection
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Connection: " + ConnectionType + "/" + ServerAddress + "/" + Id;
        }

        string IHasSiteItemId.Id => Id;
    }
}
