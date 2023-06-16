using System;
using System.Xml;
using TableauAPI.FilesLogging;

namespace TableauAPI.ServerData
{
    public class ExtractRefreshTask
    {
        public readonly string datasourceId;
        public readonly string siteId;

        public readonly string extractRefreshId;
        public readonly string extractRefreshPriority;
        public readonly string consecutiveFailedCount;
        public readonly string extractRefreshType;

        public readonly string scheduleId;
        public readonly string scheduleName;
        public readonly string scheduleState;
        public readonly string schedulePriority;
        public readonly string scheduleCreatedAt;
        public readonly string scheduleUpdatedAt;
        public readonly string scheduleType;
        public readonly string scheduleFrequency;
        public readonly string scheduleNextRunAt;



        public ExtractRefreshTask(XmlNamespaceManager nsManager, XmlNode task, string _siteId) {

            var extractRefresh = task.LastChild;
            var schedule = task.LastChild?.FirstChild;
            var datasource = task.LastChild?.LastChild;
            
            datasourceId = datasource?.Attributes?["id"]?.Value;

            extractRefreshId = extractRefresh?.Attributes?["id"]?.Value;
            extractRefreshPriority = extractRefresh?.Attributes?["priority"]?.Value;
            consecutiveFailedCount = extractRefresh?.Attributes?["consecutiveFailedCount"]?.Value;
            extractRefreshType = extractRefresh?.Attributes?["type"]?.Value;

            scheduleId = schedule?.Attributes?["id"]?.Value;
            scheduleName = schedule?.Attributes?["name"]?.Value;
            scheduleState = schedule?.Attributes?["state"]?.Value;
            schedulePriority = schedule?.Attributes?["priority"]?.Value;
            scheduleCreatedAt = schedule?.Attributes?["createdAt"]?.Value;
            scheduleUpdatedAt = schedule?.Attributes?["updatedAt"]?.Value;
            scheduleType = schedule?.Attributes?["type"]?.Value;
            scheduleFrequency = schedule?.Attributes?["frequency"]?.Value;
            scheduleNextRunAt = schedule?.Attributes?["nextRunAt"]?.Value;

            siteId = _siteId;
        }
    }
}
