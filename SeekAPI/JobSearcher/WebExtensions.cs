using System;
using HtmlAgilityPack;
using System.Net;


namespace JobSearcher
{
    static class WebExtensions
    {
        /// <summary>
        /// generate the HtmlDocument from url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HtmlDocument LoadHtmlDocumentForUrl(this string url)
        {
            using (WebClient client = new WebClient())
            {
                string html = client.DownloadString(new Uri(url));
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                return htmlDocument;
            }
        }

        /// <summary>
        /// html decode for string as exention method
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string value)
        {
            return WebUtility.HtmlDecode(value);
        }
    }
}
