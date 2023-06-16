using System;

namespace TableauAPI.FilesLogging
{
    public interface ITaskStatusLogger
    {
        void AddError(string errorText);
        void AddError(string errorText, Exception ex);
        void AddError(string errorText, Exception ex, string extraData);
        void AddStatus(string statusText, int statusLevel = 0);
    }
}