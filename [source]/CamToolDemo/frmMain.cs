using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Uberware.Tools;

namespace CamToolDemo
{
  /// <summary>
  /// Summary description for Form1.
  /// </summary>
  public class frmMain : System.Windows.Forms.Form
  {
    
    #region Class Variables
    
    private CamTool m_Cam = null;
    
    #region Controls
    
    private System.Windows.Forms.PictureBox picPreview;
    private System.Windows.Forms.Button btnCapture;
    private System.Windows.Forms.Button button1;
    
    /// <summary> Required designer variable. </summary>
    private System.ComponentModel.Container components = null;
    
    #endregion
    
    #endregion
    
    #region Class Construction
    
    public frmMain ()
    {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      //
      // TODO: Add any constructor code after InitializeComponent call
      //
    }
    
    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if (components != null) 
        { components.Dispose(); }
      } base.Dispose( disposing );
    }
    
    #region Windows Form Designer generated code
    
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.picPreview = new System.Windows.Forms.PictureBox();
      this.btnCapture = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // picPreview
      // 
      this.picPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.picPreview.Location = new System.Drawing.Point(8, 8);
      this.picPreview.Name = "picPreview";
      this.picPreview.Size = new System.Drawing.Size(642, 482);
      this.picPreview.TabIndex = 0;
      this.picPreview.TabStop = false;
      // 
      // btnCapture
      // 
      this.btnCapture.Location = new System.Drawing.Point(664, 12);
      this.btnCapture.Name = "btnCapture";
      this.btnCapture.Size = new System.Drawing.Size(96, 36);
      this.btnCapture.TabIndex = 1;
      this.btnCapture.Text = "&Capture";
      this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(676, 68);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(80, 36);
      this.button1.TabIndex = 2;
      this.button1.Text = "Format...";
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // frmMain
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(768, 498);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.button1,
                                                                  this.btnCapture,
                                                                  this.picPreview});
      this.Name = "frmMain";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Form1";
      this.Load += new System.EventHandler(this.frmMain_Load);
      this.ResumeLayout(false);

    }
    
    #endregion
    
    #endregion
    
    
    #region Entry Point of Application
    
    /// <summary> The main entry point for the application. </summary>
    [STAThread]
    static void Main() 
    {
      Application.Run(new frmMain());
    }
    
    #endregion
    
    
    
    private void frmMain_Load(object sender, System.EventArgs e)
    {
      m_Cam = new CamTool(this);
      
      m_Cam.PreviewRate = 66;
      m_Cam.SetPreviewCallback(new CamPreviewCallback(PreviewCallback));
    }
    
    private void btnCapture_Click(object sender, System.EventArgs e)
    {
      picPreview.Image = m_Cam.GrabFrame();
    }
    
    private void button1_Click(object sender, System.EventArgs e)
    {
      CamVideoFormat vf;
      m_Cam.ShowDlgVideoFormat();
      vf = m_Cam.VideoFormat;
    }
    
    private void PreviewCallback (Image img)
    { picPreview.Image = img; }
  }
}
