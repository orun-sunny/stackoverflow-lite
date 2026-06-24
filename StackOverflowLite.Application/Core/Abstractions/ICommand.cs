using MediatR;

namespace StackOverflowLite.Application.Core.Abstractions;

public interface ICommand<TResponse> : IRequest<TResponse> { }
public interface ICommand : IRequest { }
