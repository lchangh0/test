using KeyboardLib;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Management;
using Microsoft.Win32;

namespace test3;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        AddControls();

        // 최상위에 보이게 한다.
        this.TopMost = true;

        this.Load += Form1_Load;
    }


    System.Windows.Forms.Timer _timer1;

    private void Form1_Load(object sender, EventArgs e)
    {
        IntPtr handle = this.Handle;
        CKeyboardApi.SetWindowNoActivate(handle);

        _timer1 = new System.Windows.Forms.Timer();
        _timer1.Interval = 1000;
        _timer1.Tick += _timer1_Tick;
        _timer1.Enabled = true;
    }

    bool _bInTimer1 = false;
    string _strDebugText = "";

    private void _timer1_Tick(object? sender, EventArgs e)
    {
        if (_bInTimer1) return;
        
        _bInTimer1 = true;
        try
        {
            string strText = "";

            if (strText != _strDebugText)
            {
                _strDebugText = strText;
                testTextBox.Text = strText;
            }
        }
        finally
        {
            _bInTimer1 = false;
        }
    }


    TextBox testTextBox;
    TextBox testTextBox2;
    TextBox testTextBox3;
    CKeyboard keys = new CKeyboard();

    private void AddControls()
    {

        keys.MakeKorKeys();

        for (int iGY = 1; iGY <= 5; iGY++)
        {
            for (int iGX = 1; iGX <= 14; iGX++)
            {
                CKey key = keys.GetKey(iGX, iGY);
                if (key != null)
                {
                    Button btn = new Button();
                    btn.Name = $"btn_{key.key}";
                    btn.Text = key.GetKeyText(bShift: false, bLocal: false);
                    btn.Tag = key.scanCode;
                    btn.Size = new Size(50, 30);
                    btn.Location = new Point(10 + (iGX * btn.Size.Width), 10 + (iGY * btn.Size.Height));
                    btn.Click += TestButton_Click;
                    this.Controls.Add(btn);
                }
            }

        }


        testTextBox = new TextBox();
        testTextBox.Name = "testTextBox";
        testTextBox.Size = new Size(400, 30);
        testTextBox.Location = new Point(10, 310);
        this.Controls.Add(testTextBox);

        testTextBox2 = new TextBox();
        testTextBox2.Name = "testTextBox2";
        testTextBox2.Size = new Size(400, 30);
        testTextBox2.Location = new Point(10, 350);
        this.Controls.Add(testTextBox2);

        testTextBox3 = new TextBox();
        testTextBox3.Name = "testTextBox3";
        testTextBox3.Size = new Size(400, 30);
        testTextBox3.Location = new Point(10, 380);
        this.Controls.Add(testTextBox3);

    }


    private void TestButton_Click(object sender, EventArgs e)
    {
        this.ActiveControl = null;
        Button clickedButton = sender as Button;
        if (clickedButton == null)
            return;

        ushort keyScanCode = (ushort)clickedButton.Tag;
        testTextBox3.Text = keyScanCode.ToString();

        CKeyboardApi.SimulateKeyPress(keyScanCode);
    }

    
}
