using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.IO.Compression;
using RabbitMqProductApi.RabbitMQ;
using Model;
using Model.Services;
using Microsoft.AspNetCore.Authorization;
using TodosApi.Services.Redis;
using System.Transactions;
using TodosApi.Data;
using Model.Enum;
using TodosApi.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace BackupApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BackupJobController : ControllerBase
    {
        private readonly IAuthUserService _authUserService;
        private readonly IBackupJobServices _backupJobServices;
        private readonly ITargetBackupServices _targetBackupServices;
        private readonly IBackupHistoryServices _backupHistoryServices;
        private readonly IBackupSchedulerServices _backupSchedulerServices;
        private ResponseHandler responseHandler = new ResponseHandler();

        public BackupJobController(IAuthUserService authUserService, IBackupJobServices backupJobServices, ITargetBackupServices targetBackupServices, IBackupHistoryServices backupHistoryServices, IBackupSchedulerServices backupSchedulerServices)
        {
            _authUserService = authUserService;
            _backupJobServices = backupJobServices;
            _targetBackupServices = targetBackupServices;
            _backupHistoryServices = backupHistoryServices;
            _backupSchedulerServices = backupSchedulerServices;
        }


        [HttpGet("GetCompanyBackupJob")]
        [Authorize]
        public async Task<IActionResult> GetCompanyBackupJob(int iPage, int iTake, string? search = null)
        {
            try
            {
                var user = await _authUserService.GetUserDetail(User);
                if (user == null)
                {
                    throw new UnauthorizedAccessException();
                }

                var sortBy = "backupjob.\"Id\"";
                var searchCriteria = " AND backupjob.\"CompanyId\" = " + user.CompanyId.ToString();
                if (!string.IsNullOrEmpty(search))
                {
                    searchCriteria = " AND (\"BackupJobName\" ILIKE '%" + search + "%' OR \"SourceServerIp\" ILIKE '%\" + search + \"%' OR \"SourceFilePath\" ILIKE '%\" + search + \"%')";
                }
                var lBackupJob = await _backupJobServices.GetAllBackupJobsAsync(searchCriteria, iPage, iTake, sortBy);

                return responseHandler.ApiReponseHandler(lBackupJob);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetCompanyBackupJobDetail")]
        [Authorize]
        public async Task<IActionResult> GetCompanyBackupJobDetail(int backupJobId)
        {
            try {
                var user = await _authUserService.GetUserDetail(User);
                if (user == null)
                {
                    throw new UnauthorizedAccessException();
                }

                BackupJobDTO oBackupJobDTO = new BackupJobDTO();
                var oBackupJob = await _backupJobServices.GetBackupJobById(backupJobId, user.CompanyId);
                if (oBackupJob != null)
                {
                    oBackupJobDTO = oBackupJobDTO.BackupJobMapToDto(oBackupJob);
                    oBackupJobDTO.oTargetBackup = await _targetBackupServices.GetTargetBackupByBackupJobId(backupJobId, user.CompanyId);
                    oBackupJobDTO.oBackupScheduler = await _backupSchedulerServices.GetBackupSchedulerByBackupJobId(backupJobId, user.CompanyId);
                }

                return responseHandler.ApiReponseHandler(oBackupJobDTO);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetCompanyBackupJobHistory")]
        [Authorize]
        public async Task<IActionResult> GetCompanyBackupJobHistory(int backupJobId, int iPage, int iTake)
        {
            try
            {
                var user = await _authUserService.GetUserDetail(User);
                if (user == null)
                {
                    throw new UnauthorizedAccessException();
                }


                var sortBy = "backuphistory.\"Id\"";
                var searchCriteria = " AND backuphistory.\"BackupJobId\" = " + backupJobId + " AND backuphistory.\"CompanyId\" = " + user.CompanyId.ToString(); ;
                var lBackupHistory = await _backupHistoryServices.GetBackupHistoryByJobId(searchCriteria, iPage, iTake, sortBy);

                return responseHandler.ApiReponseHandler(lBackupHistory);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("SaveCompanyBackupJob")]
        [Authorize]
        public async Task<IActionResult> SaveCompanyBackupJob(BackupJobDTO oBackupJobParam)
        {
            try {
                Console.WriteLine("SaveCompanyBackupJob");
                Console.WriteLine(oBackupJobParam);
                string jsonString = JsonSerializer.Serialize(oBackupJobParam, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine("jsonString");
                Console.WriteLine(jsonString);
                var user = await _authUserService.GetUserDetail(User);
                if (user == null)
                {
                    throw new UnauthorizedAccessException();
                }

                #region validation
                if (oBackupJobParam == null || oBackupJobParam.oTargetBackup == null)
                    throw new BadHttpRequestException("BackupJob cannot be empty.");
                else if (string.IsNullOrEmpty(oBackupJobParam.BackupJobName))
                    throw new BadHttpRequestException("Backup Job Name cannot be empty.");
                else if (string.IsNullOrEmpty(oBackupJobParam.oTargetBackup.SourceServerIp))
                    throw new BadHttpRequestException("Source Server Ip cannot be empty.");
                else if (string.IsNullOrEmpty(oBackupJobParam.oTargetBackup.SourceFilePath))
                    throw new BadHttpRequestException("Source File cannot be empty.");
                else if (string.IsNullOrEmpty(oBackupJobParam.oTargetBackup.TargetServerIp))
                    throw new BadHttpRequestException("Target Server Ip cannot be empty.");
                else if (string.IsNullOrEmpty(oBackupJobParam.oTargetBackup.TargetFolderPath))
                    throw new BadHttpRequestException("Target Folder Path cannot be empty.");
                else if (string.IsNullOrEmpty(oBackupJobParam.oTargetBackup.TargetUsername))
                    throw new BadHttpRequestException("Target Server Username cannot be empty.");
                else if (string.IsNullOrEmpty(oBackupJobParam.oTargetBackup.TargetPassword))
                    throw new BadHttpRequestException("Target Server Password cannot be empty.");
                else if (oBackupJobParam.IsUseScheduler)
                {
                    if (oBackupJobParam.oBackupScheduler.BackupSchedulerType == null || oBackupJobParam.oBackupScheduler.BackupSchedulerType == 0)
                        throw new BadHttpRequestException("Backup Scheduler Type cannot be empty.");
                    else if (string.IsNullOrEmpty(oBackupJobParam.oBackupScheduler.SchedulerDateDaySet))
                        throw new BadHttpRequestException("Scheduler Run Date Day cannot be empty.");
                    else if (oBackupJobParam.oBackupScheduler.SchedulerStartDate == DateTime.MinValue)
                        throw new BadHttpRequestException("Scheduler Start Date cannot be empty.");
                }
                #endregion

                BackupJob oBackupJob = null;
                if (oBackupJobParam.Id != null && oBackupJobParam.Id != 0)
                {
                    #region oBackupJob Edit
                    oBackupJob = await _backupJobServices.GetBackupJobById(oBackupJobParam.Id, user.CompanyId);
                    if (oBackupJob == null)
                    {
                        throw new BadHttpRequestException("Backup Job not found.");
                    }
                    oBackupJob.BackupJobName = oBackupJobParam.BackupJobName;
                    oBackupJob.UpdatedBy = user.Id;
                    oBackupJob.UpdatedDate = DateTime.UtcNow;
                    oBackupJob.IsUseScheduler = oBackupJobParam.IsUseScheduler;
                    TargetBackup oTargetBackup = await _targetBackupServices.GetTargetBackupByBackupJobId(oBackupJob.Id, user.CompanyId);
                    if (oTargetBackup == null)
                    {
                        throw new BadHttpRequestException("Target Backup Job not found.");
                    }
                    oTargetBackup.CompanyId = user.CompanyId;
                    oTargetBackup.SourceServerIp = oBackupJobParam.oTargetBackup.SourceServerIp;
                    oTargetBackup.SourceFilePath = oBackupJobParam.oTargetBackup.SourceFilePath;
                    oTargetBackup.TargetServerIp = oBackupJobParam.oTargetBackup.TargetServerIp;
                    oTargetBackup.TargetFolderPath = oBackupJobParam.oTargetBackup.TargetFolderPath;
                    oTargetBackup.TargetFileName = Path.GetFileName(oBackupJobParam.oTargetBackup.SourceFilePath);
                    oTargetBackup.TargetUsername = oBackupJobParam.oTargetBackup.TargetUsername;
                    oTargetBackup.TargetPassword = oBackupJobParam.oTargetBackup.TargetPassword;
                    oTargetBackup.UpdatedDate = DateTime.UtcNow;
                    BackupScheduler oBackupScheduler = await _backupSchedulerServices.GetBackupSchedulerByBackupJobId(oBackupJobParam.Id, user.CompanyId);
                    if (oBackupScheduler == null)
                    {
                        oBackupScheduler = new BackupScheduler();
                        oBackupScheduler.BackupJobId = oBackupJobParam.Id;
                        oBackupScheduler.CompanyId = user.CompanyId;
                        oBackupScheduler.CreatedBy = user.Id;
                        oBackupScheduler.CreatedDate = DateTime.UtcNow;
                    }
                    oBackupScheduler.BackupSchedulerType = oBackupJobParam.oBackupScheduler.BackupSchedulerType;
                    oBackupScheduler.SchedulerDateDaySet = oBackupJobParam.oBackupScheduler.SchedulerDateDaySet;
                    oBackupScheduler.SchedulerClockTimeSet = oBackupJobParam.oBackupScheduler.SchedulerClockTimeSet;
                    oBackupScheduler.SchedulerStartDate = oBackupJobParam.oBackupScheduler.SchedulerStartDate.ToUniversalTime();
                    oBackupScheduler.StatusId = (int)EnumStatus.Active;
                    oBackupScheduler.UpdatedBy = user.Id;
                    oBackupScheduler.UpdatedDate = DateTime.UtcNow;

                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        oBackupJob = await _backupJobServices.UpdateBackupJob(oBackupJob);
                        oTargetBackup = await _targetBackupServices.UpdateTargetBackup(oTargetBackup);
                        if (oBackupScheduler.Id == null || oBackupScheduler.Id == 0)
                            oBackupScheduler = await _backupSchedulerServices.AddBackupScheduler(oBackupScheduler);
                        else
                            oBackupScheduler = await _backupSchedulerServices.UpdateBackupScheduler(oBackupScheduler);
                        scope.Complete();
                    }
                    #endregion
                }
                else
                {
                    #region oBackupJob Add
                    oBackupJob = new BackupJob
                    {
                        BackupJobName = oBackupJobParam.BackupJobName,
                        CompanyId = user.CompanyId,
                        IsUseScheduler = oBackupJobParam.IsUseScheduler,
                        StatusId = 1,
                        CreatedBy = user.Id,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                    };
                    TargetBackup oTargetBackup = new TargetBackup
                    {
                        CompanyId = oBackupJob.CompanyId,
                        SourceServerIp = oBackupJobParam.oTargetBackup.SourceServerIp,
                        SourceFilePath = oBackupJobParam.oTargetBackup.SourceFilePath,
                        TargetServerIp = oBackupJobParam.oTargetBackup.TargetServerIp,
                        TargetFolderPath = oBackupJobParam.oTargetBackup.TargetFolderPath,
                        TargetFileName = Path.GetFileName(oBackupJobParam.oTargetBackup.SourceFilePath),
                        TargetUsername = oBackupJobParam.oTargetBackup.TargetUsername,
                        TargetPassword = oBackupJobParam.oTargetBackup.TargetPassword,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                    };
                    BackupScheduler oBackupScheduler = new BackupScheduler
                    {
                        CompanyId = user.CompanyId,
                        CreatedBy = user.Id,
                        CreatedDate = DateTime.UtcNow,
                        BackupSchedulerType = oBackupJobParam.oBackupScheduler.BackupSchedulerType,
                        SchedulerDateDaySet = oBackupJobParam.oBackupScheduler.SchedulerDateDaySet,
                        SchedulerClockTimeSet = oBackupJobParam.oBackupScheduler.SchedulerClockTimeSet,
                        SchedulerStartDate = oBackupJobParam.oBackupScheduler.SchedulerStartDate.ToUniversalTime(),
                        StatusId = (int)EnumStatus.Active,
                        UpdatedBy = user.Id,
                        UpdatedDate = DateTime.UtcNow,
                    };

                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        oBackupJob = await _backupJobServices.AddBackupJob(oBackupJob);
                        oTargetBackup.BackupJobId = oBackupJob.Id;
                        oBackupScheduler.BackupJobId = oBackupJob.Id;
                        oTargetBackup = await _targetBackupServices.AddTargetBackup(oTargetBackup);
                        oBackupScheduler = await _backupSchedulerServices.AddBackupScheduler(oBackupScheduler);
                        scope.Complete();
                    }
                    #endregion
                }

                return responseHandler.ApiReponseHandler(oBackupJob);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
