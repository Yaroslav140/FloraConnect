using FlowerShop.WpfClient.ApiClient;
using FlowerShop.WpfClient.Services;
using FlowerShop.WpfClient.Timers;
using FlowerShop.WpfClient.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FlowerShop.WpfClient
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var sc = new ServiceCollection();

            sc.AddSingleton<Main>();

            sc.AddTransient<MainViewModel>();
            sc.AddTransient<UserViewModel>();
            sc.AddTransient<OrderViewModel>();
            sc.AddTransient<BouquetViewModel>();
            sc.AddTransient<SoftToyViewModel>();

            sc.AddHttpClient(); 

            sc.AddSingleton<BaseApiClient>();
            sc.AddSingleton<UserApi>();
            sc.AddSingleton<OrderApi>();
            sc.AddSingleton<BouquetApi>();
            sc.AddSingleton<SoftToyApi>();

            sc.AddSingleton<UserPollingService>();
            sc.AddSingleton<OrderPollingService>();
            sc.AddSingleton<BouquetPollingService>();
            sc.AddSingleton<SoftToyPollingService>();

            sc.AddSingleton<IDialogService, DialogService>();

            Services = sc.BuildServiceProvider();

            var main = Services.GetRequiredService<Main>();
            main.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (Services is IDisposable d)
                d.Dispose();

            base.OnExit(e);
        }
    }
}
