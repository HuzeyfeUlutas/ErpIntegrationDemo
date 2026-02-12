namespace PersonnelAccessManagement.Application.Common.Exceptions;

public abstract class AppException : Exception
{
    protected AppException(string message) : base(message) { }

    public abstract string Code { get; }
    public virtual int StatusCode => 400;
}

public sealed class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message) { }

    public override string Code => "not_found";
    public override int StatusCode => 404;
}

public sealed class ConflictException : AppException
{
    public ConflictException(string message) : base(message) { }

    public override string Code => "conflict";
    public override int StatusCode => 409;
}