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
using System.Windows;
using System.Windows.Input;

namespace FlowerShop.WpfClient.ViewModel
{
    public sealed class OrderViewModel : IViewModelBase, INotifyPropertyChanged, IDisposable
    {
        public string Title => "Заказы";

        private readonly OrderApi _orderApi;
        private readonly BouquetApi _bouquetApi;
        private readonly IDialogService _dialog;
        private readonly OrderPollingService _pollingService;

        private ObservableCollection<GetOrderDto> _orders = new();
        public ObservableCollection<GetOrderDto> Orders
        {
            get => _orders;
            private set { _orders = value; OnPropertyChanged(); }
        }

        private string? _searchText;
        public string? SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }

        private GetOrderDto? _selectedOrder;
        public GetOrderDto? SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand ViewCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public OrderViewModel(OrderApi orderApi, BouquetApi bouquetApi, IDialogService dialog)
        {
            _orderApi = orderApi ?? throw new ArgumentNullException(nameof(orderApi));
            _bouquetApi = bouquetApi ?? throw new ArgumentNullException(nameof(bouquetApi));
            _dialog = dialog ?? throw new ArgumentNullException(nameof(dialog));

            SearchCommand = new RelayCommand(_ => _ = SearchOrderAsync());
            ViewCommand = new RelayCommand(p => ViewDetails(p as GetOrderDto), p => p is GetOrderDto);
            CreateCommand = new RelayCommand(_ => _ = CreateAsync());
            EditCommand = new RelayCommand(_ => _ = EditOrderAsync(), _ => SelectedOrder != null);
            DeleteCommand = new RelayCommand(_ => _ = DeleteOrderAsync(), _ => SelectedOrder != null);

            _pollingService = new OrderPollingService(_orderApi, OnOrdersUpdated, TimeSpan.FromSeconds(10));
            _pollingService.Start();

            _ = LoadAsync();
        }

        private void ViewDetails(GetOrderDto? order)
        {
            if (order == null) return;
            _dialog.ShowDialog(new OrderDetailsViewModel(order));
        }

        private void OnOrdersUpdated(List<GetOrderDto>? orders)
        {
            if (orders == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Orders = new ObservableCollection<GetOrderDto>(orders);
            });
        }

        public async Task LoadAsync()
        {
            try
            {
                var orders = await _orderApi.GetAllOrders();
                Orders = orders != null
                    ? new ObservableCollection<GetOrderDto>(orders)
                    : new ObservableCollection<GetOrderDto>();
            }
            catch
            {
                MessageBox.Show("Ошибка при загрузке заказов");
            }
        }

        private async Task SearchOrderAsync()
        {
            _pollingService.Stop();
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadAsync();
                    return;
                }

                var orders = await _orderApi.GetOrdersByName(SearchText);
                Orders = orders != null
                    ? new ObservableCollection<GetOrderDto>(orders)
                    : new ObservableCollection<GetOrderDto>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при поиске заказа: " + ex.Message);
            }
            finally
            {
                _pollingService.Start();
            }
        }

        private async Task CreateAsync()
        {
            var vm = new OrderEditViewModel();

            var bouquets = await _bouquetApi.GetAllBouquets();
            if (bouquets == null) return;

            vm.Bouquets.Clear();
            foreach (var b in bouquets) vm.Bouquets.Add(b);

            var ok = _dialog.ShowDialog(vm);
            if (ok != true) return;

            var dto = new CreateOrderDto(
                Guid.NewGuid(),
                vm.CustomerName,
                DateTime.SpecifyKind(vm.Date.Date, DateTimeKind.Utc),
                vm.TotalPrice,
                vm.Status,
                vm.BuildCreateItems()
            );

            try
            {
                var response = await _orderApi.CreateOrder(dto);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show(body);
                    return;
                }

                MessageBox.Show("Заказ успешно создан.");
                await LoadAsync();
            }
            catch
            {
                MessageBox.Show("Ошибка при создании заказа.");
            }
        }

        private async Task EditOrderAsync()
        {
            if (SelectedOrder == null) return;

            _pollingService.Stop();
            try
            {
                var vm = new OrderEditViewModel(SelectedOrder);

                var bouquets = await _bouquetApi.GetAllBouquets();
                if (bouquets == null) return;

                vm.Bouquets.Clear();
                foreach (var b in bouquets) vm.Bouquets.Add(b);

                var ok = _dialog.ShowDialog(vm);
                if (ok != true) return;

                var dto = new UpdateOrderDto(
                    SelectedOrder.Id,
                    vm.CustomerName,
                    DateTime.SpecifyKind(vm.Date.Date, DateTimeKind.Utc),
                    vm.Status,
                    vm.TotalPrice,
                    vm.BuildUpdateItems()
                );

                var response = await _orderApi.UpdateOrder(dto);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show(body);
                    return;
                }

                MessageBox.Show("Заказ успешно обновлён.");
                await LoadAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при редактировании заказа: " + ex.Message);
            }
            finally
            {
                _pollingService.Start();
            }
        }

        private async Task DeleteOrderAsync()
        {
            if (SelectedOrder == null) return;

            try
            {
                var response = await _orderApi.DeleteOrder(SelectedOrder.Id);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show(body);
                    return;
                }

                MessageBox.Show("Заказ успешно удален.");
                await LoadAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении заказа: " + ex.Message);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void Dispose() => _pollingService.Stop();
    }
}
