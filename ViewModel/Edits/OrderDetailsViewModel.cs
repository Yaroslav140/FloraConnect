using FlowerShop.Data.Models;
using FlowerShop.Dto.DTOGet;
using FlowerShop.WpfClient.Services;
using FlowerShop.WpfClient.ViewModel.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FlowerShop.WpfClient.ViewModel.Edits
{
    public sealed class OrderDetailsViewModel : IRequestClose
    {
        public string Title => "Детали заказа";
        public string Subtitle => $"Клиент: {CustomerName}";
        public string OkText => "Закрыть";

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool?>? RequestClose;

        public string OrderId { get; set; }
        public string CustomerName { get; }
        public OrderStatus Status { get; }
        public DateTime Date { get; }
        public ObservableCollection<OrderItemDetailsVm> Items { get; }

        public decimal TotalPrice => Items.Sum(x => x.LineTotal);

        public OrderDetailsViewModel(GetOrderDto order)
        {
            CustomerName = order.UserName;
            Status = order.Status;
            Date = Convert.ToDateTime(order.PickupDate);
            OrderId = order.Id.ToString()[..8];
            Items = new ObservableCollection<OrderItemDetailsVm>(
                order.OrderItem.Select(it => new OrderItemDetailsVm
                {
                    BouquetName = it.Bouquet?.Name ?? "(без названия)",
                    Quantity = it.Quantity,
                    Price = it.Bouquet.Price
                }));

            OkCommand = new RelayCommand(_ => RequestClose?.Invoke(true));
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }
    }

    public sealed class OrderItemDetailsVm
    {
        public string BouquetName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal => Price * Quantity;
    }
}
