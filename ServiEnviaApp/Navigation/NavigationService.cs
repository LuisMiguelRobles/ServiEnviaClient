namespace ServiEnviaApp.Navigation
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    public interface IActivable
    {
        Task ActivateAsync(object parameter);
    }

    public class NavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ShowAsync<T>(object parameter = null) where T : Window
        {
            var window = _serviceProvider.GetRequiredService<T>();
            if (window is IActivable activableWindow)
            {
                await activableWindow.ActivateAsync(parameter);
            }

            window.Show();
        }
    }
}
