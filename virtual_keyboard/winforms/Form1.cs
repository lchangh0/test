using KeyboardLib;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Management;
using Microsoft.Win32;
using System.Text;

namespace test3;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();

    }


    System.Windows.Forms.Timer _timer1;

    private void Form1_Load(object sender, EventArgs e)
    {
        AddControls();

        //this.Width = 880;
        //this.Height = 300;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MinimizeBox = false;
        this.MaximizeBox = false;
        this.Text = "Keyboard";

        // 최상위에 보이게 한다.
        this.TopMost = true;

        this.Load += Form1_Load;

        IntPtr handle = this.Handle;
        CKeyboardLib.SetWindowNoActivate(handle);

        _timer1 = new System.Windows.Forms.Timer();
        _timer1.Interval = 1000;
        _timer1.Tick += _timer1_Tick;
        _timer1.Enabled = true;
    }

    //Label lblDebug;
    CKeyboard _keyboard = new CKeyboard();

    private void AddControls()
    {
        _keyboard.MakeKorKeys();

        int iMarginX = 10;
        int iMarginY = 10;
        int iCellWidth = 60;
        int iCellHeight = 50;

        int columns = _keyboard.GetKeyboardColumns();
        int rows = _keyboard.GetKeyboardRows();

        for (int rowIdx = 0; rowIdx < rows; rowIdx++)
        {
            for (int columnIdx = 0; columnIdx < columns; columnIdx++)
            {
                CKey key = _keyboard.GetKey(columnIdx, rowIdx);
                if (key != null)
                {
                    Button btn = new Button();
                    btn.Name = $"btn_{key.keyChar}";
                    btn.Text = key.GetKeyText(bShift: false, bLocalLang: false, bCapsLock:false);
                    btn.Tag = key;
                    btn.Size = new Size(iCellWidth * key.columnSpan , iCellHeight);
                    btn.Location = new Point(iMarginX + (columnIdx * iCellWidth), iMarginY + (rowIdx * iCellHeight));
                    btn.BackColor = _btnColorNormal;
                    btn.TabStop = false;
                    btn.Click += Button_Click;
                    this.Controls.Add(btn);
                }
            }
        }

        //lblDebug = new Label();
        //lblDebug.Location = new Point(10, 250);
        //this.Controls.Add(lblDebug);
    }


    private void Button_Click(object sender, EventArgs e)
    {
        this.ActiveControl = null;
        Button clickedButton = sender as Button;
        if (clickedButton == null)
            return;

        CKey key = clickedButton.Tag as CKey;
        if (key != null)
        {
            ushort keyScanCode = key.scanCode;

            bool bWithShift = (key.HasShiftKey() && _keyboard.GetShift());
            CKeyboardLib.SimulateKeyPress(keyScanCode, bWithShift);

            if (_keyboard.IsCapsLockKey(key))
            {
                _keyboard.ToggleCapsLock();
                ChangeKeyboardText();
            }
            else if (_keyboard.IsShiftKey(key))
            {
                _keyboard.ToggleShift();
                ChangeKeyboardText();
            }
            else if(_keyboard.IsLocalLanguageKey(key))
            {
                _keyboard.ToggleLocalLanguage();
                ChangeKeyboardText();
            }
        }
    }


    Color _btnColorNormal = Color.FromArgb(250, 250, 250);
    Color _btnColorPressed = Color.FromArgb(150, 150, 150);

    void ChangeKeyboardText()
    {
        foreach (var child in this.Controls)
        {
            Button button = child as Button;
            if (button == null) continue;

            CKey key = button.Tag as CKey;
            if (key == null) continue;

            string strKeyText = _keyboard.GetKeyText(key);
            button.Text = strKeyText;

            if ((_keyboard.IsShiftKey(key) && _keyboard.GetShift()) ||
                (_keyboard.IsCapsLockKey(key) && _keyboard.GetCapsLock()) ||
                (_keyboard.IsLocalLanguageKey(key) && _keyboard.GetLocalLanguage()) )
                button.BackColor = _btnColorPressed;
            else
                button.BackColor = _btnColorNormal;
        }
    }


    bool _bInTimer1 = false;
    string _strDebugText = "";

    private void _timer1_Tick(object? sender, EventArgs e)
    {
        if (_bInTimer1) return;

        _bInTimer1 = true;
        try
        {
            DetectKeyboardStatus();
            DisplayDebugText();
        }
        finally
        {
            _bInTimer1 = false;
        }
    }


    void DisplayDebugText()
    {
        if (lblDebug.Text != _strDebugText)
        {
            lblDebug.Text = _strDebugText;
        }
    }


    IntPtr _handleFocusedControl = IntPtr.Zero;

    void DetectKeyboardStatus()
    {
        StringBuilder sbDebug = new StringBuilder();

        IntPtr handle = CKeyboardLib.GetFocusedControlHandle(out string strInfo1);
        sbDebug.Append(strInfo1);

        if (handle != IntPtr.Zero && 
            handle != _handleFocusedControl)
        {
            // 키가 눌리기 전 컨트롤의 텍스트
            string strText = CKeyboardLib.GetControlText(handle);
            sbDebug.AppendLine($"Text={strText}");

            // 'a'키 눌림 처리
            CKeyboardLib.SimulateKeyPress((ushort)CKeyboard.EScanCode.A, false);
            Thread.Sleep(10);

            // 키가 눌린 후 컨트롤의 텍스트
            string strTextA = CKeyboardLib.GetControlText(handle);
            sbDebug.AppendLine($"TextA={strTextA}");

            // BackSpace키 눌림 처리
            CKeyboardLib.SimulateKeyPress((ushort)CKeyboard.EScanCode.BackSpace, false);
            Thread.Sleep(10);

            // 'a'키가 IME에서 소문자로 처리됨
            if (strText + 'a' == strTextA)
            {
                bool bNeedChange = false;
                if (_keyboard.GetLocalLanguage())
                {
                    _keyboard.SetLocalLanguage(false);
                    bNeedChange = true;
                }

                if (_keyboard.GetCapsLock())
                {
                    _keyboard.SetCapsLock(false);
                    bNeedChange = true;
                }

                if (bNeedChange)
                    ChangeKeyboardText();
            }
            // 'a'키가 IME에서 대문자로 처리됨
            else if (strText + 'A' == strTextA)
            {
                bool bNeedChange = false;
                if (_keyboard.GetLocalLanguage())
                { 
                    _keyboard.SetLocalLanguage(false);
                    bNeedChange = true;
                }

                if (!_keyboard.GetCapsLock())
                {
                    _keyboard.SetCapsLock(true);
                    bNeedChange = true;
                }

                if (bNeedChange)
                    ChangeKeyboardText();
            }
            // 'a'키가 IME에서 한글로 처리됨
            else if (strText + 'ㅁ' == strTextA)
            {
                if (!_keyboard.GetLocalLanguage())
                {
                    _keyboard.SetLocalLanguage(true);
                    ChangeKeyboardText();
                }
            }
        }

        _handleFocusedControl = handle;

        IntPtr hWnd = CWindowsApi.GetForegroundWindow();
        //IntPtr hWnd = this.Handle;
        string strImeStatus = CKeyboardLib.GetImeStatus(hWnd, out uint iConversion, out uint iSentence);
        sbDebug.AppendLine($"HWnd={hWnd}, ImeStatus={strImeStatus}, Conversion=0x{iConversion:X4}, Sendtence=0x{iSentence:X4}");

        _strDebugText = sbDebug.ToString();
    }





}
