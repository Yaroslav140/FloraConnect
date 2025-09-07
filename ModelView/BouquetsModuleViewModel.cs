using CommunityToolkit.Mvvm.ComponentModel;
using FlowerShop.Dto.DTOGet;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;

namespace FlowerShop.WpfClient.ModelView
{
    public partial class BouquetsModuleViewModel : ObservableObject, ICrudModule
    {
        public string Title => "Букеты";

        [ObservableProperty] private string searchText = "";
        [ObservableProperty] private object? selectedItem;

        public ICommand SearchCommand => null!;
        public ICommand AddCommand => null!;
        public ICommand EditCommand => null!;
        public ICommand DeleteCommand => null!;

        public IEnumerable Items => bouquetsView;

        private readonly ObservableCollection<GetBouquetDto> bouquets;
        private readonly ListCollectionView bouquetsView;
    }
}
