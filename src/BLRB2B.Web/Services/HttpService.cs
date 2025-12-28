using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;

namespace BLRB2B.Web.Services;

public class HttpService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<HttpService> _logger;

    public HttpService(
        HttpClient httpClient,
        NavigationManager navigationManager,
        ILogger<HttpService> logger)
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
        _logger = logger;
    }

    private JsonSerializerOptions Options => new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<T?> GetAsync<T>(string uri)
    {
        try
        {
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(Options);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error GET request to {Uri}", uri);
            throw;
        }
    }

    public async Task<TResult?> PostAsync<TModel, TResult>(string uri, TModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(uri, model, Options);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResult>(Options);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error POST request to {Uri}", uri);
            throw;
        }
    }

    public async Task<T?> PostAsync<T>(string uri, T model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(uri, model, Options);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(Options);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error POST request to {Uri}", uri);
            throw;
        }
    }

    public async Task PutAsync<T>(string uri, T model)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(uri, model, Options);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error PUT request to {Uri}", uri);
            throw;
        }
    }

    public async Task DeleteAsync(string uri)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(uri);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error DELETE request to {Uri}", uri);
            throw;
        }
    }

    public async Task<T?> PostFileAsync<T>(string uri, MultipartFormDataContent content)
    {
        try
        {
            var response = await _httpClient.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(Options);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error POST file request to {Uri}", uri);
            throw;
        }
    }
}
