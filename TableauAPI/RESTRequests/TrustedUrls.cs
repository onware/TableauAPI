using System;
using System.Collections.Generic;
using System.Linq;
using TableauAPI.RESTHelpers;

namespace TableauAPI.RESTRequests
{

    /// <summary>
    /// Create a class containing Trusted Urls for use when a client is trusted by a Tableau server.
    /// See https://onlinehelp.tableau.com/current/server/en-us/trusted_auth.htm for information.
    /// </summary>
    public class TrustedUrls : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _workbookId;
        private readonly string _viewId;
        private Dictionary<string, string> _reportParameters;

        /// <summary>
        /// Tabs hidden if true; visible otherwise.
        /// </summary>
        public bool? HideTabs { get; set; }

        /// <summary>
        /// Toolbar hidden if true; visible otherwise.
        /// </summary>
        public bool? HideToolbar { get; set; }

        /// <summary>
        /// Creates an instance of the Trusted URLs API helpers.
        /// </summary>
        /// <param name="workbookId">Workbook ID</param>
        /// <param name="viewId">View ID</param>
        /// <param name="onlineUrls">Tableau Server Connection</param>
        /// <param name="loginInfo">Tableau Sign In Information</param>
        public TrustedUrls(string workbookId, string viewId, TableauServerUrls onlineUrls, TableauServerSignIn loginInfo) : base(loginInfo)
        {
            _onlineUrls = onlineUrls;
            _workbookId = workbookId;
            _viewId = viewId;
        }

        /// <summary>
        /// Add a parameter for a Tableau Server view
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddViewParameter(string name, string value)
        {
            if (_reportParameters == null)
            {
                _reportParameters = new Dictionary<string, string>();
            }

            _reportParameters.Add(name, value);
        }

        /// <summary>
        /// With the provided view parameters, get a URL for a Tableau view to be exported as a PDF.
        /// </summary>
        /// <returns></returns>
        public string GetExportPdfUrl()
        {
            var ticketRequest = new TableauServerTicket(_onlineUrls, OnlineSession);
            var ticket = ticketRequest.Ticket();

            var url = $"{_onlineUrls.ServerUrlWithProtocol}/trusted/{ticket}/t/{_onlineUrls.SiteUrlSegement}/views/{_workbookId}/{_viewId}.pdf";
            url = _AddParamtersToUrl(url);
            return url;
        }

        /// <summary>
        /// With the provided view paramaeters, get a URL for a Tableau view as a preview image.
        /// </summary>
        /// <returns></returns>
        public string GetPreviewImageUrl()
        {
            var ticketRequest = new TableauServerTicket(_onlineUrls, OnlineSession);
            var ticket = ticketRequest.Ticket();

            var url = $"{_onlineUrls.ServerUrlWithProtocol}/trusted/{ticket}/t/{_onlineUrls.SiteUrlSegement}/views/{_workbookId}/{_viewId}.png";
            url = _AddParamtersToUrl(url);
            return url;
        }

        /// <summary>
        /// With the provided view parameters, get a URL for a Tableau view to be exported as a PDF.
        /// </summary>
        /// <returns></returns>
        public string GetTrustedViewUrl()
        {
            var ticketRequest = new TableauServerTicket(_onlineUrls, OnlineSession);
            var ticket = ticketRequest.Ticket();

            var url = $"{_onlineUrls.ServerUrlWithProtocol}/trusted/{ticket}/t/{_onlineUrls.SiteUrlSegement}/views/{_workbookId}/{_viewId}?:embed=yes";
            url = _AddParamtersToUrl(url);
            return url;
        }

        #region Private Methods

        private string _AddParamtersToUrl(string url)
        {
            if (_reportParameters != null && _reportParameters.Any())
            {
                var parameters = string.Join("&", _reportParameters.Select(x => $"{x.Key}={x.Value}"));
                if (!url.Contains("?"))
                {
                    url += "?";
                }
                url += parameters;
            }

            if (HideTabs.GetValueOrDefault())
            {
                if (!url.Contains("?"))
                {
                    url += "?:tabs=no";
                }
                else
                {
                    if (url.Substring(url.Length - 1) != "?")
                    {
                        url += "&";
                    }
                    url += ":tabs=no";
                }
            }

            if (HideToolbar.GetValueOrDefault())
            {
                if (!url.Contains("?"))
                {
                    url += "?:toolbar=no";
                }
                else
                {
                    if (url.Substring(url.Length - 1) != "?")
                    {
                        url += "&";
                    }
                    url += ":toolbar=no";
                }
            }

            return url;
        }

        #endregion

    }
}
