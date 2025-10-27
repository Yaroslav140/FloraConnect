using FlowerShop.Dto.DTOGet;
using FlowerShop.WpfClient.ApiClient;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FlowerShop.WpfClient.ViewModel
{
    public class MainVm(OrderApi api) : INotifyPropertyChanged
    {
        public ObservableCollection<GetOrderDto> Orders { get; } = new();
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly OrderApi _api = api;

        public async Task LoadAsync()
        {
            var list = await _api.GetAllOrders() ?? new List<GetOrderDto>();
            Orders.Clear();
            foreach (var o in list) Orders.Add(o);
        }
    }

}
