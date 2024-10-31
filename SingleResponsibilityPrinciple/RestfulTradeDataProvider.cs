using SingleResponsibilityPrinciple.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SingleResponsibilityPrinciple
{
    public class RestfulTradeDataProvider : ITradeDataProvider
    {
        private readonly string _url;
        private readonly ILogger _logger;

        public RestfulTradeDataProvider(string url, ILogger logger)
        {
            _url = url;
            _logger = logger;
        }

        public IEnumerable<string> GetTradeData()
        {
            return GetTradeDataAsync().Result; 
        }

        private async Task<IEnumerable<string>> GetTradeDataAsync()
        {
            List<string> tradeData = new List<string>();
            _logger.LogInfo("Fetching trade data from URL: " + _url);

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(_url);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogInfo($"Failed to fetch trades. Status Code: {response.StatusCode}");
                        throw new Exception("Could not retrieve trade data from the URL.");
                    }

                    string jsonData = await response.Content.ReadAsStringAsync();
                    tradeData = JsonSerializer.Deserialize<List<string>>(jsonData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("An error occurred while fetching trade data: " + ex.Message);
                throw;
            }

            return tradeData;
        }
    }
}