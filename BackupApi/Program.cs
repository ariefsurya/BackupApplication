using Microsoft.EntityFrameworkCore;
using Model;
using RabbitMqProductApi.RabbitMQ;
using Model.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IRabitMQProducer, RabitMQProducer>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<ICompanyServices, CompanyServices>();
builder.Services.AddScoped<IBackupHistoryServices, BackupHistoryServices>();
builder.Services.AddScoped<ITargetBackupServices, TargetBackupServices>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PostgresDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection") ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found."),
    b => b.MigrationsAssembly("BackupApi")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
