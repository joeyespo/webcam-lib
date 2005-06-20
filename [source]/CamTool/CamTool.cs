using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;


namespace Uberware.Tools
{

  #region Public Classes and Delegates
  
  public delegate void CamPreviewCallback (Image PreviewImage);
  
  [StructLayout(LayoutKind.Sequential)]
  public struct CamVideoFormat
  {
    public int   biSize; 
    public int   biWidth; 
    public int   biHeight; 
    public short biPlanes; 
    public short biBitCount; 
    public int   biCompression; 
    public int   biSizeImage; 
    public int   biXPelsPerMeter; 
    public int   biYPelsPerMeter; 
    public int   biClrUsed; 
    public int   biClrImportant; 
  }
  
  public class CamException : ApplicationException
  {
    public CamException (string message, Exception innerException) : base(message, innerException)
    {}
    public CamException (string message) : base(message)
    {}
    public CamException () : base()
    {}
  }
  
  #endregion
    
    public class CamTool : IDisposable
    {
    
    #region API Declarations
    
    #region API Constants
        
    const int WM_USER = 1024;
    
    // Defines start of the message range
    const int WM_CAP_START                    = WM_USER;

    // start of unicode messages
    const int WM_CAP_UNICODE_START            = WM_USER+100;

    const int WM_CAP_GET_CAPSTREAMPTR         = (WM_CAP_START+  1);

    const int WM_CAP_SET_CALLBACK_ERRORW      = (WM_CAP_UNICODE_START+  2);
    const int WM_CAP_SET_CALLBACK_STATUSW     = (WM_CAP_UNICODE_START+  3);
    const int WM_CAP_SET_CALLBACK_ERRORA      = (WM_CAP_START+  2);
    const int WM_CAP_SET_CALLBACK_STATUSA     = (WM_CAP_START+  3);
    #if UNICODE
    const int WM_CAP_SET_CALLBACK_ERROR       = WM_CAP_SET_CALLBACK_ERRORW;
    const int WM_CAP_SET_CALLBACK_STATUS      = WM_CAP_SET_CALLBACK_STATUSW;
    #else
    const int WM_CAP_SET_CALLBACK_ERROR       = WM_CAP_SET_CALLBACK_ERRORA;
    const int WM_CAP_SET_CALLBACK_STATUS      = WM_CAP_SET_CALLBACK_STATUSA;
    #endif

    const int WM_CAP_SET_CALLBACK_YIELD       = (WM_CAP_START+  4);
    const int WM_CAP_SET_CALLBACK_FRAME       = (WM_CAP_START+  5);
    const int WM_CAP_SET_CALLBACK_VIDEOSTREAM = (WM_CAP_START+  6);
    const int WM_CAP_SET_CALLBACK_WAVESTREAM  = (WM_CAP_START+  7);
    const int WM_CAP_GET_USER_DATA              = (WM_CAP_START+  8);
    const int WM_CAP_SET_USER_DATA              = (WM_CAP_START+  9);

    const int WM_CAP_DRIVER_CONNECT           = (WM_CAP_START+  10);
    const int WM_CAP_DRIVER_DISCONNECT        = (WM_CAP_START+  11);

    const int WM_CAP_DRIVER_GET_NAMEA         = (WM_CAP_START+  12);
    const int WM_CAP_DRIVER_GET_VERSIONA      = (WM_CAP_START+  13);
    const int WM_CAP_DRIVER_GET_NAMEW         = (WM_CAP_UNICODE_START+  12);
    const int WM_CAP_DRIVER_GET_VERSIONW      = (WM_CAP_UNICODE_START+  13);
    #if UNICODE
    const int WM_CAP_DRIVER_GET_NAME          = WM_CAP_DRIVER_GET_NAMEW;
    const int WM_CAP_DRIVER_GET_VERSION       = WM_CAP_DRIVER_GET_VERSIONW;
    #else
    const int WM_CAP_DRIVER_GET_NAME          = WM_CAP_DRIVER_GET_NAMEA;
    const int WM_CAP_DRIVER_GET_VERSION       = WM_CAP_DRIVER_GET_VERSIONA;
    #endif

    const int WM_CAP_DRIVER_GET_CAPS          = (WM_CAP_START+  14);

    const int WM_CAP_FILE_SET_CAPTURE_FILEA   = (WM_CAP_START+  20);
    const int WM_CAP_FILE_GET_CAPTURE_FILEA   = (WM_CAP_START+  21);
    const int WM_CAP_FILE_SAVEASA             = (WM_CAP_START+  23);
    const int WM_CAP_FILE_SAVEDIBA            = (WM_CAP_START+  25);
    const int WM_CAP_FILE_SET_CAPTURE_FILEW   = (WM_CAP_UNICODE_START+  20);
    const int WM_CAP_FILE_GET_CAPTURE_FILEW   = (WM_CAP_UNICODE_START+  21);
    const int WM_CAP_FILE_SAVEASW             = (WM_CAP_UNICODE_START+  23);
    const int WM_CAP_FILE_SAVEDIBW            = (WM_CAP_UNICODE_START+  25);
    #if UNICODE
    const int WM_CAP_FILE_SET_CAPTURE_FILE    = WM_CAP_FILE_SET_CAPTURE_FILEW;
    const int WM_CAP_FILE_GET_CAPTURE_FILE    = WM_CAP_FILE_GET_CAPTURE_FILEW;
    const int WM_CAP_FILE_SAVEAS              = WM_CAP_FILE_SAVEASW;
    const int WM_CAP_FILE_SAVEDIB             = WM_CAP_FILE_SAVEDIBW;
    #else
    const int WM_CAP_FILE_SET_CAPTURE_FILE    = WM_CAP_FILE_SET_CAPTURE_FILEA;
    const int WM_CAP_FILE_GET_CAPTURE_FILE    = WM_CAP_FILE_GET_CAPTURE_FILEA;
    const int WM_CAP_FILE_SAVEAS              = WM_CAP_FILE_SAVEASA;
    const int WM_CAP_FILE_SAVEDIB             = WM_CAP_FILE_SAVEDIBA;
    #endif

    // out of order to save on ifdefs
    const int WM_CAP_FILE_ALLOCATE            = (WM_CAP_START+  22);
    const int WM_CAP_FILE_SET_INFOCHUNK       = (WM_CAP_START+  24);

    const int WM_CAP_EDIT_COPY                = (WM_CAP_START+  30);

    const int WM_CAP_SET_AUDIOFORMAT          = (WM_CAP_START+  35);
    const int WM_CAP_GET_AUDIOFORMAT          = (WM_CAP_START+  36);

    const int WM_CAP_DLG_VIDEOFORMAT          = (WM_CAP_START+  41);
    const int WM_CAP_DLG_VIDEOSOURCE          = (WM_CAP_START+  42);
    const int WM_CAP_DLG_VIDEODISPLAY         = (WM_CAP_START+  43);
    const int WM_CAP_GET_VIDEOFORMAT          = (WM_CAP_START+  44);
    const int WM_CAP_SET_VIDEOFORMAT          = (WM_CAP_START+  45);
    const int WM_CAP_DLG_VIDEOCOMPRESSION     = (WM_CAP_START+  46);

    const int WM_CAP_SET_PREVIEW              = (WM_CAP_START+  50);
    const int WM_CAP_SET_OVERLAY              = (WM_CAP_START+  51);
    const int WM_CAP_SET_PREVIEWRATE          = (WM_CAP_START+  52);
    const int WM_CAP_SET_SCALE                = (WM_CAP_START+  53);
    const int WM_CAP_GET_STATUS               = (WM_CAP_START+  54);
    const int WM_CAP_SET_SCROLL               = (WM_CAP_START+  55);

    const int WM_CAP_GRAB_FRAME               = (WM_CAP_START+  60);
    const int WM_CAP_GRAB_FRAME_NOSTOP        = (WM_CAP_START+  61);

    const int WM_CAP_SEQUENCE                 = (WM_CAP_START+  62);
    const int WM_CAP_SEQUENCE_NOFILE          = (WM_CAP_START+  63);
    const int WM_CAP_SET_SEQUENCE_SETUP       = (WM_CAP_START+  64);
    const int WM_CAP_GET_SEQUENCE_SETUP       = (WM_CAP_START+  65);

    const int WM_CAP_SET_MCI_DEVICEA          = (WM_CAP_START+  66);
    const int WM_CAP_GET_MCI_DEVICEA          = (WM_CAP_START+  67);
    const int WM_CAP_SET_MCI_DEVICEW          = (WM_CAP_UNICODE_START+  66);
    const int WM_CAP_GET_MCI_DEVICEW          = (WM_CAP_UNICODE_START+  67);
    #if UNICODE
    const int WM_CAP_SET_MCI_DEVICE           = WM_CAP_SET_MCI_DEVICEW;
    const int WM_CAP_GET_MCI_DEVICE           = WM_CAP_GET_MCI_DEVICEW;
    #else
    const int WM_CAP_SET_MCI_DEVICE           = WM_CAP_SET_MCI_DEVICEA;
    const int WM_CAP_GET_MCI_DEVICE           = WM_CAP_GET_MCI_DEVICEA;
    #endif

    const int WM_CAP_STOP                     = (WM_CAP_START+  68);
    const int WM_CAP_ABORT                    = (WM_CAP_START+  69);

    const int WM_CAP_SINGLE_FRAME_OPEN        = (WM_CAP_START+  70);
    const int WM_CAP_SINGLE_FRAME_CLOSE       = (WM_CAP_START+  71);
    const int WM_CAP_SINGLE_FRAME             = (WM_CAP_START+  72);

    const int WM_CAP_PAL_OPENA                = (WM_CAP_START+  80);
    const int WM_CAP_PAL_SAVEA                = (WM_CAP_START+  81);
    const int WM_CAP_PAL_OPENW                = (WM_CAP_UNICODE_START+  80);
    const int WM_CAP_PAL_SAVEW                = (WM_CAP_UNICODE_START+  81);
    #if UNICODE
    const int WM_CAP_PAL_OPEN                 = WM_CAP_PAL_OPENW;
    const int WM_CAP_PAL_SAVE                 = WM_CAP_PAL_SAVEW;
    #else
    const int WM_CAP_PAL_OPEN                 = WM_CAP_PAL_OPENA;
    const int WM_CAP_PAL_SAVE                 = WM_CAP_PAL_SAVEA;
    #endif

    const int WM_CAP_PAL_PASTE                = (WM_CAP_START+  82);
    const int WM_CAP_PAL_AUTOCREATE           = (WM_CAP_START+  83);
    const int WM_CAP_PAL_MANUALCREATE         = (WM_CAP_START+  84);

    // Following added post VFW 1.1
    const int WM_CAP_SET_CALLBACK_CAPCONTROL  = (WM_CAP_START+  85);

    // Defines end of the message range
    const int WM_CAP_UNICODE_END              = WM_CAP_PAL_SAVEW;
    const int WM_CAP_END                      = WM_CAP_UNICODE_END;
        
    #endregion
    
    #region API Structures
    
    [StructLayout(LayoutKind.Sequential)]
    struct VideoHeader
    {
      public IntPtr lpData;
      public int  dwBufferLength;
      public int  dwBytesUsed;
      public uint dwTimeCaptured;
      public uint dwUser;
      public uint dwFlags;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    struct CapDriverCaps
    { 
      public uint   wDeviceIndex; 
      public int    fHasOverlay; 
      public int    fHasDlgVideoSource; 
      public int    fHasDlgVideoFormat; 
      public int    fHasDlgVideoDisplay; 
      public int    fCaptureInitialized; 
      public int    fDriverSuppliesPalettes; 
      public IntPtr hVideoIn; 
      public IntPtr hVideoOut; 
      public IntPtr hVideoExtIn; 
      public IntPtr hVideoExtOut; 
    }
    
    #endregion
    
    [DllImport("user32", EntryPoint="DestroyWindow")]
    static extern int DestroyWindow (IntPtr hWnd);
    
    [DllImport("user32", EntryPoint="SendMessage")]
    static extern int SendMessage (IntPtr hWnd, uint Msg, int wParam, int lParam);
    
    [DllImport("user32", EntryPoint="SendMessage")]
    static extern int SendMessage (IntPtr hWnd, uint Msg, int wParam, FrameCallback fpProc);
    
    [DllImport("user32", EntryPoint="SendMessage")]
    static extern int SendMessage (IntPtr hWnd, uint Msg, int wParam, ref CapDriverCaps caps);
    
    [DllImport("user32", EntryPoint="SendMessage")]
    static extern int SendMessage (IntPtr hWnd, uint Msg, int wParam, IntPtr ptr);
    
    [DllImport("avicap32.dll", EntryPoint="capCreateCaptureWindowA", CharSet=CharSet.Ansi)]
    static extern IntPtr capCreateCaptureWindow (string lpszWindowName, int dwStyle, int X, int Y, int nWidth, int nHeight, int hwndParent, int nID);
    
    [DllImport("user32", EntryPoint="OpenClipboard")]
    static extern int OpenClipboard (IntPtr hWnd);
    
    [DllImport("user32", EntryPoint="EmptyClipboard")]
    static extern int EmptyClipboard ();
    
    [DllImport("user32", EntryPoint="CloseClipboard")]
    static extern int CloseClipboard ();
    
      #endregion
    
    
    #region Class Variables
    
    private bool m_Disposed = false;
    private delegate int FrameCallback (IntPtr hWnd, ref VideoHeader VideoHeader);
    
    private IWin32Window m_Owner;
    private IntPtr m_hCamWnd = IntPtr.Zero;
    
    private bool m_HasDlgVideoDisplay = false;
    private bool m_HasDlgVideoFormat  = false;
    private bool m_HasDlgVideoSource  = false;
    
    private bool m_LocalGrab = false;
    private Image m_GrabImage = null;
    
    // Required to bypass GC
    private FrameCallback m_FrameCallback;
    
    private CamPreviewCallback m_PreviewProc;
    private Timer m_PreviewTimer;
    
    #endregion
        
    #region Class Construction
    
    public CamTool (IWin32Window Owner) : this(Owner, null, 66)
    {}
    public CamTool (IWin32Window Owner, CamPreviewCallback PreviewProc) : this(Owner, PreviewProc, 66)
    {}
    public CamTool (IWin32Window Owner, CamPreviewCallback PreviewProc, int PreviewRate)
    {
      // Store owner window
      m_Owner = Owner;
      
      // Create and set the preview (capture) window
      SetPreviewWindow(CreatePreviewWindow());
      
      // Set up timer
      m_PreviewTimer = new Timer();
      m_PreviewTimer.Enabled = false;
      m_PreviewTimer.Tick += new EventHandler(PreviewTick);
      
      // Init variables
      this.PreviewRate = PreviewRate;
      this.SetPreviewCallback(PreviewProc);
    }
    
    ~CamTool ()
    { Dispose(); }
    
    public void Dispose ()
    {
      if (m_Disposed) return;
      m_Disposed = true;
      
      // Destroy preview window
      DestroyPreviewWindow();
    }
    
    #endregion
    
    
    
    #region Local Functions
    
    private IntPtr CreatePreviewWindow ()
    { return capCreateCaptureWindow("CamTool Window", 0x00000000, 0, 0, 320, 240, m_Owner.Handle.ToInt32(), 0); }
    
    private void SetPreviewWindow (IntPtr hWnd)
    {
      // Set capture window
      m_hCamWnd = hWnd;
      
      try
      {
        // Connect to the capture device
        if (SendMessage(m_hCamWnd, WM_CAP_DRIVER_CONNECT, 0, 0) == 0)
          throw new CamException("Could not connect to device.");
        SendMessage(m_hCamWnd, WM_CAP_SET_PREVIEW, 0, 0);
        SendMessage(m_hCamWnd, WM_CAP_SET_SCALE, 0, 0);
        
        // Get device capabilities
        CapDriverCaps caps = new CapDriverCaps();
        SendMessage(m_hCamWnd, WM_CAP_DRIVER_GET_CAPS, Marshal.SizeOf(caps), ref caps);
        m_HasDlgVideoDisplay = caps.fHasDlgVideoDisplay != 0;
        m_HasDlgVideoFormat = caps.fHasDlgVideoFormat != 0;
        m_HasDlgVideoSource = caps.fHasDlgVideoSource != 0;
        
        // Set callbacks
        m_FrameCallback = new FrameCallback(FrameCallbackProc);
        if (SendMessage(m_hCamWnd, WM_CAP_SET_CALLBACK_FRAME, 0, m_FrameCallback) == 0)
          throw new CamException("Could not set internal device callback.");
      }
      catch
      {
        // Clean up
        Dispose();
        
        // Forward error
        throw;
      }
    }
    
    private void DestroyPreviewWindow ()
    {
      // Clean up capture window
      if (m_hCamWnd != IntPtr.Zero)
      {
        SendMessage(m_hCamWnd, WM_CAP_DRIVER_DISCONNECT, 0, 0);
      
        DestroyWindow(m_hCamWnd);
        m_hCamWnd = IntPtr.Zero;
      }
      
      m_FrameCallback = null;
    }
    
    private int FrameCallbackProc (IntPtr hWnd, ref VideoHeader VideoHeader)
    {
      // Failsafe
      if (!m_LocalGrab) return 1;
      
      // Failsafe .. return False if bad
      if (VideoHeader.lpData == IntPtr.Zero) return 0;
      
      // Get image size and dimensions
      int size = SendMessage(m_hCamWnd, WM_CAP_GET_VIDEOFORMAT, 0, 0);
      IntPtr lpVideoFormat = Marshal.AllocHGlobal(size);
      SendMessage(m_hCamWnd, WM_CAP_GET_VIDEOFORMAT, size, lpVideoFormat);
      
      int cx = Marshal.ReadInt32(lpVideoFormat, 4);
      int cy = Marshal.ReadInt32(lpVideoFormat, 8);
      int cbw = cx * 3;
      int area = (cbw*Math.Abs(cy));
      
      Marshal.FreeHGlobal(lpVideoFormat); lpVideoFormat = IntPtr.Zero;
      
      // Create normal bitmap
      Image img = null;
      try
      {
        img = new Bitmap(cx, Math.Abs(cy), ( (( VideoHeader.dwBytesUsed > 0 )?( VideoHeader.dwBytesUsed - area ):( 0 )) + cbw), PixelFormat.Format24bppRgb, VideoHeader.lpData);
        if (cy > 0) img.RotateFlip(RotateFlipType.RotateNoneFlipY);
      }
      catch (NullReferenceException)
      { return 0; }
      
      // Set image
      m_GrabImage = img;
      
      return 1; // True
    }
    
    private void PreviewTick (object sender, System.EventArgs e)
    {
      // Failsafe
      if (m_PreviewProc == null)
      { m_PreviewTimer.Enabled = false; return; }
      
      // Call preview proc
      m_PreviewProc(GrabFrame());
    }
    
    #endregion
    
    
    public bool HasDlgVideoDisplay ()
    { return m_HasDlgVideoDisplay; }
    public bool HasDlgVideoFormat ()
    { return m_HasDlgVideoFormat; }
    public bool HasDlgVideoSource ()
    { return m_HasDlgVideoSource; }
    
    public bool ShowDlgVideoDisplay ()
    { return (SendMessage(m_hCamWnd, WM_CAP_DLG_VIDEODISPLAY, 0, 0) != 0); }
    public bool ShowDlgVideoFormat ()
    {
      if (SendMessage(m_hCamWnd, WM_CAP_DLG_VIDEOFORMAT, 0, 0) == 0)
        return false;
      
      // TODO: Check and update internal format (ex: I420)
      
      return true;
    }
    public bool ShowDlgVideoSource ()
    { return (SendMessage(m_hCamWnd, WM_CAP_DLG_VIDEOSOURCE, 0, 0) != 0); }
    
    /// <summary> Grabs a single frame and returns it as an image. </summary>
    public Image GrabFrame ()
    {
      m_GrabImage = null;
      
      // Grab image
      m_LocalGrab = true;
      if (SendMessage(m_hCamWnd, WM_CAP_GRAB_FRAME_NOSTOP, 0, 0) == 0)
        return null;
      m_LocalGrab = false;
      
      // Return image and clear internal image
      Image res = m_GrabImage;
      m_GrabImage = null;
      return res;
    }
    
    /// <summary> Sets the preview callback (set to null to disable preview mode). </summary>
    public void SetPreviewCallback (CamPreviewCallback PreviewProc)
    {
      m_PreviewProc = PreviewProc;
      m_PreviewTimer.Enabled = (m_PreviewProc != null);
    }
    
    
    
    #region Class Properties
    
    /// <summary> Gets the value indicating whether the class has been disposed of. </summary>
    public bool IsDisposed
    { get { return m_Disposed; } }
    
    /// <summary> The rate in of capture when in preview mode (in miliseconds). </summary>
    public int PreviewRate
    {
      get { return m_PreviewTimer.Interval; }
      set { m_PreviewTimer.Interval = value; }
    }
    
    /// <summary> Gets the video format used for capture. </summary>
    public CamVideoFormat VideoFormat
    {
      get 
      {
        CamVideoFormat res;
        int size = SendMessage(m_hCamWnd, WM_CAP_GET_VIDEOFORMAT, 0, 0);
        IntPtr lpVideoFormat = Marshal.AllocHGlobal(size);
        SendMessage(m_hCamWnd, WM_CAP_GET_VIDEOFORMAT, size, lpVideoFormat);
        res = (CamVideoFormat)Marshal.PtrToStructure(lpVideoFormat, typeof(CamVideoFormat));
        Marshal.FreeHGlobal(lpVideoFormat); lpVideoFormat = IntPtr.Zero;
        return res;
      }
      
      set 
      {
        // TODO: Get to work
        
        int size = SendMessage(m_hCamWnd, WM_CAP_GET_VIDEOFORMAT, 0, 0);
        IntPtr lpVideoFormat = Marshal.AllocHGlobal(size);
        SendMessage(m_hCamWnd, WM_CAP_GET_VIDEOFORMAT, size, lpVideoFormat);
        Marshal.StructureToPtr(value, lpVideoFormat, true);
        int i = SendMessage(m_hCamWnd, WM_CAP_SET_VIDEOFORMAT, size, lpVideoFormat);
        Marshal.FreeHGlobal(lpVideoFormat); lpVideoFormat = IntPtr.Zero;
      }
    }
    
    /// <summary> Gets/sets the capture frame size. </summary>
    public Size FrameSize
    {
      get
      {
        int size = SendMessage(m_hCamWnd, WM_CAP_GET_VIDEOFORMAT, 0, 0);
        IntPtr lpVideoFormat = Marshal.AllocHGlobal(size);
        SendMessage(m_hCamWnd, WM_CAP_GET_VIDEOFORMAT, size, lpVideoFormat);
        
        int cx = Marshal.ReadInt32(lpVideoFormat, 4);
        int cy = Marshal.ReadInt32(lpVideoFormat, 8);
        
        Marshal.FreeHGlobal(lpVideoFormat); lpVideoFormat = IntPtr.Zero;
        return new Size(cx, cy);
      }
      
      set
      {
        int size = SendMessage(m_hCamWnd, WM_CAP_GET_VIDEOFORMAT, 0, 0);
        IntPtr lpVideoFormat = Marshal.AllocHGlobal(size);
        SendMessage(m_hCamWnd, WM_CAP_GET_VIDEOFORMAT, size, lpVideoFormat);
        
        Marshal.WriteInt32(lpVideoFormat, 4, value.Width);
        Marshal.WriteInt32(lpVideoFormat, 8, value.Height);
        
        SendMessage(m_hCamWnd, WM_CAP_SET_VIDEOFORMAT, size, lpVideoFormat);
        
        Marshal.FreeHGlobal(lpVideoFormat); lpVideoFormat = IntPtr.Zero;
      }
    }
    
    #endregion
    
    }
}
