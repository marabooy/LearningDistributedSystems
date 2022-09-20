namespace Core;

public interface IRequest { }

public interface IResult { }

/// <summary>
/// Interface for all the application we want to wrap up with the distributed algorithm.
/// </summary>
public interface IDistributedApplication
{
    IResult Execute(IRequest request);
}