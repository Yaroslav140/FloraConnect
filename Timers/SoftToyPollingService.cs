using FlowerShop.Dto.DTOGet;
using FlowerShop.WpfClient.ApiClient;
using System.Windows;

namespace FlowerShop.WpfClient.Timers
{
    public class SoftToyPollingService
    {
        private readonly Timer _timer;
        private readonly SoftToyApi _softToyApi;
        private readonly Action<List<GetSoftToyDto>?> _onSoftToysUpdated;
        private bool _isRunning;

        public SoftToyPollingService(SoftToyApi softToyApi, Action<List<GetSoftToyDto>?> onSoftToysUpdated, TimeSpan interval)
        {
            _softToyApi = softToyApi;
            _onSoftToysUpdated = onSoftToysUpdated;
            _timer = new Timer(async _ => await PollBouquets(), null, Timeout.Infinite, (int)interval.TotalMilliseconds);
        }

        public void Start()
        {
            if (!_isRunning)
            {
                _timer.Change(0, Timeout.Infinite);
                _isRunning = true;
            }
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _isRunning = false;
        }

        private async Task PollBouquets()
        {
            try
            {
                var bouquets = await _softToyApi.GetAllSoftToys();
                Application.Current?.Dispatcher.Invoke(() => _onSoftToysUpdated(bouquets));
            }
            catch
            {
            }
            finally
            {
                if (_isRunning)
                    _timer.Change((int)TimeSpan.FromSeconds(5).TotalMilliseconds, Timeout.Infinite);
            }
        }
    }
}
