using System.Text;
using System.Text.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Model;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Channels;
using Renci.SshNet;
using Model.Enum;
using Serilog;

namespace BackupWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private IConnection _connection;
        private IModel _channel;
        private readonly string _companyId;

        public Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory, IOptions<WorkerOptions> options)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _companyId = options.Value.CompanyId;
            InitializeRabbitMQ();
        }
        private void InitializeRabbitMQ2()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = "rabbitmqbackup"
                };
                //Create the RabbitMQ connection using connection factory details as i mentioned above
                _connection = factory.CreateConnection();
                //Here we create channel with session and model
                _channel = _connection.CreateModel();
                //declare the queue after mentioning name and a few property related to that
                _channel.QueueDeclare("productbackup", exclusive: false);
                //Set Event object which listen message from chanel which is sent by producer
                //var consumer = new EventingBasicConsumer(channel);
                //consumer.Received += (model, eventArgs) => {
                //    _logger.LogInformation("new message:");
                //    var body = eventArgs.Body.ToArray();
                //    var message = Encoding.UTF8.GetString(body);
                //    Console.WriteLine($"productbackup message received: {message}");

                //    var task = JsonSerializer.Deserialize<TargetBackup>(message);

                //    // Process the backup task
                //    BackupDatabase(task.SourceFilePath, task.TargetFolderPath);
                //};
                ////read the message
                //channel.BasicConsume(queue: "productbackup", autoAck: true, consumer: consumer);
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Log.Information("Error connecting to RabbitMQ");
                _logger.LogError(ex, "Error connecting to RabbitMQ.");
                throw;
            }
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory
            {
                UserName = "guest",
                Password = "guest",
                Port = 5672,
                HostName = "localhost", // rabbitmqbackup kalau mau pakai yang di docker rabbitmq:3.13.3-management-alpine
                VirtualHost = "/"
            };// Use the service name defined in docker-compose
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            Log.Information("connected to RabbitMQ");
            var exchangeName = "backup_exchange." + _companyId;
            var queueName = "backupQueue." + _companyId;
            var routingKey = "backup.database." + _companyId;
            _channel.ExchangeDeclare(exchange: exchangeName, type: "topic", durable: true, autoDelete: false);
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var queueName = "backupQueue." + _companyId;
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("ExecuteAsync");

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    Log.Information("new Message");
                    _logger.LogInformation("new Message");
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var task = JsonSerializer.Deserialize<TargetBackup>(message);

                    Log.Information("Received message: {Message}", message);
                    _logger.LogInformation("Received message: {Message}", message);

                    NotifBackupTaskReceived(task);
                    // Process the backup task
                    await BackupDatabase(task);
                };

                _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                await Task.Delay(1000);
                await Task.CompletedTask;
            }
        }
        private async Task NotifBackupTaskReceived(TargetBackup oTargetBackup)
        {
            try
            {
                Log.Information("new NotifBackupTaskReceived");
                _logger.LogInformation("new NotifBackupTaskReceived");
                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
                };
                using HttpClient httpClient = new HttpClient(handler);
                HttpResponseMessage response = null;
                response = await httpClient.PostAsJsonAsync("https://localhost:5001/BackupRunner/sendBackupHistory", new BackupHistory
                {
                    BackupJobId = oTargetBackup.BackupJobId,
                    BackupJobName = oTargetBackup.TargetFileName.ToString(),
                    SourceFilePath = oTargetBackup.SourceFilePath,
                    TargetFolderPath = oTargetBackup.TargetFolderPath,
                    TargetServerIp = oTargetBackup.TargetServerIp,
                    TargetBackupId = oTargetBackup.Id,
                    BackupSchedulerId = 0,
                    BackupStatusId = (int)EnumBackupStatus.TaskReceived,
                    CompanyId = Int32.Parse(_companyId.Replace("company-", "")),
                    CreatedBy = -2,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Log.Information("error NotifBackupTaskReceived: {Message}", ex.Message);
                _logger.LogInformation("error NotifBackupTaskReceived: {Message}", ex.Message);
            }
        }

        private async Task BackupDatabase(TargetBackup oTargetBackup)
        {
            // Implement your backup logic here
            Log.Information($"Backing up database '{oTargetBackup.SourceFilePath}' to '{oTargetBackup.TargetFolderPath}'");
            _logger.LogInformation($"Backing up database '{oTargetBackup.SourceFilePath}' to '{oTargetBackup.TargetFolderPath}'");
            string isSuccessServerBackup = await BackupDatabaseServerToServer(oTargetBackup);

            // Example backup logic (this should be replaced with actual backup code)
            System.Threading.Thread.Sleep(1000); // Simulate time taken to backup

            Log.Information($"api history Backup of database '{oTargetBackup.SourceFilePath}' completed");
            _logger.LogInformation($"api history Backup of database '{oTargetBackup.SourceFilePath}' completed");
            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
            };
            using HttpClient httpClient = new HttpClient(handler);
            HttpResponseMessage response = null;
            if (string.IsNullOrEmpty(isSuccessServerBackup))
            {
                response = await httpClient.PostAsJsonAsync("https://localhost:5001/BackupRunner/sendBackupHistory", new BackupHistory
                {

                    BackupJobId = oTargetBackup.BackupJobId,
                    BackupJobName = oTargetBackup.TargetFileName.ToString(),
                    SourceFilePath = oTargetBackup.SourceFilePath,
                    TargetFolderPath = oTargetBackup.TargetFolderPath,
                    TargetServerIp = oTargetBackup.TargetServerIp,
                    TargetBackupId = oTargetBackup.Id,
                    BackupSchedulerId = 0,
                    BackupStatusId = (int)EnumBackupStatus.Success,
                    CompanyId = Int32.Parse(_companyId.Replace("company-", "")),
                    CreatedBy = -2,
                    Message = isSuccessServerBackup,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                });
            }
            else
            {
                response = await httpClient.PostAsJsonAsync("https://localhost:5001/BackupRunner/sendBackupHistory", new BackupHistory
                {

                    BackupJobId = oTargetBackup.BackupJobId,
                    BackupJobName = oTargetBackup.TargetFileName.ToString(),
                    SourceFilePath = oTargetBackup.SourceFilePath,
                    TargetFolderPath = oTargetBackup.TargetFolderPath,
                    TargetServerIp = oTargetBackup.TargetServerIp,
                    TargetBackupId = oTargetBackup.Id,
                    BackupSchedulerId = 0,
                    BackupStatusId = (int)EnumBackupStatus.Failed,
                    CompanyId = Int32.Parse(_companyId.Replace("company-", "")),
                    CreatedBy = -2,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    Message = isSuccessServerBackup
                });
            }

            if (response.IsSuccessStatusCode)
            {
                Log.Information("Notified API about the backup completion successfully.");
                _logger.LogInformation("Notified API about the backup completion successfully.");
            }
            else
            {
                Log.Information("Failed to notify API about the backup completion.");
                _logger.LogError("Failed to notify API about the backup completion.");
            }
        }

        private async Task<string> BackupDatabaseServerToServer(TargetBackup oTargetBackup)
        {

            // Define the source file path
            string sourceFilePath = oTargetBackup.SourceFilePath; // Ensure this is the correct pathHttpClientHandler 
            string targetFolderPath = oTargetBackup.TargetFolderPath; // Target directory on CentOS server

            // Define the server details
            string serverIp = oTargetBackup.TargetServerIp;
            string username = oTargetBackup.TargetUsername;
            string password = oTargetBackup.TargetPassword;

            try
            {
                // Create the target directory if it doesn't exist
                using (var client = new SshClient(serverIp, username, password))
                {
                    client.Connect();
                    var command = client.CreateCommand($"mkdir -p {targetFolderPath}");
                    command.Execute();
                    client.Disconnect();
                }

                // Upload the file using SCP
                using (var scp = new ScpClient(serverIp, username, password))
                {
                    scp.Connect();
                    using (var fileStream = new FileStream(sourceFilePath, FileMode.Open))
                    {
                        // Ensure the path separator is correct for Unix-based systems
                        string targetFilePath = Path.Combine(targetFolderPath, Path.GetFileName(sourceFilePath)).Replace("\\", "/");
                        scp.Upload(fileStream, targetFilePath);
                    }
                    scp.Disconnect();
                }

                Log.Information($"File copied successfully from {sourceFilePath} to {serverIp}:{targetFolderPath}");
                _logger.LogInformation($"File copied successfully from {sourceFilePath} to {serverIp}:{targetFolderPath}");
                return "";
            }
            catch (Exception ex)
            {
                Log.Information($"Error copying file: {ex.Message}");
                _logger.LogError($"Error copying file: {ex.Message}");
                return ex.Message.ToString();
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }

        public class WorkerOptions
        {
            public string CompanyId { get; set; }
        }
    }
}
