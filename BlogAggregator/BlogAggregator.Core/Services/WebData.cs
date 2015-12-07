using System;
using System.Net;
using System.Text;

namespace BlogAggregator.Core.Services
{
    public class WebData
    {
        public static WebData Instance => new WebData();

        // Read data from input webUrl,
        //  adding schema to webUrl if necessary
        // Return result of read as string
        // If read is unsuccessful, return empty string
        public string GetWebDataFixUrl(string webUrl)
        {
            return GetWebData(FixWebUrl(webUrl));
        }


        // Read data from input webUrl
        // Return result of read as string
        // If read is unsuccessful, return empty string
        public string GetWebData(string webUrl)
        {
            string webData = "";

            try
            {
                // Read the data at the input URL
                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    webData = webClient.DownloadString(webUrl);
                }
                
            }
            catch (Exception e)
            {

                webData = "";
            }

            return webData;
            
        }

        // Return the full form of the input web Url,
        //  by adding scheme (e.g. http) if it is missing
        public string FixWebUrl (string webUrl)
        {
            // Insert scheme if missing
            Uri fullWebUri = new UriBuilder(webUrl).Uri;
            return fullWebUri.AbsoluteUri;
        }
       
    }
}
