using System;
using System.Xml;
using TableauAPI.FilesLogging;

namespace TableauAPI.ServerData
{
    /// <summary>
    /// Information about a Site in Server
    /// </summary>
    public class SiteInfoSite
    {
        /// <summary>
        /// Site ID
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// Name of Site
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Server relative URL of Site
        /// </summary>
        public readonly string ContentUrl;

        /// <summary>
        /// ContentOnly or ContentAndUsers
        /// </summary>
        public readonly string AdminMode;

        /// <summary>
        /// Active or Suspended
        /// </summary>
        public readonly string State;

        /// <summary>
        /// Any developer/diagnostic notes we want to indicate
        /// </summary>
        public readonly string DeveloperNotes;

        /// <summary>
        /// Creates an instance of SiteInfoSite from XML returned by the Tableau server
        /// </summary>
        /// <param name="content">XML content returned by Tableau server</param>
        public SiteInfoSite(XmlNode content)
        {
            if(content.Name.ToLower() != "site")
            {
                AppDiagnostics.Assert(false, "Not a site");
                throw new Exception("Unexpected content - not site");
            }

            Name = content.Attributes?["name"].Value;
            Id = content.Attributes?["id"].Value;
            ContentUrl = content.Attributes?["contentUrl"].Value;
            AdminMode = content.Attributes?["adminMode"].Value;
            State = content.Attributes?["state"].Value;
        }
    }
}
