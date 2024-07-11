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

namespace BackupApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BackupJobController : ControllerBase
    {
        private readonly IAuthUserService _authUserService;
        private readonly IBackupJobServices _backupJobServices;
        private readonly ITargetBackupServices _targetBackupServices;

        public BackupJobController(IAuthUserService authUserService, IBackupJobServices backupJobServices, ITargetBackupServices targetBackupServices)
        {
            _authUserService = authUserService;
            _backupJobServices = backupJobServices;
            _targetBackupServices = targetBackupServices;
        }


        [HttpGet("GetCompanyBackupJob")]
        [Authorize]
        public async Task<IActionResult> GetCompanyBackupJob(int iPage, int iTake, string? search = null)
        {
            try {
                var user = await _authUserService.GetUserDetail(User);
                if (user == null)
                {
                    throw new UnauthorizedAccessException();
                }

                var sortBy = "backupjob.\"Id\"";
                var searchCriteria = "";
                if (!string.IsNullOrEmpty(search))
                {
                    searchCriteria = "backupjob.\"CompanyId\" = " + user.CompanyId.ToString() + " AND (\"BackupJobName\" ILIKE '%" + search + "%' OR \"SourceServerIp\" ILIKE '%\" + search + \"%' OR \"SourceFilePath\" ILIKE '%\" + search + \"%')";
                }
                var lBackupJob = await _backupJobServices.GetAllBackupJobsAsync(searchCriteria, iPage, iTake, sortBy);

                return Ok();
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
                    oBackupJobDTO.BackupJobMapToDto(oBackupJob);
                    oBackupJobDTO.oTargetBackup = await _targetBackupServices.GetTargetBackupByBackupJobId(backupJobId, user.CompanyId);
                }

                return Ok(oBackupJobDTO);
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
                var user = await _authUserService.GetUserDetail(User);
                if (user == null)
                {
                    throw new UnauthorizedAccessException();
                }

                BackupJob oBackupJob = null;
                if (oBackupJobParam.Id != null && oBackupJobParam.Id != 0)
                {
                    oBackupJob = await _backupJobServices.GetBackupJobById(oBackupJobParam.Id, user.CompanyId);
                    if (oBackupJob == null)
                    {
                        throw new BadHttpRequestException("Backup Job not found.");
                    }
                    oBackupJob.BackupJobName = oBackupJobParam.BackupJobName;
                    oBackupJob.UpdatedBy = user.Id;
                    oBackupJob.UpdatedDate = DateTime.UtcNow;
                    TargetBackup oTargetBackup = await _targetBackupServices.GetTargetBackupByBackupJobId(oBackupJob.Id, user.CompanyId);
                    if (oTargetBackup == null)
                    {
                        throw new BadHttpRequestException("Target Backup Job not found.");
                    }
                    oTargetBackup.CompanyId = oBackupJobParam.CompanyId;
                    oTargetBackup.SourceServerIp = oBackupJobParam.oTargetBackup.SourceServerIp;
                    oTargetBackup.SourceFilePath = oBackupJobParam.oTargetBackup.SourceFilePath;
                    oTargetBackup.TargetServerIp = oBackupJobParam.oTargetBackup.TargetServerIp;
                    oTargetBackup.TargetFolderPath = oBackupJobParam.oTargetBackup.TargetFolderPath;
                    oTargetBackup.TargetFileName = oBackupJobParam.oTargetBackup.TargetFileName;
                    oTargetBackup.TargetUsername = oBackupJobParam.oTargetBackup.TargetUsername;
                    oTargetBackup.TargetPassword = oBackupJobParam.oTargetBackup.TargetPassword;
                    oTargetBackup.UpdatedDate = DateTime.UtcNow;

                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        oBackupJob = await _backupJobServices.UpdateBackupJob(oBackupJob);
                        oTargetBackup = await _targetBackupServices.UpdateTargetBackup(oTargetBackup);
                        scope.Complete();
                    }
                }
                else
                {
                    oBackupJob = new BackupJob
                    {
                        BackupJobName = oBackupJobParam.BackupJobName,
                        CompanyId = user.CompanyId,
                        StatusId = 1,
                        CreatedBy = user.Id,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                    };
                    TargetBackup oTargetBackup = new TargetBackup
                    {
                        CompanyId = oBackupJobParam.CompanyId,
                        SourceServerIp = oBackupJobParam.oTargetBackup.SourceServerIp,
                        SourceFilePath = oBackupJobParam.oTargetBackup.SourceFilePath,
                        TargetServerIp = oBackupJobParam.oTargetBackup.TargetServerIp,
                        TargetFolderPath = oBackupJobParam.oTargetBackup.TargetFolderPath,
                        TargetFileName = oBackupJobParam.oTargetBackup.TargetFileName,
                        TargetUsername = oBackupJobParam.oTargetBackup.TargetUsername,
                        TargetPassword = oBackupJobParam.oTargetBackup.TargetPassword,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                    };

                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        oBackupJob = await _backupJobServices.AddBackupJob(oBackupJob);
                        oTargetBackup.BackupJobId = oBackupJob.Id;
                        oTargetBackup = await _targetBackupServices.AddTargetBackup(oTargetBackup);
                        scope.Complete();
                    }
                }

                return Ok(oBackupJob);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
