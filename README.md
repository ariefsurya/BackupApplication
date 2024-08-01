# BackupApplication

#how to migrate postgre database
dotnet ef migrations add PostgreMigration5 --context PostgresDbContext --output-dir Migrations/Postgres
dotnet ef database update --context PostgresDbContext

#how to remove all migrate postgre database
dotnet ef migrations remove --force


#how to run services in powershell
sc create BackupWorkerService binPath = "C:\dev\BackupWorkerServicePublish\BackupWorkerService.exe"
New-Service -Name "BackupWorkerService" `
            -BinaryPathName "C:\dev\BackupWorkerServicePublish\BackupWorkerService.exe" `
            -DisplayName "BackupWorkerService" `
            -StartupType Automatic


#how to delete services in powershell
Get-Service -Name "BackupWorkerService" | Stop-Service
Get-WmiObject -Class Win32_Service -Filter "Name='BackupWorkerService'" | Remove-WmiObject

#how to delete services in cmd
SC STOP BackupWorkerService
SC DELETE BackupWorkerService



#how to deploy to docker
docker-compose build --no-cache
docker-compose down
docker-compose up -d



{
  "id": 0,
  "sourceFilePath": "C:/dev/HackTheBox.txt",
  "targetFolderPath": "/home/kakekakek/TargetBackup",
  "serverName": "centos 9",
  "serverIp": "192.168.220.133",
  "username": "kakekakek",
  "password": "root",
  "companyId": "1",
  "createdBy": 0,
  "createdDate": "2024-07-01T08:50:13.146Z",
  "updatedDate": "2024-07-01T08:50:13.146Z"
}