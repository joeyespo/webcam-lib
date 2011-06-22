using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using WebcamLib;

namespace WebcamLibDemo
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public sealed partial class MainForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        #region Event Handler Methods

        /// <summary>
        /// Handles the Load event of the MainForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MainForm_Load(object sender, System.EventArgs e)
        {
            try
            {
                webcam = new Webcam(this);
                webcam.PreviewRate = 66;
                webcam.SetPreviewCallback(new CamPreviewCallback(PreviewCallback));
            }
            catch(CamException ex)
            {
                webcam = null;
                labelError.Text = "Error: " + ex.Message;
                labelError.Visible = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the captureButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void captureButton_Click(object sender, System.EventArgs e)
        {
            if(webcam == null)
            {
                MessageBox.Show("There's no webcam device set up.", Text + " Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            previewPictureBox.Image = webcam.GrabFrame();
        }

        /// <summary>
        /// Handles the Click event of the formatButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void formatButton_Click(object sender, System.EventArgs e)
        {
            if(webcam == null)
            {
                MessageBox.Show("There's no webcam device set up.", Text + " Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            CamVideoFormat vf;
            webcam.ShowVideoFormatDialog();
            vf = webcam.VideoFormat;
        }

        /// <summary>
        /// Handles the Click event of the closeButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        /// <summary>
        /// Occurs when a new frame is ready for display.
        /// </summary>
        private void PreviewCallback(Image img)
        {
            previewPictureBox.Image = img;
        }

        private Webcam webcam = null;
    }
}
