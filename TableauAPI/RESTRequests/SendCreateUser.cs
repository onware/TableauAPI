using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    public class SendCreateUser : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _name;
        private readonly string _fullName;
        private readonly string _siteRole;
        private readonly string _authSetting;

        /// <summary>
        /// Create an instance of a Create User REST API request.
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="name">Username of the user</param>
        /// <param name="siteRole">Site role assigned to the user</param>
        /// <param name="authSetting">Authentication type</param>
        public SendCreateUser(
            TableauServerUrls onlineUrls,
            TableauServerSignIn logInInfo,
            string name,
            string fullName,
            string siteRole,
            string authSetting)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _name = name;
            _fullName = fullName;
            _siteRole = siteRole;
            _authSetting = authSetting;
        }

        /// <summary>
        /// Create a user on server
        /// </summary>
        public string ExecuteRequest()
        {
            try
            {
                var newUser = _CreateUser(_name, _fullName, _siteRole, _authSetting);
                StatusLog.AddStatus("User created. " + newUser);
                return newUser;
            }
            catch (Exception exProject)
            {
                StatusLog.AddError("Error attempting to create user '" + _name + "'", exProject);
                return null;
            }
        }

        private string _CreateUser(string name, string fullName, string siteRole, string authSetting)
        {
            //ref: https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_usersgroups.htm#add_user_to_site
            var sb = new StringBuilder();
            var xmlSettings = new XmlWriterSettings();
            xmlSettings.OmitXmlDeclaration = true;
            var xmlWriter = XmlWriter.Create(sb, xmlSettings);
            xmlWriter.WriteStartElement("tsRequest");
            xmlWriter.WriteStartElement("user");
            xmlWriter.WriteAttributeString("name", name);
            xmlWriter.WriteAttributeString("fullName", fullName);
            xmlWriter.WriteAttributeString("siteRole", siteRole);
            xmlWriter.WriteAttributeString("authSetting", authSetting);
            xmlWriter.WriteEndElement();//</user>
            xmlWriter.WriteEndElement();//</tsRequest>
            xmlWriter.Close();

            var xmlText = sb.ToString(); //Get the XML text out

            //Create a web request
            var urlCreateUser = _onlineUrls.Url_CreateUser(OnlineSession);
            //var urlCreateUser = "https://tableau.onware.com/api/2.7/sites/25d20eac-3daf-4159-a490-1f941e09c89d/users";
            var webRequest = CreateLoggedInWebRequest(urlCreateUser, "POST");
            SendRequestContents(webRequest, xmlText);

            //Get the response
            var response = GetWebResponseLogErrors(webRequest, "create user");
            var location = response.Headers.Get("Location");
            string split = "users/";
            string tableauUserId = location.Substring(location.IndexOf(split) + split.Length);

            return tableauUserId;
            
        }

    }
}
