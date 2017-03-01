using Newtonsoft.Json;
using RobinhoodDataUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;

namespace RobinhoodDataUtility
{
    public class Robinhood
    {
        private string token = null;

        public Boolean Connected { get { return token != null; } }

        public event EventHandler ConnectionEstablished;

        public async Task Login(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.robinhood.com/");
                var request = new HttpRequestMessage(HttpMethod.Post, "api-token-auth/");
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("Accept-Language", "en;q=1, fr;q=0.9, de;q=0.8, ja;q=0.7, nl;q=0.6, it;q=0.5");
                client.DefaultRequestHeaders.Add("X-Robinhood-API-Version", "1.0.0");
                client.DefaultRequestHeaders.Connection.Add("keep-alive");
                client.DefaultRequestHeaders.Add("User-Agent", "Robinhood/823 (iPhone; iOS 7.1.2; Scale/2.00)");

                List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();
                data.Add(new KeyValuePair<string, string>("username", username));
                data.Add(new KeyValuePair<string, string>("password", password));

                request.Content = new FormUrlEncodedContent(data);
                HttpResponseMessage response = await client.SendAsync(request);
                string loginResponse = await response.Content.ReadAsStringAsync();

                try
                {
                    var jss = new JavaScriptSerializer();
                    Wrappers.ApiTokenAuth auth = jss.Deserialize<Wrappers.ApiTokenAuth>(loginResponse);
                    token = auth.token;
                }
                catch
                {
                    throw new ConnectionException(loginResponse);
                }

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new ConnectionException(loginResponse);
                }

                if (token != null)
                {
                    this.ConnectionEstablished(this, null);
                }
                else
                {
                    throw new ConnectionException(loginResponse);
                }
            }
        }

        public async Task<string> GetEndpoint(string endpoint)
        {
            if (!Connected) throw new ConnectionException("Cannot request endpoint: " + endpoint + " Not currently connected.");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.robinhood.com/");
                var request = new HttpRequestMessage(HttpMethod.Get, endpoint + "/");
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("Accept-Language", "en;q=1, fr;q=0.9, de;q=0.8, ja;q=0.7, nl;q=0.6, it;q=0.5");
                client.DefaultRequestHeaders.Add("X-Robinhood-API-Version", "1.0.0");
                client.DefaultRequestHeaders.Connection.Add("keep-alive");
                client.DefaultRequestHeaders.Add("User-Agent", "Robinhood/823 (iPhone; iOS 7.1.2; Scale/2.00)");
                client.DefaultRequestHeaders.Add("Authorization", "Token " + token);

                HttpResponseMessage response = await client.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
        }

    }

    class ConnectionException : Exception
    {
        public ConnectionException(string message)
        : base(message) { }
    }
}