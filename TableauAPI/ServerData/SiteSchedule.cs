using System;
using System.Text;
using System.Xml;
using TableauAPI.FilesLogging;

namespace TableauAPI.ServerData
{
    public class SiteSchedule
    {
        public readonly string Id;
        public readonly string name;
        public readonly string state;
        public readonly string priority;
        public readonly string createdAt;
        public readonly string updatedAt;
        public readonly string scheduleType;
        public readonly string frequency;
        public readonly string nextRunAt;
        public readonly string executionOrder;
        public SiteSchedule(XmlNode ScheduleNode)
        {
            if (ScheduleNode.Name.ToLower() != "schedule") {
                throw new Exception("Unexpected content - not schedule");
            }
            if (ScheduleNode.Attributes != null && ScheduleNode.Attributes["id"] != null)
            {
                Id = ScheduleNode.Attributes?["id"].Value;
            }
            if (ScheduleNode.Attributes != null && ScheduleNode.Attributes["name"] != null)
            {
                name = ScheduleNode.Attributes?["name"].Value;
            }
            if (ScheduleNode.Attributes != null && ScheduleNode.Attributes["state"] != null)
            {
                state = ScheduleNode.Attributes?["state"].Value;
            }
            if (ScheduleNode.Attributes != null && ScheduleNode.Attributes["priority"] != null)
            {
                priority = ScheduleNode.Attributes?["priority"].Value;
            }
            if (ScheduleNode.Attributes != null && ScheduleNode.Attributes["createdAt"] != null)
            {
                createdAt = ScheduleNode.Attributes?["createdAt"].Value;
            }
            if (ScheduleNode.Attributes != null && ScheduleNode.Attributes["updatedAt"] != null)
            {
                updatedAt = ScheduleNode.Attributes?["updatedAt"].Value;
            }
            if (ScheduleNode.Attributes != null && ScheduleNode.Attributes["type"] != null)
            {
                scheduleType = ScheduleNode.Attributes?["type"].Value;
            }
            if (ScheduleNode.Attributes != null && ScheduleNode.Attributes["frequency"] != null)
            {
                frequency = ScheduleNode.Attributes?["frequency"].Value;
            }
            if (ScheduleNode.Attributes != null && ScheduleNode.Attributes["nextRunAt"] != null)
            {
                nextRunAt = ScheduleNode.Attributes?["nextRunAt"].Value;
            }
            if (ScheduleNode.Attributes != null && ScheduleNode.Attributes["executionOrder"] != null)
            {
                executionOrder = ScheduleNode.Attributes?["executionOrder"].Value;
            }
        }

    }
}
