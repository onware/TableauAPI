using System;
using System.Collections.Generic;
using System.Linq;
using TableauAPI.RESTHelpers;

namespace TableauAPI.RESTRequests
{
    public class TrustedUrls : TableauServerSignedInRequestBase
    {
        private readonly TableauServerUrls _onlineUrls;
        private readonly string _workbookId;
        private readonly string _viewId;

        private Dictionary<string, string> _reportParameters;

        public TrustedUrls(string workbookId, string viewId, TableauServerUrls onlineUrls, TableauServerSignIn login) : base(login)
        {
            _onlineUrls = onlineUrls;
            _workbookId = workbookId;
            _viewId = viewId;
        }

        public void AddReportParameter(string name, string value)
        {
            if (_reportParameters == null)
            {
                _reportParameters = new Dictionary<string, string>();
            }

            _reportParameters.Add(name, value);
        }

        public string GetExportPdfUrl()
        {
            var ticketRequest = new TableauServerTicket(_onlineUrls, OnlineSession);
            var ticket = ticketRequest.Ticket();

            var url = $"{_onlineUrls.ServerUrlWithProtocol}/trusted/{ticket}/t/{_onlineUrls.SiteUrlSegement}/views/{_workbookId}/{_viewId}.pdf";
            url = _AddParamtersToUrl(url);
            return url;
        }

        public string GetThumbnailUrl()
        {
            var ticketRequest = new TableauServerTicket(_onlineUrls, OnlineSession);
            var ticket = ticketRequest.Ticket();

            var url = $"{_onlineUrls.ServerUrlWithProtocol}/trusted/{ticket}/t/{_onlineUrls.SiteUrlSegement}/views/{_workbookId}/{_viewId}.png";
            url = _AddParamtersToUrl(url);
            return url;
        }

        private string _AddParamtersToUrl(string url)
        {
            if (_reportParameters != null && _reportParameters.Any())
            {
                var parameters = string.Join("&", _reportParameters.Select(x => $"{x.Key}={x.Value}"));
                url = url + "?" + parameters;
            }

            return url;
        }

    }
}
