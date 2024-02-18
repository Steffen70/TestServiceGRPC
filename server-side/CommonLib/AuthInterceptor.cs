using Grpc.Core;
using Grpc.Core.Interceptors;

namespace CommonLib;

public class AuthInterceptor : Interceptor
{
    private readonly string _token;

    public AuthInterceptor(string token) => _token = token;

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var headers = new Metadata { new("Authorization", $"Bearer {_token}") };

        var newOptions = context.Options.WithHeaders(headers);

        var newContext = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method,
            context.Host,
            newOptions);

        return base.AsyncUnaryCall(request, newContext, continuation);
    }
}
