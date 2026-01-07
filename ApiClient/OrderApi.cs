using FlowerShop.Dto.DTOCreate;
using FlowerShop.Dto.DTOGet;
using FlowerShop.Dto.DTOUpdate;
using System.Net.Http;

namespace FlowerShop.WpfClient.ApiClient
{
    public class OrderApi(BaseApiClient apiClient)
    {
        private readonly BaseApiClient _apiClient = apiClient;

        public Task<List<GetOrderDto>?> GetAllOrders() => _apiClient.GetAsync<List<GetOrderDto>>("api/orders");
        public Task<List<GetOrderDto>?> GetOrdersByName(string name) => _apiClient.GetAsyncByName<List<GetOrderDto>>("api/orders", name);
        public Task<HttpResponseMessage> CreateOrder(CreateOrderDto orders) => _apiClient.PostAsync("api/orders", orders);
        public Task<HttpResponseMessage> UpdateOrder(UpdateOrderDto orders) => _apiClient.UpdateAsync("api/orders", orders);
        public Task<HttpResponseMessage> DeleteOrder(Guid id) => _apiClient.DeleteAsync($"api/orders/{id}");
    }
}
