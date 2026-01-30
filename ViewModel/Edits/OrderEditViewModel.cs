using FlowerShop.Data.Models;
using FlowerShop.Dto.DTOCreate;
using FlowerShop.Dto.DTOGet;
using FlowerShop.Dto.DTOUpdate;
using FlowerShop.WpfClient.Services;
using FlowerShop.WpfClient.ViewModel.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace FlowerShop.WpfClient.ViewModel.Edits
{
    public sealed class OrderEditViewModel : IRequestClose, INotifyPropertyChanged
    {
        public bool IsEdit { get; }
        public string Title => IsEdit ? "Редактировать заказ" : "Добавление нового заказа";
        public string Subtitle => IsEdit ? "Обновите данные заказа." : "Создайте новый заказ для клиента.";
        public string OkText => IsEdit ? "Сохранить" : "Добавить";

        public string CustomerName { get; set; } = "";
        public string Login { get; set; } = "";
        public string? DeleveryAddress { get; set; } = "";
        public OrderStatus Status { get; set; } = OrderStatus.New;
        public DateTime Date { get; set; } = DateTime.Now;

        public IEnumerable<OrderStatus> Statuses => Enum.GetValues<OrderStatus>().Cast<OrderStatus>();

        public ObservableCollection<GetBouquetDto> Bouquets { get; } = [];

        private GetBouquetDto? _selectedBouquet;
        public GetBouquetDto? SelectedBouquet
        {
            get => _selectedBouquet;
            set { _selectedBouquet = value; OnPropertyChanged(); RaiseCanExecutes(); }
        }

        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); RaiseCanExecutes(); }
        }

        public ObservableCollection<OrderItemVm> OrderItems { get; } = new();

        private OrderItemVm? _selectedItem;
        public OrderItemVm? SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); RaiseCanExecutes(); }
        }

        public decimal TotalPrice =>
            OrderItems.Sum(x => x.Price * x.Quantity);

        public ICommand AddBouquetCommand { get; }
        public ICommand RemoveBouquetCommand { get; }
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool?>? RequestClose;
        public event PropertyChangedEventHandler? PropertyChanged;

        public OrderEditViewModel()
        {
            IsEdit = false;

            AddBouquetCommand = new RelayCommand(
                _ => AddBouquet(),
                _ => SelectedBouquet != null && Quantity > 0
            );

            RemoveBouquetCommand = new RelayCommand(
                _ => RemoveBouquet(),
                _ => SelectedItem != null
            );

            OkCommand = new RelayCommand(_ => RequestClose?.Invoke(true));
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }

        public OrderEditViewModel(GetOrderDto existing) : this()
        {
            IsEdit = true;
            CustomerName = existing.UserName;
            Login = existing.Login;
            DeleveryAddress = existing.DeliveryAddress;
            Status = existing.Status;
            Date = existing.PickupDate;

            OrderItems.Clear();

            if (existing.OrderItem != null)
            {
                foreach (var it in existing.OrderItem)
                {
                    OrderItems.Add(new OrderItemVm
                    {
                        OrderItemId = it.OrderItemId,
                        BouquetId = it.BouquetId,
                        SoftToyId = it.SoftToyId,       
                        Bouquet = it.Bouquet,
                        SoftToy = it.SoftToy,           
                        Quantity = it.Quantity,
                        Price = it.Price
                    });
                }
            }

            OnPropertyChanged(nameof(TotalPrice));
        }

        private void AddBouquet()
        {
            if (SelectedBouquet is null) return;

            if (Quantity <= 0)
            {
                MessageBox.Show("Количество должно быть больше 0.");
                return;
            }

            var available = SelectedBouquet.Quantity;
            if (available <= 0)
            {
                MessageBox.Show("Недостаточно букетов на складе.");
                return;
            }

            var item = OrderItems.FirstOrDefault(x => x.BouquetId == SelectedBouquet.BouquetId);

            if (item is null)
            {
                var qtyToAdd = Math.Min(Quantity, available);

                OrderItems.Add(new OrderItemVm
                {
                    OrderItemId = null,
                    BouquetId = SelectedBouquet.BouquetId,
                    Bouquet = SelectedBouquet,
                    Quantity = qtyToAdd,
                    Price = SelectedBouquet.Price
                });

                OnPropertyChanged(nameof(TotalPrice));
                return;
            }

            var canAdd = available - item.Quantity;
            if (canAdd <= 0)
            {
                MessageBox.Show("Достигнут максимум доступного количества.");
                return;
            }

            item.Quantity += Math.Min(Quantity, canAdd);

            OnPropertyChanged(nameof(TotalPrice));
        }

        private void RemoveBouquet()
        {
            if (SelectedItem == null) return;

            OrderItems.Remove(SelectedItem);
            SelectedItem = null;
            OnPropertyChanged(nameof(TotalPrice));
            RaiseCanExecutes();
        }

        private void RaiseCanExecutes()
        {
            (AddBouquetCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (RemoveBouquetCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        public List<CreateOrderItemDto> BuildCreateItems() =>
            [.. OrderItems.Select(x => new CreateOrderItemDto(
                x.BouquetId,
                x.SoftToyId,
                null,
                x.Quantity,
                x.Price
            ))];

        public List<UpdateOrderItemDto> BuildUpdateItems() =>
            [.. OrderItems.Select(x => new UpdateOrderItemDto(
                x.OrderItemId,
                x.BouquetId,
                x.SoftToyId,
                x.Quantity,
                x.Price
            ))];

        private void OnPropertyChanged([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }

    public sealed class OrderItemVm : INotifyPropertyChanged
    {
        public Guid? OrderItemId { get; set; }
        public Guid? BouquetId { get; set; }
        public Guid? SoftToyId { get; set; }
        public GetBouquetDto? Bouquet { get; set; }
        public GetSoftToyDto? SoftToy { get; set; }
        public decimal LineTotal => Price * Quantity;
        public string ProductName => Bouquet?.Name ?? SoftToy?.Name ?? "Неизвестно";
        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LineTotal));
            }
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LineTotal));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
