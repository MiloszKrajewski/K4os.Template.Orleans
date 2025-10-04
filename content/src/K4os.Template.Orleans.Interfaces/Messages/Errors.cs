using Orleans;

namespace K4os.Template.Orleans.Interfaces.Messages;

[GenerateSerializer, Alias("WellKnownError.v1")]
public abstract class WellKnownError: Exception
{
    protected WellKnownError(string message): base(message) { }
}

[GenerateSerializer, Alias("UnauthorisedError.v1")]
public class UnauthorisedError: WellKnownError
{
    public UnauthorisedError(string message): base(message) { }
}

[GenerateSerializer, Alias("ForbiddenError.v1")]
public class ForbiddenError: WellKnownError
{
    public ForbiddenError(string message): base(message) { }
}

[GenerateSerializer, Alias("NotFoundError.v1")]
public class NotFoundError: WellKnownError
{
    public NotFoundError(string message): base(message) { }
}

[GenerateSerializer, Alias("BadRequestError.v1")]
public class BadRequestError: WellKnownError
{
    public BadRequestError(string message): base(message) { }
}

[GenerateSerializer, Alias("TimeoutError.v1")]
public class TimeoutError: WellKnownError
{
    public TimeoutError(string message): base(message) { }
}