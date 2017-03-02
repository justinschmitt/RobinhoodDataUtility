using System;
using System.Windows;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RobinhoodDataUtility
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Robinhood connection = new Robinhood();

        public MainWindow()
        {
            InitializeComponent();
            connection.ConnectionEstablished += Connection_ConnectionEstablished;
        }

        private void Connection_ConnectionEstablished(object sender, EventArgs e)
        {
            LoginTab.IsEnabled = false;
            ToolTab.IsEnabled = true;
            ToolTab.Focus();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await connection.Login(tbUsername.Text, pbPassword.Password);
                tbStatus.Text = "Logged in!";
            }
            catch(ConnectionException ex)
            {
                MessageBox.Show(ex.Message);
                tbStatus.Text = "Login Failed";
            }  
        }


        private void btnExportOrders_Click(object sender, RoutedEventArgs e)
        {
            GetOrders();
        }

        public async void GetOrders()
        {

            Orders orders = JsonConvert.DeserializeObject<Orders>(await connection.GetEndpoint("orders"));

            tbStatus.Text = "Got Orders!";

            String s = "";

            //header row
            Type type = orders.results[0].GetType();
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo p in properties)
            {
                s += p.Name + ",";
            }

            s += "\n";

            foreach (Result order in orders.results)
            {
                Type t = order.GetType();
                PropertyInfo[] pi = t.GetProperties();

                foreach (PropertyInfo p in pi)
                {
                    s += p.GetValue(order, null) + ",";
                }
                s += "\n";
            }

            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.GetFullPath("OrderExport.csv")))
            {
                await outputFile.WriteAsync(s);

            }

            MessageBox.Show("file saved!");
        }

        private async void btnGetOrders_Click(object sender, RoutedEventArgs e)
        {
            Orders orders = JsonConvert.DeserializeObject<Orders>(await connection.GetEndpoint("orders"));

            //resolve instruments into dictionary

            IDictionary<string, Instrument> instrumentMap = new Dictionary<string, Instrument>();

            foreach (Result order in orders.results)
            {
                if(!instrumentMap.ContainsKey(order.instrument))
                {
                    instrumentMap.Add(order.instrument, JsonConvert.DeserializeObject<Instrument>(await connection.GetEndpoint(order.instrument.Substring(order.instrument.LastIndexOf("instruments")))));
                }
            }

            //iterate over orders to apply Instrument object

            foreach (Result order in orders.results)
            {
                Instrument i = null;
                instrumentMap.TryGetValue(order.instrument, out i);
                order.Instrument = i;
            }

            dgExecutions.ItemsSource = orders.results;
        }
    }
}
