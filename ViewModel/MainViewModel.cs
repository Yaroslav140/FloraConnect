using FlowerShop.Dto.DTOGet;
using FlowerShop.WpfClient.ViewModel.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FlowerShop.WpfClient.ViewModel
{
    public sealed class MainViewModel: INotifyPropertyChanged
    {
        public UserViewModel UsersVM { get; }
        public OrderViewModel OrdersVM { get; }
        public BouquetViewModel BouquetsVM { get; }
        public SoftToyViewModel SoftToysVM { get; }

        private object _current;
        public object Current
        {
            get => _current;
            private set
            {
                _current = value;
                OnPropertyChanged();
                (SearchCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
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
                if (value)
                {
                    Current = UsersVM;
                    SearchText = UsersVM.SearchText ?? "";
                }
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
                if (value)
                {
                    Current = OrdersVM;
                    SearchText = OrdersVM.SearchText ?? "";
                }
            }
        }

        private bool _isBouquetsSelected;
        public bool IsBouquetsSelected
        {
            get => _isBouquetsSelected;
            set
            {
                if (_isBouquetsSelected == value) return;
                _isBouquetsSelected = value;
                OnPropertyChanged();
                if (value)
                {
                    Current = BouquetsVM;
                    SearchText = BouquetsVM.SearchText ?? "";
                }
            }
        }

        private bool _isSoftToysSelected;
        public bool IsSoftToysSelected
        {
            get => _isSoftToysSelected;
            set
            {
                if (_isSoftToysSelected == value) return;
                _isSoftToysSelected = value;
                OnPropertyChanged();
                if (value)
                {
                    Current = SoftToysVM;
                    SearchText = SoftToysVM.SearchText ?? "";
                }
            }
        }
        private string? _searchText;
        public string? SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();

                    if (Current is BouquetViewModel bouquetVM)
                        bouquetVM.SearchText = value;
                    else if (Current is UserViewModel userVM)
                        userVM.SearchText = value;
                    else if (Current is OrderViewModel orderVM)
                        orderVM.SearchText = value;
                    else if (Current is SoftToyViewModel softToyVM)
                        softToyVM.SearchText = value;
                }
            }
        }


        public ICommand SearchCommand { get; }

        public MainViewModel(UserViewModel usersVM, OrderViewModel ordersVM, BouquetViewModel boquetsVM, SoftToyViewModel softToyVM)
        {
            UsersVM = usersVM;
            OrdersVM = ordersVM;
            BouquetsVM = boquetsVM;
            SoftToysVM = softToyVM;

            Current = OrdersVM;
            IsOrdersSelected = true;
            SearchCommand = new RelayCommand(_ => ExecuteSearch(), _ => CanSearch());
        }

        private bool CanSearch()
        {
            var canSearch = Current is BouquetViewModel or UserViewModel or OrderViewModel or SoftToyViewModel;
            return canSearch;
        }

        private void ExecuteSearch()
        {
            if (Current is BouquetViewModel bouquetVM)
            {
                bouquetVM.SearchText = SearchText;
                _ = bouquetVM.SearchBouquetAsync();
            }
            else if (Current is UserViewModel userVM)
            {
                userVM.SearchText = SearchText;
                _ = userVM.SearchUserAsync();
            }
            else if (Current is OrderViewModel orderVM)
            {
                orderVM.SearchText = SearchText;
                _ = orderVM.SearchOrderAsync();
            }
            else if (Current is SoftToyViewModel softToyVM)
            {
                softToyVM.SearchText = SearchText;
                _ = softToyVM.SearchSoftToyAsync();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
        public void Dispose()
        {
            (UsersVM as IDisposable)?.Dispose();
            (OrdersVM as IDisposable)?.Dispose();
            (BouquetsVM as IDisposable)?.Dispose();
            (SoftToysVM as IDisposable)?.Dispose();
        }
    }
}
