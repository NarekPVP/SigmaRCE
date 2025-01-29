namespace SigmaRCE.Core.Interfaces;

public interface ICodeExecutor
{
    Task<ExecutionResult> ExecuteAsync(string code, int timeoutSeconds);
}

public interface ICodeExecutorFactory
{
    ICodeExecutor CreateExecutor(string language);
}