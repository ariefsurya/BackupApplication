using Renci.SshNet;
using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        // Define the source file path
        string sourceFilePath = @"C:/dev/HackTheBox.txt"; // Ensure this is the correct path
        string targetFolderPath = "/home/kakekakek/TargetBackup"; // Target directory on CentOS server

        // Define the server details
        string serverIp = "192.168.220.133";
        string username = "kakekakek";
        string password = "root";

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

            Console.WriteLine($"File copied successfully from {sourceFilePath} to {serverIp}:{targetFolderPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error copying file: {ex.Message}");
        }
    }
}