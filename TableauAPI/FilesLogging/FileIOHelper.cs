using System;
using System.IO;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.FilesLogging
{
    /// <summary>
    /// Helper for File IO
    /// </summary>
    internal static class FileIOHelper
    {

        /// <summary>
        /// Ensures the specified path exists 
        /// </summary>
        /// <param name="localPath"></param>
        public static void CreatePathIfNeeded(string localPath)
        {
            if (Directory.Exists(localPath)) return;
            Directory.CreateDirectory(localPath);
        }


        public static string GenerateWindowsSafeFilename(string fileNameIn)
        {
            string fileNameOut = fileNameIn;
            fileNameOut = fileNameOut.Replace("\\", "-SLASH-");
            fileNameOut = fileNameOut.Replace("/", "-SLASH-");
            fileNameOut = fileNameOut.Replace("$", "-DOLLAR-");
            fileNameOut = fileNameOut.Replace("*", "STAR");
            fileNameOut = fileNameOut.Replace("?", "-QQQ-");
            fileNameOut = fileNameOut.Replace("%", "-PERCENT-");
            fileNameOut = fileNameOut.Replace(":", "-COLON-");
            fileNameOut = fileNameOut.Replace("|", "-PIPE-");
            fileNameOut = fileNameOut.Replace("\"", "-QUOTE-");
            fileNameOut = fileNameOut.Replace(">", "-GT-");
            fileNameOut = fileNameOut.Replace("<", "-LT-");
            return fileNameOut;
        }

        /// <summary>
        /// Creates a high-probabilty-unique path based on the current date-time
        /// </summary>
        /// <param name="basePath">File system path where the unique directories will be created.</param>
        /// <param name="createDirectory">true if the method should create the directory; false otherwise.</param>
        /// <param name="newDirectoryPrefix">Prefix for new subdirectories.</param>
        /// <param name="when">DateTime which should be used in the folder name.</param>
        /// <returns></returns>
        public static string PathDateTimeSubdirectory(string basePath, bool createDirectory, string newDirectoryPrefix = "", Nullable<DateTime> when = null)
        {
            //Subdirectory name
            DateTime now;
            if(when.HasValue)
            {
                now = when.Value;
            }
            else
            {
                now = DateTime.Now;
            }

            string subDirectory = now.Year.ToString() + "-" + now.Month.ToString("00") + "-" + now.Day.ToString("00") + "-" + now.Hour.ToString("00") + now.Minute.ToString("00") + "-" + now.Second.ToString("00");
            if(!string.IsNullOrWhiteSpace(newDirectoryPrefix))
            {
                subDirectory = newDirectoryPrefix + subDirectory;
            }

            //Combined path
            string fullPathToDateTime = Path.Combine(basePath, subDirectory);
            //Create if specified
            if (createDirectory)
            {
                CreatePathIfNeeded(fullPathToDateTime);
            }
            return fullPathToDateTime;
        }


        /// <summary>
        /// Gives us a high probability unique file name
        /// </summary>
        /// <param name="baseName">Filename to be used for generated files.</param>
        /// <param name="when">DateTime value to be used in the generated filename.</param>
        /// <returns></returns>
        public static string FilenameWithDateTimeUnique(string baseName, Nullable<DateTime> when = null)
        {
            string rootName = Path.GetFileNameWithoutExtension(baseName);
            string extension = Path.GetExtension(baseName);

            //Subdirectory name

            DateTime now;
            if(when.HasValue)
            {
                now = when.Value;
            }
            else
            {
                now = DateTime.Now;
            }

            string subNameDateTime = now.Year.ToString() + "-" + now.Month.ToString("00") + "-" + now.Day.ToString("00") + "-" + now.Hour.ToString("00") + now.Minute.ToString("00") + "-" + now.Second.ToString("00");

            //Combined path
            return rootName + "_" + subNameDateTime + extension;
        }

        /// <summary>
        /// If we have Project Mapping information, generate a project based path for the download
        /// </summary>
        /// <param name="basePath">File system location which will be the root of project paths.</param>
        /// <param name="projectList">Collection of projects which this method will ensure paths exist.</param>
        /// <param name="project">Project record.</param>
        /// <param name="statusLog">Logging object.</param>
        /// <returns></returns>
        public static string EnsureProjectBasedPath(string basePath, IProjectsList projectList, IHasProjectId project, TaskStatusLogs statusLog)
        {
            //If we have no project list to do lookups in then just return the base path
            if (projectList == null) return basePath;

            //Look up the project name
            var projWithId = projectList.FindProjectWithId(project.ProjectId);
            if(projWithId == null)
            {
                statusLog.AddError("Project not found with id " + project.ProjectId);
                return basePath;
            }

            //Turn the project name into a directory name
            var safeDirectoryName = GenerateWindowsSafeFilename(projWithId.Name);

            var pathWithProject = Path.Combine(basePath, safeDirectoryName);
            //If needed, create the directory
            if(!Directory.Exists(pathWithProject))
            {
                Directory.CreateDirectory(pathWithProject);
            }

            return pathWithProject;
        }

        /// <summary>
        /// If we have Project Mapping information, generate a project based path for the download
        /// </summary>
        /// <param name="basePath">File system location which will be the root of project paths.</param>
        /// <param name="project">Project record.</param>
        /// <param name="statusLog">Logging object.</param>
        /// <returns></returns>
        public static string EnsureProjectBasedPath(string basePath, SiteProject project, TaskStatusLogs statusLog)
        {
            //If we have no project list to do lookups in then just return the base path
            if (project == null)
            {
                return basePath;
            }
            
            //Turn the project name into a directory name
            var safeDirectoryName = GenerateWindowsSafeFilename(project.Name);

            var pathWithProject = Path.Combine(basePath, safeDirectoryName);

            //If needed, create the directory
            if (!Directory.Exists(pathWithProject))
            {
                Directory.CreateDirectory(pathWithProject);
            }

            return pathWithProject;
        }

        /// <summary>
        /// If we have Project Mapping information, generate a project based path for the download
        /// </summary>
        /// <param name="basePath">File system location which will be the root of project paths.</param>
        /// <param name="workbook">Workbook record.</param>
        /// <param name="statusLog">Logging object.</param>
        /// <returns></returns>
        public static string EnsureProjectBasedPath(string basePath, SiteWorkbook workbook, TaskStatusLogs statusLog)
        {
            //If we have no project list to do lookups in then just return the base path
            if (workbook == null)
            {
                return basePath;
            }

            //Turn the project name into a directory name
            var safeDirectoryName = GenerateWindowsSafeFilename(workbook.Name);

            var pathWithProject = Path.Combine(basePath, safeDirectoryName);

            //If needed, create the directory
            if (!Directory.Exists(pathWithProject))
            {
                Directory.CreateDirectory(pathWithProject);
            }

            return pathWithProject;
        }
    }
}
