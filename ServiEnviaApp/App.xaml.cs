using System.Net.Http.Headers;
using ServiEnviaApp.Windows;

namespace ServiEnviaApp
{
    using Microsoft.Extensions.DependencyInjection;
    using Navigation;
    using System;
    using System.Windows;

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
            services.AddTransient<OrderWindow>();
            
            services.AddHttpClient("API", c =>
            {
                c.BaseAddress = new Uri("https://localhost:44332/api/");
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });
        }

        private void OnStartUp(Object sender, StartupEventArgs eventArgs)
        {
            var navigationService = _provider.GetRequiredService<NavigationService>();
            var task = navigationService.ShowAsync<MainWindow>();
        }


    }
}
