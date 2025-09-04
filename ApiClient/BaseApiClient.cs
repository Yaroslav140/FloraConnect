using System.Net.Http;
using System.Net.Http.Json;

namespace FlowerShop.WpfClient.ApiClient
{
    public sealed class BaseApiClient
    {
        private readonly HttpClient _httpClient;

        public BaseApiClient(string baseAddress)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };
        }

        // Универсальные методы (можно расширять под GET/POST/PUT/DELETE)
        public async Task<T?> GetAsync<T>(string url)
        {
            return await _httpClient.GetFromJsonAsync<T>(url);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
        {
            return await _httpClient.PostAsJsonAsync(url, data);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return await _httpClient.DeleteAsync(url);
        }
    }
}
