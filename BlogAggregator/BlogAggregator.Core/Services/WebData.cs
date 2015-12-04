using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Core.Services
{
    public class WebData
    {
        public static WebData Instance => new WebData();

        // Read data from input webUrl
        // Return result of read as string
        // If read is unsuccessful, return empty string
        public string GetWebData(string webUrl)
        {
            string webData;

            try
            {
                // Read the data at the input URL
                using (WebClient webClient = new WebClient())
                {
                    webData = webClient.DownloadString(webUrl);
                }
            }
            catch (Exception e)
            {

                webData = "";
            }

            return webData;
            
        }
       
    }
}
