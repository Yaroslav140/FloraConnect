using FlowerShop.Dto.DTOGet;
using FlowerShop.WpfClient.ApiClient;
using System.Windows;

namespace FlowerShop.WpfClient.Timers
{
    public class OrderPollingService
    {
        private readonly Timer _timer;
        private readonly OrderApi _orderApi;
        private readonly Action<List<GetOrderDto>?> _onOrdersUpdated;
        private bool _isRunning;

        public OrderPollingService(OrderApi orderApi, Action<List<GetOrderDto>?> onOrdersUpdated, TimeSpan interval)
        {
            _orderApi = orderApi;
            _onOrdersUpdated = onOrdersUpdated;
            _timer = new Timer(async _ => await PollOrders(), null, Timeout.Infinite, (int)interval.TotalMilliseconds);
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

        private async Task PollOrders()
        {
            try
            {
                var orders = await _orderApi.GetAllOrders();
                Application.Current?.Dispatcher.Invoke(() => _onOrdersUpdated(orders));
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
