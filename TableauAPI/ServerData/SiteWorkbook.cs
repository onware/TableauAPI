using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;

namespace TableauAPI.ServerData
{
    /// <summary>
    /// Information about a Workbook in a Server's site
    /// </summary>
    public class SiteWorkbook : SiteDocumentBase, IEditDataConnectionsSet
    {
        /// <summary>
        /// true if tabs are shown on the Workbook; false otherwise.
        /// </summary>
        public readonly bool ShowTabs;

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
        /// Size
        /// </summary>
        public readonly string Size;

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
        /// Create a SiteWorkbook from XML returned by the Tableau server
        /// </summary>
        /// <param name="workbookNode">XML node representing a Workbook</param>
        public SiteWorkbook(XmlNode workbookNode) : base(workbookNode)
        {
            if (workbookNode.Name.ToLower() != "workbook")
            {
                AppDiagnostics.Assert(false, "Not a workbook");
                throw new Exception("Unexpected content - not workbook");
            }

            if (workbookNode.Attributes != null && workbookNode.Attributes["description"] != null)
            {
                Description = workbookNode.Attributes?["description"].Value;
            }
            
            //Note: [2015-10-28] Datasources presently don't return this information, so we need to make this workbook specific
            ContentUrl = workbookNode.Attributes?["contentUrl"].Value;

            WebpageUrl = workbookNode.Attributes?["webpageUrl"].Value;

            CreatedAt = workbookNode.Attributes?["createdAt"].Value;

            UpdatedAt = workbookNode.Attributes?["updatedAt"].Value;

            Size = workbookNode.Attributes?["size"].Value;

            //Do we have tabs?
            ShowTabs = XmlHelper.SafeParseXmlAttribute_Bool(workbookNode, "showTabs", false);
        }


        /// <summary>
        /// Friendly text
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Workbook: " + Name + "/" + ContentUrl + "/" + Id + ", Proj: " + ProjectId;
        }

        /// <summary>
        /// Interface for inserting the set of data connections associated with this content
        /// </summary>
        /// <param name="connections">List of data connections to be associated them with the workbook</param>
        void IEditDataConnectionsSet.SetDataConnections(IEnumerable<SiteConnection> connections)
        {
            _dataConnections = connections == null ? null : new List<SiteConnection>(connections);
        }
    }
}
