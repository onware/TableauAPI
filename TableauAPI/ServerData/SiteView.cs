using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using TableauAPI.FilesLogging;

namespace TableauAPI.ServerData
{
    public class SiteView : SiteDocumentBase, IEditDataConnectionsSet
    {

        public readonly string ContentUrl;
        public readonly string WorkbookId;

        /// <summary>
        /// If set, contains the set of data connections embedded in this workbooks
        /// </summary>
        private List<SiteConnection> _dataConnections;

        public ReadOnlyCollection<SiteConnection> DataConnections
        {
            get
            {
                var dataConnections = _dataConnections;
                if (dataConnections == null) return null;
                return dataConnections.AsReadOnly();
            }
        }

        public SiteView(XmlNode xmlNode) : base(xmlNode)
        {
            if (xmlNode.Name.ToLower() != "view")
            {
                AppDiagnostics.Assert(false, "Not a view");
                throw new Exception("Unexpected content - not a view");
            }

            this.ContentUrl = xmlNode.Attributes["contentUrl"].Value;
            var workbookNode = xmlNode.SelectSingleNode("iwsOnline:workbook", NamespaceManager);
            if (workbookNode != null)
            {
                this.WorkbookId = xmlNode.Attributes["id"].Value;
            }
        }

        public override string ToString()
        {
            return $"View: {Name}/{ContentUrl}/{Id}, Project: {ProjectId}";
        }

        public void SetDataConnections(IEnumerable<SiteConnection> connections)
        {
            if (connections == null)
            {
                _dataConnections = null;
            }
            _dataConnections = new List<SiteConnection>(connections);
        }
    }
}
