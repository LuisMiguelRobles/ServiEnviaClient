namespace ServiEnviaApp
{
    using Models;
    using Navigation;
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
    public partial class CustomerWindow : Window, IActivable
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly HttpClient _client;
        private ObservableCollection<Customer> Customers { get; set; }

        public CustomerWindow(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;

            _client = _clientFactory.CreateClient("API");
            InitializeComponent();

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

        private async void Add_Customer_OnClick(object sender, RoutedEventArgs e)
        {
            var customer = new Customer
            {
                document = Document.Text,
                firstName = FirstName.Text,
                lastName = LastName.Name,
                email = Email.Text,
                birthDate = BirthDate.SelectedDate ?? DateTime.Now
            };

            await CreateItemAsync(customer);
        }


        private async Task CreateItemAsync(Customer customer)
        {
            var content = new StringContent(JsonSerializer.Serialize(customer), Encoding.UTF8, "application/json");

            using var httpResponse =
                await _client.PostAsync("Customer", content);
            
            if (httpResponse.IsSuccessStatusCode)
            {
                MessageBox.Show("Customer Added");
                await GetCustomers();
            }
            else
            {
                MessageBox.Show($"{httpResponse.StatusCode}");
            }
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            await SearchCustomer(TxtSearch.Text);
        }

        private async Task SearchCustomer(string document)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"Customer/{document}");
            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                await using var responseStream = await response.Content.ReadAsStreamAsync();
                var customer = await JsonSerializer.DeserializeAsync<Customer>(responseStream);
                MessageBox.Show(customer.ToString());
            }
            else
            {
                MessageBox.Show("Not found");
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid.DataContext = GetCustomers();
        }

    }
}
