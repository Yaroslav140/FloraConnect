using CommunityToolkit.Mvvm.ComponentModel;
using FlowerShop.Dto.DTOGet;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;

namespace FlowerShop.WpfClient.ModelView
{
    public partial class UsersModuleViewModel : ObservableObject, ICrudModule
    {
        public string Title => "Пользователи";

        [ObservableProperty] private string searchText = "";
        [ObservableProperty] private object? selectedItem;

        public ICommand SearchCommand => null!;
        public ICommand AddCommand => null!;
        public ICommand EditCommand => null!;
        public ICommand DeleteCommand => null!;

        public IEnumerable Items => usersView;

        private readonly ObservableCollection<GetUserDto> users;
        private readonly ListCollectionView usersView;
    }
}
