using System.Collections.Generic;

namespace TableauAPI.ServerData
{
    /// <summary>
    /// Object Allows the setting of the Data Connections
    /// </summary>
    internal interface IEditDataConnectionsSet
    {
        void SetDataConnections(IEnumerable<SiteConnection> connections);
    }
}