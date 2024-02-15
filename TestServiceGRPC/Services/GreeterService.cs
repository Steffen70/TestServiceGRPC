using Grpc.Core;
using System.Data;
using CommonLib.Model;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using static CommonLib.DataTableConversionExtensions;

namespace TestServiceGRPC.Services;

[Authorize]
public class GreeterService : Greeter.GreeterBase
{
    private readonly SessionData _sessionData;

    public GreeterService(SessionData sessionData) => _sessionData = sessionData;


    public override Task<TableReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        var dataTable = GetHardcodedDataTable(request.Name);

        var reply = dataTable.ToTableReply();

        return Task.FromResult(reply);
    }

    public override Task<Empty> ChangeQuote(ChangeQuoteRequest request, ServerCallContext context)
    {
        _sessionData.Quote = request.Quote;

        return base.ChangeQuote(request, context);
    }

    private DataTable GetHardcodedDataTable(string inputName)
    {
        var dataTable = new DataTable();

        dataTable.Columns.Add("Name", typeof(string));
        dataTable.Columns.Add("Message", typeof(string));

        var names = new[]
        {
            "David",
            "Ajanth",
            "Fredy", 
            "Biljana"
        };

        foreach (var name in names)
            dataTable.Rows.Add(name, $"{name}: {_sessionData.Quote} {inputName}");

        return dataTable;
    }
}
