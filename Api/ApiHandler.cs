using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SKTestBot.Api
{
    // this should be a base class
    public class ApiHandler
    {
        // this is static because we only want one instance of HttpClient in the entire application
        public static HttpClient ApiClient { get; set; }

        public static void InitializeClient()
        {
            // set up ApiClient
            ApiClient = new HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            // we add a header that says "give us type json only" whenever we request data
            ApiClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        }
    }
}
