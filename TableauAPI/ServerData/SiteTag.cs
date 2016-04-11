using System;
using System.Xml;
using TableauAPI.FilesLogging;

namespace TableauAPI.ServerData
{
    /// <summary>
    /// Information about a Tag
    /// </summary>
    public class SiteTag
    {
        /// <summary>
        /// The Tag Label
        /// </summary>
        public readonly string Label;
        
        /// <summary>
        /// Any developer/diagnostic notes we want to indicate
        /// </summary>
        public readonly string DeveloperNotes;

        /// <summary>
        /// Create an instance of a Site Tag from XML returned by the Tableau server
        /// </summary>
        /// <param name="tagNode"></param>
        public SiteTag(XmlNode tagNode)
        {
            if (tagNode.Name.ToLower() != "tag")
            {
                AppDiagnostics.Assert(false, "Not a tag");
                throw new Exception("Unexpected content - not tag");
            }

            Label = tagNode.Attributes?["label"].Value;
        }

        /// <summary>
        /// The label for the current tag.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Tag: " + Label;
        }

    }
}
