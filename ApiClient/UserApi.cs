using FlowerShop.Dto;
using System.Net.Http;

namespace FlowerShop.WpfClient.ApiClient
{
    public class UserApi
    {
        private readonly BaseApiClient _apiClient;

        public UserApi(BaseApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public Task<List<UserDto>?> GetAllUsers() => _apiClient.GetAsync<List<UserDto>>("api/users");

        public Task<HttpResponseMessage> CreateUser(UserDto dto) => _apiClient.PostAsync("api/users", dto);

        public Task<HttpResponseMessage> DeleteUser(string email) => _apiClient.DeleteAsync($"api/users?email={email}");
    }
}
