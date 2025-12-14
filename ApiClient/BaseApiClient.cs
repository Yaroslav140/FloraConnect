using System.Net.Http;
using System.Net.Http.Json;

namespace FlowerShop.WpfClient.ApiClient
{
    public sealed class BaseApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        public BaseApiClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7241/")
            };
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            return await _httpClient.GetFromJsonAsync<T>(url);
        }

        public async Task<T?> GetAsyncByName<T>(string url, string name)
        {
            var fullUrl = $"{url}?name={Uri.EscapeDataString(name)}";
            return await _httpClient.GetFromJsonAsync<T>(fullUrl);
        }


        public async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
        {
            return await _httpClient.PostAsJsonAsync(url, data);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return await _httpClient.DeleteAsync(url);
        }

        public void Dispose() => _httpClient.Dispose();
    }
}
