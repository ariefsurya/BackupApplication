using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.IO.Compression;
using RabbitMqProductApi.RabbitMQ;
using Model;
using Model.Services;
using Microsoft.AspNetCore.Authorization;
using TodosApi.Services.Redis;
using Model.Enum;
using TodosApi.Data;

namespace BackupApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BackupRunnerController : ControllerBase
    {
        private readonly IAuthUserService _authUserService;
        private readonly IBackupHistoryServices _backupHistoryServices;
        private readonly IBackupJobServices _backupJobServices;
        private readonly ITargetBackupServices _targetBackupServices;
        private readonly IConfiguration _configuration;
        private readonly IRabitMQProducer _rabitMQProducer;

        private ResponseHandler responseHandler = new ResponseHandler();
        public BackupRunnerController(IConfiguration configuration, IRabitMQProducer rabitMQProducer, ITargetBackupServices targetBackupServices, IBackupHistoryServices backupHistoryServices, IAuthUserService authUserService, IBackupJobServices backupJobServices)
        {
            _configuration = configuration;
            _rabitMQProducer = rabitMQProducer;
            _targetBackupServices = targetBackupServices;
            _backupHistoryServices = backupHistoryServices;
            _authUserService = authUserService;
            _backupJobServices = backupJobServices;
        }

        [HttpPost("RunBackupJobServerByUser")]
        [Authorize]
        public async Task<IActionResult> RunBackupJobServerByUser(int backupJobId)
        {
            try
            {
                var user = await _authUserService.GetUserDetail(User);
                if (user == null)
                {
                    throw new UnauthorizedAccessException();
                }

                BackupJob oBackupJob = await _backupJobServices.GetBackupJobById(backupJobId, user.CompanyId);
                if (oBackupJob == null)
                    throw new UnauthorizedAccessException();
                TargetBackup oTargetBackup = await _targetBackupServices.GetTargetBackupByBackupJobId(backupJobId, user.CompanyId);
                if (oTargetBackup == null)
                    throw new UnauthorizedAccessException();

                BackupHistory oBackupHistory = new BackupHistory
                {
                    BackupJobId = oBackupJob.Id,
                    BackupJobName = oBackupJob.BackupJobName,
                    SourceFilePath = oTargetBackup.SourceFilePath,
                    TargetFolderPath = oTargetBackup.TargetFolderPath,
                    TargetServerIp = oTargetBackup.TargetServerIp,
                    TargetBackupId = oTargetBackup.Id,
                    BackupSchedulerId = 0,
                    BackupStatusId = (int)EnumBackupStatus.Started,
                    CompanyId = user.CompanyId,
                    CreatedBy = user.Id,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };
                BackupHistory result = await _backupHistoryServices.AddBackupHistory(oBackupHistory);
                //send the inserted product data to the queue and consumer will listening this data from queue
                await _rabitMQProducer.SendBackupMessage(oTargetBackup);
                return responseHandler.ApiReponseHandler(result);
                //return Ok(new { Message = "Database backup completed successfully asd", Path = Path.Combine(oTargetBackup.SourceFilePath, oTargetBackup.TargetFolderPath) });
            }
            catch (Exception ex)
            {
                throw;
                //return StatusCode(500, new { Message = "Error backing up sendToBackupServer", Error = ex.Message });
            }
        }

        [HttpPost("sendBackupHistory")]
        public async Task<IActionResult> sendBackupHistory(BackupHistory oBackupResult)
        {
            try
            {
                oBackupResult.CreatedDate = DateTime.UtcNow;
                oBackupResult.UpdatedDate = DateTime.UtcNow;
                BackupHistory result = await _backupHistoryServices.AddBackupHistory(oBackupResult);
                //send the inserted product data to the queue and consumer will listening this data from queue
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
                //return StatusCode(500, new { Message = "Error backing up sendBackupHistory", Error = ex.Message });
            }
        }


        private readonly string _basePath = @"/app/files"; // Ensure this is the correct path in the container

        [HttpGet("{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_basePath, fileName);
                Console.WriteLine($"Looking for file at: {filePath}");

                if (!System.IO.File.Exists(filePath))
                {
                    Console.WriteLine("File not found.");
                    return NotFound(new { Message = "File not found", Path = filePath });
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                Console.WriteLine("File found and read successfully.");
                return File(memory, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { Message = "Error accessing file", Error = ex.Message });
            }
        }



        private readonly string _uploadPath = @"/app/uploads";
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "Invalid file" });
            }

            try
            {
                var filePath = Path.Combine(_uploadPath, file.FileName);
                Console.WriteLine($"Saving file to: {filePath}");

                // Ensure the directory exists before saving the file
                Directory.CreateDirectory(_uploadPath);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                Console.WriteLine("File uploaded successfully.");
                return Ok(new { Message = "File uploaded successfully", Path = filePath });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                return StatusCode(500, new { Message = "Error uploading file", Error = ex.Message });
            }
        }


        private readonly string _backupPath = @"/app/backups";
        [HttpPost("backupDatabase")]
        public async Task<IActionResult> BackupDatabase()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                var backupPath = _configuration.GetValue<string>("BackupSettings:BackupPath");
                var backupFileName = _configuration.GetValue<string>("BackupSettings:BackupFileName");
                var backupZipName = _configuration.GetValue<string>("BackupSettings:BackupZipName");

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var backupQuery = $"BACKUP DATABASE [VerintTest] TO DISK = '{backupPath}{backupFileName}' WITH FORMAT, NAME = 'Full Backup of VerintTest'";

                    using (var command = new SqlCommand(backupQuery, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
                CreateZipFromBak(Path.Combine(_backupPath, backupFileName), Path.Combine(_backupPath, backupZipName));

                return Ok(new { Message = "Database backup completed successfully", Path = Path.Combine(backupPath, backupFileName) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error backing up database", Error = ex.Message });
            }
        }

        private void CreateZipFromBak(string bakFilePath, string zipFilePath)
        {
            if (!System.IO.File.Exists(bakFilePath))
            {
                throw new FileNotFoundException("The specified .bak file does not exist.");
            }

            using (FileStream zipToOpen = new FileStream(zipFilePath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(bakFilePath, Path.GetFileName(bakFilePath));
                }
            }
        }

        [HttpPost("sendToBackupServer")]
        public async Task<IActionResult> sendToBackupServer(TargetBackup oTargetBackup)
        {
            try
            {
                TargetBackup result = await _targetBackupServices.AddTargetBackup(oTargetBackup);
                //send the inserted product data to the queue and consumer will listening this data from queue
                await _rabitMQProducer.SendBackupMessage(result);
                return Ok(new { Message = "Database backup completed successfully asd", Path = Path.Combine(oTargetBackup.SourceFilePath, oTargetBackup.TargetFolderPath) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error backing up sendToBackupServer", Error = ex.Message });
            }
        }


    }
}
