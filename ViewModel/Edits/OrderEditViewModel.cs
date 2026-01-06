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
        public string OkText => IsEdit ? "Сохранить" : "Добавить заказ";

        public string CustomerName { get; set; } = "";
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime Date { get; set; } = DateTime.Today;

        public IReadOnlyList<string> Statuses { get; } = new[] { "Pending", "Completed", "Canceled" };

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool?>? RequestClose;

        public OrderEditViewModel(bool isEdit /*, GetOrderDto? existing = null */)
        {
            IsEdit = isEdit;

            // если edit — заполни из existing (добавишь позже)

            OkCommand = new RelayCommand(_ => RequestClose?.Invoke(true));
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }
    }
}
