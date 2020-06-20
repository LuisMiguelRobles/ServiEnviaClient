using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace ServiEnviaApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _provider;
        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _provider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<NavigationService>();
            services.AddTransient<MainWindow>();
            services.AddTransient<CustomerWindow>();
            
            services.AddHttpClient("API", c =>
            {
                c.BaseAddress = new Uri("https://localhost:44332/api/");
            });
        }

        private void OnStartUp(Object sender, StartupEventArgs eventArgs)
        {
            //var mainWindow = _provider.GetService<MainWindow>();
            //mainWindow.Show();
            var navigationService = _provider.GetRequiredService<NavigationService>();
            var task = navigationService.ShowAsync<MainWindow>();
        }


    }
}
