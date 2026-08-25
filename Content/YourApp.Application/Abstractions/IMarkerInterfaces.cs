namespace YourApp.Application.Abstractions;

/// <summary>
/// Marker for write operations. Implementations live in Application layer.
/// </summary>
public interface ICommand<TResponse> : MediatR.IRequest<TResponse> { }

public interface ICommandHandler<TCommand, TResponse>
    : MediatR.IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse> { }

/// <summary>
/// Marker for read operations. Implementations live in Application layer.
/// </summary>
public interface IQuery<TResponse> : MediatR.IRequest<TResponse> { }

public interface IQueryHandler<TQuery, TResponse>
    : MediatR.IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse> { }