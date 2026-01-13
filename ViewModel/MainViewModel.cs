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
                    System.Diagnostics.Debug.WriteLine("Switched to OrdersVM");
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
                }
            }
        }


        public ICommand SearchCommand { get; }

        public MainViewModel(UserViewModel usersVM, OrderViewModel ordersVM, BouquetViewModel boquetsVM)
        {
            UsersVM = usersVM;
            OrdersVM = ordersVM;
            BouquetsVM = boquetsVM;

            Current = OrdersVM;
            IsOrdersSelected = true;
            SearchCommand = new RelayCommand(_ => ExecuteSearch(), _ => CanSearch());
        }

        private bool CanSearch()
        {
            var canSearch = Current is BouquetViewModel or UserViewModel or OrderViewModel;
            System.Diagnostics.Debug.WriteLine($"CanSearch: {canSearch}, Current: {Current?.GetType().Name}");
            return canSearch;
        }

        private void ExecuteSearch()
        {
            System.Diagnostics.Debug.WriteLine("=== ExecuteSearch CALLED ===");
            System.Diagnostics.Debug.WriteLine($"Current: {Current?.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"SearchText: '{SearchText}'");

            if (Current is BouquetViewModel bouquetVM)
            {
                System.Diagnostics.Debug.WriteLine("Executing BouquetVM search");
                bouquetVM.SearchText = SearchText;
                _ = bouquetVM.SearchBouquetAsync();
            }
            else if (Current is UserViewModel userVM)
            {
                System.Diagnostics.Debug.WriteLine("Executing UserVM search");
                userVM.SearchText = SearchText;
                _ = userVM.SearchUserAsync();
            }
            else if (Current is OrderViewModel orderVM)
            {
                System.Diagnostics.Debug.WriteLine("Executing OrderVM search");
                orderVM.SearchText = SearchText;
                _ = orderVM.SearchOrderAsync();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: Unknown Current type: {Current?.GetType().Name}");
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
        }
    }
}
