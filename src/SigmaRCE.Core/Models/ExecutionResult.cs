namespace SigmaRCE.Core.Models;

public record ExecutionResult(
    bool Success,
    string Output,
    string Error,
    long ExecutionTimeMs);