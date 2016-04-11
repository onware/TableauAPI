using System;
using TableauAPI.RESTHelpers;

namespace TableauAPI.RESTRequests
{
    /// <summary>
    /// Sends a request to delete a tag from a site's workbook
    /// </summary>
    public class SendDeleteWorkbookTag : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _contentId;
        private readonly string _tagText;

        /// <summary>
        /// Create an instance of a Delete Workbook Tag request
        /// </summary>
        /// <param name="onlineUrls">Tableau Server Information</param>
        /// <param name="logInInfoIn">Tableau Sign In Information</param>
        /// <param name="workbookId">Workbook ID</param>
        /// <param name="tagText">Tag</param>
        public SendDeleteWorkbookTag(
            TableauServerUrls onlineUrls,
            TableauServerSignIn logInInfoIn,
            string workbookId,
            string tagText)
            : base(logInInfoIn)
        {
            if (string.IsNullOrWhiteSpace(tagText))
            {
                throw new ArgumentException("Not allowed to delete a blank tag");
            }

            if (string.IsNullOrWhiteSpace(workbookId))
            {
                throw new ArgumentException("Not allowed to delete a tag without workbook id");
            }

            _onlineUrls = onlineUrls;
            _contentId = workbookId;
            _tagText = tagText;
        }

        /// <summary>
        /// Delete the tag from the workbook.
        /// </summary>
        public void ExecuteRequest()
        {
            try
            {
                //Attempt the delete
                _DeleteTagFromContent(_contentId, _tagText);
                StatusLog.AddStatus("Tag deleted from workbook " + _contentId + "/" + _tagText);
            }
            catch (Exception exProject)
            {
                StatusLog.AddError("Error attempting to delete content tag " + _contentId + "/" + _tagText + "', " + exProject.Message);
            }
        }

        #region Private Methods

        private void _DeleteTagFromContent(string workbookId, string tagText)
        {
            //ref: http://onlinehelp.tableau.com/current/api/rest_api/en-us/help.htm#REST/rest_api_ref.htm#Delete_Tag_from_Workbook%3FTocPath%3DAPI%2520Reference%7C_____20

            //Create a web request 
            var urlDeleteContentTag = _onlineUrls.Url_DeleteWorkbookTag(OnlineSession, workbookId, tagText);
            var webRequest = CreateLoggedInWebRequest(urlDeleteContentTag, "DELETE");
            GetWebResponseLogErrors(webRequest, "delete tag from content request");
        }

        #endregion

    }
}
