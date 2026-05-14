namespace EnterpriseCMS.Core.Exceptions;

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }
    public ValidationException() : base("One or more validation failures occurred.")
        => Errors = new Dictionary<string, string[]>();
    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation failures occurred.")
        => Errors = errors;
}
