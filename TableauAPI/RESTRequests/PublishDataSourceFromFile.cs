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
    /// Publishes a data file as a data source
    /// </summary>
    public class PublishDataSourceFromFile : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;

        /// <summary>
        /// Create the request to publish a data file as a data source in the Tableau REST API
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        public PublishDataSourceFromFile(TableauServerUrls onlineUrls, TableauServerSignIn logInInfo) : base(logInInfo)
        {
            _onlineUrls = onlineUrls;
        }

        /// <summary>
        /// Uploads a file.
        /// </summary>
        public void PublishFile(string filePath, XmlDocument requestXml, string datasourceFileType = null)
        {
            if (datasourceFileType == null)
            {
                datasourceFileType = Path.GetExtension(filePath).Substring(1);
            }

            var fileUploadInitiator = new InitiateFileUpload(_onlineUrls, OnlineSession);
            var fileUploadSession = fileUploadInitiator.ExecuteRequest();
            var fileUploadAppender = new AppendToFileUpload(_onlineUrls, OnlineSession, fileUploadSession);
            using (var source = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[64 * 1024 * 1024];
                int bytesRead;
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileUploadAppender.ExecuteRequest(buffer, bytesRead);
                }
            }
            var fileUploadFinisher = new PublishDataSourceFinish(_onlineUrls, OnlineSession, fileUploadSession, datasourceFileType, requestXml);
            fileUploadFinisher.ExecuteRequest();
        }
    }
}