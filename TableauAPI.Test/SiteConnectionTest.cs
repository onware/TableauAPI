using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TableauAPI.FilesLogging;
using TableauAPI.RESTHelpers;
using TableauAPI.RESTRequests;

namespace TableauAPI.Test
{
    [TestClass]
    public class SiteConnectionTest
    {
        [TestMethod]
        public void DownloadWorkbooksTest()
        {
            var url = new TableauServerUrls("http://", "10.107.0.240:8000", "ITCostSurvey", 10, ServerVersion.server9);
            
            var signIn = new TableauServerSignIn(
                url, "public",
                "tableau1user%", new TaskStatusLogs());
            signIn.ExecuteRequest();
            var a =
                new DownloadWorkbooksList(url, signIn);
            a.ExecuteRequest();
            Assert.AreEqual(2, a.Workbooks.Count);

            var b = new DownloadProjectsList(url,signIn);
            b.ExecuteRequest();
            Assert.AreEqual(1, b.Projects.Count());

            signIn = new TableauServerSignIn(
                new TableauServerUrls("http://", "10.107.0.240:8000", "ITCostSurvey", 10, ServerVersion.server9),
                "tableauAdmin",
                "ta1user%", new TaskStatusLogs());
            signIn.ExecuteRequest();
            a =
                new DownloadWorkbooksList(
                    new TableauServerUrls("http://", "10.107.0.240:8000", "ITCostSurvey", 10, ServerVersion.server9),
                    signIn);
            
            a.ExecuteRequest();
            Assert.AreEqual(4, a.Workbooks.Count);

            b = new DownloadProjectsList(url,signIn);
            b.ExecuteRequest();
            Assert.AreEqual(1, b.Projects.Count());
        }

        [TestMethod]
        public void TicketTest()
        {
            var url = new TableauServerUrls("http://", "10.107.0.240:8000", "ITCostSurvey", 10, ServerVersion.server9);

            var signIn = new TableauServerSignIn(
                url,
                "tableauAdmin",
                "ta1user%", new TaskStatusLogs());
            signIn.ExecuteRequest();

            var a = new DownloadView(url, signIn);
            var ticket = a.Ticket("tableauAdmin");
            Assert.AreNotEqual(string.Empty, ticket);
        }
    }
}
