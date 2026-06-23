using System.ComponentModel;
using System.Diagnostics;

namespace LR2Nexus.Services
{
    internal static class ProcessManager
    {
        public static string? LR2BodyPath { get; set; }

        public static bool IsLR2Running
        {
            get
            {
                if (string.IsNullOrEmpty(LR2BodyPath) || !File.Exists(LR2BodyPath))
                    return false;

                string fileName = Path.GetFileNameWithoutExtension(LR2BodyPath);
                var processes = Process.GetProcessesByName(fileName);

                return processes.Any(p =>
                {
                    try
                    {
                        return p.MainModule?.FileName != null &&
                               Path.GetFullPath(p.MainModule.FileName).Equals(
                                   Path.GetFullPath(LR2BodyPath), StringComparison.OrdinalIgnoreCase);
                    }
                    catch (Win32Exception)
                    {
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unexpected error checking process: {ex.Message}");
                        return false;
                    }
                });
            }
        }

        public static event EventHandler? LR2Exited;

        public static void LaunchLR2Body()
        {
            if (string.IsNullOrEmpty(LR2BodyPath) ||
                !File.Exists(LR2BodyPath))
            {
                throw new FileNotFoundException("The specified executable path is invalid.");
            }

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = LR2BodyPath,
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(LR2BodyPath)
                };

                var process = new Process
                {
                    StartInfo = startInfo,
                    EnableRaisingEvents = true
                };

                process.Exited += (s, e) =>
                {
                    var exitCode = process.ExitCode;
                    Console.WriteLine($"Process exited with code: {exitCode}");
                    LR2Exited?.Invoke(null, EventArgs.Empty);
                    process.Dispose();
                };

                process.Start();

                Console.WriteLine($"Successfully launched process: {LR2BodyPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to launch process: {ex.Message}");
            }
        }   
    }
}
