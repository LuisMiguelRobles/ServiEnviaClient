using System.Linq;
using System.Net;
using ServiEnviaApp.Navigation;

namespace ServiEnviaApp
{
    using ServiEnviaApp.Models;
    using System;
    using System.Collections.ObjectModel;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Lógica de interacción para CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window , IActivable
    {
        private readonly IHttpClientFactory _clientFactory;
        private ObservableCollection<Customer> Customers { get;  set; }

        public CustomerWindow(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;

            InitializeComponent();

            DataGrid.DataContext = GetCustomers();

        }


        private async Task<ObservableCollection<Customer>> GetCustomers()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "Customer");
            var client = _clientFactory.CreateClient("API");
            
            var response = await client.SendAsync(request);

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var x = await JsonSerializer.DeserializeAsync<ObservableCollection<Customer>>(responseStream);
            DataGrid.DataContext = x;
            return await Task.FromResult(x);
        }

        public Task ActivateAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        private void Add_Customer_OnClick(object sender, RoutedEventArgs e)
        {
            var customer = new Customer
            {
                document = Document.Text,
                firstName = FirstName.Text,
                lastName = LastName.Name,
                email = Email.Text,
                birthDate = BirthDate.SelectedDate ?? DateTime.Now
            };

            CreateItemAsync(customer);
        }


        private async Task CreateItemAsync(Customer customer)
        {
            var client = _clientFactory.CreateClient("API");

            var content = new StringContent(JsonSerializer.Serialize(customer), Encoding.UTF8, "application/json");

            using var httpResponse =
                await client.PostAsync( "Customer", content);

            MessageBox.Show(httpResponse.StatusCode == HttpStatusCode.OK
                ? "Customer added"
                : $"{httpResponse.StatusCode}");


            httpResponse.EnsureSuccessStatusCode();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _ = this.SearchCustomer(CustomerDocument.Text);
        }

        private async Task SearchCustomer(string document)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,  $"Customer/{document}");
            var client = _clientFactory.CreateClient("API");

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                await using var responseStream = await response.Content.ReadAsStreamAsync();
                var customer = await JsonSerializer.DeserializeAsync<Customer>(responseStream);
                MessageBox.Show(this.CustomerData(customer));
            }
            else
            {
                MessageBox.Show("Not found");
            }

        }


        private string CustomerData(Customer customer)
        {
            return $"Document:{customer.document}, Full Name: {customer.firstName} {customer.lastName}";
        }
    }
}
