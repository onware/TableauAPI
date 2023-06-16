using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using TableauAPI.FilesLogging;

namespace TableauAPI.ServerData
{
    /// <summary>
    /// Information about a Flow in a Server's site
    /// </summary>
    public class SiteFlow : SiteDocumentBase, IEditDataConnectionsSet
    {
        /// <summary>
        /// Web page url
        /// </summary>
        public readonly string WebpageUrl;
        /// <summary>
        /// Description of the Flow
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// Type of file
        /// </summary>
        public readonly string FileType;

        /// <summary>
        /// Creation date
        /// </summary>
        public readonly string CreatedAt;
        /// <summary>
        /// Update date
        /// </summary>
        public readonly string UpdatedAt;


        private List<SiteConnection> _dataConnections;
        /// <summary>
        /// Return a set of data connections (if they were downloaded)
        /// </summary>
        public ReadOnlyCollection<SiteConnection> DataConnections
        {
            get
            {
                var dataConnections = _dataConnections;
                return dataConnections?.AsReadOnly();
            }
        }

        /// <summary>
        /// Creates an instance of a Flow from XML returned by the Tableau server
        /// </summary>
        /// <param name="flowNode"></param>
        public SiteFlow(XmlNode flowNode) : base(flowNode)
        {
            if (flowNode.Name.ToLower() != "flow")
            {
                throw new Exception("Unexpected content - not flow");
            }

            //Get the underlying data source type
            WebpageUrl = flowNode.Attributes?["webpageUrl"].Value;
            FileType = flowNode.Attributes?["fileType"].Value;
            CreatedAt = flowNode.Attributes?["createdAt"].Value;
            UpdatedAt = flowNode.Attributes?["updatedAt"].Value;

            if (flowNode.Attributes != null && flowNode.Attributes["description"] != null)
            {
                Description = flowNode.Attributes?["description"].Value;
            }
        }

        /// <summary>
        /// Flow description
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Flow: " + Name +  "/" + Id + ", Proj: " + ProjectId;
        }

        /// <summary>
        /// Interface for inserting the set of data connections associated with this content
        /// </summary>
        /// <param name="connections"></param>
        void IEditDataConnectionsSet.SetDataConnections(IEnumerable<SiteConnection> connections)
        {
            _dataConnections = connections == null ? null : new List<SiteConnection>(connections);
        }
    }
}
