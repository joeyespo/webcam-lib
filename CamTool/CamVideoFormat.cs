using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Runtime.InteropServices;

namespace CamTool
{
    /// <summary>
    /// Represents the video format of a webcam.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CamVideoFormat
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
    }
}
