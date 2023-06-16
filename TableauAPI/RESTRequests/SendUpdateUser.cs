using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    public class SendUpdateUser : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _userId;
        private readonly string _fullName;
        private readonly string _email;
        private readonly string _password;

        /// <summary>
        /// Create an instance of a Create User Rest API request.
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="userId">User to update</param>
        /// <param name="fullName">Name of the user</param>
        /// <param name="email">Email of the user</param>
        /// <param name="password">Password of the user</param>
        
        public SendUpdateUser(
            TableauServerUrls onlineUrls,
            TableauServerSignIn logInInfo,
            string userId,
            string fullName,
            string email,
            string password)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _userId = userId;
            _fullName = fullName;
            _email = email;
            _password = password;
        }

        /// <summary>
        /// Update a user on server
        /// </summary>
       
        public void ExecuteRequest()
        {
            try
            {
                _UpdateUser(_userId, _fullName, _email, _password);
                StatusLog.AddStatus("User updated. " + _userId);
            }
            catch (Exception exUser)
            {
                StatusLog.AddError("Error attempting to update user '" + _userId + "'", exUser);
            }
        }

        private void _UpdateUser(string userId, string fullName, string email, string password)
        {
            //ref: https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_usersgroups.htm#update_user

            var sb = new StringBuilder();
            var xmlSettings = new XmlWriterSettings();
            xmlSettings.OmitXmlDeclaration = true;
            var xmlWriter = XmlWriter.Create(sb, xmlSettings);
            xmlWriter.WriteStartElement("tsRequest");
            xmlWriter.WriteStartElement("user");
            xmlWriter.WriteAttributeString("fullName", fullName);
            xmlWriter.WriteAttributeString("email", email);
            xmlWriter.WriteAttributeString("password", password);
            xmlWriter.WriteEndElement(); //</user>
            xmlWriter.WriteEndElement(); //</tsRequest>
            xmlWriter.Close();

            var xmlText = sb.ToString(); //Get the XML text out

            //Create a web request
            var urlUpdateUser = _onlineUrls.Url_UpdateUser(OnlineSession, userId);
            var webRequest = CreateLoggedInWebRequest(urlUpdateUser, "PUT");
            SendRequestContents(webRequest, xmlText, "PUT");

            //Get the response
            var response = GetWebResponseLogErrors(webRequest, "update user");
            using (response)
            {
                var xmlDoc = GetWebResponseAsXml(response);

                //Get all the workbook nodes
                var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
                var xNodeUser = xmlDoc.SelectSingleNode("//iwsOnline:user", nsManager);
            }
        }
    }
}
