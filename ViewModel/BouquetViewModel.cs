using FlowerShop.Dto.DTOCreate;
using FlowerShop.Dto.DTOGet;
using FlowerShop.Dto.DTOUpdate;
using FlowerShop.WpfClient.ApiClient;
using FlowerShop.WpfClient.Services;
using FlowerShop.WpfClient.Timers;
using FlowerShop.WpfClient.ViewModel.Base;
using FlowerShop.WpfClient.ViewModel.Edits;
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
        private readonly IDialogService _dialog;
        private ObservableCollection<GetBouquetDto> _bouquets;
        private readonly BouquetPollingService _pollingService;

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
        private GetBouquetDto? _selectedBouquet;
        public GetBouquetDto? SelectedBouquet
        {
            get => _selectedBouquet;
            set
            {
                _selectedBouquet = value;
                OnPropertyChanged();
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand CreateCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public BouquetViewModel(BouquetApi bouquetApi, IDialogService dialog)
        {
            _bouquetApi = bouquetApi ?? throw new ArgumentNullException(nameof(bouquetApi));
            Bouquets = [];
            _dialog = dialog;

            CreateCommand = new RelayCommand(_ => _ = CreateBouquetAsync());
            EditCommand = new RelayCommand(_ => _ = EditBouquetAsync(), _ => SelectedBouquet != null);
            DeleteCommand = new RelayCommand(_ => _ = DeleteBouquetAsync(), _ => SelectedBouquet != null);
            _ = LoadAsync();

            _pollingService = new BouquetPollingService(_bouquetApi, OnBouquetsUpdated, TimeSpan.FromSeconds(10));
            _pollingService.Start();
        }

        private void OnBouquetsUpdated(List<GetBouquetDto>? bouquets)
        {
            if (bouquets == null) return;

            if (!string.IsNullOrWhiteSpace(_searchText)) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Bouquets = new ObservableCollection<GetBouquetDto>(bouquets);
            });
        }

        public async Task SearchBouquetAsync()
        {
            _pollingService.Stop();

            try
            {
                var bouquets = await _bouquetApi.GetBoquetByName(_searchText ?? "");
                if (bouquets == null) return;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Bouquets = new ObservableCollection<GetBouquetDto>(bouquets);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске букета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _pollingService.Start();
            }
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
                MessageBox.Show($"Ошибка при загрузке букетов {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task CreateBouquetAsync()
        {
            _pollingService.Stop();
            try
            {
                var vm = new BouquetEditViewModel();

                var ok = _dialog.ShowDialog(vm);
                if (ok != true) return;

                var dto = new CreateBouquetDto(
                    vm.BouquetName,
                    vm.Price,
                    vm.BouquetDescription,
                    vm.Quantity,
                    vm.ImagePath
                );

                var response = await _bouquetApi.CreateBouquet(dto);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show(body, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Букет успешно создан.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании букета: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _pollingService.Start();
            }
        }
        private async Task EditBouquetAsync()
        {
            if (SelectedBouquet == null) return;

            _pollingService.Stop();
            try
            {
                var vm = new BouquetEditViewModel(SelectedBouquet);

                var ok = _dialog.ShowDialog(vm);
                if (ok != true) return;

                var dto = new UpdateBouquetDto(
                    vm.BouquetName,
                    vm.BouquetDescription,
                    vm.Price,
                    vm.Quantity,
                    vm.ImagePath
                );

                var response = await _bouquetApi.UpdateBouquet(SelectedBouquet.BouquetId, dto);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show(body, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Букет успешно обновлён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при редактировании букета: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _pollingService.Start();
            }
        }
        private async Task DeleteBouquetAsync()
        {
            if (SelectedBouquet == null) return;

            try
            {
                var response = await _bouquetApi.DeleteBouquet(SelectedBouquet.Name);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show(body, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Букет успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении букета: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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