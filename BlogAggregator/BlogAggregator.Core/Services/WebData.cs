using NLog;
using System;
using System.Net;
using System.Text;

namespace BlogAggregator.Core.Services
{
    public class WebData
    {
        public static WebData Instance => new WebData();

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        // Read data from input webUrl,
        //  adding schema to webUrl if necessary
        // Return the data and the HTTP status code        
        // If read is unsuccessful, return empty string   
        public string GetWebData(string webUrl, out HttpStatusCode HTTPresult)
        {           
            string webData = "";
 
            try
            {
                // Read the data at the input URL
                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    webData = webClient.DownloadString(webUrl.FixWebUrl());
                    HTTPresult = HttpStatusCode.OK;
                }               
            }
            catch (WebException ex)
            {
                webData = "";
                // Return HTTP status code if available
                HTTPresult = ex.Response == null ? HttpStatusCode.Unused :
                                        ((HttpWebResponse)ex.Response).StatusCode;

                _logger.Warn("Web exception reading from URL: {0}, HTTP status code: {1}\nException: {2}\nStackTrace: {3}", 
                        webUrl, HTTPresult, ex.Message, ex.StackTrace);              
            }
            return webData;
            
        }

        // Overload for GetWebData if HTTP status code is not needed
        public string GetWebData(string webUrl)
        {
            HttpStatusCode HTTPresult;
            return GetWebData(webUrl, out HTTPresult);
        }

    }
}
