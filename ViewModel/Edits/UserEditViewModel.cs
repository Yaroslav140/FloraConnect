using FlowerShop.Dto.DTOGet;
using FlowerShop.Dto.DTOUpdate;
using FlowerShop.WpfClient.Services;
using FlowerShop.WpfClient.ViewModel.Base;
using System.Windows.Input;

namespace FlowerShop.WpfClient.ViewModel.Edits
{
    public sealed class UserEditViewModel : IRequestClose
    {
        public bool IsEdit { get; }
        public string Title => IsEdit ? "Редактирование пользователя" : "Добавление нового пользователя";
        public string Subtitle => IsEdit ? "Обновите данные клиента." : "Создайте нового клиента.";
        public string OkText => IsEdit ? "Сохранить" : "Добавить";

        public string Username { get; set; } = "";
        public string Login { get; set; } = "";

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool?>? RequestClose;

        public UserEditViewModel()
        {
            IsEdit = false;
            OkCommand = new RelayCommand(_ => RequestClose?.Invoke(true));
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }

        public UserEditViewModel(GetUserDto existing)
        {
            IsEdit = true;

            Username = existing.Username;
            Login = existing.Login;

            OkCommand = new RelayCommand(_ => RequestClose?.Invoke(true));
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }
    }
}
