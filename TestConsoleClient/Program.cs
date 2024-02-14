using Grpc.Net.Client;
using TestServiceGRPC;

// The address must match the address of your gRPC server
var serverAddress = "https://localhost:5001";

// Create a channel to communicate with the server
using var channel = GrpcChannel.ForAddress(serverAddress);

// Create a client with the channel
var client = new Greeter.GreeterClient(channel);

// Create a request
var request = new HelloRequest { Name = "John Snow" }; // Replace "YourName" with appropriate value

try
{
    // Make the call
    var reply = await client.SayHelloAsync(request);

    // Process the reply (for example, print it)
    foreach (var row in reply.Rows)
    {
        foreach (var column in row.Columns)
        {
            Console.WriteLine($"{column.Column}: {column.Value}");
        }
        Console.WriteLine("-----------");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error calling gRPC service: {ex.Message}");
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
