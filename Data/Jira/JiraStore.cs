using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using JiraWriter.Config;


namespace JiraWriter.Data.Jira
{
    public abstract class JiraStore
    {
        protected readonly HttpClient _client;
        protected readonly JiraConfig _config;

        public JiraStore(HttpClient client)
        {
            _client = client;
        }

        public JiraStore(JiraConfig config)
        {
            _config = config;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Authorization", _config.ApiKey);
        }

        protected List<JToken> Get(string uri, Dictionary<string, string> options, string tokenName)
        {
            return GetAsync(uri, options, tokenName).Result;
        }

        private async Task<List<JToken>> GetAsync(string uri, Dictionary<string, string> options, string tokenName)
        {
            var endOfResults = false;

            var results = new List<JToken>();

            while (!endOfResults)
            {
                int startAt = results.Count;

                if (options.TryGetValue("startAt", out _))
                {
                    options["startAt"] = startAt.ToString();
                }
                else
                {
                    options.Add("startAt", startAt.ToString());
                }

                var builder = new UriBuilder(uri);
                var urlContent = new FormUrlEncodedContent(options);

                builder.Query = urlContent.ReadAsStringAsync().Result;

                var task = _client.GetStringAsync(builder.ToString());

                var response = await task;
                var jsonResponse = JObject.Parse(response);
                results.AddRange(jsonResponse.SelectToken(tokenName).ToList());

                endOfResults = results.Count >= jsonResponse.SelectToken("total").Value<int>();
            }

            return results;
        }
    }
}
