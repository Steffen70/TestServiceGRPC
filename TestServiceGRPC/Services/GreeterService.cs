using Grpc.Core;
using System.Data;
using static CommonLib.DataTableConversionExtensions;

namespace TestServiceGRPC.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        public override Task<TableReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var dataTable = GetHardcodedDataTable(request.Name);

            var reply = dataTable.ToTableReply();

            return Task.FromResult(reply);
        }

        private static DataTable GetHardcodedDataTable(string inputName)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Message", typeof(string));

            var names = new[] { "David", "Ajanth", "Fredy", "Biljana" };
            foreach (var name in names)
            {
                dataTable.Rows.Add(name, $"{name}: Ciao {inputName}");
            }

            return dataTable;
        }
    }
}
