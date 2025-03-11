using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Platform;
using KeyboardLib;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Timers;

namespace testAvalonia
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

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
            this.DataContext = this;
            InitializeComponent();

            AddControls();

            this.Topmost = true;
            this.Opened += OnWindowOpened;

            _timer1 = new Timer();
            _timer1.Interval = 1000;
            _timer1.Elapsed += OnTimer1Elapsed;
            _timer1.Enabled = true;
        }

        Timer _timer1;
        CKeyboard _keyboard = new CKeyboard();

        void AddControls()
        {
            var canvas = new Canvas();

            _keyboard.MakeKorKeys();
            int xPosMax = _keyboard.GetKeyboardXPosMax();
            int yPosMax = _keyboard.GetKeyboardYPosMax();

            for(int iY = 0; iY <= yPosMax; iY++)
            {
                for(int iX = 0; iX <= xPosMax; iX++)
                {
                    CKey key = _keyboard.GetKey(iX, iY);
                    if (key == null) continue;

                    Button button = new Button();
                    button.Content = _keyboard.GetKeyText(key);
                    button.Tag = key;
                    button.Click += OnClickButton;
                    button.Width = 60;
                    button.Height = 50;
                    button.Margin = new Thickness(5);
                    button.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                    button.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;

                    Canvas.SetLeft(button, 60 * iX);
                    Canvas.SetTop(button, 50 * iY);

                    canvas.Children.Add(button);
                }
            }

            TextBlock textBlock = new TextBlock();
            textBlock.Bind(TextBlock.TextProperty, new Binding("DebugText"));
            Canvas.SetLeft(textBlock, 50);
            Canvas.SetTop(textBlock, 350);
            
            canvas.Children.Add(textBlock);

            this.Content = canvas;
        }

        private void OnWindowOpened(object sender, EventArgs e)
        {
            // 윈도우에 포커스가 가지 않도록 설정
            SetNoActivate();
        }


        private void SetNoActivate()
        {
            var platformHandle = this.TryGetPlatformHandle();

            if (platformHandle is not null)
            {
                if (platformHandle.HandleDescriptor == "HWND")
                {
                    IntPtr hwnd = platformHandle.Handle;
                    CKeyboardApi.SetWindowNoActivate(hwnd);
                }
            }
        }


        string _strLang = "";
        public string Lang
        {  
            get { return _strLang; } 
            set { _strLang = value; OnPropertyChanged(); }
        }

        string _strDebugText = "";
        public string DebugText
        {
            get { return _strDebugText; }
            set { _strDebugText = value; OnPropertyChanged(); }
        }


        bool _bInTimer1 = false;

        void OnTimer1Elapsed(object sender, EventArgs e)
        {
            if (_bInTimer1) return;

            try
            {
                _bInTimer1 = true;

                string strLang = CKeyboardApi.GetCurrentKeyboardLanguage(
                    out uint iImeConversion, out uint iImeSentence);

                string strText = string.Format("{0}, {1}", iImeConversion, iImeSentence);

                if (strText != _strDebugText)
                {
                    DebugText = strText;
                }

                if (strLang != _strLang)
                {
                    Lang = strLang;
                }
            }
            finally
            {
                _bInTimer1 = false;
            }
        }


        private void OnClickButton(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            CKey? key = button?.Tag as CKey;
            if (key != null)
            {
                CKeyboardApi.SimulateKeyPress(key.scanCode);

                if (_keyboard.IsShiftKey(key))
                {
                    _keyboard.ToggleShift();
                    ChangeKeyboardText();
                }
                else if(_keyboard.IsLocalKey(key))
                {
                    _keyboard.ToggleLocal();
                    ChangeKeyboardText();
                }
            }
        }

        void ChangeKeyboardText()
        {
            Canvas? canvas = this.Content as Canvas;
            if (canvas == null) return;

            foreach(var child in canvas.Children)
            {
                Button? button = child as Button;
                if (button == null) continue;

                CKey? key = button.Tag as CKey;
                if (key == null) continue;

                string strKeyText = _keyboard.GetKeyText(key);
                button.Content = strKeyText;
            }

            this.InvalidateVisual();
        }

    }
}