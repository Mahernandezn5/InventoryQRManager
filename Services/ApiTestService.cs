using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using InventoryQRManager.Models.DTOs;

namespace InventoryQRManager.Services
{
    public class ApiTestService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiTestService()
        {
            _httpClient = new HttpClient();
            _baseUrl = "http://localhost:5000/api";
        }

        public async Task<bool> TestApiConnection()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/inventory/summary");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ApiResponse<InventorySummaryDto>?> GetInventorySummary()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/inventory/summary");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse<InventorySummaryDto>>(json);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ApiResponse<List<InventoryItemDto>>?> GetAllItems()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/inventory");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse<List<InventoryItemDto>>>(json);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ApiResponse<InventoryItemDto>?> CreateItem(CreateInventoryItemDto item)
        {
            try
            {
                var json = JsonConvert.SerializeObject(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_baseUrl}/inventory", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse<InventoryItemDto>>(responseJson);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ApiResponse<InventoryItemDto>?> UpdateItem(int id, UpdateInventoryItemDto item)
        {
            try
            {
                var json = JsonConvert.SerializeObject(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"{_baseUrl}/inventory/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse<InventoryItemDto>>(responseJson);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteItem(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/inventory/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ApiResponse<List<InventoryItemDto>>?> SearchItems(string term)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/inventory/search?term={Uri.EscapeDataString(term)}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ApiResponse<List<InventoryItemDto>>>(json);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
