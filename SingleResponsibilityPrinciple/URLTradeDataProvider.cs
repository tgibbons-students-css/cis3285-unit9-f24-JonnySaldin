using SingleResponsibilityPrinciple.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SingleResponsibilityPrinciple
{
    public class URLTradeDataProvider : ITradeDataProvider
    {
        private readonly string _url;
        private readonly ILogger _logger;

        public URLTradeDataProvider(string url, ILogger logger)
        {
            _url = url;
            _logger = logger;
        }

        public IEnumerable<string> GetTradeData()
        {
            List<string> tradeData = new List<string>();

            _logger.LogInfo("Reading trades from URL: " + _url);

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(_url).Result;

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogInfo($"Failed to fetch trades. Status Code: {response.StatusCode}");
                    throw new Exception("Could not retrieve trade data from the URL.");
                }

                using (Stream stream = response.Content.ReadAsStreamAsync().Result)
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        tradeData.Add(line);
                    }
                }
            }

            return tradeData;
        }
    }
}