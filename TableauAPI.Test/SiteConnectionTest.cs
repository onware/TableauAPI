using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.RESTRequests;
using System.Net;

namespace TableauAPI.Test
{
    [Ignore]
    [TestClass]
    public class SiteConnectionTest
    {
        [TestMethod]
        public void DownloadWorkbooksTest()
        {
            var url = new TableauServerUrls(ServerProtocol.Http, "10.107.0.240:8000", "ITCostSurvey", 10, ServerVersion.Server9);

            var signIn = new TableauServerSignIn(
                url, "public",
                "tableau1user%", new TaskStatusLogs());
            signIn.ExecuteRequest();
            var a =
                new DownloadWorkbooksList(url, signIn);
            a.ExecuteRequest();
            Assert.AreEqual(2, a.Workbooks.Count);

            var b = new DownloadProjectsList(url, signIn);
            b.ExecuteRequest();
            Assert.AreEqual(1, b.Projects.Count());

            signIn = new TableauServerSignIn(
                new TableauServerUrls(ServerProtocol.Http, "10.107.0.240:8000", "ITCostSurvey", 10, ServerVersion.Server9),
                "tableauAdmin",
                "ta1user%", new TaskStatusLogs());
            signIn.ExecuteRequest();
            a =
                new DownloadWorkbooksList(
                    new TableauServerUrls(ServerProtocol.Http, "10.107.0.240:8000", "ITCostSurvey", 10, ServerVersion.Server9),
                    signIn);

            a.ExecuteRequest();
            Assert.AreEqual(4, a.Workbooks.Count);

            foreach (var workbook in a.Workbooks)
            {
                var viewQuery = new DownloadViewsForWorkbookList(workbook.Id, url, signIn);
                viewQuery.ExecuteRequest();
                Assert.AreEqual(1, viewQuery.Views.Count);
                foreach (var view in viewQuery.Views)
                {
                    var thumbnailQuery = new DownloadView(url, signIn);
                    var result = thumbnailQuery.GetPreviewImage(workbook.Id, view.Id);
                    Assert.AreNotEqual(0, result.Length);
                }
            }

            b = new DownloadProjectsList(url, signIn);
            b.ExecuteRequest();
            Assert.AreEqual(1, b.Projects.Count());

            var siteViews = new DownloadViewsForSiteList(url, signIn);
            siteViews.ExecuteRequest();
            Assert.AreEqual(0, siteViews.Views.Count);


        }

        [TestMethod]
        public void TicketTest()
        {
            var url = new TableauServerUrls(ServerProtocol.Http, "10.107.0.240:8000", "ITCostSurvey", 10, ServerVersion.Server9);

            var signIn = new TableauServerSignIn(
                url,
                "tableauAdmin",
                "ta1user%", new TaskStatusLogs());
            signIn.ExecuteRequest();

            var a = new TableauServerTicket(url, signIn);
            var ticket = a.Ticket();
            Assert.AreNotEqual("-1", ticket);
        }

        [TestMethod]
        public void ViewPdfLinkTest()
        {
            var url = new TableauServerUrls(ServerProtocol.Http, "10.107.0.240:8000", "ITCostSurvey", 10, ServerVersion.Server9);

            var signIn = new TableauServerSignIn(
                url,
                "tableauAdmin",
                "ta1user%", new TaskStatusLogs());
            signIn.ExecuteRequest();
            var workbooks =
                new DownloadWorkbooksList(
                    new TableauServerUrls(ServerProtocol.Http, "10.107.0.240:8000", "ITCostSurvey", 10, ServerVersion.Server9),
                    signIn);

            workbooks.ExecuteRequest();
            var workbook = workbooks.Workbooks.First();
            var viewQuery = new DownloadViewsForWorkbookList(workbook.Id, url, signIn);
            viewQuery.ExecuteRequest();
            var view = viewQuery.Views.First();
            var a = new TrustedUrls(workbook.Name.Replace(" ", ""), view.Name.Replace(" ", ""), url, signIn);
            var exportPdfUrl = a.GetExportPdfUrl();
            Assert.IsFalse(string.IsNullOrEmpty(exportPdfUrl));
            var thumbnailUrl = a.GetPreviewImageUrl();
            Assert.IsFalse(string.IsNullOrEmpty(thumbnailUrl));
            var viewUrl = a.GetTrustedViewUrl();
            Assert.IsFalse(string.IsNullOrEmpty(viewUrl));

            var client = new WebClient();

            var data = client.DownloadData(exportPdfUrl);
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Any());

            data = null;
            
            var downloadView = new DownloadView(url, signIn);
            data = downloadView.GetPreviewImage(workbook.Id, view.Id);
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Any());

            a.AddViewParameter("Gabba", "1");
            a.AddViewParameter("Hey", "2");

            exportPdfUrl = a.GetExportPdfUrl();
            Assert.IsTrue(exportPdfUrl.Contains("?"));
            Assert.IsTrue(exportPdfUrl.EndsWith("Gabba=1&Hey=2"));

            thumbnailUrl = a.GetPreviewImageUrl();
            Assert.IsTrue(thumbnailUrl.Contains("?"));
            Assert.IsTrue(thumbnailUrl.EndsWith("Gabba=1&Hey=2"));

            a.HideToolbar = true;
            viewUrl = a.GetTrustedViewUrl();
            Assert.IsTrue(viewUrl.EndsWith("&:toolbar=no"));

            a.HideToolbar = null;
            viewUrl = a.GetTrustedViewUrl();
            Assert.IsFalse(viewUrl.EndsWith("&:toolbar=no"));

            a.HideToolbar = false;
            viewUrl = a.GetTrustedViewUrl();
            Assert.IsFalse(viewUrl.EndsWith("&:toolbar=no"));

            a.HideTabs = true;
            viewUrl = a.GetTrustedViewUrl();
            Assert.IsTrue(viewUrl.EndsWith("&:tabs=no"));

            a.HideTabs = null;
            viewUrl = a.GetTrustedViewUrl();
            Assert.IsFalse(viewUrl.EndsWith("&:tabs=no"));

            a.HideTabs = false;
            viewUrl = a.GetTrustedViewUrl();
            Assert.IsFalse(viewUrl.EndsWith("&:tabs=no"));

            a = new TrustedUrls(workbook.Name.Replace(" ", ""), view.Name.Replace(" ", ""), url, signIn);
            a.HideToolbar = true;
            viewUrl = a.GetTrustedViewUrl();
            Assert.IsTrue(viewUrl.EndsWith("?:toolbar=no"));

            a.HideToolbar = null;
            viewUrl = a.GetTrustedViewUrl();
            Assert.IsFalse(viewUrl.EndsWith("?:toolbar=no"));

            a.HideToolbar = false;
            viewUrl = a.GetTrustedViewUrl();
            Assert.IsFalse(viewUrl.EndsWith("?:toolbar=no"));

            a.HideTabs = true;
            viewUrl = a.GetTrustedViewUrl();
            Assert.IsTrue(viewUrl.EndsWith("?:tabs=no"));

            a.HideTabs = null;
            viewUrl = a.GetTrustedViewUrl();
            Assert.IsFalse(viewUrl.EndsWith("?:tabs=no"));

            a.HideTabs = false;
            viewUrl = a.GetTrustedViewUrl();
            Assert.IsFalse(viewUrl.EndsWith("?:tabs=no"));

            a.HideToolbar = true;
            a.HideTabs = true;
            viewUrl = a.GetTrustedViewUrl();
            Assert.IsTrue(viewUrl.EndsWith("?:tabs=no&:toolbar=no"));
        }
    }
}
