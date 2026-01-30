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
    public sealed class SoftToyViewModel : IViewModelBase, INotifyPropertyChanged, IDisposable
    {
        public string Title => "Мягкие игрушки";

        private readonly SoftToyApi _softToyApi;
        private readonly IDialogService _dialog;
        private ObservableCollection<GetSoftToyDto> _softToys;
        private readonly SoftToyPollingService _pollingService;

        public ObservableCollection<GetSoftToyDto> SoftToys
        {
            get => _softToys;
            set
            {
                if (!Equals(_softToys, value))
                {
                    _softToys = value;
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
        private GetSoftToyDto? _selectedSoftToy;
        public GetSoftToyDto? SelectedSoftToy
        {
            get => _selectedSoftToy;
            set
            {
                _selectedSoftToy = value;
                OnPropertyChanged();
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand CreateCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public SoftToyViewModel(SoftToyApi softToyApi, IDialogService dialog)
        {
            _softToyApi = softToyApi ?? throw new ArgumentNullException(nameof(softToyApi));
            SoftToys = [];
            _dialog = dialog;

            CreateCommand = new RelayCommand(_ => _ = CreateSoftToyAsync());
            EditCommand = new RelayCommand(_ => _ = EditSoftToyAsync(), _ => SelectedSoftToy != null);
            DeleteCommand = new RelayCommand(_ => _ = DeleteSoftToyAsync(), _ => SelectedSoftToy != null);
            _ = LoadAsync();

            _pollingService = new SoftToyPollingService(_softToyApi, OnSoftToysUpdated, TimeSpan.FromSeconds(10));
            _pollingService.Start();
        }

        private void OnSoftToysUpdated(List<GetSoftToyDto>? softToys)
        {
            if (softToys == null) return;

            if (!string.IsNullOrWhiteSpace(_searchText)) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                SoftToys = new ObservableCollection<GetSoftToyDto>(softToys);
            });
        }

        public async Task SearchSoftToyAsync()
        {
            _pollingService.Stop();

            try
            {
                var softToy = await _softToyApi.GetSoftToyByName(_searchText ?? "");
                if (softToy == null) return;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    SoftToys = new ObservableCollection<GetSoftToyDto>(softToy);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске мягкой игрушки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                var softToys = await _softToyApi.GetAllSoftToys();
                if (softToys != null)
                {
                    SoftToys = new ObservableCollection<GetSoftToyDto>(softToys);
                }
                else
                {
                    SoftToys.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке мягких игрушек {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task CreateSoftToyAsync()
        {
            _pollingService.Stop();
            try
            {
                var vm = new SoftToyEditViewModel();

                var ok = _dialog.ShowDialog(vm);
                if (ok != true) return;

                var dto = new CreateSoftToyDto(
                    vm.SoftToyName,
                    vm.SoftToyDescription,
                    vm.Quantity,
                    vm.Price,
                    vm.ImagePath
                );

                var response = await _softToyApi.CreateSoftToy(dto);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show(body, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Мягкая игрушка успешно создана.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании мягкой игрушки: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _pollingService.Start();
            }
        }
        private async Task EditSoftToyAsync()
        {
            if (SelectedSoftToy == null) return;

            _pollingService.Stop();
            try
            {
                var vm = new SoftToyEditViewModel(SelectedSoftToy);

                var ok = _dialog.ShowDialog(vm);
                if (ok != true) return;

                var dto = new UpdateBouquetDto(
                    vm.SoftToyName,
                    vm.SoftToyDescription,
                    vm.Price,
                    vm.Quantity,
                    vm.ImagePath
                );

                var response = await _softToyApi.UpdateSoftToy(SelectedSoftToy.SoftToyId, dto);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show(body, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Мягкая игрушка успешно обновлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при редактировании мягкой игрушки: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _pollingService.Start();
            }
        }
        private async Task DeleteSoftToyAsync()
        {
            if (SelectedSoftToy == null) return;

            try
            {
                var response = await _softToyApi.DeleteSoftToy(SelectedSoftToy.Name);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show(body, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Мягкая игрушка успешно удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении мягкой игрушки: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
