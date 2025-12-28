using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BLRB2B.Web.Services;

public class AuthService
{
    private readonly HttpService _httpService;
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _jsRuntime;
    private const string BasePath = "api/auth";
    private const string TokenKey = "authToken";
    private const string UserKey = "authUser";

    public bool IsAuthenticated { get; private set; }
    public UserInfoModel? CurrentUser { get; private set; }

    public AuthService(HttpService httpService, NavigationManager navigationManager, IJSRuntime jsRuntime)
    {
        _httpService = httpService;
        _navigationManager = navigationManager;
        _jsRuntime = jsRuntime;
    }

    public async Task<LoginResultModel?> LoginAsync(LoginModel model)
    {
        try
        {
            var result = await _httpService.PostAsync<LoginModel, LoginResultModel>($"{BasePath}/login", model);

            if (result != null && result.Success && !string.IsNullOrEmpty(result.Token))
            {
                // JWT token'ı localStorage'a kaydet
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, result.Token);

                // Kullanıcı bilgisini kaydet
                if (result.User != null)
                {
                    var userJson = JsonSerializer.Serialize(result.User);
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserKey, userJson);
                    CurrentUser = result.User;
                }

                IsAuthenticated = true;
                return result;
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Giriş işlemi başarısız oldu", ex);
        }
    }

    public async Task<RegisterResultModel?> RegisterAsync(RegisterModel model)
    {
        try
        {
            var result = await _httpService.PostAsync<RegisterModel, RegisterResultModel>($"{BasePath}/register", model);
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Kayıt işlemi başarısız oldu", ex);
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _httpService.PostAsync($"{BasePath}/logout", new { });
        }
        catch
        {
            // API call fail olsa da localStorage'ı temizle
        }
        finally
        {
            // localStorage'ı temizle
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", UserKey);

            IsAuthenticated = false;
            CurrentUser = null;
            _navigationManager.NavigateTo("/login");
        }
    }

    public async Task<UserInfoModel?> GetCurrentUserInfoAsync()
    {
        return await _httpService.GetAsync<UserInfoModel>($"{BasePath}/me");
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordModel model)
    {
        var result = await _httpService.PostAsync<ChangePasswordModel, bool>($"{BasePath}/change-password", model);
        return result;
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
    }

    public async Task InitializeAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
        var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", UserKey);

        if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userJson))
        {
            try
            {
                CurrentUser = JsonSerializer.Deserialize<UserInfoModel>(userJson);
                IsAuthenticated = true;
            }
            catch
            {
                await LogoutAsync();
            }
        }
    }
}

// Models
public class LoginModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}

public class LoginResultModel
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public UserInfoModel? User { get; set; }
}

public class UserInfoModel
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
}

public class ChangePasswordModel
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class RegisterModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class RegisterResultModel
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}
