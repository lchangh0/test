﻿using System.Globalization;
using System.Runtime.InteropServices;

namespace KeyboardLib
{

    public static class CKeyboardApi
    {

        public static void SimulateKeyPress(ushort keyScanCode)
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


        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int GWL_EXSTYLE = -20;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        public static void SetWindowNoActivate(IntPtr hwnd)
        {
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_NOACTIVATE);
        }


        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint threadId);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("imm32.dll")]
        private static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("imm32.dll")]
        private static extern bool ImmGetConversionStatus(IntPtr hIMC, out uint lpfdwConversion, out uint lpfdwSentence);


        public static string GetCurrentKeyboardLanguage(
            out uint iImeConversion, out uint iImeSentence)
        {
            iImeConversion = 0;
            iImeSentence = 0;

            IntPtr foregroundWindow = GetForegroundWindow();
            uint threadId = GetWindowThreadProcessId(foregroundWindow, out uint processId);
            //iThreadId = threadId;

            IntPtr layout = GetKeyboardLayout(threadId);

            //CultureInfo culture = new CultureInfo((layout & 0xffff).ToInt32());
            //string strName = culture.DisplayName;

            if (((uint)layout & 0xFFFF) == 0x0412) // 한글
            {
                return "ko";
            }
            else if (((uint)layout & 0xFFFF) == 0x0409) // 영문
            {
                return "en";
            }
            else
                return "";
        }

    }



    public class CKey
    {
        public ushort scanCode;

        public string keyText;
        public char key;
        public char keyShift;
        public char keyLocal;
        public char keyLocalShift;

        public int posX;
        public int posY;

        public string GetKeyText(bool bShift, bool bLocal)
        {
            string strText = null;

            if (bLocal)
            {
                if (bShift)
                {
                    if (keyLocalShift != 0)
                        strText = keyLocalShift.ToString();
                }
                else
                {
                    if (keyLocal != 0)
                        strText = keyLocal.ToString();
                }
            }
            else
            {
                if (bShift)
                {
                    if (keyShift != 0)
                        strText = keyShift.ToString();
                }
                else
                {
                    if (key != 0)
                        strText = key.ToString();
                }
            }

            if (strText == null && keyText != null)
                strText = keyText;

            return strText;
        }

    }


    public class CKeyboard
    {
        List<CKey> _keys;

        public CKeyboard()
        {
            _keys = new List<CKey>();
        }

        public CKey GetKey(int x, int y)
        {
            foreach (CKey key in _keys)
            {
                if (key.posX == x && key.posY == y)
                {
                    return key;
                }
            }

            return null;
        }


        public int GetKeyboardXPosMax()
        {
            int xCount = -1;

            foreach (CKey key in _keys)
            {
                if (key.posX > xCount)
                    xCount = key.posX;
            }

            return xCount;
        }

        public int GetKeyboardYPosMax()
        {
            int yCount = -1;

            foreach (CKey key in _keys)
            {
                if (key.posY > yCount)
                    yCount = key.posY;
            }

            return yCount;
        }

        bool _bShift = false;
        bool _bLocal = false;

        public void SetShift(bool bShift)
        {
            _bShift = bShift;
        }

        public void ToggleShift()
        {
            _bShift = !_bShift;
        }

        public void SetLocal(bool bLocal)
        { 
            _bLocal = bLocal; 
        }

        public void ToggleLocal()
        {
            _bLocal = !_bLocal;
        }


        public string GetKeyText(CKey key)
        {
            return key.GetKeyText(_bShift, _bLocal);
        }

        public bool IsShiftKey(CKey key)
        {
            return (key?.scanCode == 42 || key?.scanCode == 54);
        }

        public bool IsLocalKey(CKey key)
        {
            return (key?.scanCode == 114);
        }


        public void MakeKorKeys()
        {
            _keys.Clear();

            _keys.Add(new CKey() { scanCode = 41, key = '`', keyShift = '~', posX = 1, posY = 1 });
            _keys.Add(new CKey() { scanCode = 2, key = '1', keyShift = '!', posX = 2, posY = 1 });
            _keys.Add(new CKey() { scanCode = 3, key = '2', keyShift = '@', posX = 3, posY = 1 });
            _keys.Add(new CKey() { scanCode = 4, key = '3', keyShift = '#', posX = 4, posY = 1 });
            _keys.Add(new CKey() { scanCode = 5, key = '4', keyShift = '$', posX = 5, posY = 1 });
            _keys.Add(new CKey() { scanCode = 6, key = '5', keyShift = '%', posX = 6, posY = 1 });
            _keys.Add(new CKey() { scanCode = 7, key = '6', keyShift = '^', posX = 7, posY = 1 });
            _keys.Add(new CKey() { scanCode = 8, key = '7', keyShift = '&', posX = 8, posY = 1 });
            _keys.Add(new CKey() { scanCode = 9, key = '8', keyShift = '*', posX = 9, posY = 1 });
            _keys.Add(new CKey() { scanCode = 10, key = '9', keyShift = '(', posX = 10, posY = 1 });
            _keys.Add(new CKey() { scanCode = 11, key = '0', keyShift = ')', posX = 11, posY = 1 });
            _keys.Add(new CKey() { scanCode = 12, key = '-', keyShift = '_', posX = 12, posY = 1 });
            _keys.Add(new CKey() { scanCode = 13, key = '=', keyShift = '+', posX = 13, posY = 1 });
            _keys.Add(new CKey() { scanCode = 14, keyText = "BackS", posX = 14, posY = 1 }); // 백스페이스

            _keys.Add(new CKey() { scanCode = 15, keyText = "Tab", posX = 1, posY = 2 });
            _keys.Add(new CKey() { scanCode = 16, key = 'q', keyShift = 'Q', keyLocal = 'ㅂ', keyLocalShift = 'ㅃ', posX = 2, posY = 2 });
            _keys.Add(new CKey() { scanCode = 17, key = 'w', keyShift = 'W', keyLocal = 'ㅈ', keyLocalShift = 'ㅉ', posX = 3, posY = 2 });
            _keys.Add(new CKey() { scanCode = 18, key = 'e', keyShift = 'E', keyLocal = 'ㄷ', keyLocalShift = 'ㄸ', posX = 4, posY = 2 });
            _keys.Add(new CKey() { scanCode = 19, key = 'r', keyShift = 'R', keyLocal = 'ㄱ', keyLocalShift = 'ㄲ', posX = 5, posY = 2 });
            _keys.Add(new CKey() { scanCode = 20, key = 't', keyShift = 'T', keyLocal = 'ㅅ', keyLocalShift = 'ㅆ', posX = 6, posY = 2 });
            _keys.Add(new CKey() { scanCode = 21, key = 'y', keyShift = 'Y', keyLocal = 'ㅛ', posX = 7, posY = 2 });
            _keys.Add(new CKey() { scanCode = 22, key = 'u', keyShift = 'U', keyLocal = 'ㅕ', posX = 8, posY = 2 });
            _keys.Add(new CKey() { scanCode = 23, key = 'i', keyShift = 'I', keyLocal = 'ㅑ', posX = 9, posY = 2 });
            _keys.Add(new CKey() { scanCode = 24, key = 'o', keyShift = 'O', keyLocal = 'ㅐ', keyLocalShift = 'ㅒ', posX = 10, posY = 2 });
            _keys.Add(new CKey() { scanCode = 25, key = 'p', keyShift = 'P', keyLocal = 'ㅔ', keyLocalShift = 'ㅖ', posX = 11, posY = 2 });
            _keys.Add(new CKey() { scanCode = 26, key = '[', keyShift = '{', posX = 12, posY = 2 });
            _keys.Add(new CKey() { scanCode = 27, key = ']', keyShift = '}', posX = 13, posY = 2 });
            _keys.Add(new CKey() { scanCode = 43, key = '\\', keyShift = '|', posX = 14, posY = 2 });

            _keys.Add(new CKey() { scanCode = 58, keyText = "Caps", posX = 1, posY = 3 });
            _keys.Add(new CKey() { scanCode = 30, key = 'a', keyShift = 'A', keyLocal = 'ㅁ', posX = 2, posY = 3 });
            _keys.Add(new CKey() { scanCode = 31, key = 's', keyShift = 'S', keyLocal = 'ㄴ', posX = 3, posY = 3 });
            _keys.Add(new CKey() { scanCode = 32, key = 'd', keyShift = 'D', keyLocal = 'ㅇ', posX = 4, posY = 3 });
            _keys.Add(new CKey() { scanCode = 33, key = 'f', keyShift = 'F', keyLocal = 'ㄹ', posX = 5, posY = 3 });
            _keys.Add(new CKey() { scanCode = 34, key = 'g', keyShift = 'G', keyLocal = 'ㅎ', posX = 6, posY = 3 });
            _keys.Add(new CKey() { scanCode = 35, key = 'h', keyShift = 'H', keyLocal = 'ㅗ', posX = 7, posY = 3 });
            _keys.Add(new CKey() { scanCode = 36, key = 'j', keyShift = 'J', keyLocal = 'ㅓ', posX = 8, posY = 3 });
            _keys.Add(new CKey() { scanCode = 37, key = 'k', keyShift = 'K', keyLocal = 'ㅏ', posX = 9, posY = 3 });
            _keys.Add(new CKey() { scanCode = 38, key = 'l', keyShift = 'L', keyLocal = 'ㅣ', posX = 10, posY = 3 });
            _keys.Add(new CKey() { scanCode = 39, key = ';', keyShift = ':', posX = 11, posY = 3 });
            _keys.Add(new CKey() { scanCode = 40, key = '\'', keyShift = '"', posX = 12, posY = 3 });
            _keys.Add(new CKey() { scanCode = 28, keyText = "Enter", posX = 13, posY = 3 });

            _keys.Add(new CKey() { scanCode = 42, keyText = "Shift", posX = 1, posY = 4 });
            _keys.Add(new CKey() { scanCode = 44, key = 'z', keyShift = 'Z', keyLocal = 'ㅋ', posX = 2, posY = 4 });
            _keys.Add(new CKey() { scanCode = 45, key = 'x', keyShift = 'X', keyLocal = 'ㅌ', posX = 3, posY = 4 });
            _keys.Add(new CKey() { scanCode = 46, key = 'c', keyShift = 'C', keyLocal = 'ㅊ', posX = 4, posY = 4 });
            _keys.Add(new CKey() { scanCode = 47, key = 'v', keyShift = 'V', keyLocal = 'ㅍ', posX = 5, posY = 4 });
            _keys.Add(new CKey() { scanCode = 48, key = 'b', keyShift = 'B', keyLocal = 'ㅠ', posX = 6, posY = 4 });
            _keys.Add(new CKey() { scanCode = 49, key = 'n', keyShift = 'N', keyLocal = 'ㅜ', posX = 7, posY = 4 });
            _keys.Add(new CKey() { scanCode = 50, key = 'm', keyShift = 'M', keyLocal = 'ㅡ', posX = 8, posY = 4 });
            _keys.Add(new CKey() { scanCode = 51, key = ',', keyShift = '<', posX = 9, posY = 4 });
            _keys.Add(new CKey() { scanCode = 52, key = '.', keyShift = '>', posX = 10, posY = 4 });
            _keys.Add(new CKey() { scanCode = 53, key = '/', keyShift = '?', posX = 11, posY = 4 });
            _keys.Add(new CKey() { scanCode = 54, keyText = "Shift", posX = 12, posY = 4 });

            _keys.Add(new CKey() { scanCode = 29, keyText = "Ctrl", posX = 1, posY = 5 });
            _keys.Add(new CKey() { scanCode = 56, keyText = "Alt", posX = 2, posY = 5 });
            _keys.Add(new CKey() { scanCode = 219, keyText = "Win", posX = 3, posY = 5 });
            _keys.Add(new CKey() { scanCode = 57, keyText = "Space", posX = 4, posY = 5 });
            _keys.Add(new CKey() { scanCode = 114, keyText = "한/영", posX = 5, posY = 5 });
            _keys.Add(new CKey() { scanCode = 184, keyText = "Alt", posX = 6, posY = 5 });
            _keys.Add(new CKey() { scanCode = 157, keyText = "Ctrl", posX = 7, posY = 5 });
        }

    }

}
