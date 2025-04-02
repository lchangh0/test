using System.Globalization;
using System.Runtime.InteropServices;
using static KeyboardLib.CWindowsApi;
using System.Text;

namespace KeyboardLib
{

    public static class CKeyboardLib
    {
        const ushort SCAN_SHIFT = 42;

        public static void SimulateKeyPress(ushort keyScanCode, bool bWithShift)
        {
            List<CWindowsApi.INPUT> listInput = new List<CWindowsApi.INPUT>();

            if (bWithShift)
            {
                listInput.Add(new CWindowsApi.INPUT
                {
                    type = 1, // INPUT_KEYBOARD
                    u = new CWindowsApi.InputUnion
                    {
                        ki = new CWindowsApi.KEYBDINPUT
                        {
                            wVk = 0,
                            wScan = SCAN_SHIFT,
                            dwFlags = 0x0008 // KEYEVENTF_SCANCODE
                        }
                    }
                });
            }

            listInput.Add(new CWindowsApi.INPUT
            {
                type = 1, // INPUT_KEYBOARD
                u = new CWindowsApi.InputUnion
                {
                    ki = new CWindowsApi.KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = keyScanCode,
                        dwFlags = 0x0008 // KEYEVENTF_SCANCODE
                    }
                }
            });

            listInput.Add(new CWindowsApi.INPUT
            {
                type = 1, // INPUT_KEYBOARD
                u = new CWindowsApi.InputUnion
                {
                    ki = new CWindowsApi.KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = keyScanCode,
                        dwFlags = 0x0008 | 0x0002 // KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP
                    }
                }
            });

            if (bWithShift)
            {
                listInput.Add(new CWindowsApi.INPUT
                {
                    type = 1, // INPUT_KEYBOARD
                    u = new CWindowsApi.InputUnion
                    {
                        ki = new CWindowsApi.KEYBDINPUT
                        {
                            wVk = 0,
                            wScan = SCAN_SHIFT,
                            dwFlags = 0x0008 | 0x0002 // KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP
                        }
                    }
                });
            }

            // 현재 입력포커스를 가진 윈도우로 키입력 메시지를 전송한다.
            CWindowsApi.SendInput((uint)listInput.Count, listInput.ToArray(), Marshal.SizeOf(typeof(CWindowsApi.INPUT)));
        }


        

        public static void SetWindowNoActivate(IntPtr hwnd)
        {
            int exStyle = CWindowsApi.GetWindowLong(hwnd, CWindowsApi.GWL_EXSTYLE);
            CWindowsApi.SetWindowLong(hwnd, CWindowsApi.GWL_EXSTYLE, exStyle | CWindowsApi.WS_EX_NOACTIVATE);
        }


        // 윈도우즈가 사용중인 IME를 구한다.
        // 주의) 한글IME를 사용해서 영문을 입력하고 있는지 한글을 입력하고 있는지는 구분하지 못한다.

        public static string GetCurrentKeyboardLanguage(
            out uint iImeConversion, out uint iImeSentence)
        {
            iImeConversion = 0;
            iImeSentence = 0;

            IntPtr foregroundWindow = CWindowsApi.GetForegroundWindow();
            uint threadId = CWindowsApi.GetWindowThreadProcessId(foregroundWindow, out uint processId);
            //iThreadId = threadId;

            IntPtr layout = CWindowsApi.GetKeyboardLayout(threadId);

            //CultureInfo culture = new CultureInfo((layout & 0xffff).ToInt32());
            //string strName = culture.DisplayName;

            if (((uint)layout & 0xFFFF) == 0x0412) // 한글IME
            {
                return "ko";
            }
            else if (((uint)layout & 0xFFFF) == 0x0409) // 영문IME
            {
                return "en";
            }
            else
                return "";
        }



        public const uint IME_CMODE_ALPHANUMERIC = 0x0000; // 영문모드
        public const uint IME_CMODE_NATIVE = 0x0001;    // 한글모드
        public const uint IME_CMODE_FULLSHAPE = 0x0008; // 전각문자모드

        public static string GetImeStatus(IntPtr hWnd,
            out uint iImeConversion, out uint iImeSentence)
        {
            iImeConversion = 0;
            iImeSentence = 0;
            string strStatus = "";

            IntPtr hIMC = CWindowsApi.ImmGetContext(hWnd);

            if (hIMC != IntPtr.Zero)
            {
                strStatus = "context";

                if (ImmGetConversionStatus(hIMC, out iImeConversion, out iImeSentence))
                {
                    if ((iImeConversion & IME_CMODE_NATIVE) != 0)
                        strStatus = "kor";
                    else if ((iImeConversion & IME_CMODE_ALPHANUMERIC) != 0)
                        strStatus = "eng";
                }

                ImmReleaseContext(hWnd, hIMC);
            }
            return strStatus;
        }



        public static IntPtr GetFocusedControlHandle(out string strInfo)
        {
            IntPtr handle = IntPtr.Zero;
            StringBuilder sbResult = new StringBuilder();

            IntPtr foregroundWindow = GetForegroundWindow();
            uint threadId = GetWindowThreadProcessId(foregroundWindow, out _);
            GUITHREADINFO guiInfo = new GUITHREADINFO { cbSize = Marshal.SizeOf(typeof(GUITHREADINFO)) };

            StringBuilder windowTitle = new StringBuilder(256);
            GetWindowText(foregroundWindow, windowTitle, windowTitle.Capacity);
            sbResult.AppendLine($"현재 활성 창: Handle={foregroundWindow}, Text={windowTitle}");

            if (GetGUIThreadInfo(threadId, ref guiInfo))
            {
                IntPtr focusedControl = guiInfo.hwndFocus;

                // 윈도우가 아니라 윈도우 안에 있는 컨트롤이면
                if (focusedControl != foregroundWindow)
                {
                    handle = focusedControl;
                }

                if (focusedControl != IntPtr.Zero)
                {
                    StringBuilder className = new StringBuilder(256);
                    GetClassName(focusedControl, className, className.Capacity);
                    sbResult.AppendLine($"포커스된 컨트롤 : Handle={focusedControl}, Class={className}");
                }
            }
            else
            {
                sbResult.AppendLine("GetGUIThreadInfo() failed");
                sbResult.AppendLine(CWindowsApi.GetLastErrorString());
            }

            strInfo = sbResult.ToString();
            return handle;
        }


        public static string GetControlText(IntPtr handleControl)
        {
            StringBuilder textBuffer = new StringBuilder(256);
            SendMessage(handleControl, WM_GETTEXT, (IntPtr)textBuffer.Capacity, textBuffer);
            return textBuffer.ToString();
        }
             

    }


    

}
