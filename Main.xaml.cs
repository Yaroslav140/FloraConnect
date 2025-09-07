using FlowerShop.WpfClient.ModelView;
using System.Windows;

namespace FlowerShop.WpfClient
{
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            var orders = new OrdersModuleViewModel();
            var users = new UsersModuleViewModel();       // потом добавишь реализацию
            var bouquets = new BouquetsModuleViewModel(); // потом добавишь реализацию
            var flowers = new FlowersModuleViewModel();   // потом добавишь реализацию

            DataContext = new MainViewModel(orders, users, bouquets, flowers);
        }
    }
}
