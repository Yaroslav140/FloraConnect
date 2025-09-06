using FlowerShop.Dto.DTOGet;
using FlowerShop.Dto.DTOCreate;
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

        public Task<List<GetUserDto>?> GetAllUsers() => _apiClient.GetAsync<List<GetUserDto>>("api/users");

        public Task<HttpResponseMessage> CreateUser(CreateUserDto dto) => _apiClient.PostAsync("api/users", dto);

        public Task<HttpResponseMessage> DeleteUser(string email) => _apiClient.DeleteAsync($"api/users?email={email}");
    }
}
