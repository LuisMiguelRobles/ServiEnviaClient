using ServiEnviaApp.Windows;

namespace ServiEnviaApp
{
    using Navigation;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NavigationService _navigationService;


        public MainWindow(NavigationService navigationService)
        {
            
            InitializeComponent();
            _navigationService = navigationService;
        }

        private void CustomerItem_OnClick(object sender, RoutedEventArgs e)
        {
            var task = _navigationService.ShowAsync<CustomerWindow>();

        }

        private void OrderItem_OnClick(object sender, RoutedEventArgs e)
        {
            var task = _navigationService.ShowAsync<OrderWindow>();

        }
    }
}
