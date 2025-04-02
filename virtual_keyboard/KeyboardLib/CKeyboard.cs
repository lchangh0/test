using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardLib
{

    public class CKey
    {
        public ushort scanCode;

        public string keyText;
        public char keyChar;
        public char keyShiftChar;
        public char keyLocalChar;
        public char keyLocalShiftChar;

        public int columnIdx;
        public int rowIdx;
        public int columnSpan;
        public int rowSpan;

        public CKey()
        {
            keyText = "";
            columnSpan = 1;
            rowSpan = 1;
        }

        public string GetKeyText(bool bShift, bool bLocalLang, bool bCapsLock)
        {
            string strText = null;

            if (bLocalLang)
            {
                if (bShift)
                {
                    if (keyLocalShiftChar != 0)
                        strText = keyLocalShiftChar.ToString();
                }
                else
                {
                    if (keyLocalChar != 0)
                        strText = keyLocalChar.ToString();
                }
            }
            else
            {
                if (bShift)
                {
                    if (bCapsLock && IsAlphabet() && keyChar != 0)
                        strText = keyChar.ToString();
                    else if (keyShiftChar != 0)
                        strText = keyShiftChar.ToString();
                }
                else
                {
                    if (bCapsLock && IsAlphabet() && keyShiftChar != 0)
                        strText = keyShiftChar.ToString();
                    else if (keyChar != 0)
                        strText = keyChar.ToString();
                }
            }

            if (strText == null && keyText != null)
                strText = keyText;

            return strText;
        }


        public bool IsAlphabet()
        {
            return char.IsAsciiLetter(keyChar);
        }

        public bool HasShiftKey()
        {
            return keyShiftChar != 0;
        }

    }


    public class CKeyboard
    {
        List<CKey> _keys;

        public CKeyboard()
        {
            _keys = new List<CKey>();
        }

        public CKey GetKey(int columnIdx, int rowIdx)
        {
            foreach (CKey key in _keys)
            {
                if (key.columnIdx == columnIdx && key.rowIdx == rowIdx)
                {
                    return key;
                }
            }

            return null;
        }


        public int GetKeyboardColumns()
        {
            int iMax = -1;

            foreach (CKey key in _keys)
            {
                if (key.columnIdx > iMax)
                    iMax = key.columnIdx;
            }

            return iMax + 1;
        }

        public int GetKeyboardRows()
        {
            int iMax = -1;

            foreach (CKey key in _keys)
            {
                if (key.rowIdx > iMax)
                    iMax = key.rowIdx;
            }

            return iMax + 1;
        }

        bool _bShift = false;

        public bool GetShift()
        {
            return _bShift;
        }

        public void SetShift(bool bShift)
        {
            _bShift = bShift;
        }

        public void ToggleShift()
        {
            _bShift = !_bShift;
        }


        bool _bLocalLanguage = false;

        public bool GetLocalLanguage()
        {
            return _bLocalLanguage;
        }

        public void SetLocalLanguage(bool bLocal)
        {
            _bLocalLanguage = bLocal;
        }

        public void ToggleLocalLanguage()
        {
            _bLocalLanguage = !_bLocalLanguage;
        }

        bool _bCapsLock = false;

        public bool GetCapsLock()
        {
            return _bCapsLock;
        }

        public void SetCapsLock(bool bCapsLock)
        {
            _bCapsLock = bCapsLock;
        }

        public void ToggleCapsLock()
        {
            _bCapsLock = !_bCapsLock;
        }


        public string GetKeyText(CKey key)
        {
            return key.GetKeyText(_bShift, _bLocalLanguage, _bCapsLock);
        }

        public bool IsShiftKey(CKey key)
        {
            return (key?.scanCode == 42 || key?.scanCode == 54);
        }

        public bool IsCapsLockKey(CKey key)
        {
            return key?.scanCode == 58;
        }

        public bool IsLocalLanguageKey(CKey key)
        {
            return (key?.scanCode == 114);
        }


        public enum EScanCode { 
            Num1 = 2, Num2 = 3, Num3 = 4, Num4 = 5, Num5 = 6, Num6 = 7, Num7 = 8, Num8 = 9, 
            Num9 = 10, Num0 = 11, Minus = 12, Equal = 13, BackSpace = 14,
            Tab = 15, Q = 16, W = 17, E = 18, R = 19, 
            T = 20, Y = 21, U = 22, I = 23, O = 24, P = 25, 
            LeftSquareBracket = 26, RightSquareBracket = 27, Enter = 28, CtrlLeft = 29,
            A = 30, S = 31, D = 32, F = 33, G = 34, H = 35, J = 36, K = 37, L = 38, 
            SemiColon = 39, 
            SingleQuote = 40, 
            Backtick = 41, // `
            ShiftLeft= 42, 
            BackSlash = 43,
            Z = 44, X = 45, C = 46, V = 47, B = 48, N = 49, 
            M = 50, Comma = 51, Dot = 52, Slash = 53, ShiftRight = 54,
            AltLeft = 56, Space = 57, CapsLock = 58,
            KorEng = 114,
            CtrlRight = 157,
            AltRight = 184,
            Windows = 219,
        };


        public void MakeKorKeys()
        {
            _keys.Clear();

            _keys.Add(new CKey() { scanCode = 41, keyChar = '`', keyShiftChar = '~', columnIdx = 0, rowIdx = 0 });
            _keys.Add(new CKey() { scanCode = 2, keyChar = '1', keyShiftChar = '!', columnIdx = 1, rowIdx = 0 });
            _keys.Add(new CKey() { scanCode = 3, keyChar = '2', keyShiftChar = '@', columnIdx = 2, rowIdx = 0 });
            _keys.Add(new CKey() { scanCode = 4, keyChar = '3', keyShiftChar = '#', columnIdx = 3, rowIdx = 0 });
            _keys.Add(new CKey() { scanCode = 5, keyChar = '4', keyShiftChar = '$', columnIdx = 4, rowIdx = 0 });
            _keys.Add(new CKey() { scanCode = 6, keyChar = '5', keyShiftChar = '%', columnIdx = 5, rowIdx = 0 });
            _keys.Add(new CKey() { scanCode = 7, keyChar = '6', keyShiftChar = '^', columnIdx = 6, rowIdx = 0 });
            _keys.Add(new CKey() { scanCode = 8, keyChar = '7', keyShiftChar = '&', columnIdx = 7, rowIdx = 0 });
            _keys.Add(new CKey() { scanCode = 9, keyChar = '8', keyShiftChar = '*', columnIdx = 8, rowIdx = 0 });
            _keys.Add(new CKey() { scanCode = 10, keyChar = '9', keyShiftChar = '(', columnIdx = 9, rowIdx = 0 });
            _keys.Add(new CKey() { scanCode = 11, keyChar = '0', keyShiftChar = ')', columnIdx = 10, rowIdx = 0 });
            _keys.Add(new CKey() { scanCode = 12, keyChar = '-', keyShiftChar = '_', columnIdx = 11, rowIdx = 0 });
            _keys.Add(new CKey() { scanCode = 13, keyChar = '=', keyShiftChar = '+', columnIdx = 12, rowIdx = 0 });
            _keys.Add(new CKey() { scanCode = 14, keyText = "BackS", columnIdx = 13, rowIdx = 0 }); // 백스페이스

            _keys.Add(new CKey() { scanCode = 15, keyText = "Tab", columnIdx = 0, rowIdx = 1 });
            _keys.Add(new CKey() { scanCode = 16, keyChar = 'q', keyShiftChar = 'Q', keyLocalChar = 'ㅂ', keyLocalShiftChar = 'ㅃ', columnIdx = 1, rowIdx = 1 });
            _keys.Add(new CKey() { scanCode = 17, keyChar = 'w', keyShiftChar = 'W', keyLocalChar = 'ㅈ', keyLocalShiftChar = 'ㅉ', columnIdx = 2, rowIdx = 1 });
            _keys.Add(new CKey() { scanCode = 18, keyChar = 'e', keyShiftChar = 'E', keyLocalChar = 'ㄷ', keyLocalShiftChar = 'ㄸ', columnIdx = 3, rowIdx = 1 });
            _keys.Add(new CKey() { scanCode = 19, keyChar = 'r', keyShiftChar = 'R', keyLocalChar = 'ㄱ', keyLocalShiftChar = 'ㄲ', columnIdx = 4, rowIdx = 1 });
            _keys.Add(new CKey() { scanCode = 20, keyChar = 't', keyShiftChar = 'T', keyLocalChar = 'ㅅ', keyLocalShiftChar = 'ㅆ', columnIdx = 5, rowIdx = 1 });
            _keys.Add(new CKey() { scanCode = 21, keyChar = 'y', keyShiftChar = 'Y', keyLocalChar = 'ㅛ', columnIdx = 6, rowIdx = 1 });
            _keys.Add(new CKey() { scanCode = 22, keyChar = 'u', keyShiftChar = 'U', keyLocalChar = 'ㅕ', columnIdx = 7, rowIdx = 1 });
            _keys.Add(new CKey() { scanCode = 23, keyChar = 'i', keyShiftChar = 'I', keyLocalChar = 'ㅑ', columnIdx = 8, rowIdx = 1 });
            _keys.Add(new CKey() { scanCode = 24, keyChar = 'o', keyShiftChar = 'O', keyLocalChar = 'ㅐ', keyLocalShiftChar = 'ㅒ', columnIdx = 9, rowIdx = 1 });
            _keys.Add(new CKey() { scanCode = 25, keyChar = 'p', keyShiftChar = 'P', keyLocalChar = 'ㅔ', keyLocalShiftChar = 'ㅖ', columnIdx = 10, rowIdx = 1 });
            _keys.Add(new CKey() { scanCode = 26, keyChar = '[', keyShiftChar = '{', columnIdx = 11, rowIdx = 1 });
            _keys.Add(new CKey() { scanCode = 27, keyChar = ']', keyShiftChar = '}', columnIdx = 12, rowIdx = 1 });
            _keys.Add(new CKey() { scanCode = 43, keyChar = '\\', keyShiftChar = '|', columnIdx = 13, rowIdx = 1 });

            _keys.Add(new CKey() { scanCode = 58, keyText = "Caps", columnIdx = 0, rowIdx = 2 });
            _keys.Add(new CKey() { scanCode = 30, keyChar = 'a', keyShiftChar = 'A', keyLocalChar = 'ㅁ', columnIdx = 1, rowIdx = 2 });
            _keys.Add(new CKey() { scanCode = 31, keyChar = 's', keyShiftChar = 'S', keyLocalChar = 'ㄴ', columnIdx = 2, rowIdx = 2 });
            _keys.Add(new CKey() { scanCode = 32, keyChar = 'd', keyShiftChar = 'D', keyLocalChar = 'ㅇ', columnIdx = 3, rowIdx = 2 });
            _keys.Add(new CKey() { scanCode = 33, keyChar = 'f', keyShiftChar = 'F', keyLocalChar = 'ㄹ', columnIdx = 4, rowIdx = 2 });
            _keys.Add(new CKey() { scanCode = 34, keyChar = 'g', keyShiftChar = 'G', keyLocalChar = 'ㅎ', columnIdx = 5, rowIdx = 2 });
            _keys.Add(new CKey() { scanCode = 35, keyChar = 'h', keyShiftChar = 'H', keyLocalChar = 'ㅗ', columnIdx = 6, rowIdx = 2 });
            _keys.Add(new CKey() { scanCode = 36, keyChar = 'j', keyShiftChar = 'J', keyLocalChar = 'ㅓ', columnIdx = 7, rowIdx = 2 });
            _keys.Add(new CKey() { scanCode = 37, keyChar = 'k', keyShiftChar = 'K', keyLocalChar = 'ㅏ', columnIdx = 8, rowIdx = 2 });
            _keys.Add(new CKey() { scanCode = 38, keyChar = 'l', keyShiftChar = 'L', keyLocalChar = 'ㅣ', columnIdx = 9, rowIdx = 2 });
            _keys.Add(new CKey() { scanCode = 39, keyChar = ';', keyShiftChar = ':', columnIdx = 10, rowIdx = 2 });
            _keys.Add(new CKey() { scanCode = 40, keyChar = '\'', keyShiftChar = '"', columnIdx = 11, rowIdx = 2 });
            _keys.Add(new CKey() { scanCode = 28, keyText = "Enter", columnIdx = 12, rowIdx = 2, columnSpan = 2 });

            _keys.Add(new CKey() { scanCode = 42, keyText = "Shift", columnIdx = 0, rowIdx = 3 });
            _keys.Add(new CKey() { scanCode = 44, keyChar = 'z', keyShiftChar = 'Z', keyLocalChar = 'ㅋ', columnIdx = 1, rowIdx = 3 });
            _keys.Add(new CKey() { scanCode = 45, keyChar = 'x', keyShiftChar = 'X', keyLocalChar = 'ㅌ', columnIdx = 2, rowIdx = 3 });
            _keys.Add(new CKey() { scanCode = 46, keyChar = 'c', keyShiftChar = 'C', keyLocalChar = 'ㅊ', columnIdx = 3, rowIdx = 3 });
            _keys.Add(new CKey() { scanCode = 47, keyChar = 'v', keyShiftChar = 'V', keyLocalChar = 'ㅍ', columnIdx = 4, rowIdx = 3 });
            _keys.Add(new CKey() { scanCode = 48, keyChar = 'b', keyShiftChar = 'B', keyLocalChar = 'ㅠ', columnIdx = 5, rowIdx = 3 });
            _keys.Add(new CKey() { scanCode = 49, keyChar = 'n', keyShiftChar = 'N', keyLocalChar = 'ㅜ', columnIdx = 6, rowIdx = 3 });
            _keys.Add(new CKey() { scanCode = 50, keyChar = 'm', keyShiftChar = 'M', keyLocalChar = 'ㅡ', columnIdx = 7, rowIdx = 3 });
            _keys.Add(new CKey() { scanCode = 51, keyChar = ',', keyShiftChar = '<', columnIdx = 8, rowIdx = 3 });
            _keys.Add(new CKey() { scanCode = 52, keyChar = '.', keyShiftChar = '>', columnIdx = 9, rowIdx = 3 });
            _keys.Add(new CKey() { scanCode = 53, keyChar = '/', keyShiftChar = '?', columnIdx = 10, rowIdx = 3 });
            //_keys.Add(new CKey() { scanCode = 54, keyText = "Shift", posX = 12, posY = 4 });
            _keys.Add(new CKey() { scanCode = 57, keyText = "Space", columnIdx = 11, rowIdx = 3, columnSpan = 2 });
            _keys.Add(new CKey() { scanCode = 114, keyText = "한/영", columnIdx = 13, rowIdx = 3 });

            //_keys.Add(new CKey() { scanCode = 29, keyText = "Ctrl", posX = 1, posY = 5 });
            //_keys.Add(new CKey() { scanCode = 56, keyText = "Alt", posX = 2, posY = 5 });
            //_keys.Add(new CKey() { scanCode = 219, keyText = "Win", posX = 3, posY = 5 });
            //_keys.Add(new CKey() { scanCode = 57, keyText = "Space", posX = 4, posY = 5 });
            //_keys.Add(new CKey() { scanCode = 114, keyText = "한/영", posX = 5, posY = 5 });
            //_keys.Add(new CKey() { scanCode = 184, keyText = "Alt", posX = 6, posY = 5 });
            //_keys.Add(new CKey() { scanCode = 157, keyText = "Ctrl", posX = 7, posY = 5 });
        }

    }

}
