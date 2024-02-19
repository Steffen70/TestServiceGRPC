Imports CommonLib
Imports Grpc.Core.Interceptors
Imports Grpc.Net.Client
Imports Microsoft.AspNetCore.SignalR.Client
Imports TestServiceGRPC
Imports Microsoft.Extensions.DependencyInjection
Imports TestFormsClient
Imports System.Windows.Forms
Imports Grpc.Core

Module Program

    <STAThread>
    Sub Main()
        ' Configure the asynchronous context for VB.NET
        Call MainAsync().GetAwaiter().GetResult()
    End Sub

    Private Async Function MainAsync() As Task
        Dim baseAddress As New Uri("https://localhost:5001/")

        Dim channel As GrpcChannel = GrpcChannel.ForAddress(baseAddress)
        Dim authClient As New Auth.AuthClient(channel)

        ' Create a login request object with email and password.
        Dim loginRequest As New LoginRequest With {
            .Email = "steffen@seventy.mx",
            .Password = "bentclub72"
        }

        ' Send a request to the login endpoint with the login request data.
        Dim loginResponse = Await authClient.LoginAsync(loginRequest)

        ' Extract the token from the login response and remove the 'Bearer ' prefix.
        Dim token As String = loginResponse.Token.Substring("Bearer ".Length)

        ' Add the token to the authorization header of the gRPC channel.
        Dim interceptor As New AuthInterceptor(token)
        Dim callInvoker As CallInvoker = channel.Intercept(interceptor)

        ' Create a new gRPC client for the Greeter service with the modified channel to include the access token for authorized endpoint access.
        authClient = New Auth.AuthClient(callInvoker)

        ' Create a new gRPC client for the Greeter service with the modified channel.
        Dim greeterClient As New Greeter.GreeterClient(callInvoker)

        ' Construct the message hub URL and append the access token as a query string parameter.
        Dim messageHubUrl As String = $"{baseAddress}hubs/session-hub?access_token={token}"

        ' Initialize a new HubConnectionBuilder and configure it with the message hub URL and automatic reconnect feature.
        Dim hubConnection As HubConnection = New HubConnectionBuilder().
            WithUrl(messageHubUrl).
            WithAutomaticReconnect().
            Build()

        ' Register an event handler that writes received messages to the console.
        hubConnection.On("ReceiveMessage", Sub(message As String)
                                               Console.WriteLine($"Received message: {message}")
                                           End Sub)

        ' Start the connection to the message hub.
        Await hubConnection.StartAsync()

        'ApplicationConfiguration.Initialize()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Dim services As New ServiceCollection()

        services.AddSingleton(authClient)
        services.AddSingleton(greeterClient)
        services.AddSingleton(hubConnection)
        services.AddTransient(Of Form1)()
        services.AddSingleton(Of Form2)()

        ' Build the service provider
        Dim serviceProvider As IServiceProvider = services.BuildServiceProvider()

        ' Resolve the main form
        Dim mainForm As Form = serviceProvider.GetRequiredService(Of Form1)()

        ' Handle the application exit event.
        AddHandler Application.ApplicationExit, Async Sub(sender, e)
                                                    If hubConnection.State = HubConnectionState.Connected Then
                                                        Await hubConnection.StopAsync()
                                                    End If
                                                End Sub

        ' Run the application with the main form.
        Application.Run(mainForm)
    End Function

End Module
