using System.IO;

namespace TableauAPI.FilesLogging
{
    /// <summary>
    /// Paths we care about
    /// </summary>
    internal static class PathHelper
    {
        /// <summary>
        /// Path to the applicaiton
        /// </summary>
        /// <returns></returns>
        public static string GetApplicaitonPath()
        {
            //Gets the path to the application
            return System.Reflection.Assembly.GetExecutingAssembly().Location;
        }


        /// <summary>
        /// Get's the directory the application is running in
        /// </summary>
        /// <returns></returns>
        public static string GetApplicaitonDirectory()
        {
            return Path.GetDirectoryName(GetApplicaitonPath());
        }

        /// <summary>
        /// Path to the template file we want to use for the Inventory Workbook
        /// </summary>
        /// <returns></returns>
        public static string GetInventoryTwbTemplatePath()
        {
            return Path.Combine(GetApplicaitonDirectory(), "_SampleFiles\\SiteInventory.twb");
        }

        /// <summary>
        /// Inventory *.twb files are named to match the *.csv files they use.  
        /// This function generates the *.twb name/path based on the *.csv name/path
        /// </summary>
        /// <param name="pathCsv"></param>
        /// <returns></returns>
        public static string GetInventoryTwbPathMatchingCsvPath(string pathCsv)
        {
            var pathDir = Path.GetDirectoryName(pathCsv);
            if (pathDir == null)
                throw new DirectoryNotFoundException();
            var fileNameNoExtension = Path.GetFileNameWithoutExtension(pathCsv);
            var pathTwbOut = Path.Combine(pathDir, fileNameNoExtension + ".twb");
            return pathTwbOut;
        }
    }
}
