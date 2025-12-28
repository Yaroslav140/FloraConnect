using FlowerShop.Dto.DTOGet;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FlowerShop.WpfClient.ViewModel
{
    public sealed class MainViewModel: INotifyPropertyChanged
    {
        public UserViewModel UsersVM { get; }
        public OrderViewModel OrdersVM { get; }
        public BouquetViewModel BouquetsVM { get; }

        private object _current;
        public object Current
        {
            get => _current;
            private set { _current = value; OnPropertyChanged(); }
        }

        private bool _isUsersSelected;
        public bool IsUsersSelected
        {
            get => _isUsersSelected;
            set
            {
                if (_isUsersSelected == value) return;
                _isUsersSelected = value;
                OnPropertyChanged();
                if (value) Current = UsersVM;
            }
        }

        private bool _isOrdersSelected;
        public bool IsOrdersSelected
        {
            get => _isOrdersSelected;
            set
            {
                if (_isOrdersSelected == value) return;
                _isOrdersSelected = value;
                OnPropertyChanged();
                if (value) Current = OrdersVM;
            }
        }

        private bool _isBouquetsSelected;
        public bool IdBouquetsSelected
        {
            get => _isBouquetsSelected;
            set
            {
                if (_isBouquetsSelected == value) return;
                _isBouquetsSelected = value;
                OnPropertyChanged();
                if (value) Current = BouquetsVM;
            }
        }

        public MainViewModel(UserViewModel usersVM, OrderViewModel ordersVM, BouquetViewModel boquetsVM)
        {
            UsersVM = usersVM;
            OrdersVM = ordersVM;
            BouquetsVM = boquetsVM;

            Current = OrdersVM;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
        public void Dispose()
        {
            (UsersVM as IDisposable)?.Dispose();
            (OrdersVM as IDisposable)?.Dispose();
            (BouquetsVM as IDisposable)?.Dispose();
        }
    }
}
