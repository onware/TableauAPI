using System;
using System.Xml;
using TableauAPI.FilesLogging;

namespace TableauAPI.ServerData
{
    public class ExtractRefreshTask
    {
        public readonly string nextRunAt;
        public readonly string datasourceId;
        public readonly string scheduleId;
        public readonly string siteId;
        public readonly string taskId;

        public ExtractRefreshTask(XmlNamespaceManager nsManager, XmlDocument xmlDoc,string _siteId)
        {
            var task = xmlDoc.SelectSingleNode("//iwsOnline:extractRefresh", nsManager);
            var schedule = xmlDoc.SelectSingleNode("//iwsOnline:schedule", nsManager);
            var datasource = xmlDoc.SelectSingleNode("//iwsOnline:datasource", nsManager);

            var taskname = task.Name.ToLower();

            if (taskname == "extractrefresh") {
                taskId = task.Attributes?["id"].Value;
            }
            if (schedule.Name.ToLower() == "schedule") { 
                nextRunAt = schedule.Attributes?["nextRunAt"].Value;
                scheduleId = schedule.Attributes?["id"].Value;
            }
            if (datasource.Name.ToLower() == "datasource") { 
                datasourceId = datasource.Attributes?["id"].Value;
            }

            siteId = _siteId;
        }
    }
}
