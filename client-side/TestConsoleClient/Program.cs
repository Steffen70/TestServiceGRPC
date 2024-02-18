using CommonLib;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.AspNetCore.SignalR.Client;
using System.Data;
using TestServiceGRPC;

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

_ = await greeterClient.ChangeQuoteAsync(new ChangeQuoteRequest { Quote = "Ciao, " });

// Create a request
var request = new HelloRequest { Name = "John Snow" };

// Make the call
var reply = await greeterClient.SayHelloAsync(request);

// Convert the TableReply to a DataTable using the extension method from the CommonLib.DataTableConversionExtensions class.
var dataTable = reply.FromTableReply();

// Convert the DataTable to a string and print it to the console.
var output = dataTable.AsEnumerable()
    .Select(row => string.Join(", ", row.ItemArray.Select(item => item!.ToString())))
    .Aggregate((current, next) => current + Environment.NewLine + next);

Console.WriteLine(output);

// Stop the connection to the message hub.
await hubConnection.StopAsync();

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();