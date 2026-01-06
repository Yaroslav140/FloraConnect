using FlowerShop.Dto.DTOGet;
using FlowerShop.WpfClient.ApiClient;
using FlowerShop.WpfClient.Services;
using FlowerShop.WpfClient.Timers;
using FlowerShop.WpfClient.ViewModel.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace FlowerShop.WpfClient.ViewModel
{
    public class OrderViewModel : IViewModelBase, INotifyPropertyChanged, IDisposable
    {
        public string Title => "Заказы";

        private readonly OrderApi _orderApi;
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
        public OrderViewModel(OrderApi orderApi, IDialogService dialog)
        {
            _orderApi = orderApi ?? throw new ArgumentNullException(nameof(orderApi));
            Orders = new ObservableCollection<GetOrderDto>();

            _pollingService = new OrderPollingService(_orderApi, OnOrdersUpdated, TimeSpan.FromSeconds(10));
            SearchCommand = new RelayCommand(_ => SearchOrderAsync());
            _ = LoadAsync();

            _pollingService.Start();
            _orderApi = orderApi;
            _dialog = dialog;

            CreateCommand = new RelayCommand(_ => Create());
            EditCommand = new RelayCommand(_ => Edit(), _ => SelectedOrder != null);
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
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void Create()
        {
            var vm = new OrderEditViewModel(isEdit: false);
            var ok = _dialog.ShowDialog(vm);

            if (ok == true)
            {
                // собрать DTO из vm и вызвать API Create
                // потом LoadAsync()
            }
        }

        private void Edit()
        {
            if (SelectedOrder == null) return;

            var vm = new OrderEditViewModel(isEdit: true /*, existing: SelectedOrder */);
            var ok = _dialog.ShowDialog(vm);

            if (ok == true)
            {
                // API Update
            }
        }
        public void Dispose() => _pollingService?.Stop();
    }
}
