using FlowerShop.Dto.DTOGet;
using FlowerShop.WpfClient.ApiClient;
using System.Windows;

namespace FlowerShop.WpfClient.Timers
{
    public class BouquetPollingService
    {
        private readonly Timer _timer;
        private readonly BouquetApi _bouquetApi;
        private readonly Action<List<GetBouquetDto>?> _onBouquetsUpdated;
        private bool _isRunning;

        public BouquetPollingService(BouquetApi bouquetApi, Action<List<GetBouquetDto>?> onBouquetsUpdated, TimeSpan interval)
        {
            _bouquetApi = bouquetApi;
            _onBouquetsUpdated = onBouquetsUpdated;
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
                var bouquets = await _bouquetApi.GetAllBouquets();
                Application.Current?.Dispatcher.Invoke(() => _onBouquetsUpdated(bouquets));
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
