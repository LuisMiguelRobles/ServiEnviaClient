using System.Windows;
namespace ServiEnviaApp.Windows
{
    using Models;
    using Navigation;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    public partial class OrderWindow : Window
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly HttpClient _client;
        public OrderWindow(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _client = _clientFactory.CreateClient("API");
            InitializeComponent();
        }

        private async Task<ObservableCollection<Order>> GetOrders()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "Order");
            var client = _clientFactory.CreateClient("API");

            var response = await client.SendAsync(request);

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var x = await JsonSerializer.DeserializeAsync<ObservableCollection<Order>>(responseStream);
            DataGrid.DataContext = x;
            return await Task.FromResult(x);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await GetOrders();

            var customers = await GetCustomers();
            if(customers != null && customers.Any())
            {
                foreach (var customer in customers)
                {
                    var item = new ComboBoxItem()
                    {
                        Tag = customer.id,
                        Content = string.Concat(customer.firstName, " ", customer.lastName)
                    };
                    Customer.Items.Add(item);
                }
            }
        }

        private async void Add_Order_OnClick(object sender, RoutedEventArgs e)
        {
            var order = new Order
            {
                senderDocument = SenderDocument.Text,
                receiverDocument = ReceiverDocument.Text,
                from = From.Name,
                destination = Destination.Text,
                weight = Convert.ToDecimal(Weight.Text),
                price = Convert.ToDecimal(Price.Text),
                customerId = new Guid(((ComboBoxItem)Customer.SelectedItem).Tag.ToString())
            };

            await CreateItemAsync(order);
        }

        private State GetStateFromCombo(string state)
        {
            switch(state)
            {
                case "Pending":
                    return Models.State.Pending;
                case "Collected":
                    return Models.State.Collected;
                case "Sending":
                    return Models.State.Sending;
                case "Delivery":
                    return Models.State.Delivery;
                default:
                    return Models.State.Pending;
            }
        }

        private async Task CreateItemAsync(Order order)
        {
            var content = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");

            using var httpResponse =
                await _client.PostAsync("Order", content);

            if (httpResponse.IsSuccessStatusCode)
            {
                MessageBox.Show("Order Added");
                await GetOrders();
            }
            else
            {
                MessageBox.Show($"{httpResponse.StatusCode}");
            }
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            await SearchOrder(TxtSearch.Text);
        }
        private async Task SearchOrder(string id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"Order/{id}");
            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                await using var responseStream = await response.Content.ReadAsStreamAsync();
                var order = await JsonSerializer.DeserializeAsync<Order>(responseStream);
                MessageBox.Show(order.ToString());
            }
            else
            {
                MessageBox.Show("Not found");
            }

        }

        private async Task<IEnumerable<Customer>> GetCustomers()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "Customer");
            var client = _clientFactory.CreateClient("API");

            var response = await client.SendAsync(request);

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var x = await JsonSerializer.DeserializeAsync<IEnumerable<Customer>>(responseStream);
           
            return await Task.FromResult(x);
        }

    }
}
