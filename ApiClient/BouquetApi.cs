using FlowerShop.Dto.DTOCreate;
using FlowerShop.Dto.DTOGet;
using FlowerShop.Dto.DTOUpdate;
using System.Net.Http;

namespace FlowerShop.WpfClient.ApiClient
{
    public class BouquetApi(BaseApiClient apiClient)
    {
        private readonly BaseApiClient _apiClient = apiClient;

        public Task<List<GetBouquetDto>?> GetAllBouquets() => _apiClient.GetAsync<List<GetBouquetDto>>("api/bouquets");
        public Task<List<GetBouquetDto>?> GetBoquetByName(string name) => _apiClient.GetAsyncByName<List<GetBouquetDto>>("api/bouquets", name);

        public Task<HttpResponseMessage> CreateBouquet(CreateBouquetDto dto) => _apiClient.PostAsync("api/bouquets", dto);

        public Task<HttpResponseMessage> UpdateBouquet(Guid Id, UpdateBouquetDto dto) => _apiClient.UpdateAsync($"api/bouquets/{Id}", dto);
        public Task<HttpResponseMessage> DeleteBouquet(string name) => _apiClient.DeleteAsync($"api/bouquets?name={name}");
    }
}
