using FlowerShop.Dto.DTOGet;
using FlowerShop.WpfClient.ApiClient;
using System.Windows;

namespace FlowerShop.WpfClient.Timers
{
    public class UserPollingService
    {
        private readonly Timer _timer;
        private readonly UserApi _userApi;
        private readonly Action<List<GetUserDto>?> _onUsersUpdated;
        private bool _isRunning;

        public UserPollingService(UserApi userApi, Action<List<GetUserDto>?> onUsersUpdated, TimeSpan interval)
        {
            _userApi = userApi;
            _onUsersUpdated = onUsersUpdated;
            _timer = new Timer(async _ => await PollUsers(), null, Timeout.Infinite, (int)interval.TotalMilliseconds);
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

        private async Task PollUsers()
        {
            try
            {
                var users = await _userApi.GetAllUsers();
                Application.Current?.Dispatcher.Invoke(() => _onUsersUpdated(users));
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
