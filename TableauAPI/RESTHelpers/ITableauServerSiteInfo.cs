namespace TableauAPI.RESTHelpers
{
    /// <summary>
    /// Information we need to know to generate correct URLs that point to content on a Tableau Server's site
    /// </summary>
    public interface ITableauServerSiteInfo
    {
        /// <summary>
        /// Return the server name
        /// </summary>
        string ServerName { get; }

        /// <summary>
        /// Return the server name with http:// or https://
        /// </summary>
        string ServerNameWithProtocol { get; }

        /// <summary>
        /// Site Identifier
        /// </summary>
        string SiteId { get; }
    }
}
