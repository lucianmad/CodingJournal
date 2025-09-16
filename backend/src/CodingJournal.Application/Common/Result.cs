namespace CodingJournal.Application.Common;

public record Result(bool IsSuccess, IReadOnlyList<string> Errors)
{
    public static Result Success() => new (true, []);
    public static Result Failure(string error) => new (false, [error]);
    public static Result Failure(IEnumerable<string> errors) => new (false, errors.ToList());
}

public record Result<T>(T? Value, bool IsSuccess, IReadOnlyList<string> Errors) : Result(IsSuccess, Errors)
{
    public static Result<T> Success(T value) => new (value, true, []);
    public new static Result<T> Failure(string error) => new (default, false, [error]);
    public new static Result<T> Failure(IEnumerable<string> errors) => new (default, false, errors.ToList());
}