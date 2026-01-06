using FlowerShop.Dto.DTOGet;
using FlowerShop.Dto.DTOUpdate;
using FlowerShop.WpfClient.ApiClient;
using FlowerShop.WpfClient.Services;
using FlowerShop.WpfClient.Timers;
using FlowerShop.WpfClient.ViewModel.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace FlowerShop.WpfClient.ViewModel
{
    public sealed class UserViewModel : IViewModelBase, INotifyPropertyChanged, IDisposable
    {
        public string Title => "Пользователи";

        private readonly UserApi _userApi;
        private ObservableCollection<GetUserDto> _users;

        public ObservableCollection<GetUserDto> Users
        {
            get => _users;
            set
            {
                if (!Equals(_users, value))
                {
                    _users = value;
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
        private GetUserDto? _selectedUser;
        public GetUserDto? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (!Equals(_selectedUser, value))
                {
                    _selectedUser = value;
                    OnPropertyChanged();
                }
            }
        }
        private readonly IDialogService _dialog;

        public ICommand SearchCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        private readonly UserPollingService _pollingService;
        public UserViewModel(UserApi userApi, IDialogService dialog)
        {
            _userApi = userApi ?? throw new ArgumentNullException(nameof(userApi));
            Users = new ObservableCollection<GetUserDto>();

            _pollingService = new UserPollingService(_userApi, OnUsersUpdated, TimeSpan.FromSeconds(10));

            SearchCommand = new RelayCommand(_ => SearchUserAsync());
            _ = LoadAsync();
            _dialog = dialog;

            _pollingService.Start();
            CreateCommand = new RelayCommand(_ => Create());
            EditCommand = new RelayCommand(p => Edit(p as GetUserDto), p => p is GetUserDto);
        }
        private void OnUsersUpdated(List<GetUserDto>? users)
        {
            if (users != null)
            {
                Users = new ObservableCollection<GetUserDto>(users);
            }
        }

        private async void SearchUserAsync()
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
                var users = await _userApi.GetUserByName(_searchText);
                if (users != null)
                {
                    Users = new ObservableCollection<GetUserDto>(users);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при поиске пользователя");
            }
            _pollingService.Start();
        }

        public async Task LoadAsync()
        {
            try
            {
                var users = await _userApi.GetAllUsers();
                if (users != null)
                {
                    Users = new ObservableCollection<GetUserDto>(users);
                }
                else
                {
                    Users.Clear(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке пользователей");
            }
        }

        private void Create()
        {
            var vm = new UserEditViewModel();
            var ok = _dialog.ShowDialog(vm);

            if (ok == true)
            {
                // собрать DTO из vm и вызвать API Create
                // потом LoadAsync()
            }
        }

        private async void Edit(GetUserDto? user)
        {
            if (user == null) return;

            var vm = new UserEditViewModel(user);
            var ok = _dialog.ShowDialog(vm);

            if (ok == true)
            {
                try
                {
                    await _userApi.UpdateUser(new UpdateUserDto(
                        user.UserId,
                        vm.Username,
                        vm.Login,
                        null,
                        null));

                    await LoadAsync();
                }
                catch
                {
                    MessageBox.Show("Ошибка при обновлении пользователя");
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