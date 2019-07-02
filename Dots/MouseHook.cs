using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Dots
{
    public class MouseHook
    {
        #region delegate

        public static event EventHandler MouseLeftButtonDownAction = delegate { };
        public static event EventHandler MouseLeftButtonUpAction = delegate { };
        public static event EventHandler MouseRightButtonDownAction = delegate { };
        public static event EventHandler MouseRightButtonUpAction = delegate { };
        public static event EventHandler MouseMoveAction = delegate { };
        public static event EventHandler MouseWheelAction = delegate { };

        #endregion

        public static void Start()
        {
            _hookID = SetHook(_proc);
        }

        public static void stop()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private static LowLevelMouseProc _proc = HookCallback;

        private static IntPtr _hookID = IntPtr.Zero;

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                  GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 )
            {
                if (MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
                {
                    MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                    MouseLeftButtonDownAction(hookStruct, new EventArgs());
                }
                //switch ((MouseMessages)wParam)
                //{
                //    case MouseMessages.WM_LBUTTONDOWN:
                //        MouseLeftButtonDownAction(hookStruct, new EventArgs());
                //        break;
                //    case MouseMessages.WM_LBUTTONUP: 
                //        MouseLeftButtonUpAction(hookStruct, new EventArgs());
                //        break;
                //    case MouseMessages.WM_RBUTTONDOWN:
                //        MouseRightButtonDownAction(hookStruct, new EventArgs());
                //        break;
                //    case MouseMessages.WM_RBUTTONUP:
                //        MouseRightButtonUpAction(hookStruct, new EventArgs());
                //        break;
                //    case MouseMessages.WM_MOUSEMOVE:
                //        MouseMoveAction(hookStruct, new EventArgs());
                //        break;
                //    case MouseMessages.WM_MOUSEWHEEL:
                //        MouseWheelAction(hookStruct, new EventArgs());
                //        break;
                //    default: break;
                //}
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        #region 參數

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        #endregion

        #region DllImport

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
          LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
          IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion
    }
}
