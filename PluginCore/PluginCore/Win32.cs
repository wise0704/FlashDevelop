using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

using PluginCore.Helpers;

namespace PluginCore
{
    public class Win32
    {
        /// <summary>
        /// Checks if Win32 functionality should be used
        /// </summary>
        public static Boolean ShouldUseWin32()
        {
            return PlatformHelper.IsRunningOnWindows();
        }

        #region Enums

        public enum SHGFI
        {
            SmallIcon = 0x00000001,
            LargeIcon = 0x00000000,
            Icon = 0x00000100,
            DisplayName = 0x00000200,
            Typename = 0x00000400,
            SysIconIndex = 0x00004000,
            UseFileAttributes = 0x00000010,
            ShellIconSize = 0x4,
            AddOverlays = 0x000000020
        }

        public enum HitTest
        {
            HTCLIENT = 1,
            HTCAPTION = 2,
            HTSYSMENU = 3,
            HTGROWBOX = 4,
            HTMENU = 5,
            HTHSCROLL = 6,
            HTVSCROLL = 7,
            HTMINBUTTON = 8,
            HTMAXBUTTON = 9,
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17,
            HTBORDER = 18,
            HTOBJECT = 19,
            HTCLOSE = 20,
            HTHELP = 21
        }

        #endregion

        #region Structs

        public struct COPYDATASTRUCT
        {
            public Int32 dwData;
            public Int32 cbData;
            public IntPtr lpData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public SHFILEINFO(Boolean b)
            {
                hIcon = IntPtr.Zero;
                iIcon = 0;
                dwAttributes = 0;
                szDisplayName = "";
                szTypeName = "";
            }
            public IntPtr hIcon;
            public Int32 iIcon;
            public UInt32 dwAttributes;
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 260)]
            public String szDisplayName;
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 80)]
            public String szTypeName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class SCROLLINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(Win32.SCROLLINFO));
            public int fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public UIntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public POINT pt;

            public MSG(Message msg)
            {
                hwnd = msg.HWnd;
                message = (uint) msg.Msg;
                unsafe { wParam = (UIntPtr) (void*) msg.WParam; }
                lParam = msg.LParam;
                time = default(uint);
                pt = default(POINT);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        #endregion

        #region Constants

        public const Int32 SB_HORZ = 0;
        public const Int32 SB_VERT = 1;
        public const Int32 SB_BOTH = 3;
        public const Int32 SB_THUMBPOSITION = 4;
        public const Int32 SB_THUMBTRACK = 5;
        public const Int32 SB_LEFT = 6;
        public const Int32 SB_RIGHT = 7;
        public const Int32 WM_HSCROLL = 0x0114;
        public const Int32 WM_VSCROLL = 0x0115;
        public const UInt32 LVM_SCROLL = 0x1014;
        public const UInt32 SWP_SHOWWINDOW = 64;
        public const Int32 SW_RESTORE = 9;
        public const Int32 WM_SETREDRAW = 0xB;
        public const Int32 WM_PRINTCLIENT = 0x0318;
        public const Int32 PRF_CLIENT = 0x00000004;
        public const Int32 TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;
        public const Int32 TVS_EX_DOUBLEBUFFER = 0x0004;
        public const Int32 TV_FIRST = 0x1100;
        public const Int32 WM_NCLBUTTONDOWN = 0x00A1;
        public const Int32 WM_LBUTTONDOWN = 0x0201;
        public const Int32 WM_LBUTTONUP = 0x0202;
        public const Int32 WM_RBUTTONDOWN = 0x0204;
        public const Int32 WM_MBUTTONDOWN = 0x0207;
        public const Int32 VK_SHIFT = 0x10;
        public const Int32 VK_CONTROL = 0x11;
        public const Int32 VK_MENU = 0x12;
        public const Int32 VK_ESCAPE = 0x1B;
        public const Int32 VK_LSHIFT = 0xA0;
        public const Int32 VK_RSHIFT = 0xA1;
        public const Int32 VK_LCONTROL = 0xA2;
        public const Int32 VK_RCONTROL = 0xA3;
        public const Int32 VK_LMENU = 0xA4;
        public const Int32 VK_RMENU = 0xA5;
        public const Int32 WM_COPYDATA = 74;
        public const Int32 WM_MOUSEWHEEL = 0x20A;
        public const Int32 WM_KEYFIRST = 0x0100;
        public const Int32 WM_KEYDOWN = 0x0100;
        public const Int32 WM_KEYUP = 0x0101;
        public const Int32 WM_CHAR = 0x0102;
        public const Int32 WM_DEADCHAR = 0x0103;
        public const Int32 WM_SYSKEYDOWN = 0x104;
        public const Int32 WM_SYSKEYUP = 0x0105;
        public const Int32 WM_SYSCHAR = 0x0106;
        public const Int32 WM_SYSDEADCHAR = 0x0107;
        public const Int32 WM_KEYLAST = 0x0108;
        public const Int32 SIF_RANGE = 0x0001;
        public const Int32 SIF_PAGE = 0x0002;
        public const Int32 SIF_POS = 0x0004;
        public const Int32 SIF_DISABLENOSCROLL = 0x0008;
        public const Int32 SIF_TRACKPOS = 0x0010;
        public const Int32 SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS);
        public const Int32 GA_ROOT = 2;
        public const Int32 PM_NOREMOVE = 0x0000;

        #endregion

        #region DllImports

        [DllImport("user32.dll")]
        public static extern Boolean IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        [DllImport("user32.dll")]
        public static extern void SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, Int32 x, Int32 y, Int32 width, Int32 height, UInt32 flags);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, Int32 msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern Int32 SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll")]
        public static extern Boolean ShowScrollBar(IntPtr hWnd, Int32 wBar, Boolean bShow);

        [DllImport("user32.dll")]
        public static extern Int32 GetScrollPos(IntPtr hWnd, Int32 nBar);

        [DllImport("user32.dll")]
        public static extern Int32 SetScrollPos(IntPtr hWnd, Int32 nBar, Int32 nPos, Boolean bRedraw);

        [DllImport("user32.dll")]
        public static extern Boolean GetScrollInfo(IntPtr hWnd, Int32 fnBar, SCROLLINFO scrollInfo);

        [DllImport("user32.dll")]
        public static extern Boolean ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("user32.dll")]
        public static extern UInt32 SetForegroundWindow(IntPtr hwnd);

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern Boolean PathCompactPathEx([MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszOut, [MarshalAs(UnmanagedType.LPTStr)] String pszSource, [MarshalAs(UnmanagedType.U4)] Int32 cchMax, [MarshalAs(UnmanagedType.U4)] Int32 dwReserved);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetLongPathName([MarshalAs(UnmanagedType.LPTStr)] String path, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder longPath, Int32 longPathLength);
        
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetShortPathName(String lpszLongPath, StringBuilder lpszShortPath, Int32 cchBuffer);

        [DllImport("shell32.dll", EntryPoint = "#28")]
        public static extern UInt32 SHILCreateFromPath([MarshalAs(UnmanagedType.LPWStr)] String pszPath, out IntPtr ppidl, ref Int32 rgflnOut);

        [DllImport("shell32.dll", EntryPoint = "SHGetPathFromIDListW")]
        public static extern Boolean SHGetPathFromIDList(IntPtr pidl, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszPath);

        [DllImport("user32.dll")]
        public static extern Int32 DestroyIcon(IntPtr hIcon);

        [DllImport("kernel32.dll")]
        public static extern Int32 GetModuleHandle(String lpModuleName);

        [DllImport("shell32.dll")]
        public static extern IntPtr ExtractIcon(Int32 hInst, String FileName, Int32 nIconIndex);

        [DllImport("shell32.dll")]
        public static extern Int32 SHGetFileInfo(String pszPath, UInt32 dwFileAttributes, out SHFILEINFO psfi, UInt32 cbfileInfo, SHGFI uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetAncestor([In] HandleRef hwnd, [In] uint flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PeekMessage([Out] out MSG lpMsg, [In, Optional] IntPtr hWnd, [In] uint wMsgFilterMin, [In] uint wMsgFilterMax, [In] uint wRemoveMsg);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMessageW([In, Optional] IntPtr hWnd, [In] uint Msg, [In] UIntPtr wParam, [In] IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ToUnicode([In] uint wVirtKey, [In] uint wScanCode, [In, Optional, MarshalAs(UnmanagedType.LPArray)] byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, [In] int cchBuff, [In] uint wFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetKeyboardState([Out, MarshalAs(UnmanagedType.LPArray)] byte[] lpKeyState);

        #endregion

        #region Window

        /// <summary>
        /// Sets the window specified by handle to fullscreen
        /// </summary>
        public static void SetWinFullScreen(IntPtr hwnd)
        {
            Screen screen = Screen.FromHandle(hwnd);
            Int32 screenTop = screen.WorkingArea.Top;
            Int32 screenLeft = screen.WorkingArea.Left;
            Int32 screenWidth = screen.WorkingArea.Width;
            Int32 screenHeight = screen.WorkingArea.Height;
            SetWindowPos(hwnd, IntPtr.Zero, screenLeft, screenTop, screenWidth, screenHeight, SWP_SHOWWINDOW);
        }

        /// <summary>
        /// Restores the window with Win32
        /// </summary>
        public static void RestoreWindow(IntPtr handle)
        {
            if (IsIconic(handle)) ShowWindow(handle, SW_RESTORE);
        }

        #endregion

        #region Scrolling

        /// <summary>
        /// 
        /// </summary>
        public static SCROLLINFO GetFullScrollInfo(Control lv, Boolean horizontalBar)
        {
            Int32 fnBar = (horizontalBar ? SB_HORZ : SB_VERT);
            SCROLLINFO scrollInfo = new SCROLLINFO();
            scrollInfo.fMask = SIF_ALL;
            if (GetScrollInfo(lv.Handle, fnBar, scrollInfo)) return scrollInfo;
            else return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Point GetScrollPos(Control ctrl)
        {
            return new Point(GetScrollPos(ctrl.Handle, SB_HORZ), GetScrollPos(ctrl.Handle, SB_VERT));
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SetScrollPos(Control ctrl, Point scrollPosition)
        {
            SetScrollPos(ctrl.Handle, SB_HORZ, scrollPosition.X, true);
            SetScrollPos(ctrl.Handle, SB_VERT, scrollPosition.Y, true);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ScrollToLeft(Control ctrl)
        {
            SendMessage(ctrl.Handle, WM_HSCROLL, SB_LEFT, 0);
        }

        #endregion

    }

}

