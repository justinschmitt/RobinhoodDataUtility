using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Web.Script.Serialization;

namespace RobinhoodDataUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String token = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (tbUsername.Text.Length != 0 && pbPassword.Password.Length != 0)
            {
                Login(tbUsername.Text, pbPassword.Password);
            }
            else
            {
                MessageBox.Show("You must enter a Username and Password");
            }
        }

        public async void Login(string username, string password)
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
                String responseContent = await response.Content.ReadAsStringAsync();


                try
                {
                    var jss = new JavaScriptSerializer();

                    Wrappers.ApiTokenAuth auth = jss.Deserialize<Wrappers.ApiTokenAuth>(responseContent);

                    token = auth.token;


                    Console.WriteLine(token);
                    lblStatus.Content = "Logged in!";
                    
                }
                catch (Exception e)
                {
                    MessageBox.Show("Problem logging in: " + responseContent);
                }


            }
        }

        private void btnExportOrders_Click(object sender, RoutedEventArgs e)
        {
             
        }
    }
}
