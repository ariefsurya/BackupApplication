using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using System.Text.Json;
using www.backupserver.com.Pages.User;

namespace www.backupserver.com.Repository
{
    public interface IBackupRunnerRepository
    {
        public Task<ApiResponse> RunBackupJobServerByUser(int backupJobId);
    }

    public class BackupRunnerRepository : IBackupRunnerRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CustomAuthenticationStateProvider _authStateProvider;

        public BackupRunnerRepository(IHttpClientFactory httpClientFactory, CustomAuthenticationStateProvider authStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authStateProvider = authStateProvider; 
        }

        public async Task<ApiResponse> RunBackupJobServerByUser(int backupJobId)
        {
            var token = await _authStateProvider.GetAuthenticationTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            string url = StaticEndpoint.BaseUrl + $"/BackupRunner/RunBackupJobServerByUser?backupJobId={backupJobId}";
            var httpClient = _httpClientFactory.CreateClient("CustomHttpClient");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.PostAsJsonAsync(url, backupJobId);
            ApiResponse apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();

            return apiResponse;
        }
    }
}
