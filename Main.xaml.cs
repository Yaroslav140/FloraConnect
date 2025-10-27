using FlowerShop.WpfClient.ApiClient;
using FlowerShop.WpfClient.ViewModel;
using System.Windows;

namespace FlowerShop.WpfClient
{ 
    public partial class Main : Window
    {
        private readonly MainVm _vm;
    
        public Main()
        {
            InitializeComponent();
    
            var apiClient = new BaseApiClient();
            var orderApi = new OrderApi(apiClient);
    
            _vm = new MainVm(orderApi);
            DataContext = _vm;
    
            Loaded += async (_, __) => await _vm.LoadAsync();
        }
    }
}
