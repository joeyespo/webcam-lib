using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Drawing;

namespace WebcamLib
{
    /// <summary>
    /// Handles webcam frame updates.
    /// </summary>
    /// <param name="PreviewImage">The new frame image.</param>
    public delegate void CamPreviewCallback(Image PreviewImage);
}
