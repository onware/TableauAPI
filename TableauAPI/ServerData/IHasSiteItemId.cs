namespace TableauAPI.ServerData
{
    /// <summary>
    /// Object has a unique identity in the site
    /// </summary>
    internal interface IHasSiteItemId
    {
        /// <summary>
        /// Site Item ID
        /// </summary>
        string Id  {get;}
    }
}