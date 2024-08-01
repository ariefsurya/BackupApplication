using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using System.Text.Json;
using www.backupserver.com.Pages.User;

namespace www.backupserver.com.Repository
{
    public interface IBackupJobRepository
    {
        public Task<ApiResponse> GetCompanyBackupJob(string search, int iPage, int iTake);
        public Task<ApiResponse> GetCompanyBackupJobDetail(int backupJobId);
        public Task<ApiResponse> GetCompanyBackupJobHistory(int backupJobId, int iPage, int iTake);
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

            string url = StaticEndpoint.BaseUrl + $"/BackupJob/GetCompanyBackupJobDetail?backupJobId={backupJobId}";
            var httpClient = _httpClientFactory.CreateClient("CustomHttpClient");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync(url);

            ApiResponse apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();

            return apiResponse;
        }
        public async Task<ApiResponse> GetCompanyBackupJobHistory(int backupJobId, int iPage, int iTake)
        {
            var token = await _authStateProvider.GetAuthenticationTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            string url = StaticEndpoint.BaseUrl + $"/BackupJob/GetCompanyBackupJobHistory?backupJobId={backupJobId}&iPage={iPage}&iTake={iTake}";
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

            //Console.WriteLine("SaveCompanyBackupJobDetail");
            //string jsonString = JsonSerializer.Serialize(oBackupJobDTO, new JsonSerializerOptions { WriteIndented = true });
            //Console.WriteLine(jsonString);
            //Console.WriteLine(oBackupJobDTO);
            var response = await httpClient.PostAsJsonAsync(url, oBackupJobDTO);
            Console.WriteLine("response 1 1");
            Console.WriteLine(response);
            Console.WriteLine(response.Content);
            Console.WriteLine(oBackupJobDTO);
            string jsonString = JsonSerializer.Serialize(oBackupJobDTO, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("jsonString");
            Console.WriteLine(jsonString);
            ApiResponse apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
            return apiResponse;
        }

    }
}
