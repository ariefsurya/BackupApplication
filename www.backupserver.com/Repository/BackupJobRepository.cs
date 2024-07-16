using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using Newtonsoft.Json;
using www.backupserver.com.Pages.User;

namespace www.backupserver.com.Repository
{
    public interface IBackupJobRepository
    {
        public Task<ApiResponse> GetCompanyBackupJob(string search, int iPage, int iTake);
        public Task<ApiResponse> GetCompanyBackupJobDetail(int backupJobId);
        public Task<ApiResponse> GetCompanyBackupJobHistory(int backupJobId);
        public Task<ApiResponse> SaveCompanyBackupJobDetail(BackupJobDTO oBackupJobDTO);
    }

    public class BackupJobRepository : IBackupJobRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CustomAuthenticationStateProvider _authStateProvider;

        public BackupJobRepository(IHttpClientFactory httpClientFactory, CustomAuthenticationStateProvider authStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authStateProvider = authStateProvider; 
        }

        public async Task<ApiResponse> GetCompanyBackupJob(string search, int iPage, int iTake)
        {
            var token = await _authStateProvider.GetAuthenticationTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            string url = StaticEndpoint.BaseUrl + $"/BackupJob/GetCompanyBackupJob?search={Uri.EscapeDataString(search)}&iPage={iPage}&iTake={iTake}";
            var httpClient = _httpClientFactory.CreateClient("CustomHttpClient");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync(url);

            ApiResponse apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();

            return apiResponse;
        }
        public async Task<ApiResponse> GetCompanyBackupJobDetail(int backupJobId)
        {
            var token = await _authStateProvider.GetAuthenticationTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            string url = StaticEndpoint.BaseUrl + $"/BackupJob/GetCompanyBackupJobDetail?id={backupJobId}";
            var httpClient = _httpClientFactory.CreateClient("CustomHttpClient");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync(url);

            ApiResponse apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();

            return apiResponse;
        }
        public async Task<ApiResponse> GetCompanyBackupJobHistory(int backupJobId)
        {
            var token = await _authStateProvider.GetAuthenticationTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            string url = StaticEndpoint.BaseUrl + $"/BackupJob/GetCompanyBackupJobHistory?id={backupJobId}";
            var httpClient = _httpClientFactory.CreateClient("CustomHttpClient");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync(url);

            ApiResponse apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();

            return apiResponse;
        }
        public async Task<ApiResponse> SaveCompanyBackupJobDetail(BackupJobDTO oBackupJobDTO)
        {
            var token = await _authStateProvider.GetAuthenticationTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            string url = StaticEndpoint.BaseUrl + "/BackupJob/SaveCompanyBackupJob";
            var httpClient = _httpClientFactory.CreateClient("CustomHttpClient");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.PostAsJsonAsync(url, oBackupJobDTO);

            ApiResponse apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
            return apiResponse;
        }

    }
}
