
using System.Data;
using static TestServiceGRPC.Greeter;
using CommonLib;
using TestServiceGRPC;
using System.Windows.Forms;
using Grpc.Core;

namespace TestFormsClient;

public partial class Form2 : Form
{
    private readonly GreeterClient _greeterClient;

    public Form2(GreeterClient greeterClient)
    {
        _greeterClient = greeterClient;
        InitializeComponent();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);

        if (e.CloseReason != CloseReason.UserClosing)
            return;

        e.Cancel = true;
        Hide();
    }

    private async void Form2_VisibleChangedAsync(object sender, EventArgs e)
    {
        if (!Visible)
            return;

        //var request = new HelloRequest { Name = "John Snow" };

        //var reply = await _greeterClient.SayHelloAsync(request);

        //var dataTable = reply.FromTableReply();

        //dataGridView1.DataSource = dataTable;

        //dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

        var dataTable = new DataTable();

        dataGridView1.DataSource = dataTable;

        var request = new HelloRequest { Name = "John Snow" };

        using var stream = _greeterClient.SayHelloStream(request);

        await foreach (var reply in stream.ResponseStream.ReadAllAsync())
        {
            dataTable.AddTableRow(reply);

            Invoke((MethodInvoker)(() =>
            {
                dataGridView1.Refresh();

                dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }));
        }
    }
}
