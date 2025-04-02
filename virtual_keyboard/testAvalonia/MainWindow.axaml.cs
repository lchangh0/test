using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using KeyboardLib;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;

namespace testAvalonia
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public new event PropertyChangedEventHandler PropertyChanged;

        string _message;
        public string Message
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public MainWindow()
        {
            _message = "";

            this.DataContext = this;

            InitializeComponent();

            this.Width = 870;
            this.Height = 300;
            this.Title = "Keyboard";
            this.Topmost = true;
            this.Opened += OnWindowOpened;
        }


        private void OnWindowOpened(object sender, EventArgs e)
        {
            AddControls();

            _timer1 = new System.Timers.Timer();
            _timer1.Interval = 1000;
            _timer1.Elapsed += OnTimer1Elapsed;
            _timer1.Enabled = true;

            // 윈도우에 포커스가 가지 않도록 설정
            SetNoActivate();
        }


        System.Timers.Timer _timer1;
        CKeyboard _keyboard = new CKeyboard();

        void AddControls()
        {
            var canvas = new Canvas();

            _keyboard.MakeKorKeys();
            int columns = _keyboard.GetKeyboardColumns();
            int rows = _keyboard.GetKeyboardRows();

            int iMarginX = 10;
            int iMarginY = 10;
            int iCellWidth = 60;
            int iCellHeight = 50;

            for(int rowIdx = 0; rowIdx < rows; rowIdx++)
            {
                for(int columnIdx = 0; columnIdx < columns; columnIdx++)
                {
                    CKey key = _keyboard.GetKey(columnIdx, rowIdx);
                    if (key == null) continue;

                    Button button = new Button();
                    button.Content = _keyboard.GetKeyText(key);
                    button.Tag = key;
                    button.Click += OnClickButton;
                    button.Width = iCellWidth * key.columnSpan;
                    button.Height = iCellHeight;
                    button.Margin = new Thickness(5);
                    button.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                    button.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
                    button.Background = _btnColorNormal;

                    Canvas.SetLeft(button, iMarginX + iCellWidth * columnIdx);
                    Canvas.SetTop(button, iMarginY + iCellHeight * rowIdx);

                    canvas.Children.Add(button);
                }
            }

            TextBlock textBlock = new TextBlock();
            textBlock.Bind(TextBlock.TextProperty, new Binding("DebugText"));
            Canvas.SetLeft(textBlock, 10);
            Canvas.SetTop(textBlock, 220);
            
            canvas.Children.Add(textBlock);

            this.Content = canvas;
        }


        private void SetNoActivate()
        {
            var platformHandle = this.TryGetPlatformHandle();

            if (platformHandle is not null)
            {
                if (platformHandle.HandleDescriptor == "HWND")
                {
                    IntPtr hwnd = platformHandle.Handle;
                    CKeyboardLib.SetWindowNoActivate(hwnd);
                }
            }
        }


        string _strDebugText = "";
        public string DebugText
        {
            get { return _strDebugText; }
            set { _strDebugText = value; OnPropertyChanged(); }
        }


        private void OnClickButton(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            CKey? key = button?.Tag as CKey;
            if (key != null)
            {
                bool bWithShift = (key.HasShiftKey() && _keyboard.GetShift());

                CKeyboardLib.SimulateKeyPress(key.scanCode, bWithShift);

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


        Brush _btnColorNormal =  new SolidColorBrush(Color.FromRgb(230, 230, 230));
        Brush _btnColorPressed = new SolidColorBrush(Color.FromRgb(150, 150, 150));

        void ChangeKeyboardText()
        {
            // UI쓰레드가 아닌 다른 쓰레드에서 이 함수를 호출해도 정상 동작하도록 처리
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Canvas? canvas = this.Content as Canvas;
                if (canvas == null) return;

                foreach (var child in canvas.Children)
                {
                    Button? button = child as Button;
                    if (button == null) continue;

                    CKey? key = button.Tag as CKey;
                    if (key == null) continue;

                    string strKeyText = _keyboard.GetKeyText(key);
                    button.Content = strKeyText;

                    if ((_keyboard.IsShiftKey(key) && _keyboard.GetShift()) ||
                       (_keyboard.IsCapsLockKey(key) && _keyboard.GetCapsLock()) ||
                       (_keyboard.IsLocalLanguageKey(key) && _keyboard.GetLocalLanguage()))
                        button.Background = _btnColorPressed;
                    else
                        button.Background = _btnColorNormal;

                }

                this.InvalidateVisual();
            });
        }


        bool _bInTimer1 = false;

        void OnTimer1Elapsed(object sender, EventArgs e)
        {
            if (_bInTimer1) return;

            try
            {
                _bInTimer1 = true;

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
            if (_strDebug != DebugText)
                DebugText = _strDebug;
        }

        string _strDebug = "";
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

            _strDebug = sbDebug.ToString();
        }


    }
}