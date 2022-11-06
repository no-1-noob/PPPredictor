using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PPPredictor.VersionChecker
{
    class VersionChecker
    {
        private static readonly string baseUrl = "https://mods.no1noob.net";
        private static readonly string pageUrl = "api/PPPredictorVersion";
        public static async Task<string> GetCurrentVersionAsync()
        {
#if (!DEBUG)
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.BaseAddress = new Uri(baseUrl);
                VersionInfo version = null;
                HttpResponseMessage response = await client.GetAsync(pageUrl);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    version = JsonConvert.DeserializeObject<VersionInfo>(result);
                }
                return version.NewestVersion;
            }
            catch
            {
                return string.Empty;
            }
#else
            return "1.0.0";
#endif
        }
    }
}
