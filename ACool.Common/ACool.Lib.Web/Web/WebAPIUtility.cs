using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public class WebAPIUtility
    {
        public static async Task<HttpResponseMessage> Post(string api, Dictionary<string, string> headers, string json)
        {
            // Set Header

            HttpClient client = new HttpClient();

            foreach (string key in headers.Keys)
            {
                client.DefaultRequestHeaders.Add(key, headers[key]);
            }

            // Set Content

            HttpContent content = new StringContent(json);

            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            return await client.PostAsync(api, content);
        }

        public static async Task<HttpResponseMessage> Put(string api, Dictionary<string, string> headers, string json)
        {
            // Set Header

            HttpClient client = new HttpClient();

            foreach (string key in headers.Keys)
            {
                client.DefaultRequestHeaders.Add(key, headers[key]);
            }

            // Set Content

            HttpContent content = new StringContent(json);

            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            return await client.PutAsync(api, content);
        }

        public static async Task<HttpResponseMessage> Delete(string api, Dictionary<string, string> headers)
        {
            // Set Header

            HttpClient client = new HttpClient();

            foreach (string key in headers.Keys)
            {
                client.DefaultRequestHeaders.Add(key, headers[key]);
            }

            return await client.DeleteAsync(api);
        }

        public static async Task<HttpResponseMessage> Get(string api, Dictionary<string, string> headers)
        {
            // Set Header

            HttpClient client = new HttpClient();

            foreach (string key in headers.Keys)
            {
                client.DefaultRequestHeaders.Add(key, headers[key]);
            }

            HttpResponseMessage result = await client.GetAsync(api);

            return result;
        }
    }
}
