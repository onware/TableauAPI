using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Update user REST API Request
    /// </summary>
    public class SendUpdateUserSiteRole : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _userID;
        private readonly string _siteRole;

        /// <summary>
        /// Create an instance of a Send Update User REST API Request
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="userId">User to update</param>
        /// <param name="siteRole"></param>
        public SendUpdateUserSiteRole(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, string userId, string siteRole)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _userID = userId;
            _siteRole = siteRole;
        }

        /// <summary>
        /// Create a project on server
        /// </summary>
        public void ExecuteRequest()
        {
            try
            {
                _UpdateUser(_userID, _siteRole);
                StatusLog.AddStatus("User updated. " + _userID);
            }
            catch (Exception exUser)
            {
                StatusLog.AddError("Error attempting to update user '" + _userID + "', " + exUser.Message);
            }
        }

        #region Private Methods

        private void _UpdateUser(string userID, string siteRole)
        { 
            var sb = new StringBuilder();
            var xmlSettings = new XmlWriterSettings();
            xmlSettings.OmitXmlDeclaration = true;
            var xmlWriter = XmlWriter.Create(sb, xmlSettings);
            xmlWriter.WriteStartElement("tsRequest");
            xmlWriter.WriteStartElement("user");
            xmlWriter.WriteAttributeString("siteRole", siteRole);
            xmlWriter.WriteEndElement();//</user>
            xmlWriter.WriteEndElement(); // </tsRequest>
            xmlWriter.Close();

            var xmlText = sb.ToString(); //Get the XML text out

            //Generate the MIME message
            //var mimeGenerator = new OnlineMimeXmlPayload(xmlText);

            //Create a web request 
            var urlUpdateUser = _onlineUrls.Url_UpdateUser(OnlineSession, userID);
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

        #endregion
    }
}
