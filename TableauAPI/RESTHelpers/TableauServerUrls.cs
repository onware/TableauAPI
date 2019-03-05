using System;
using TableauAPI.FilesLogging;
using TableauAPI.RESTRequests;
using TableauAPI.ServerData;

namespace TableauAPI.RESTHelpers
{
    /// <summary>
    /// Creates the set of server specific URLs
    /// </summary>
    public class TableauServerUrls : ITableauServerSiteInfo
    {
        /// <summary>
        /// What version of Server do we thing we are talking to? (URLs and APIs may differ)
        /// </summary>
        public ServerVersion ServerVersion { get; }

        /// <summary>
        /// Url for API login
        /// </summary>
        public readonly string UrlLogin;

        /// <summary>
        /// Template for URL to acess workbooks list
        /// </summary>
        private readonly string _urlListWorkbooksForUserTemplate;
        private readonly string _urlViewThumbnailTemplate;
        private readonly string _urlViewDataTemplate;
        private readonly string _urlViewsListForSiteTemplate;
        private readonly string _urlWorkbookTemplate;
        private readonly string _urlListViewsForWorkbookTemplate;
        private readonly string _urlListWorkbookConnectionsTemplate;
        private readonly string _urlListDatasourcesTemplate;
        private readonly string _urlListProjectsTemplate;
        private readonly string _urlListGroupsTemplate;
        private readonly string _urlListUsersTemplate;
        private readonly string _urlListUsersInGroupTemplate;
        private readonly string _urlDownloadWorkbookTemplate;
        private readonly string _urlDownloadDatasourceTemplate;
        private readonly string _urlDatasourceConnectionsTemplate;
        private readonly string _urlSiteInfoTemplate;
        private readonly string _urlInitiateUploadTemplate;
        private readonly string _urlAppendUploadChunkTemplate;
        private readonly string _urlFinalizeUploadDatasourceTemplate;
        private readonly string _urlFinalizeUploadWorkbookTemplate;
        private readonly string _urlCreateProjectTemplate;
        private readonly string _urlDeleteWorkbookTagTemplate;
        private readonly string _urlDeleteDatasourceTagTemplate;
        private readonly string _urlUpdateUserTemplate;

        /// <summary>
        /// Server url with protocol
        /// </summary>
        public readonly string ServerUrlWithProtocol;

        /// <summary>
        /// String representation of Server Protocol
        /// </summary>
        public readonly ServerProtocol ServerProtocol;

        /// <summary>
        /// Part of the URL that designates the site id
        /// </summary>
        public readonly string SiteUrlSegement;

        /// <summary>
        /// Server Name
        /// </summary>
        public readonly string ServerName;

        /// <summary>
        /// Page Size when dealing with large result sets
        /// </summary>
        public readonly int PageSize;

        /// <summary>
        /// Size of chunks uploaded to Tableau server
        /// </summary>
        public const int UploadFileChunkSize = 8000000; //8MB

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverName">Server IP, hostname or FQDN</param>
        /// <param name="siteName">Tableau Site Name</param>
        /// <param name="protocol">HTTP protocol</param>
        /// <param name="pageSize">Page size, defaults to 1000</param>
        /// <param name="serverVersion">Tableau Server version</param>
        public TableauServerUrls(ServerProtocol protocol, string serverName, string siteName, int pageSize = 1000, ServerVersion serverVersion = ServerVersion.Server9)
        {
            PageSize = 1000;
            ServerProtocol = protocol;

            PageSize = pageSize;
            var serverNameWithProtocol = (protocol == ServerProtocol.Http ? "http://" : "https://") + serverName;
            ServerVersion = serverVersion;
            SiteUrlSegement = siteName;
            ServerName = serverName;
            ServerUrlWithProtocol = serverNameWithProtocol;
            UrlLogin = serverNameWithProtocol + "/api/3.3/auth/signin";
            _urlListWorkbooksForUserTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/users/{{iwsUserId}}/workbooks?pageSize={{iwsPageSize}}&pageNumber={{iwsPageNumber}}";
            _urlViewsListForSiteTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/views?pageSize={{iwsPageSize}}&pageNumber={{iwsPageNumber}}";
            _urlViewThumbnailTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/workbooks/{{iwsWorkbookId}}/views/{{iwsViewId}}/previewImage";
            _urlViewDataTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/views/{{iwsViewId}}/data";
            _urlListViewsForWorkbookTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/workbooks/{{iwsWorkbookId}}/views";
            _urlListWorkbookConnectionsTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/workbooks/{{iwsWorkbookId}}/connections";
            _urlWorkbookTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/workbooks/{{iwsWorkbookId}}";
            _urlListDatasourcesTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/datasources?pageSize={{iwsPageSize}}&pageNumber={{iwsPageNumber}}";
            _urlListProjectsTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/projects?pageSize={{iwsPageSize}}&pageNumber={{iwsPageNumber}}";
            _urlListGroupsTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/groups?pageSize={{iwsPageSize}}&pageNumber={{iwsPageNumber}}";
            _urlListUsersTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/users?pageSize={{iwsPageSize}}&pageNumber={{iwsPageNumber}}";
            _urlListUsersInGroupTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/groups/{{iwsGroupId}}/users?pageSize={{iwsPageSize}}&pageNumber={{iwsPageNumber}}";
            _urlDownloadDatasourceTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/datasources/{{iwsRepositoryId}}/content";
            _urlDatasourceConnectionsTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/datasources/{{iwsRepositoryId}}/connections";
            _urlDownloadWorkbookTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/workbooks/{{iwsRepositoryId}}/content";
            _urlSiteInfoTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}";
            _urlInitiateUploadTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/fileUploads";
            _urlAppendUploadChunkTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/fileUploads/{{iwsUploadSession}}";
            _urlFinalizeUploadDatasourceTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/datasources?uploadSessionId={{iwsUploadSession}}&datasourceType={{iwsDatasourceType}}&overwrite=true";
            _urlFinalizeUploadWorkbookTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/workbooks?uploadSessionId={{iwsUploadSession}}&workbookType={{iwsWorkbookType}}&overwrite=true";
            _urlCreateProjectTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/projects";
            _urlDeleteWorkbookTagTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/workbooks/{{iwsWorkbookId}}/tags/{{iwsTagText}}";
            _urlDeleteDatasourceTagTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/datasources/{{iwsDatasourceId}}/tags/{{iwsTagText}}";
            _urlUpdateUserTemplate = serverNameWithProtocol + "/api/3.3/sites/{{iwsSiteId}}/users/{{iwsUserId}}";

            //Any server version specific things we want to do?
            switch (serverVersion)
            {
                case ServerVersion.Server9:
                    break;
                default:
                    AppDiagnostics.Assert(false, "Unknown server version");
                    throw new Exception("Unknown server version");
            }
        }

        private static ServerProtocol _GetProtocolFromUrl(string url)
        {
            const string protocolIndicator = "://";
            int idxProtocol = url.IndexOf(protocolIndicator, StringComparison.Ordinal);
            if (idxProtocol < 1)
            {
                throw new Exception("No protocol found in " + url);
            }

            string protocol = url.Substring(0, idxProtocol + protocolIndicator.Length);

            return protocol.ToLower().Equals("https") ? ServerProtocol.Https : ServerProtocol.Http;
        }

        /// <summary>
        /// Parse out the server-user and site name from the content URL
        /// </summary>
        /// <param name="userContentUrl">e.g. https://online.tableausoftware.com/t/tableausupport/workbooks </param>
        /// <param name="pageSize">Size of page to use when interacting with the Tableau Server</param>
        /// <returns></returns>
        public static TableauServerUrls FromContentUrl(string userContentUrl, int pageSize)
        {
            userContentUrl = userContentUrl.Trim();
            var foundProtocol = _GetProtocolFromUrl(userContentUrl);

            //Find where the server name ends
            string urlAfterProtocol = userContentUrl.Substring(userContentUrl.IndexOf("://", StringComparison.Ordinal) + 3);
            var urlParts = urlAfterProtocol.Split('/');
            string serverName = urlParts[0];

            string siteUrlSegment;
            ServerVersion serverVersion;
            //Check for the site specifier.  Infer the server version based on this URL
            if ((urlParts[1] == "#") && (urlParts[2] == "site"))
            {
                siteUrlSegment = urlParts[3];
                serverVersion = ServerVersion.Server9;
            }
            else if (urlParts[1] == "#")
            {
                siteUrlSegment = ""; //Default site
                serverVersion = ServerVersion.Server9;
            }
            else
            {
                throw new Exception("Could not infer version of Tableau Server.");
            }

            return new TableauServerUrls(foundProtocol, serverName, siteUrlSegment, pageSize, serverVersion);
        }

        /// <summary>
        /// The URL to get site info
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <returns></returns>
        public string Url_SiteInfo(TableauServerSignIn logInInfo)
        {
            string workingText = _urlSiteInfoTemplate.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// The URL to start an upload
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <returns></returns>
        public string Url_InitiateFileUpload(TableauServerSignIn logInInfo)
        {
            string workingText = _urlInitiateUploadTemplate.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// The URL to continue an upload
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="uploadSessionId">ID for the upload session</param>
        /// <returns></returns>
        public string Url_AppendFileUploadChunk(TableauServerSignIn logInInfo, string uploadSessionId)
        {
            string workingText = _urlAppendUploadChunkTemplate.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsUploadSession}}", uploadSessionId);
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }


        /// <summary>
        /// URL to finish publishing a datasource
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="uploadSessionId">ID for the upload session</param>
        /// <param name="datasourceType">Data Source Type: one of tds, tdsx, or tde</param>
        /// <returns></returns>
        public string Url_FinalizeDataSourcePublish(TableauServerSignIn logInInfo, string uploadSessionId, string datasourceType)
        {

            string workingText = _urlFinalizeUploadDatasourceTemplate.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsUploadSession}}", uploadSessionId);
            workingText = workingText.Replace("{{iwsDatasourceType}}", datasourceType);
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL to finish publishing a workbook
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="uploadSessionId">ID for the upload session</param>
        /// <param name="workbookType">Workbook Type: one of twb or twbx</param>
        /// <returns></returns>
        public string Url_FinalizeWorkbookPublish(TableauServerSignIn logInInfo, string uploadSessionId, string workbookType)
        {

            string workingText = _urlFinalizeUploadWorkbookTemplate.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsUploadSession}}", uploadSessionId);
            workingText = workingText.Replace("{{iwsWorkbookType}}", workbookType);
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for the View thumbnail.
        /// </summary>
        /// <param name="workbookId">Workbook ID</param>
        /// <param name="viewId">View ID</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <returns></returns>
        public string Url_ViewThumbnail(string workbookId, string viewId, TableauServerSignIn logInInfo)
        {
            var workingText = _urlViewThumbnailTemplate.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsWorkbookId}}", workbookId);
            workingText = workingText.Replace("{{iwsViewId}}", viewId);
            _ValidateTemplateReplaceComplete(workingText);
            return workingText;
        }

        /// <summary>
        /// URL for the View Data.
        /// </summary>
        /// <param name="viewId">View ID</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <returns></returns>
        public string Url_ViewData(string viewId, TableauServerSignIn logInInfo)
        {
            var workingText = _urlViewDataTemplate.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsViewId}}", viewId);
            _ValidateTemplateReplaceComplete(workingText);
            return workingText;
        }

        /// <summary>
        /// URL for the Views list
        /// </summary>
        /// <param name="workbookId">Workbook ID</param>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <returns></returns>
        public string Url_ViewsListForWorkbook(string workbookId, TableauServerSignIn logInInfo)
        {
            var workingText = _urlListViewsForWorkbookTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsWorkbookId}}", workbookId);
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }


        /// <summary>
        /// URL for the Views list
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="pageSize">Page size to use when retrieving results from Tableau server</param>
        /// <param name="pageNumber">Which page of the results to return. Defaults to 1.</param>
        /// <returns></returns>
        public string Url_ViewsListForSite(TableauServerSignIn logInInfo, int pageSize, int pageNumber = 1)
        {
            string workingText = _urlViewsListForSiteTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsPageSize}}", pageSize.ToString());
            workingText = workingText.Replace("{{iwsPageNumber}}", pageNumber.ToString());
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for the Workbooks list
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="userId">User ID who's workbooks we should retrieve</param>
        /// <param name="pageSize">Size of result set to retrieve from Tableau Server</param>
        /// <param name="pageNumber">Which page of the results to return. Defaults to 1.</param>
        /// <returns></returns>
        public string Url_WorkbooksListForUser(TableauServerSignIn logInInfo, string userId, int pageSize, int pageNumber = 1)
        {
            string workingText = _urlListWorkbooksForUserTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsUserId}}", userId);
            workingText = workingText.Replace("{{iwsPageSize}}", pageSize.ToString());
            workingText = workingText.Replace("{{iwsPageNumber}}", pageNumber.ToString());
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for the Workbook's data source connections list
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="workbookId">Workbook ID</param>
        /// <returns></returns>
        public string Url_Workbook(TableauServerSignIn logInInfo, string workbookId)
        {
            string workingText = _urlWorkbookTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsWorkbookId}}", workbookId);
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for the Workbook's data source connections list
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="workbookId">Workbook ID</param>
        /// <returns></returns>
        public string Url_WorkbookConnectionsList(TableauServerSignIn logInInfo, string workbookId)
        {
            string workingText = _urlListWorkbookConnectionsTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsWorkbookId}}", workbookId);
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for the Workbook's data source connections list
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="datasourceId">Datasource ID</param>
        /// <returns></returns>
        public string Url_DatasourceConnectionsList(TableauServerSignIn logInInfo, string datasourceId)
        {
            string workingText = _urlDatasourceConnectionsTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsRepositoryId}}", datasourceId);
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for the Datasources list
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="pageSize">Page size to use when retrieving results from Tableau server</param>
        /// <param name="pageNumber">Which page of the results to return. Defaults to 1.</param>
        /// <returns></returns>
        public string Url_DatasourcesList(TableauServerSignIn logInInfo, int pageSize, int pageNumber = 1)
        {
            string workingText = _urlListDatasourcesTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsPageSize}}", pageSize.ToString());
            workingText = workingText.Replace("{{iwsPageNumber}}", pageNumber.ToString());
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for creating a project
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <returns></returns>
        public string Url_CreateProject(TableauServerSignIn logInInfo)
        {
            string workingText = _urlCreateProjectTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for deleting a tag from a workbook
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="workbookId">Workbook ID</param>
        /// <param name="tagText">Tag we want to delete</param>
        /// <returns></returns>
        public string Url_DeleteWorkbookTag(TableauServerSignIn logInInfo, string workbookId, string tagText)
        {
            string workingText = _urlDeleteWorkbookTagTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsWorkbookId}}", workbookId);
            workingText = workingText.Replace("{{iwsTagText}}", tagText);
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for deleting a tag from a datasource
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="datasourceId">Data Source ID</param>
        /// <param name="tagText">Tag we want to delete</param>
        /// <returns></returns>
        public string Url_DeleteDatasourceTag(TableauServerSignIn logInInfo, string datasourceId, string tagText)
        {
            string workingText = _urlDeleteDatasourceTagTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsDatasourceId}}", datasourceId);
            workingText = workingText.Replace("{{iwsTagText}}", tagText);
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }


        /// <summary>
        /// URL for the Projects list
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="pageSize">Page size to use when retrieving results from Tableau server</param>
        /// <param name="pageNumber">Which page of the results to return. Defaults to 1.</param>
        /// <returns></returns>
        public string Url_ProjectsList(TableauServerSignIn logInInfo, int pageSize, int pageNumber = 1)
        {
            string workingText = _urlListProjectsTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsPageSize}}", pageSize.ToString());
            workingText = workingText.Replace("{{iwsPageNumber}}", pageNumber.ToString());
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for the Groups list
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="pageSize">Page size to use when retrieving results from Tableau server</param>
        /// <param name="pageNumber">Which page of the results to return. Defaults to 1.</param>
        /// <returns></returns>
        public string Url_GroupsList(TableauServerSignIn logInInfo, int pageSize, int pageNumber = 1)
        {
            string workingText = _urlListGroupsTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsPageSize}}", pageSize.ToString());
            workingText = workingText.Replace("{{iwsPageNumber}}", pageNumber.ToString());
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL for the Users list
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="pageSize">Page size to use when retrieving results from Tableau server</param>
        /// <param name="pageNumber">Which page of the results to return. Defaults to 1.</param>/// <returns></returns>
        public string Url_UsersList(TableauServerSignIn logInInfo, int pageSize, int pageNumber = 1)
        {
            string workingText = _urlListUsersTemplate.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsPageSize}}", pageSize.ToString());
            workingText = workingText.Replace("{{iwsPageNumber}}", pageNumber.ToString());
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL to get the list of Users in a Group
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="groupId">Group ID</param>
        /// <param name="pageSize">Page size to use when retrieving results from Tableau server</param>
        /// <param name="pageNumber">Which page of the results to return. Defaults to 1.</param>/// <returns></returns>
        public string Url_UsersListInGroup(TableauServerSignIn logInInfo, string groupId, int pageSize, int pageNumber = 1)
        {
            string workingText = _urlListUsersInGroupTemplate.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsGroupId}}", groupId);
            workingText = workingText.Replace("{{iwsPageSize}}", pageSize.ToString());
            workingText = workingText.Replace("{{iwsPageNumber}}", pageNumber.ToString());
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL to get update a user
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="userId">User ID</param>
        public string Url_UpdateUser(TableauServerSignIn logInInfo, string userId)
        {
            string workingText = _urlUpdateUserTemplate.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsUserId}}", userId);
            _ValidateTemplateReplaceComplete(workingText);

            return workingText;
        }

        /// <summary>
        /// URL to download a workbook
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="workbook">Tableau Workbook</param>
        /// <returns></returns>
        public string Url_WorkbookDownload(TableauServerSignIn logInInfo, SiteWorkbook workbook)
        {
            string workingText = _urlDownloadWorkbookTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsRepositoryId}}", workbook.Id);

            _ValidateTemplateReplaceComplete(workingText);
            return workingText;
        }

        /// <summary>
        /// URL to download a datasource
        /// </summary>
        /// <param name="logInInfo">Tableau Sign In Information</param>
        /// <param name="datasource">Tableau Data Source</param>
        /// <returns></returns>
        public string Url_DatasourceDownload(TableauServerSignIn logInInfo, SiteDatasource datasource)
        {
            string workingText = _urlDownloadDatasourceTemplate;
            workingText = workingText.Replace("{{iwsSiteId}}", logInInfo.SiteId);
            workingText = workingText.Replace("{{iwsRepositoryId}}", datasource.Id);

            _ValidateTemplateReplaceComplete(workingText);
            return workingText;
        }


        string ITableauServerSiteInfo.ServerName => ServerName;

        string ITableauServerSiteInfo.SiteId => SiteUrlSegement;

        string ITableauServerSiteInfo.ServerNameWithProtocol => ServerUrlWithProtocol;

        #region Private Methods

        /// <summary>
        /// Returns true if the all required parameters are filled in; false otherwise.
        /// </summary>
        /// <param name="str">URL string</param>
        /// <returns></returns>
        private static bool _ValidateTemplateReplaceComplete(string str)
        {
            if (str.Contains("{{iws"))
            {
                AppDiagnostics.Assert(false, "Template has incomplete parts that need to be replaced");
                return false;
            }

            return true;
        }

        #endregion

    }
}
