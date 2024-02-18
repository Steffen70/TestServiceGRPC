
using static TestServiceGRPC.Greeter;
using CommonLib;
using TestServiceGRPC;
using System.Windows.Forms;

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

        var request = new HelloRequest { Name = "John Snow" };

        var reply = await _greeterClient.SayHelloAsync(request);

        var dataTable = reply.FromTableReply();

        dataGridView1.DataSource = dataTable;

        dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
    }
}
