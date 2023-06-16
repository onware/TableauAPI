using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using TableauAPI.FilesLogging;

namespace TableauAPI.ServerData
{
    /// <summary>
    /// Information on a View in a Tableau Server's site
    /// </summary>
    public class SiteView : SiteDocumentBase, IEditDataConnectionsSet
    {

        /// <summary>
        /// Relative URL to access the Workbook
        /// </summary>
        public readonly string ContentUrl;

        /// <summary>
        /// Workbook ID
        /// </summary>
        public readonly string WorkbookId;
        /// <summary>
        /// Creation date
        /// </summary>
        public readonly string CreatedAt;
        /// <summary>
        /// Update date
        /// </summary>
        public readonly string UpdatedAt;
        /// <summary>
        /// Order position
        /// </summary>
        public readonly string Position;

        private List<SiteConnection> _dataConnections;

        /// <summary>
        /// If set, contains the set of data connections embedded in this workbooks
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
        /// Create a SiteView from XML returned by the Tableau server
        /// </summary>
        /// <param name="viewNode"></param>
        public SiteView(XmlNode viewNode) : base(viewNode)
        {
            if (viewNode.Name.ToLower() != "view")
            {
                throw new Exception("Unexpected content - not a view");
            }

            this.ContentUrl = viewNode.Attributes?["contentUrl"].Value;
            CreatedAt = viewNode.Attributes?["createdAt"].Value;
            UpdatedAt = viewNode.Attributes?["updatedAt"].Value;            

            var workbookNode = viewNode.SelectSingleNode("iwsOnline:workbook", NamespaceManager);
            if (workbookNode != null)
            {
                this.WorkbookId = workbookNode.Attributes?["id"].Value;                
            }

            if (viewNode.ParentNode != null && viewNode.ParentNode.Attributes["position"] != null)
            {
                Position = viewNode.ParentNode.Attributes?["position"].Value;
            }
        }

        /// <summary>
        /// Friendly reprsentation of a Site View.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"View: {Name}/{ContentUrl}/{Id}, Project: {ProjectId}";
        }

        /// <summary>
        /// Set data connections for the Site View
        /// </summary>
        /// <param name="connections"></param>
        public void SetDataConnections(IEnumerable<SiteConnection> connections)
        {
            _dataConnections = connections == null ? null : new List<SiteConnection>(connections);
        }
    }
}
