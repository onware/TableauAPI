using System;
using System.Xml;

namespace TableauAPI.RESTHelpers
{
    /// <summary>
    /// Helper functions for REST API functions that download lists
    /// </summary>
    internal static class DownloadPaginationHelper 
    {
        /// <summary>
        /// Determine the # of pages we need to download from pagination data
        /// </summary>
        /// <param name="xNodePagination"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static int GetNumberOfPagesFromPagination(XmlNode xNodePagination, int pageSize)
        {
            //Sanity check
            if(xNodePagination.Name != "pagination")
            {
                throw new Exception("Internal error - expected XML 'pagination' node: " + xNodePagination.Name);
            }
        
            var totalItemsText = xNodePagination.Attributes?["totalAvailable"].Value;
            var totalItems = System.Convert.ToInt32(totalItemsText);
            var numFullPages = totalItems / pageSize;

            //If we have extra pages that don't align on a page boundary then add one
            if (totalItems % pageSize > 0)
            {
                return numFullPages + 1;
            }

            return numFullPages;
        }
    }
}
