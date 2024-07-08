using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using Solutaris.InfoWARE.ProtectedBrowserStorage.Extensions;
using System.Net.Security;
using www.backupserver.com.Data;
using www.backupserver.com.Repository;
using www.backupserver.com.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
    });


//builder.Services.AddHttpClient("CustomHttpClient").ConfigurePrimaryHttpMessageHandler(() => new CustomHttpClientHandler());
// Configure HttpClient with custom handler
builder.Services.AddHttpClient("CustomHttpClient")
    .ConfigurePrimaryHttpMessageHandler(() => new CustomHttpClientHandler());


//builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
//builder.Services.AddSession();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Set the session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthorizationCore();
builder.Services.AddSweetAlert2();
builder.Services.AddBlazorBootstrap();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
//builder.Services.AddScoped<TokenStorage>();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddTransient<ICookieService, CookieService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
public class CustomHttpClientHandler : HttpClientHandler
{
    public CustomHttpClientHandler()
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
    }
}
