using FlowerShop.Dto.DTOCreate;
using FlowerShop.Dto.DTOGet;
using FlowerShop.Dto.DTOUpdate;
using System.Net.Http;

namespace FlowerShop.WpfClient.ApiClient
{
    public class SoftToyApi(BaseApiClient apiClient)
    {
        private readonly BaseApiClient _apiClient = apiClient;

        public Task<List<GetSoftToyDto>?> GetAllSoftToys() => _apiClient.GetAsync<List<GetSoftToyDto>>("api/softtoys");
        public Task<List<GetSoftToyDto>?> GetSoftToyByName(string name) => _apiClient.GetAsyncByName<List<GetSoftToyDto>>("api/bouquets", name);

        public Task<HttpResponseMessage> CreateSoftToy(CreateSoftToyDto dto) => _apiClient.PostAsync("api/softtoys", dto);

        public Task<HttpResponseMessage> UpdateSoftToy(Guid Id, UpdateBouquetDto dto) => _apiClient.UpdateAsync($"api/softtoys/{Id}", dto);
        public Task<HttpResponseMessage> DeleteSoftToy(string name) => _apiClient.DeleteAsync($"api/softtoys?name={name}");
    }
}
