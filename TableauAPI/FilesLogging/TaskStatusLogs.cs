namespace TableauAPI.FilesLogging
{
    /// <summary>
    /// Records status of a set of API calls. Contains a log of statuses, and a log specific to errors thrown by the
    /// REST API.
    /// </summary>
    public class TaskStatusLogs
    {
        readonly Logger _statusLog = new Logger();
        readonly Logger _errorLog = new Logger();
        private int _minimumStatusLevel;

        /// <summary>
        /// Sets the logging level.
        /// </summary>
        /// <param name="statusLevel"></param>
        public void SetStatusLoggingLevel(int statusLevel)
        {
            _minimumStatusLevel = statusLevel;
        }

        /// <summary>
        /// Returns the current Status as text.
        /// </summary>
        public string StatusText => _statusLog.StatusText;

        /// <summary>
        /// Add a header/splitter line
        /// </summary>
        public void AddStatusHeader(string headerText, int statusLevel = 0)
        {
            AddStatus("****************************************************************", statusLevel);
            AddStatus(headerText, statusLevel);
            AddStatus("****************************************************************", statusLevel);
        }

        /// <summary>
        /// Add a Status to the Log.
        /// </summary>
        /// <param name="statusText"></param>
        /// <param name="statusLevel"></param>
        /// <remarks>If no status level is provided, a default value of 0 is used.</remarks>
        public void AddStatus(string statusText, int statusLevel = 0)
        {
            if(statusLevel >= _minimumStatusLevel)
            {
                //Indent the lower status items
                string prefixText = "";
                if (statusLevel < 0) { prefixText = "     "; }

                _statusLog.AddStatus(prefixText + statusText);
            }
        }

        /// <summary>
        /// The current number of errors logged.
        /// </summary>
        public int ErrorCount => _errorLog.Count;

        /// <summary>
        /// Text of errors logged.
        /// </summary>
        public string ErrorText => _errorLog.StatusText;

        /// <summary>
        /// Adds an Error to the Error Log.
        /// </summary>
        /// <param name="errorText"></param>
        public void AddError(string errorText)
        {
            _statusLog.AddStatus("Error: " + errorText);
            _errorLog.AddStatus(errorText);
        }
    }
}
