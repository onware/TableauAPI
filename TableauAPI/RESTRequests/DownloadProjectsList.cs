using System;
using System.Collections.Generic;
using System.Xml;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.ServerData;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Request for a list of Projects form the Tableau REST API
    /// </summary>
    public class DownloadProjectsList : TableauServerSignedInRequestBase, IProjectsList
    {

        private readonly TableauServerUrls _onlineUrls;
        private List<SiteProject> _projects;

        /// <summary>
        /// Projects we've parsed from server results
        /// </summary>
        public IEnumerable<SiteProject> Projects
        {
            get
            {
                var ds = _projects;
                return ds?.AsReadOnly();
            }
        }

        /// <summary>
        /// Create an instance of a request for Projects
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="login"></param>
        public DownloadProjectsList(TableauServerUrls onlineUrls, TableauServerSignIn login)
            : base(login)
        {
            _onlineUrls = onlineUrls;
        }

        /// <summary>
        /// Execute the request for the list of Projects
        /// </summary>
        public void ExecuteRequest()
        {
            var onlineProjects = new List<SiteProject>();

            int numberPages = 1; //Start with 1 page (we will get an updated value from server)
            //Get subsequent pages
            for (int thisPage = 1; thisPage <= numberPages; thisPage++)
            {
                try
                {
                    ExecuteRequest_ForPage(onlineProjects, thisPage, out numberPages);
                }
                catch (Exception exPageRequest)
                {
                    StatusLog.AddError("Projects error during page request: " + exPageRequest.Message);
                }
            }

            _projects = onlineProjects;
        }

        /// <summary>
        /// Finds a project with matching name
        /// </summary>
        /// <param name="findProjectName"></param>
        /// <returns></returns>
        public SiteProject FindProjectWithName(string findProjectName)
        {
            foreach (var proj in _projects)
            {
                if (proj.Name == findProjectName)
                {
                    return proj;
                }
            }

            return null; //Not found
        }

        /// <summary>
        /// Returns a Project with a given ID
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        SiteProject IProjectsList.FindProjectWithId(string projectId)
        {
            foreach (var proj in _projects)
            {
                if (proj.Id == projectId) { return proj; }
            }

            return null;
        }

        /// <summary>
        /// Adds a project to the list
        /// </summary>
        /// <param name="newProject"></param>
        internal void AddProject(SiteProject newProject)
        {
            _projects.Add(newProject);
        }
        
        #region Private Methods

        /// <summary>
        /// Get a page's worth of Projects listing
        /// </summary>
        /// <param name="onlineProjects"></param>
        /// <param name="pageToRequest">Page # we are requesting (1 based)</param>
        /// <param name="totalNumberPages">Total # of pages of data that Server can return us</param>
        private void ExecuteRequest_ForPage(List<SiteProject> onlineProjects, int pageToRequest, out int totalNumberPages)
        {
            int pageSize = _onlineUrls.PageSize;
            //Create a web request, in including the users logged-in auth information in the request headers
            var urlQuery = _onlineUrls.Url_ProjectsList(OnlineSession, pageSize, pageToRequest);
            var webRequest = CreateLoggedInWebRequest(urlQuery);
            webRequest.Method = "GET";

            OnlineSession.StatusLog.AddStatus("Web request: " + urlQuery, -10);
            var response = GetWebResponseLogErrors(webRequest, "get projects list");
            var xmlDoc = GetWebResponseAsXml(response);

            //Get all the project nodes
            var nsManager = XmlHelper.CreateTableauXmlNamespaceManager("iwsOnline");
            var projects = xmlDoc.SelectNodes("//iwsOnline:project", nsManager);

            //Get information for each of the data sources
            foreach (XmlNode itemXml in projects)
            {
                try
                {
                    var proj = new SiteProject(itemXml);
                    onlineProjects.Add(proj);

                    _SanityCheckProject(proj, itemXml);
                }
                catch
                {
                    AppDiagnostics.Assert(false, "Project parse error");
                    OnlineSession.StatusLog.AddError("Error parsing project: " + itemXml.OuterXml);
                }
            } //end: foreach

            //-------------------------------------------------------------------
            //Get the updated page-count
            //-------------------------------------------------------------------
            totalNumberPages = DownloadPaginationHelper.GetNumberOfPagesFromPagination(
                xmlDoc.SelectSingleNode("//iwsOnline:pagination", nsManager),
                pageSize);
        }

        /// <summary>
        /// Does sanity checking and error logging on missing data in projects
        /// </summary>
        private void _SanityCheckProject(SiteProject project, XmlNode xmlNode)
        {
            if (string.IsNullOrWhiteSpace(project.Id))
            {
                OnlineSession.StatusLog.AddError(project.Name + " is missing a project ID. Not returned from server! xml=" + xmlNode.OuterXml);
            }
        }

        #endregion
        
    }
}
