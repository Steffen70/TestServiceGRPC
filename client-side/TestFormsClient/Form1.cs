using TestServiceGRPC;
using static TestServiceGRPC.Greeter;

namespace TestFormsClient;

public partial class Form1 : Form
{
    private readonly GreeterClient _greeterClient;
    private readonly Form2 _form2;

    public Form1(GreeterClient greeterClient, Form2 form2)
    {
        _greeterClient = greeterClient;
        _form2 = form2;

        InitializeComponent();
    }

    private async void button1_ClickAsync(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(textBox1.Text))
        {
            MessageBox.Show(@"Please enter a quote.");
            return;
        }

        _ = await _greeterClient.ChangeQuoteAsync(new ChangeQuoteRequest { Quote = textBox1.Text });

        textBox1.Text = string.Empty;
    }

    private void button2_Click(object sender, EventArgs e)
    {
        _form2.Show();
    }
}