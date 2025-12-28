using FlowerShop.WpfClient.ApiClient;
using FlowerShop.WpfClient.Timers;
using FlowerShop.WpfClient.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Windows;

namespace FlowerShop.WpfClient
{
    public partial class App : Application
    {
        public static ServiceProvider Service = null!;
        public App()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var sc = new ServiceCollection();
            sc.AddSingleton<Main>();
            sc.AddTransient<MainViewModel>();
            sc.AddTransient<UserViewModel>();
            sc.AddTransient<OrderViewModel>();
            sc.AddTransient<BouquetViewModel>();

            sc.AddScoped<HttpClient>();
            sc.AddScoped<BaseApiClient>();
            sc.AddScoped<UserApi>();
            sc.AddScoped<OrderApi>();
            sc.AddScoped<BouquetApi>();
            sc.AddScoped<UserPollingService>();
            sc.AddScoped<OrderPollingService>();
            sc.AddScoped<BouquetPollingService>();

            Service = sc.BuildServiceProvider();
            var main = Service.GetRequiredService<Main>();
            main.Show();
            base.OnStartup(e);
        }
    }
}
