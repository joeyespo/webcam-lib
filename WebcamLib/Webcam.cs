using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WebcamLib
{
    /// <summary>
    /// Represents a webcam.
    /// </summary>
    public sealed class Webcam : IDisposable
    {
        #region Class Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="Webcam"/> class.
        /// </summary>
        /// <param name="Owner">The owner window.</param>
        public Webcam(IWin32Window Owner)
            : this(Owner, null, 66)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Webcam"/> class.
        /// </summary>
        /// <param name="Owner">The owner window.</param>
        /// <param name="PreviewProc">The method to call when new frames are taken.</param>
        public Webcam(IWin32Window Owner, CamPreviewCallback PreviewProc)
            : this(Owner, PreviewProc, 66)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Webcam"/> class.
        /// </summary>
        /// <param name="Owner">The owner window.</param>
        /// <param name="PreviewProc">The method to call when new frames are taken.</param>
        /// <param name="PreviewRate">The rate at which to call <paramref name="previewHandler"/>.</param>
        public Webcam(IWin32Window Owner, CamPreviewCallback PreviewProc, int PreviewRate)
        {
            // Store owner window
            ownerWindow = Owner;

            // Create and set the preview (capture) window
            SetPreviewWindow(CreatePreviewWindow());

            // Set up timer
            previewTimer = new Timer();
            previewTimer.Enabled = false;
            previewTimer.Tick += PreviewTick;

            // Init variables
            this.PreviewRate = PreviewRate;
            this.SetPreviewCallback(PreviewProc);
        }
        ~Webcam()
        {
            Dispose();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the value indicating whether the class has been disposed of.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return isDisposed;
            }
        }

        /// <summary>
        /// The rate in of capture when in preview mode (in miliseconds).
        /// </summary>
        public int PreviewRate
        {
            get
            {
                return previewTimer.Interval;
            }
            set
            {
                previewTimer.Interval = value;
            }
        }

        /// <summary>
        /// Gets the video format used for capture.
        /// </summary>
        public CamVideoFormat VideoFormat
        {
            get
            {
                CamVideoFormat res;
                int size = Win32.SendMessage(camWindowHandle, Win32.WM_CAP_GET_VIDEOFORMAT, 0, 0);
                IntPtr lpVideoFormat = Marshal.AllocHGlobal(size);
                Win32.SendMessage(camWindowHandle, Win32.WM_CAP_GET_VIDEOFORMAT, size, lpVideoFormat);
                res = (CamVideoFormat)Marshal.PtrToStructure(lpVideoFormat, typeof(CamVideoFormat));
                Marshal.FreeHGlobal(lpVideoFormat);
                lpVideoFormat = IntPtr.Zero;
                return res;
            }

            set
            {
                // TODO: Get to work

                int size = Win32.SendMessage(camWindowHandle, Win32.WM_CAP_GET_VIDEOFORMAT, 0, 0);
                IntPtr lpVideoFormat = Marshal.AllocHGlobal(size);
                Win32.SendMessage(camWindowHandle, Win32.WM_CAP_GET_VIDEOFORMAT, size, lpVideoFormat);
                Marshal.StructureToPtr(value, lpVideoFormat, true);
                int i = Win32.SendMessage(camWindowHandle, Win32.WM_CAP_SET_VIDEOFORMAT, size, lpVideoFormat);
                Marshal.FreeHGlobal(lpVideoFormat);
                lpVideoFormat = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Gets or sets the capture frame size.
        /// </summary>
        /// <remarks>
        /// The getter and setter may cause side effects.
        /// </remarks>
        public Size FrameSize
        {
            get
            {
                int size = Win32.SendMessage(camWindowHandle, Win32.WM_CAP_GET_VIDEOFORMAT, 0, 0);
                IntPtr lpVideoFormat = Marshal.AllocHGlobal(size);
                Win32.SendMessage(camWindowHandle, Win32.WM_CAP_GET_VIDEOFORMAT, size, lpVideoFormat);

                int cx = Marshal.ReadInt32(lpVideoFormat, 4);
                int cy = Marshal.ReadInt32(lpVideoFormat, 8);

                Marshal.FreeHGlobal(lpVideoFormat);
                lpVideoFormat = IntPtr.Zero;
                return new Size(cx, cy);
            }

            set
            {
                int size = Win32.SendMessage(camWindowHandle, Win32.WM_CAP_GET_VIDEOFORMAT, 0, 0);
                IntPtr lpVideoFormat = Marshal.AllocHGlobal(size);
                Win32.SendMessage(camWindowHandle, Win32.WM_CAP_GET_VIDEOFORMAT, size, lpVideoFormat);

                Marshal.WriteInt32(lpVideoFormat, 4, value.Width);
                Marshal.WriteInt32(lpVideoFormat, 8, value.Height);

                Win32.SendMessage(camWindowHandle, Win32.WM_CAP_SET_VIDEOFORMAT, size, lpVideoFormat);

                Marshal.FreeHGlobal(lpVideoFormat);
                lpVideoFormat = IntPtr.Zero;
            }
        }

        public bool HasDlgVideoDisplay
        {
            get
            {
                return hasDlgVideoDisplay;
            }
        }

        public bool HasDlgVideoFormat
        {
            get
            {
                return hasDlgVideoFormat;
            }
        }

        public bool HasDlgVideoSource
        {
            get
            {
                return hasDlgVideoSource;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if(isDisposed)
                return;

            // Destroy preview window
            DestroyPreviewWindow();

            isDisposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Shows the video display dialog.
        /// </summary>
        /// <returns>true if the dialog was successfully shown; otherwise, false.</returns>
        public bool ShowVideoDisplayDialog()
        {
            return (Win32.SendMessage(camWindowHandle, Win32.WM_CAP_DLG_VIDEODISPLAY, 0, 0) != 0);
        }

        /// <summary>
        /// Shows the video format dialog.
        /// </summary>
        /// <returns>true if the dialog was successfully shown; otherwise, false.</returns>
        public bool ShowVideoFormatDialog()
        {
            if(Win32.SendMessage(camWindowHandle, Win32.WM_CAP_DLG_VIDEOFORMAT, 0, 0) == 0)
                return false;

            // TODO: Check and update internal format (ex: I420)

            return true;
        }

        /// <summary>
        /// Shows the video source dialog.
        /// </summary>
        /// <returns>true if the dialog was successfully shown; otherwise, false.</returns>
        public bool ShowVideoSourceDialog()
        {
            return (Win32.SendMessage(camWindowHandle, Win32.WM_CAP_DLG_VIDEOSOURCE, 0, 0) != 0);
        }

        /// <summary>
        /// Grabs a single frame and returns it as an image.
        /// </summary>
        public Image GrabFrame()
        {
            frameImage = null;

            // Grab image
            localGrab = true;
            if(Win32.SendMessage(camWindowHandle, Win32.WM_CAP_GRAB_FRAME_NOSTOP, 0, 0) == 0)
                return null;
            localGrab = false;

            // Return image and clear internal image
            Image res = frameImage;
            frameImage = null;
            return res;
        }

        /// <summary>
        /// Sets the preview callback (set to null to disable preview mode).
        /// </summary>
        public void SetPreviewCallback(CamPreviewCallback PreviewProc)
        {
            previewHandler = PreviewProc;
            previewTimer.Enabled = (previewHandler != null);
        }

        #endregion

        /// <summary>
        /// Creates the preview window.
        /// </summary>
        private IntPtr CreatePreviewWindow()
        {
            return Win32.CapCreateCaptureWindow("WebcamLib Window", 0x00000000, 0, 0, 320, 240, ownerWindow.Handle.ToInt32(), 0);
        }

        /// <summary>
        /// Sets the preview window.
        /// </summary>
        private void SetPreviewWindow(IntPtr windowHandle)
        {
            // Set capture window
            camWindowHandle = windowHandle;

            try
            {
                // Connect to the capture device
                if(Win32.SendMessage(camWindowHandle, Win32.WM_CAP_DRIVER_CONNECT, 0, 0) == 0)
                    throw new CamException("Could not connect to device.");
                Win32.SendMessage(camWindowHandle, Win32.WM_CAP_SET_PREVIEW, 0, 0);
                Win32.SendMessage(camWindowHandle, Win32.WM_CAP_SET_SCALE, 0, 0);

                // Get device capabilities
                Win32.CapDriverCaps caps = new Win32.CapDriverCaps();
                Win32.SendMessage(camWindowHandle, Win32.WM_CAP_DRIVER_GET_CAPS, Marshal.SizeOf(caps), ref caps);
                hasDlgVideoDisplay = caps.fHasDlgVideoDisplay != 0;
                hasDlgVideoFormat = caps.fHasDlgVideoFormat != 0;
                hasDlgVideoSource = caps.fHasDlgVideoSource != 0;

                // Set callbacks
                frameCallback = new Win32.FrameCallback(FrameCallbackProc);
                if(Win32.SendMessage(camWindowHandle, Win32.WM_CAP_SET_CALLBACK_FRAME, 0, frameCallback) == 0)
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

        /// <summary>
        /// Destroys the preview window.
        /// </summary>
        private void DestroyPreviewWindow()
        {
            // Clean up capture window
            if(camWindowHandle != IntPtr.Zero)
            {
                Win32.SendMessage(camWindowHandle, Win32.WM_CAP_DRIVER_DISCONNECT, 0, 0);

                Win32.DestroyWindow(camWindowHandle);
                camWindowHandle = IntPtr.Zero;
            }

            frameCallback = null;
        }

        /// <summary>
        /// Called on each frame update.
        /// </summary>
        /// <remarks>
        /// In order to draw on the resulting image, it must first be copied. This is because setting the bitmap data
        /// pointer directly and rotating the image can lead to random exceptions. This is expensive though. So by
        /// design, this doesn't happen automatically.
        /// 
        /// See the following sites for details about this:
        /// http://msdn.microsoft.com/en-us/library/system.drawing.image.rotateflip.aspx
        /// http://social.msdn.microsoft.com/Forums/en-IE/csharplanguage/thread/affa1855-e1ec-476d-bfb1-d0985971f394
        /// </remarks>
        private int FrameCallbackProc(IntPtr hWnd, ref Win32.VideoHeader VideoHeader)
        {
            // Failsafe
            if(!localGrab)
                return 1;

            // Failsafe .. return False if bad
            if(VideoHeader.lpData == IntPtr.Zero)
                return 0;

            // Get image size and dimensions
            int size = Win32.SendMessage(camWindowHandle, Win32.WM_CAP_GET_VIDEOFORMAT, 0, 0);
            IntPtr lpVideoFormat = Marshal.AllocHGlobal(size);
            Win32.SendMessage(camWindowHandle, Win32.WM_CAP_GET_VIDEOFORMAT, size, lpVideoFormat);

            int cx = Marshal.ReadInt32(lpVideoFormat, 4);
            int cy = Marshal.ReadInt32(lpVideoFormat, 8);
            int cbw = cx * 3;
            int area = (cbw * Math.Abs(cy));

            Marshal.FreeHGlobal(lpVideoFormat);
            lpVideoFormat = IntPtr.Zero;

            // Create normal bitmap
            Image img = null;
            try
            {
                img = new Bitmap(cx, Math.Abs(cy), (((VideoHeader.dwBytesUsed > 0) ? (VideoHeader.dwBytesUsed - area) : (0)) + cbw), PixelFormat.Format24bppRgb, VideoHeader.lpData);
            }
            catch(NullReferenceException)
            {
                return 0;
            }

            // Rotate the image, if necessary
            if(cy > 0)
            {
                Bitmap bitmap = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
                using(Graphics g = Graphics.FromImage(bitmap))
                    g.DrawImage(img, 0, 0, new Rectangle(0, 0, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                img = bitmap;
            }

            // Set image
            frameImage = img;

            return 1; // True
        }

        /// <summary>
        /// Grab the next frame.
        /// </summary>
        private void PreviewTick(object sender, System.EventArgs e)
        {
            // Failsafe
            if(previewHandler == null)
            {
                previewTimer.Enabled = false;
                return;
            }

            // Call preview proc
            previewHandler(GrabFrame());
        }

        private bool isDisposed = false;
        private IWin32Window ownerWindow;
        private IntPtr camWindowHandle = IntPtr.Zero;
        private bool hasDlgVideoDisplay = false;
        private bool hasDlgVideoFormat = false;
        private bool hasDlgVideoSource = false;
        private bool localGrab = false;
        private Image frameImage = null;
        // Required to bypass a GC
        private Win32.FrameCallback frameCallback;
        private CamPreviewCallback previewHandler;
        private Timer previewTimer;
    }
}
