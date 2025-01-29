namespace SigmaRCE.Infrastructure.Services;

public class ProcessExecutor : ICodeExecutor
{
    private readonly string _executablePath;
    private readonly string _arguments;

    public ProcessExecutor(string executablePath, string arguments)
    {
        _executablePath = executablePath;
        _arguments = arguments;
    }

    public async Task<ExecutionResult> ExecuteAsync(string code, int timeoutSeconds)
    {
        var startTime = DateTime.UtcNow;

        var processInfo = new ProcessStartInfo
        {
            FileName = _executablePath,
            Arguments = $"{_arguments} \"{EscapeCode(code)}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processInfo };
        process.Start();

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        var completedTask = await Task.WhenAny(
            Task.WhenAll(outputTask, errorTask),
            Task.Delay(TimeSpan.FromSeconds(timeoutSeconds))
        );

        if (completedTask.IsCanceled || !process.HasExited)
        {
            process.Kill();
            throw new TimeoutException("Execution timed out");
        }

        return new ExecutionResult(
            process.ExitCode == 0,
            await outputTask,
            await errorTask,
            (long)(DateTime.UtcNow - startTime).TotalMilliseconds);
    }

    private static string EscapeCode(string code) =>
        code.Replace("\"", "\\\"");
}