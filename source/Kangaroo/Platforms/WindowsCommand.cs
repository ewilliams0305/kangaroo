using System.Diagnostics;

namespace Kangaroo.Platforms;

public sealed class WindowsCommand
{
    public static async Task<string> RunCommandAsync(string command, CancellationToken token = default)
    {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        process.Start();
        var result = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync(token);
        return result;
    }
}