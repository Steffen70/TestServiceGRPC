# Define the root path of the DLLs
$assemblyRootPath = "..\TestConsoleClient\bin\Debug\net8.0"

# Load the necessary assembly from NuGet package because it is not available in bin folder but required for gRPC
$loggingAbstractionsPath = Join-Path $Env:USERPROFILE ".nuget\packages\microsoft.extensions.logging.abstractions\3.0.3\lib\netstandard2.0\Microsoft.Extensions.Logging.Abstractions.dll"

Add-Type -Path $loggingAbstractionsPath

# Load the necessary .NET assemblies from console client bin folder
$requiredAssemblies = @(
    "Google.Protobuf.dll",                  # Base dependency
    "Grpc.Core.Api.dll",                    # Base dependency for gRPC
    "Grpc.Net.Common.dll",                  # Common functionality for Grpc.Net
    "Grpc.Net.Client.dll",                  # Grpc.Net.Client
    "Grpc.AspNetCore.Server.ClientFactory.dll", # Factory for creating gRPC clients
    "CommonLib.dll"                         # Custom library
)


foreach ($assembly in $requiredAssemblies) {
    $assemblyPath = Join-Path $assemblyRootPath $assembly
    Add-Type -Path $assemblyPath
}

# Create a GrpcChannel
$channel = [Grpc.Net.Client.GrpcChannel]::ForAddress("https://localhost:5001")

# Instantiate the client using the channel
$client = [TestServiceGRPC.Greeter+GreeterClient]::new($channel)

# Create a request
$request = [TestServiceGRPC.HelloRequest]::new()
$request.Name = "Jackie Chan"

# Make the gRPC call and get the response
$response = $client.SayHelloAsync($request).ResponseAsync.Result

# Process and output the response
foreach ($row in $response.Rows) {
    foreach ($column in $row.Columns) {
        Write-Host "$($column.Column): $($column.Value)"
    }
    Write-Host "-----------"
}

# Shutdown the channel
$channel.ShutdownAsync().Wait()