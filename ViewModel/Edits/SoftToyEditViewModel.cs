using FlowerShop.Dto.DTOGet;
using FlowerShop.WpfClient.Services;
using FlowerShop.WpfClient.ViewModel.Base;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FlowerShop.WpfClient.ViewModel.Edits
{
    public sealed class SoftToyEditViewModel : IRequestClose, INotifyPropertyChanged
    {
        public bool IsEdit { get; }
        public string Title => IsEdit ? "Редактирование мягкой игрушки" : "Добавление мягкой игрушки";
        public string Subtitle => IsEdit ? "Обновите данные мягкой игрушки." : "Создайте новую мягкую игрушку.";
        public string OkText => IsEdit ? "Сохранить" : "Добавить";

        public string SoftToyName { get; set; } = "";
        public string SoftToyDescription { get; set; } = "";
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

        public SoftToyEditViewModel()
        {
            IsEdit = false;
            OkCommand = new RelayCommand(_ => RequestClose?.Invoke(true));
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }

        public SoftToyEditViewModel(GetSoftToyDto existing)
        {
            IsEdit = true;

            SoftToyName = existing.Name;
            SoftToyDescription = existing.Description;
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
