namespace TableauAPI.ServerData
{
    /// <summary>
    /// Object can answer questions about tags
    /// </summary>
    internal interface ITagSetInfo
    {
        /// <summary>
        /// True of if the content has the specified tag
        /// </summary>
        /// <param name="tagText"></param>
        /// <returns></returns>
        bool IsTaggedWith(string tagText);

        /// <summary>
        /// Text string containing all the tags
        /// </summary>
        /// <returns></returns>
        string TagSetText
        {
            get;
        }
    }
}