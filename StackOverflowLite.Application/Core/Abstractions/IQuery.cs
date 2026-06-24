using MediatR;

namespace StackOverflowLite.Application.Core.Abstractions;

public interface IQuery<TResponse> : IRequest<TResponse> { }
