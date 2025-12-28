using FlowerShop.Dto.DTOGet;
using FlowerShop.WpfClient.ApiClient;
using FlowerShop.WpfClient.Timers;
using FlowerShop.WpfClient.ViewModel.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace FlowerShop.WpfClient.ViewModel
{
    public sealed class BouquetViewModel : IViewModelBase, INotifyPropertyChanged, IDisposable
    {
        public string Title => "Букеты";

        private readonly BouquetApi _bouquetApi;
        private ObservableCollection<GetBouquetDto> _bouquets;

        public ObservableCollection<GetBouquetDto> Bouquets
        {
            get => _bouquets;
            set
            {
                if (!Equals(_bouquets, value))
                {
                    _bouquets = value;
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

        public ICommand SearchCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        private readonly BouquetPollingService _pollingService;
        public BouquetViewModel(BouquetApi bouquetApi)
        {
            _bouquetApi = bouquetApi ?? throw new ArgumentNullException(nameof(bouquetApi));
            Bouquets = new ObservableCollection<GetBouquetDto>();

            _pollingService = new BouquetPollingService(_bouquetApi, OnBouquetsUpdated, TimeSpan.FromSeconds(10)); 

            SearchCommand = new RelayCommand(_ => SearchUserAsync());
            _ = LoadAsync();

            _pollingService.Start();
        }
        private void OnBouquetsUpdated(List<GetBouquetDto>? boquets)
        {
            if (boquets != null)
            {
                Bouquets = new ObservableCollection<GetBouquetDto>(boquets);
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
                var boquets = await _bouquetApi.GetBoquetByName(_searchText);
                if (boquets != null)
                {
                    Bouquets = new ObservableCollection<GetBouquetDto>(boquets);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске букета {ex.Message}");
            }
            _pollingService.Start();
        }

        public async Task LoadAsync()
        {
            try
            {
                var boquets = await _bouquetApi.GetAllBouquets();
                if (boquets != null)
                {
                    Bouquets = new ObservableCollection<GetBouquetDto>(boquets);
                }
                else
                {
                    Bouquets.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке букетов {ex.Message}");
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