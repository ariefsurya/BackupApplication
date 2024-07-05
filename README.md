# BackupApplication

#how to migrate postgre database
dotnet ef migrations add PostgreMigration1 --context PostgresDbContext --output-dir Migrations/Postgres
dotnet ef database update --context PostgresDbContext

#how to remove all migrate postgre database
dotnet ef migrations remove --force


#how to run services in powershell
sc create BackupWorkerService binPath= "C:\dev\BackupWorkerServicePublish\BackupWorkerService.exe"
New-Service -Name "BackupWorkerService" `
            -BinaryPathName "C:\dev\BackupWorkerServicePublish\BackupWorkerService.exe" `
            -DisplayName "BackupWorkerService" `
            -StartupType Automatic


#how to delete services in powershell
SC STOP BackupWorkerService
SC DELETE BackupWorkerService



#how to deploy to docker
docker-compose build --no-cache
docker-compose down
docker-compose up -d
