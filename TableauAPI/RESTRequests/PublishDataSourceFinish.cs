using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Finishes publication of a data source from previously uploaded data
    /// </summary>
    public class PublishDataSourceFinish : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _uploadSession;
        private readonly string _datasourceFileType;
        private readonly XmlDocument _requestXml;

        /// <summary>
        /// Create the request to finish publication of a data source from previously uploaded data in the Tableau REST API
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="uploadSession">The upload session identifier</param>
        /// <param name="datasourceFileType">The datasource file type</param>
        /// <param name="requestXml">The request XML for datasource publication</param>
        public PublishDataSourceFinish(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo, string uploadSession, string datasourceFileType, XmlDocument requestXml)
            : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
            _uploadSession = uploadSession;
            _datasourceFileType = datasourceFileType;
            _requestXml = requestXml;
        }

        /// <summary>
        /// Creates the XML used in the request to finish publication of a data source from previously uploaded data in the Tableau REST API
        /// </summary>
        /// <param name="datasourceName">The name of the datasource</param>
        /// <param name="datasourceDescription">The description for the datasource (can be null)</param>
        /// <param name="projectId">The project id for the datasource (can be null)</param>
        public static XmlDocument BuildRequestXml(string datasourceName, string datasourceDescription, string projectId)
        {
            XmlDocument doc = new XmlDocument();
            var tsRequest = doc.CreateElement("tsRequest");
            doc.AppendChild(tsRequest);
            var datasource = doc.CreateElement("datasource");
            tsRequest.AppendChild(datasource);
            datasource.SetAttribute("name", datasourceName);
            if (!string.IsNullOrEmpty(datasourceDescription))
            {
                datasource.SetAttribute("description", datasourceDescription);
            }
            if (!string.IsNullOrEmpty(projectId))
            {
                var project = doc.CreateElement("project");
                datasource.AppendChild(project);
                project.SetAttribute("id", projectId);
            }
            return doc;
        }

        /// <summary>
        /// Execute the REST API call to publish a file uploaded in seperate requests.
        /// </summary>
        public void ExecuteRequest()
        {
            var statusLog = OnlineSession.StatusLog;

            string urlPublish = _onlineUrls.Url_FinalizeDataSourcePublish(OnlineSession, _uploadSession, _datasourceFileType);
            statusLog.AddStatus("Publishing previously uploaded file");
            try
            {
                var webRequest = CreateAndSendMimeLoggedInRequest(urlPublish, "POST", new MimeWriterXml(_requestXml.OuterXml));
                var response = GetWebResponseLogErrors(webRequest, "file publish uploaded");
            }
            catch (Exception ex)
            {
                statusLog.AddError("Error during file upload append: " + urlPublish, ex);
            }
        }
    }
}