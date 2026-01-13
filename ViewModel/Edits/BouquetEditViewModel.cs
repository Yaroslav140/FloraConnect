using FlowerShop.Dto.DTOGet;
using FlowerShop.WpfClient.Services;
using FlowerShop.WpfClient.ViewModel.Base;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FlowerShop.WpfClient.ViewModel.Edits
{
    public sealed class BouquetEditViewModel : IRequestClose, INotifyPropertyChanged
    {
        public bool IsEdit { get; }
        public string Title => IsEdit ? "Редактирование букета" : "Добавление нового букета";
        public string Subtitle => IsEdit ? "Обновите данные букета." : "Создайте новый букет.";
        public string OkText => IsEdit ? "Сохранить" : "Добавить";

        public string BouquetName { get; set; } = "";
        public string BouquetDescription { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        private string _imagePath = "";
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged();
            }
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool?>? RequestClose;
        public event PropertyChangedEventHandler? PropertyChanged;

        public BouquetEditViewModel()
        {
            IsEdit = false;
            OkCommand = new RelayCommand(_ => RequestClose?.Invoke(true));
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }

        public BouquetEditViewModel(GetBouquetDto existing)
        {
            IsEdit = true;

            BouquetName = existing.Name;
            BouquetDescription = existing.Description;
            Price = existing.Price;
            Quantity = existing.Quantity;
            ImagePath = existing.ImagePath;

            OkCommand = new RelayCommand(_ => RequestClose?.Invoke(true));
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
