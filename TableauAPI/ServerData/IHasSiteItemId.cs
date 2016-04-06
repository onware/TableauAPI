namespace TableauAPI.ServerData
{
    /// <summary>
    /// Object has a unique identity in the site
    /// </summary>
    interface IHasSiteItemId
    {
        string Id  {get;}
    }
}