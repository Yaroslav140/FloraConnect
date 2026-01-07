using FlowerShop.Dto.DTOCreate;
using FlowerShop.Dto.DTOGet;
using FlowerShop.Dto.DTOUpdate;
using FlowerShop.WpfClient.ApiClient;
using FlowerShop.WpfClient.Services;
using FlowerShop.WpfClient.Timers;
using FlowerShop.WpfClient.ViewModel.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FlowerShop.WpfClient.ViewModel
{
    public class OrderViewModel : IViewModelBase, INotifyPropertyChanged, IDisposable
    {
        public string Title => "Заказы";

        private readonly OrderApi _orderApi;
        private readonly BouquetApi _bouquetApi;
        private ObservableCollection<GetOrderDto> _orders;

        public ObservableCollection<GetOrderDto> Orders
        {
            get => _orders;
            set
            {
                if(!Equals(value, _orders))
                {
                    _orders = value;
                    OnPropertyChanged();
                }
            }
        }
        private string? _searchText;
        public string? SearchText
        {
            get => _searchText;
            set
            {
                if (!Equals(_searchText, value))
                {
                    _searchText = value;
                    OnPropertyChanged();
                }
            }
        }
        private GetOrderDto? _selectedOrder;
        public GetOrderDto? SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (!Equals(_selectedOrder, value))
                {
                    _selectedOrder = value;
                    OnPropertyChanged();
                }
            }
        }

        private readonly IDialogService _dialog;
        public ICommand SearchCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        private readonly OrderPollingService _pollingService;
        public OrderViewModel(OrderApi orderApi, BouquetApi bouquetApi, IDialogService dialog)
        {
            _orderApi = orderApi ?? throw new ArgumentNullException(nameof(orderApi));
            Orders = new ObservableCollection<GetOrderDto>();

            _pollingService = new OrderPollingService(_orderApi, OnOrdersUpdated, TimeSpan.FromSeconds(10));
            SearchCommand = new RelayCommand(_ => SearchOrderAsync());
            _ = LoadAsync();

            _pollingService.Start();
            _orderApi = orderApi;
            _bouquetApi = bouquetApi;
            _dialog = dialog;

            CreateCommand = new RelayCommand(_ => Create());
            EditCommand = new RelayCommand(p => Edit(p as GetOrderDto), p => p is GetOrderDto);
            DeleteCommand = new RelayCommand(_ => DeleteOrder());
        }

        private async Task DeleteOrder()
        {
            if(SelectedOrder != null)
            {
                var respone = await _orderApi.DeleteOrder(SelectedOrder.Id);

                var body = await respone.Content.ReadAsStringAsync();

                if(respone.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show(body);
                    return;
                }
                else
                {
                    MessageBox.Show("Заказ успешно удален.");
                }
                await LoadAsync();
            }
        }

        private void OnOrdersUpdated(List<GetOrderDto>? orders)
        {
            if (orders != null)
            {
                Orders = new ObservableCollection<GetOrderDto>(orders);
            }
        }

        private async void SearchOrderAsync()
        {
            _pollingService.Stop();
            if (string.IsNullOrWhiteSpace(_searchText))
            {
                await LoadAsync();
                _pollingService.Start();
                return;
            }
            try
            {
                var order = await _orderApi.GetOrdersByName(_searchText);
                if(order != null)
                {
                    Orders = new ObservableCollection<GetOrderDto>(order);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при поиске заказа");
            }
            _pollingService.Start();
        }

        public async Task LoadAsync()
        {
            try
            {
                var orders = await _orderApi.GetAllOrders();
                if (orders != null)
                {
                    Orders = new ObservableCollection<GetOrderDto>(orders);
                }
                else
                {
                    Orders.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке заказов");
            }
        }
        private async Task Create()
        {
            var vm = new OrderEditViewModel();

            var bouquets = await _bouquetApi.GetAllBouquets();
            vm.Bouquets.Clear();
            if(bouquets == null) return;
            foreach (var b in bouquets)
                vm.Bouquets.Add(b);
            var items = vm.BuildCreateItems();
            var ok = _dialog.ShowDialog(vm);

            if (ok != true) return;

            try
            {
                var dto = new CreateOrderDto(
                    Guid.NewGuid(),
                    vm.Date,
                    vm.TotalPrice,
                    vm.Status,
                    items);

                var respone = await _orderApi.CreateOrder(dto);
                var body = await respone.Content.ReadAsStringAsync();

                if (respone.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    MessageBox.Show(body);
                    return;
                }
                await LoadAsync();
            }
            catch
            {
                MessageBox.Show("Ошибка при создании заказа.");
            }
        }

        private async void Edit(GetOrderDto? order)
        {
            if (order == null) return;

            var vm = new OrderEditViewModel(order);

            var bouquets = await _bouquetApi.GetAllBouquets();
            vm.Bouquets.Clear();
            if (bouquets == null) return;
            foreach (var b in bouquets)
                vm.Bouquets.Add(b);
            var items = vm.BuildUpdateItems();

            var ok = _dialog.ShowDialog(vm);

            if (ok == true)
            {
                try
                {
                    await _orderApi.UpdateOrder(new UpdateOrderDto(
                        order.Id,
                        order.Status,
                        items));
                    await LoadAsync();
                }
                catch
                {
                    MessageBox.Show("Ошибка при обновлении заказа");
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void Dispose() => _pollingService?.Stop();
    }
}
