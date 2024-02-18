using CommonLib;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.AspNetCore.SignalR.Client;
using TestServiceGRPC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestFormsClient;

var baseAddress = new Uri("https://localhost:5001/");

var channel = GrpcChannel.ForAddress(baseAddress);
var authClient = new Auth.AuthClient(channel);

// Create a login request object with email and password.
var loginRequest = new LoginRequest
{
    Email = "steffen@seventy.mx",
    Password = "bentclub72"
};

// Send a request to the login endpoint with the login request data.
var loginResponse = await authClient.LoginAsync(loginRequest);

// Extract the token from the login response and remove the 'Bearer ' prefix.
var token = loginResponse!.Token["Bearer ".Length..];

// Add the token to the authorization header of the gRPC channel.
var interceptor = new AuthInterceptor(token);
var callInvoker = channel.Intercept(interceptor);

// Create a new gRPC client for the Greeter service with the modified channel to include the access token for authorized endpoint access.
authClient = new Auth.AuthClient(callInvoker);

// Create a new gRPC client for the Greeter service with the modified channel.
var greeterClient = new Greeter.GreeterClient(callInvoker);

// Construct the message hub URL and append the access token as a query string parameter.
var messageHubUrl = $"{baseAddress}hubs/session-hub?access_token={token}";

// Initialize a new HubConnectionBuilder and configure it with the message hub URL and automatic reconnect feature.
var hubConnection = new HubConnectionBuilder()
    .WithUrl(messageHubUrl)
    .WithAutomaticReconnect()
    .Build();

// Register an event handler that writes received messages to the console.
hubConnection.On<string>("ReceiveMessage", Console.WriteLine);

// Start the connection to the message hub.
await hubConnection.StartAsync();

ApplicationConfiguration.Initialize();
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((_, services) =>
    {
        services.AddSingleton(authClient);
        services.AddSingleton(greeterClient);
        services.AddSingleton(hubConnection);
        services.AddTransient<Form1>();
        services.AddSingleton<Form2>();
    }).Build();

var mainForm = host.Services.GetRequiredService<Form1>();

Application.ApplicationExit += async (_, _) =>
{
    if (hubConnection.State == HubConnectionState.Connected) 
        await hubConnection.StopAsync();
};

Application.Run(mainForm);