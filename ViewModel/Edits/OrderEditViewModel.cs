using FlowerShop.Data.Models;
using FlowerShop.Dto.DTOGet;
using FlowerShop.WpfClient.Services;
using FlowerShop.WpfClient.ViewModel.Base;
using System.Windows.Input;

namespace FlowerShop.WpfClient.ViewModel
{
    public sealed class OrderEditViewModel : IRequestClose
    {
        public bool IsEdit { get; }
        public string Title => IsEdit ? "Редактировать заказ" : "Добавление нового заказа";
        public string Subtitle => IsEdit ? "Обновите данные заказа." : "Создайте новый заказ для клиента.";
        public string OkText => IsEdit ? "Сохранить" : "Добавить";

        public string CustomerName { get; set; } = "";
        public decimal TotalPrice { get; set; } = decimal.Zero;
        public OrderStatus Status { get; set; } = OrderStatus.New;
        public DateTime Date { get; set; } = DateTime.Today;

        public IEnumerable<OrderStatus> Statuses => Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>();

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool?>? RequestClose;

        public OrderEditViewModel()
        {
            IsEdit = false;
            OkCommand = new RelayCommand(_ => RequestClose?.Invoke(true));
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }

        public OrderEditViewModel(GetOrderDto existing)
        {
            IsEdit = true;

            CustomerName = existing.UserName;
            Status = existing.Status;

            OkCommand = new RelayCommand(_ => RequestClose?.Invoke(true));
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }
    }
}
