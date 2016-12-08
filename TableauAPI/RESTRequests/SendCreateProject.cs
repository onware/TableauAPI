using System;
using System.Text;
using System.Xml;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Create a Project REST API request
    /// </summary>
    public class SendCreateProject : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _projectName;
        private readonly string _projectDesciption = "";

        /// <summary>
        /// Create an instance of a Create Project REST API request.
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="projectName">Project Name</param>
        public SendCreateProject(
            TableauServerUrls onlineUrls,
            TableauServerSignIn logInInfo,
            string projectName)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _projectName = projectName;
        }

        /// <summary>
        /// Create a project on server
        /// </summary>
        public SiteProject ExecuteRequest()
        {
            try
            {
                var newProj = _CreateProject(_projectName, _projectDesciption);
                StatusLog.AddStatus("Project created. " + newProj);
                return newProj;
            }
            catch (Exception exProject)
            {
                StatusLog.AddError("Error attempting to create project '" + _projectName + "', " + exProject.Message);
                return null;
            }
        }

        #region Private Methods

        private SiteProject _CreateProject(string projectName, string projectDescription)
        {
            //ref: http://onlinehelp.tableau.com/current/api/rest_api/en-us/help.htm#REST/rest_api_ref.htm#Create_Project%3FTocPath%3DAPI%2520Reference%7C_____12  
            var sb = new StringBuilder();
            var xmlSettings = new XmlWriterSettings();
            xmlSettings.OmitXmlDeclaration = true;
            var xmlWriter = XmlWriter.Create(sb, xmlSettings);
            xmlWriter.WriteStartElement("tsRequest");
            xmlWriter.WriteStartElement("project");
            xmlWriter.WriteAttributeString("name", projectName);
            xmlWriter.WriteAttributeString("description", projectDescription);
            xmlWriter.WriteEndElement();//</project>
            xmlWriter.WriteEndElement(); // </tsRequest>
            xmlWriter.Close();

            var xmlText = sb.ToString(); //Get the XML text out

            //Generate the MIME message
            //var mimeGenerator = new OnlineMimeXmlPayload(xmlText);

            //Create a web request 
            var urlCreateProject = _onlineUrls.Url_CreateProject(OnlineSession);
            var webRequest = CreateLoggedInWebRequest(urlCreateProject, "POST");
            SendRequestContents(webRequest, xmlText);

            //Get the response
            var response = GetWebResponseLogErrors(webRequest, "create project");
            using (response)
            {
                var xmlDoc = GetWebResponseAsXml(response);


                //Get all the workbook nodes
                var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
                var xNodeProject = xmlDoc.SelectSingleNode("//iwsOnline:project", nsManager);

                try
                {
                    return new SiteProject(xNodeProject);
                }
                catch (Exception parseXml)
                {
                    StatusLog.AddError("Data source upload, error parsing XML resposne " + parseXml.Message + "\r\n" + xNodeProject.InnerXml);
                    return null;
                }

            }
        }

        #endregion

    }
}
