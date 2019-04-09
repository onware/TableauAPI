using System;
using System.Xml;
using TableauAPI.FilesLogging;

namespace TableauAPI.ServerData
{
    /// <summary>
    /// Information about a User in a Server's site
    /// </summary>
    public class SiteUser : IHasSiteItemId
    {
        /// <summary>
        /// User Name
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// User's ID
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// Role on the current Tableau site
        /// </summary>
        public readonly string SiteRole;
        
        /// <summary>
        /// Any developer/diagnostic notes we want to indicate
        /// </summary>
        public readonly string DeveloperNotes;

        /// <summary>
        /// Any developer/diagnostic notes we want to indicate
        /// </summary>
        public readonly string FullName;

        /// <summary>
        /// Any developer/diagnostic notes we want to indicate
        /// </summary>
        public readonly string ExternalAuthUserId;

        /// <summary>
        /// Any developer/diagnostic notes we want to indicate
        /// </summary>
        public readonly string AuthSetting;

        /// <summary>
        /// The last login date of the user
        /// </summary>
        public readonly DateTime? LastLogin;

        /// <summary>
        /// Create a SiteUser from XML returned by the Tableau server
        /// </summary>
        /// <param name="userNode"></param>
        public SiteUser(XmlNode userNode)
        {
            if (userNode.Name.ToLower() != "user")
            {
                AppDiagnostics.Assert(false, "Not a user");
                throw new Exception("Unexpected content - not user");
            }

            Id = userNode.Attributes?["id"].Value;
            Name = userNode.Attributes?["name"].Value;
            SiteRole = userNode.Attributes?["siteRole"].Value;
            FullName = userNode.Attributes?["fullName"].Value;
            ExternalAuthUserId = userNode.Attributes?["externalAuthUserId"].Value;
            AuthSetting = userNode.Attributes?["authSetting"].Value;

            var lastLogin = userNode.Attributes["lastLogin"]?.Value;
            if (lastLogin != null)
                LastLogin = Convert.ToDateTime(lastLogin);
        }

        /// <summary>
        /// Returns the Name, ID and Role for the current User.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "User: " + Name + "/" + Id + "/" + SiteRole;
        }

        /// <summary>
        /// User's ID
        /// </summary>
        string IHasSiteItemId.Id => Id;
    }
}
