namespace EnterpriseCMS.Application.Common.Models;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? Error { get; private set; }
    public IDictionary<string, string[]>? ValidationErrors { get; private set; }

    private Result() { }

    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Failure(string error) => new() { IsSuccess = false, Error = error };
    public static Result<T> ValidationFailure(IDictionary<string, string[]> errors)
        => new() { IsSuccess = false, ValidationErrors = errors, Error = "Validation failed" };
}
