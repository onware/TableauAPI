using System.Xml;
using TableauAPI.RESTHelpers;

namespace TableauAPI.ServerData
{
    /// <summary>
    /// Base class for information common to Workbooks and Data Sources, so we don't have lots of redundant code
    /// </summary>
    public abstract class SiteDocumentBase : IHasProjectId, ITagSetInfo, IHasSiteItemId
    {
        /// <summary>
        /// Document ID
        /// </summary>
        public readonly string Id;
        /// <summary>
        /// Document Name
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Project ID
        /// </summary>
        public readonly string ProjectId;

        /// <summary>
        /// Project Name
        /// </summary>
        public readonly string ProjectName;

        /// <summary>
        /// Owner ID
        /// </summary>
        public readonly string OwnerId;
        
        /// <summary>
        /// Tags
        /// </summary>
        public readonly SiteTagsSet TagsSet;

        public readonly string DeveloperNotes;
        protected readonly XmlNamespaceManager NamespaceManager;

        protected SiteDocumentBase(XmlNode xmlNode)
        {
            this.Name = xmlNode.Attributes["name"].Value;
            this.Id = xmlNode.Attributes["id"].Value;

            //Note: [2015-10-28] Datasources presently don't return this information
            //        this.ContentUrl = xmlNode.Attributes["contentUrl"].Value;

            //Namespace for XPath queries
            NamespaceManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");

            //Get the project attributes
            var projectNode = xmlNode.SelectSingleNode("iwsOnline:project", NamespaceManager);
            if (projectNode != null)
            {
                this.ProjectId = projectNode.Attributes["id"].Value;
                this.ProjectName = projectNode.Attributes["name"].Value;
            }

            //Get the owner attributes
            var ownerNode = xmlNode.SelectSingleNode("iwsOnline:owner", NamespaceManager);
            if (ownerNode != null)
            {
                this.OwnerId = ownerNode.Attributes["id"].Value;
            }

            //See if there are tags
            var tagsNode = xmlNode.SelectSingleNode("iwsOnline:tags", NamespaceManager);
            if (tagsNode != null)
            {
                this.TagsSet = new SiteTagsSet(tagsNode);
            }
        }

        /// <summary>
        /// Space delimited list of tags
        /// </summary>
        public string TagSetText
        {
            get
            {
                var tagSet = this.TagsSet;
                if (tagSet == null) return "";
                return tagSet.TagSetText;
            }
        }

        string IHasProjectId.ProjectId
        {
            get { return this.ProjectId; }
        }

        public bool IsTaggedWith(string tagText)
        {
            var tagSet = this.TagsSet;
            if (tagSet == null)
            {
                return false;
            }
            return TagsSet.IsTaggedWith(tagText);
        }

        string IHasSiteItemId.Id
        {
            get { return this.Id; }
        }
    }
}
