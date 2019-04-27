using System.IO;
using System.Net;
using System.Text;

namespace TableauAPI.RESTHelpers
{
    /// <summary>
    /// Base class on top of which requests to Tableau Server are based
    /// </summary>
    public abstract class TableauServerRequestBase
    {
        /// <summary>
        /// Sends the body text up to the server
        /// </summary>
        /// <param name="request"></param>
        /// <param name="bodyText"></param>
        /// <param name="method"></param>
        protected static void SendRequestContents(WebRequest request, string bodyText, string method = "POST")
        {
            request.Method = method;
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/xml;charset=utf-8";
            // Set the ContentLength property of the WebRequest.
            byte[] byteArray = Encoding.UTF8.GetBytes(bodyText);
            request.ContentLength = byteArray.Length;

            // Get the request stream.
            var dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.

            dataStream.Close();
        }


        /// <summary>
        /// Debugging function: Allows us to test that all of our content was replaced
        /// </summary>
        /// <param name="text"></param>
        protected void AssertTemplateFullyReplaced(string text)
        {
            if (text.Contains("{{iws"))
            {
                System.Diagnostics.Debug.Assert(false, "Text still contains template fragments");
            }
        }

        /// <summary>
        /// Gets the web response as a XML document
        /// </summary>
        protected static System.Xml.XmlDocument GetWebResponseAsXml(WebResponse response)
        {
            var streamText = string.Empty;
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream != null)
                {
                    var streamReader = new StreamReader(responseStream);
                    using (streamReader)
                    {
                        streamText = streamReader.ReadToEnd();
                        streamReader.Close();
                    }
                }
            }

            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(streamText);
            return xmlDoc;
        }

        /// <summary>
        /// Gets the web response as a XML document
        /// </summary>
        protected static string GetWebResponseAsText(WebResponse response)
        {
            string streamText = null;
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream != null)
                {
                    var streamReader = new StreamReader(responseStream);
                    using (streamReader)
                    {
                        streamText = streamReader.ReadToEnd();
                        streamReader.Close();
                    }
                }
            }

            return streamText;
        }

    }
}
