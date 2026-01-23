using FlowerShop.WpfClient.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FlowerShop.WpfClient
{ 
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            DataContext = App.Services.GetRequiredService<MainViewModel>();
        }
    }
}
