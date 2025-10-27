using FlowerShop.Dto.DTOCreate;
using FlowerShop.Dto.DTOGet;
using System.Net.Http;

namespace FlowerShop.WpfClient.ApiClient
{
    public class OrderApi(BaseApiClient apiClient)
    {
        private readonly BaseApiClient _apiClient = apiClient;

        public Task<List<GetOrderDto>?> GetAllOrders() => _apiClient.GetAsync<List<GetOrderDto>>("api/orders");
        public Task<HttpResponseMessage> CreateOrder(CreateOrderDto orders) => _apiClient.PostAsync("api/orders", orders);
    }
}
