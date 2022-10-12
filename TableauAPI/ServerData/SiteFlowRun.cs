using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using TableauAPI.FilesLogging;

namespace TableauAPI.ServerData
{
    public class SiteFlowRun: SiteDocumentBase, IEditDataConnectionsSet
    {
        public readonly string flowId;
        public readonly string status;
        public readonly string startedAt;
        public readonly string completedAt;
        public readonly string progress;

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

        public SiteFlowRun(XmlNode flowNode) : base(flowNode)
        {
            if (flowNode.Name.ToLower() != "flowruns") 
            {
                AppDiagnostics.Assert(false, "Not a flow run");
                throw new Exception("Unexpected content - not flow run");
            }
            //Get the underlying data source type
            flowId = flowNode.Attributes?["flowId"]?.Value;
            status = flowNode.Attributes?["status"]?.Value;
            startedAt = flowNode.Attributes?["startedAt"]?.Value;
            completedAt = flowNode.Attributes?["completedAt"]?.Value;
            progress = flowNode.Attributes?["progress"]?.Value;
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
