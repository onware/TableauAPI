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
        public readonly string Label;
        /// <summary>
        /// Any developer/diagnostic notes we want to indicate
        /// </summary>
        public readonly string DeveloperNotes;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userNode"></param>
        public SiteTag(XmlNode tagNode)
        {
            if (tagNode.Name.ToLower() != "tag")
            {
                AppDiagnostics.Assert(false, "Not a tag");
                throw new Exception("Unexpected content - not tag");
            }

            this.Label = tagNode.Attributes["label"].Value;
        }

        public override string ToString()
        {
            return "Tag: " + this.Label;
        }

    }
}
