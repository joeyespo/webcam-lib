using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Runtime.InteropServices;

namespace CamTool
{
    /// <summary>
    /// Provides fields and methods for interfacing with Win32.
    /// </summary>
    internal static class Win32
    {
        // Start of USER messages
        internal const int WM_USER = 1024;

        // Defines start of the message range
        internal const int WM_CAP_START = WM_USER;

        // Start of unicode messages
        internal const int WM_CAP_UNICODE_START = WM_USER + 100;

        internal const int WM_CAP_GET_CAPSTREAMPTR = (WM_CAP_START + 1);

        internal const int WM_CAP_SET_CALLBACK_ERRORW = (WM_CAP_UNICODE_START + 2);
        internal const int WM_CAP_SET_CALLBACK_STATUSW = (WM_CAP_UNICODE_START + 3);
        internal const int WM_CAP_SET_CALLBACK_ERRORA = (WM_CAP_START + 2);
        internal const int WM_CAP_SET_CALLBACK_STATUSA = (WM_CAP_START + 3);
#if UNICODE
        internal const int WM_CAP_SET_CALLBACK_ERROR       = WM_CAP_SET_CALLBACK_ERRORW;
        internal const int WM_CAP_SET_CALLBACK_STATUS      = WM_CAP_SET_CALLBACK_STATUSW;
#else
        internal const int WM_CAP_SET_CALLBACK_ERROR = WM_CAP_SET_CALLBACK_ERRORA;
        internal const int WM_CAP_SET_CALLBACK_STATUS = WM_CAP_SET_CALLBACK_STATUSA;
#endif

        internal const int WM_CAP_SET_CALLBACK_YIELD = (WM_CAP_START + 4);
        internal const int WM_CAP_SET_CALLBACK_FRAME = (WM_CAP_START + 5);
        internal const int WM_CAP_SET_CALLBACK_VIDEOSTREAM = (WM_CAP_START + 6);
        internal const int WM_CAP_SET_CALLBACK_WAVESTREAM = (WM_CAP_START + 7);
        internal const int WM_CAP_GET_USER_DATA = (WM_CAP_START + 8);
        internal const int WM_CAP_SET_USER_DATA = (WM_CAP_START + 9);

        internal const int WM_CAP_DRIVER_CONNECT = (WM_CAP_START + 10);
        internal const int WM_CAP_DRIVER_DISCONNECT = (WM_CAP_START + 11);

        internal const int WM_CAP_DRIVER_GET_NAMEA = (WM_CAP_START + 12);
        internal const int WM_CAP_DRIVER_GET_VERSIONA = (WM_CAP_START + 13);
        internal const int WM_CAP_DRIVER_GET_NAMEW = (WM_CAP_UNICODE_START + 12);
        internal const int WM_CAP_DRIVER_GET_VERSIONW = (WM_CAP_UNICODE_START + 13);
#if UNICODE
        internal const int WM_CAP_DRIVER_GET_NAME          = WM_CAP_DRIVER_GET_NAMEW;
        internal const int WM_CAP_DRIVER_GET_VERSION       = WM_CAP_DRIVER_GET_VERSIONW;
#else
        internal const int WM_CAP_DRIVER_GET_NAME = WM_CAP_DRIVER_GET_NAMEA;
        internal const int WM_CAP_DRIVER_GET_VERSION = WM_CAP_DRIVER_GET_VERSIONA;
#endif

        internal const int WM_CAP_DRIVER_GET_CAPS = (WM_CAP_START + 14);

        internal const int WM_CAP_FILE_SET_CAPTURE_FILEA = (WM_CAP_START + 20);
        internal const int WM_CAP_FILE_GET_CAPTURE_FILEA = (WM_CAP_START + 21);
        internal const int WM_CAP_FILE_SAVEASA = (WM_CAP_START + 23);
        internal const int WM_CAP_FILE_SAVEDIBA = (WM_CAP_START + 25);
        internal const int WM_CAP_FILE_SET_CAPTURE_FILEW = (WM_CAP_UNICODE_START + 20);
        internal const int WM_CAP_FILE_GET_CAPTURE_FILEW = (WM_CAP_UNICODE_START + 21);
        internal const int WM_CAP_FILE_SAVEASW = (WM_CAP_UNICODE_START + 23);
        internal const int WM_CAP_FILE_SAVEDIBW = (WM_CAP_UNICODE_START + 25);
#if UNICODE
        internal const int WM_CAP_FILE_SET_CAPTURE_FILE    = WM_CAP_FILE_SET_CAPTURE_FILEW;
        internal const int WM_CAP_FILE_GET_CAPTURE_FILE    = WM_CAP_FILE_GET_CAPTURE_FILEW;
        internal const int WM_CAP_FILE_SAVEAS              = WM_CAP_FILE_SAVEASW;
        internal const int WM_CAP_FILE_SAVEDIB             = WM_CAP_FILE_SAVEDIBW;
#else
        internal const int WM_CAP_FILE_SET_CAPTURE_FILE = WM_CAP_FILE_SET_CAPTURE_FILEA;
        internal const int WM_CAP_FILE_GET_CAPTURE_FILE = WM_CAP_FILE_GET_CAPTURE_FILEA;
        internal const int WM_CAP_FILE_SAVEAS = WM_CAP_FILE_SAVEASA;
        internal const int WM_CAP_FILE_SAVEDIB = WM_CAP_FILE_SAVEDIBA;
#endif

        // Out of order to save on ifdefs
        internal const int WM_CAP_FILE_ALLOCATE = (WM_CAP_START + 22);
        internal const int WM_CAP_FILE_SET_INFOCHUNK = (WM_CAP_START + 24);

        internal const int WM_CAP_EDIT_COPY = (WM_CAP_START + 30);

        internal const int WM_CAP_SET_AUDIOFORMAT = (WM_CAP_START + 35);
        internal const int WM_CAP_GET_AUDIOFORMAT = (WM_CAP_START + 36);

        internal const int WM_CAP_DLG_VIDEOFORMAT = (WM_CAP_START + 41);
        internal const int WM_CAP_DLG_VIDEOSOURCE = (WM_CAP_START + 42);
        internal const int WM_CAP_DLG_VIDEODISPLAY = (WM_CAP_START + 43);
        internal const int WM_CAP_GET_VIDEOFORMAT = (WM_CAP_START + 44);
        internal const int WM_CAP_SET_VIDEOFORMAT = (WM_CAP_START + 45);
        internal const int WM_CAP_DLG_VIDEOCOMPRESSION = (WM_CAP_START + 46);

        internal const int WM_CAP_SET_PREVIEW = (WM_CAP_START + 50);
        internal const int WM_CAP_SET_OVERLAY = (WM_CAP_START + 51);
        internal const int WM_CAP_SET_PREVIEWRATE = (WM_CAP_START + 52);
        internal const int WM_CAP_SET_SCALE = (WM_CAP_START + 53);
        internal const int WM_CAP_GET_STATUS = (WM_CAP_START + 54);
        internal const int WM_CAP_SET_SCROLL = (WM_CAP_START + 55);

        internal const int WM_CAP_GRAB_FRAME = (WM_CAP_START + 60);
        internal const int WM_CAP_GRAB_FRAME_NOSTOP = (WM_CAP_START + 61);

        internal const int WM_CAP_SEQUENCE = (WM_CAP_START + 62);
        internal const int WM_CAP_SEQUENCE_NOFILE = (WM_CAP_START + 63);
        internal const int WM_CAP_SET_SEQUENCE_SETUP = (WM_CAP_START + 64);
        internal const int WM_CAP_GET_SEQUENCE_SETUP = (WM_CAP_START + 65);

        internal const int WM_CAP_SET_MCI_DEVICEA = (WM_CAP_START + 66);
        internal const int WM_CAP_GET_MCI_DEVICEA = (WM_CAP_START + 67);
        internal const int WM_CAP_SET_MCI_DEVICEW = (WM_CAP_UNICODE_START + 66);
        internal const int WM_CAP_GET_MCI_DEVICEW = (WM_CAP_UNICODE_START + 67);
#if UNICODE
        internal const int WM_CAP_SET_MCI_DEVICE           = WM_CAP_SET_MCI_DEVICEW;
        internal const int WM_CAP_GET_MCI_DEVICE           = WM_CAP_GET_MCI_DEVICEW;
#else
        internal const int WM_CAP_SET_MCI_DEVICE = WM_CAP_SET_MCI_DEVICEA;
        internal const int WM_CAP_GET_MCI_DEVICE = WM_CAP_GET_MCI_DEVICEA;
#endif

        internal const int WM_CAP_STOP = (WM_CAP_START + 68);
        internal const int WM_CAP_ABORT = (WM_CAP_START + 69);

        internal const int WM_CAP_SINGLE_FRAME_OPEN = (WM_CAP_START + 70);
        internal const int WM_CAP_SINGLE_FRAME_CLOSE = (WM_CAP_START + 71);
        internal const int WM_CAP_SINGLE_FRAME = (WM_CAP_START + 72);

        internal const int WM_CAP_PAL_OPENA = (WM_CAP_START + 80);
        internal const int WM_CAP_PAL_SAVEA = (WM_CAP_START + 81);
        internal const int WM_CAP_PAL_OPENW = (WM_CAP_UNICODE_START + 80);
        internal const int WM_CAP_PAL_SAVEW = (WM_CAP_UNICODE_START + 81);
#if UNICODE
        internal const int WM_CAP_PAL_OPEN                 = WM_CAP_PAL_OPENW;
        internal const int WM_CAP_PAL_SAVE                 = WM_CAP_PAL_SAVEW;
#else
        internal const int WM_CAP_PAL_OPEN = WM_CAP_PAL_OPENA;
        internal const int WM_CAP_PAL_SAVE = WM_CAP_PAL_SAVEA;
#endif

        internal const int WM_CAP_PAL_PASTE = (WM_CAP_START + 82);
        internal const int WM_CAP_PAL_AUTOCREATE = (WM_CAP_START + 83);
        internal const int WM_CAP_PAL_MANUALCREATE = (WM_CAP_START + 84);

        // Following added post VFW 1.1
        internal const int WM_CAP_SET_CALLBACK_CAPCONTROL = (WM_CAP_START + 85);

        // Defines end of the message range
        internal const int WM_CAP_UNICODE_END = WM_CAP_PAL_SAVEW;
        internal const int WM_CAP_END = WM_CAP_UNICODE_END;

        [StructLayout(LayoutKind.Sequential)]
        internal struct VideoHeader
        {
            public IntPtr lpData;
            public int dwBufferLength;
            public int dwBytesUsed;
            public uint dwTimeCaptured;
            public uint dwUser;
            public uint dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CapDriverCaps
        {
            public uint wDeviceIndex;
            public int fHasOverlay;
            public int fHasDlgVideoSource;
            public int fHasDlgVideoFormat;
            public int fHasDlgVideoDisplay;
            public int fCaptureInitialized;
            public int fDriverSuppliesPalettes;
            public IntPtr hVideoIn;
            public IntPtr hVideoOut;
            public IntPtr hVideoExtIn;
            public IntPtr hVideoExtOut;
        }

        internal delegate int FrameCallback(IntPtr hWnd, ref VideoHeader VideoHeader);

        [DllImport("user32", EntryPoint = "DestroyWindow")]
        internal static extern int DestroyWindow(IntPtr hWnd);

        [DllImport("user32", EntryPoint = "SendMessage")]
        internal static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32", EntryPoint = "SendMessage")]
        internal static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, FrameCallback fpProc);

        [DllImport("user32", EntryPoint = "SendMessage")]
        internal static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, ref CapDriverCaps caps);

        [DllImport("user32", EntryPoint = "SendMessage")]
        internal static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr ptr);

        [DllImport("avicap32.dll", EntryPoint = "capCreateCaptureWindowA", CharSet = CharSet.Ansi)]
        internal static extern IntPtr CapCreateCaptureWindow(string lpszWindowName, int dwStyle, int X, int Y, int nWidth, int nHeight, int hwndParent, int nID);

        [DllImport("user32", EntryPoint = "OpenClipboard")]
        internal static extern int OpenClipboard(IntPtr hWnd);

        [DllImport("user32", EntryPoint = "EmptyClipboard")]
        internal static extern int EmptyClipboard();

        [DllImport("user32", EntryPoint = "CloseClipboard")]
        internal static extern int CloseClipboard();
    }
}
