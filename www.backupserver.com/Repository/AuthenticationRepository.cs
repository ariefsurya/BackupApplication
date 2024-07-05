using Model;
using Newtonsoft.Json;

namespace www.backupserver.com.Repository
{
    public interface IAuthenticationRepository
    {
        public Task<ApiResponse> HandleLogin(LoginDTO user);
        public Task<ApiResponse> HandleRegister(RegisterDTO oRegister);
    }

    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CustomAuthenticationStateProvider _authStateProvider;

        public AuthenticationRepository(IHttpClientFactory httpClientFactory, CustomAuthenticationStateProvider authStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authStateProvider = authStateProvider;
        }

        public async Task<ApiResponse> HandleLogin(LoginDTO user)
        {
            string url = StaticEndpoint.BaseUrl + "/Authentication/Login";
            var httpClient = _httpClientFactory.CreateClient("CustomHttpClient");
            httpClient.BaseAddress = new Uri(StaticEndpoint.BaseUrl); // Ensure BaseAddress is set

            var response = await httpClient.PostAsJsonAsync(url, user);

            ApiResponse apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
            if (response.IsSuccessStatusCode)
            {
                var loginData = JsonConvert.DeserializeObject<User>(apiResponse.Data.ToString());
                _authStateProvider.MarkUserAsAuthenticated(loginData.Token);
            }

            return apiResponse;
        }
        //public async Task<ApiResponse> HandleLogin(LoginDTO user)
        //{
        //    string url = StaticEndpoint.BaseUrl + "/Authentication/Login";
        //    var httpClient = _httpClientFactory.CreateClient("CustomHttpClient");
        //    var response = await httpClient.PostAsJsonAsync(url, user);

        //    ApiResponse apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var loginData = JsonConvert.DeserializeObject<User>(apiResponse.Data.ToString());
        //        _authStateProvider.MarkUserAsAuthenticated(loginData.Token);
        //    }

        //    return apiResponse;
        //}


        public async Task<ApiResponse> HandleRegister(RegisterDTO oRegister)
        {
            string url = StaticEndpoint.BaseUrl + "/Authentication/Register";
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsJsonAsync(url, oRegister);

            ApiResponse apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
            if (response.IsSuccessStatusCode)
            {
                var loginData = JsonConvert.DeserializeObject<User>(apiResponse.Data.ToString());
                _authStateProvider.MarkUserAsAuthenticated(loginData.Token);
            }

            return apiResponse;
        }
    }
}
