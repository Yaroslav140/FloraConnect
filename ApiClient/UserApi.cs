using FlowerShop.Dto.DTOCreate;
using FlowerShop.Dto.DTOGet;
using System.Net.Http;

namespace FlowerShop.WpfClient.ApiClient
{
    public class UserApi(BaseApiClient apiClient)
    {
        private readonly BaseApiClient _apiClient = apiClient;

        public Task<List<GetUserDto>?> GetAllUsers() => _apiClient.GetAsync<List<GetUserDto>>("api/users");
        public Task<List<GetUserDto>?> GetUserByName(string name) => _apiClient.GetAsyncByName<List<GetUserDto>>("api/users", name);

        public Task<HttpResponseMessage> CreateUser(CreateUserDto dto) => _apiClient.PostAsync("api/users", dto);

        public Task<HttpResponseMessage> DeleteUser(string email) => _apiClient.DeleteAsync($"api/users?email={email}");
    }
}
