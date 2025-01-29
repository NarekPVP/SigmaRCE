namespace SigmaRCE.Application.Services;

public class CodeExecutionOrchestrator
{
    private readonly ICodeExecutorFactory _factory;
    private readonly ILogger<CodeExecutionOrchestrator> _logger;

    public CodeExecutionOrchestrator(
        ICodeExecutorFactory factory, 
        ILogger<CodeExecutionOrchestrator> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<ExecutionResult> ExecuteCodeAsync(
        string language, 
        string code, 
        int timeoutSeconds)
    {
        try
        {
            var executor = _factory.CreateExecutor(language);
            return await executor.ExecuteAsync(code, timeoutSeconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Execution failed for {Language}", language);
            return new ExecutionResult(false, "", ex.Message, 0);
        }
    }
}