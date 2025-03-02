using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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

    private void Form1_Load(object sender, EventArgs e)
    {
    }

    // 폼에 입력 포커스가 가지 않게 한다.
    // SendInput() API가 정상 동작하려면 가상키보드 화면에 포커스가 설정되면 안된다.
    protected override CreateParams CreateParams
    {
        get 
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x08000000;
            return cp;
        }
    }


    TextBox testTextBox;
    TextBox testTextBox2;
    TextBox testTextBox3;
    CKeys keys = new CKeys();

    private void AddControls()
    {
        
        keys.MakeKorKeys();

        for(int iGY = 1; iGY <= 5; iGY++)
        {
            for(int iGX = 1; iGX <= 14; iGX++)
            {
                CKey key = keys.GetKey(iGX, iGY);
                if (key != null)
                {
                    Button btn = new Button();
                    btn.Name = $"btn_{key.key}";
                    btn.Text = key.GetKeyText(bShift:false, bLocal:false);
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
        testTextBox.Size = new Size(100, 30);
        testTextBox.Location = new Point(10, 210);
        this.Controls.Add(testTextBox);

        testTextBox2 = new TextBox();
        testTextBox2.Name = "testTextBox2";
        testTextBox2.Size = new Size(100, 30);
        testTextBox2.Location = new Point(10, 250);
        this.Controls.Add(testTextBox2);

        testTextBox3 = new TextBox();
        testTextBox3.Name = "testTextBox3";
        testTextBox3.Size = new Size(100, 30);
        testTextBox3.Location = new Point(10, 280);
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

        SimulateKeyPress(keyScanCode);
    }

    private void SimulateKeyPress(ushort keyScanCode)
    {
        INPUT[] inputs = new INPUT[]
        {
            new INPUT
            {
                type = 1, // INPUT_KEYBOARD
                u = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = keyScanCode,
                        dwFlags = 0x0008 // KEYEVENTF_SCANCODE
                    }
                }
            },
            new INPUT
            {
                type = 1, // INPUT_KEYBOARD
                u = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = keyScanCode,
                        dwFlags = 0x0008 | 0x0002 // KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP
                    }
                }
            }
        };

        SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
    }

    [DllImport("user32.dll")]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint type;
        public InputUnion u;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)] public MOUSEINPUT mi;
        [FieldOffset(0)] public KEYBDINPUT ki;
        [FieldOffset(0)] public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct HARDWAREINPUT
    {
        public uint uMsg;
        public ushort wParamL;
        public ushort wParamH;
    }
}
