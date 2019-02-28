using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using TableauAPI.FilesLogging;

namespace TableauAPI.ServerData
{
    /// <summary>
    /// Information about a Datasource in a Server's site
    /// </summary>
    public class SiteDatasource : SiteDocumentBase, IEditDataConnectionsSet
    {
        /// <summary>
        /// The underlying source of the data (e.g. SQL Server, MySQL, Excel, CSV)
        /// </summary>
        public readonly string Type;

        /// <summary>
        /// Relative URL to access the Workbook
        /// </summary>
        ///<remark>Note: [2015-10-28] Datasources presently don't return this information, so we need to make this workbook specific</remark>
        public readonly string ContentUrl;
        /// <summary>
        /// Web page url
        /// </summary>
        public readonly string WebpageUrl;
        /// <summary>
        /// Description of the workbook
        /// </summary>
        public readonly string Description;
        /// <summary>
        /// Creation date
        /// </summary>
        public readonly string CreatedAt;
        /// <summary>
        /// Update date
        /// </summary>
        public readonly string UpdatedAt;

        /// <summary>
        /// Is certified
        /// </summary>
        public readonly string IsCertified;

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
        /// Creates an instance of a Datasource from XML returned by the Tableau server
        /// </summary>
        /// <param name="datasourceNode"></param>
        public SiteDatasource(XmlNode datasourceNode) : base(datasourceNode)
        {
            if (datasourceNode.Name.ToLower() != "datasource")
            {
                AppDiagnostics.Assert(false, "Not a datasource");
                throw new Exception("Unexpected content - not datasource");
            }

            //Get the underlying data source type
            Type = datasourceNode.Attributes?["type"].Value;
            ContentUrl = datasourceNode.Attributes?["contentUrl"].Value;
            WebpageUrl = datasourceNode.Attributes?["webpageUrl"].Value;
            CreatedAt = datasourceNode.Attributes?["createdAt"].Value;
            UpdatedAt = datasourceNode.Attributes?["updatedAt"].Value;
            IsCertified = datasourceNode.Attributes?["isCertified"].Value;

            if (datasourceNode.Attributes != null && datasourceNode.Attributes["description"] != null)
            {
                Description = datasourceNode.Attributes?["description"].Value;
            }
        }

        /// <summary>
        /// Datasource description
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Datasource: " + Name + "/" + Type + "/" + Id;
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
