namespace homeworkMaltimedia
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.flowLayoutPanelControls = new System.Windows.Forms.FlowLayoutPanel();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.lblColorSystemTitle = new System.Windows.Forms.Label();
            this.cmbColorSystem = new System.Windows.Forms.ComboBox();
            this.grpColorReduction = new System.Windows.Forms.GroupBox();
            this.btnReduceColors = new System.Windows.Forms.Button();
            this.numColors = new System.Windows.Forms.NumericUpDown();
            this.grpChannels = new System.Windows.Forms.GroupBox();
            this.chkLuminance = new System.Windows.Forms.CheckBox();
            this.grpRGB = new System.Windows.Forms.GroupBox();
            this.lblCh1 = new System.Windows.Forms.Label();
            this.trkR = new System.Windows.Forms.TrackBar();
            this.lblCh2 = new System.Windows.Forms.Label();
            this.trkG = new System.Windows.Forms.TrackBar();
            this.lblCh3 = new System.Windows.Forms.Label();
            this.trkB = new System.Windows.Forms.TrackBar();
            this.lblCh4 = new System.Windows.Forms.Label();
            this.trk4 = new System.Windows.Forms.TrackBar();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabDisplay = new System.Windows.Forms.TabPage();
            this.picDisplay = new System.Windows.Forms.PictureBox();
            this.tab3DColorSpace = new System.Windows.Forms.TabPage();
            this.tabMetadata = new System.Windows.Forms.TabPage();
            this.lstMetadata = new System.Windows.Forms.ListView();
            this.colProperty = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.lblStatusPixel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.flowLayoutPanelControls.SuspendLayout();
            this.grpColorReduction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numColors)).BeginInit();
            this.grpChannels.SuspendLayout();
            this.grpRGB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk4)).BeginInit();
            this.tabControlMain.SuspendLayout();
            this.tabDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picDisplay)).BeginInit();
            this.tabMetadata.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.flowLayoutPanelControls);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.tabControlMain);
            this.splitContainerMain.Size = new System.Drawing.Size(1134, 784);
            this.splitContainerMain.SplitterDistance = 250;
            this.splitContainerMain.TabIndex = 0;
            // 
            // flowLayoutPanelControls
            // 
            this.flowLayoutPanelControls.AutoScroll = true;
            this.flowLayoutPanelControls.Controls.Add(this.btnBrowse);
            this.flowLayoutPanelControls.Controls.Add(this.btnSave);
            this.flowLayoutPanelControls.Controls.Add(this.btnReset);
            this.flowLayoutPanelControls.Controls.Add(this.lblColorSystemTitle);
            this.flowLayoutPanelControls.Controls.Add(this.cmbColorSystem);
            this.flowLayoutPanelControls.Controls.Add(this.grpColorReduction);
            this.flowLayoutPanelControls.Controls.Add(this.grpChannels);
            this.flowLayoutPanelControls.Controls.Add(this.grpRGB);
            this.flowLayoutPanelControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelControls.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelControls.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelControls.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.flowLayoutPanelControls.Name = "flowLayoutPanelControls";
            this.flowLayoutPanelControls.Padding = new System.Windows.Forms.Padding(11, 12, 11, 12);
            this.flowLayoutPanelControls.Size = new System.Drawing.Size(250, 784);
            this.flowLayoutPanelControls.TabIndex = 0;
            this.flowLayoutPanelControls.WrapContents = false;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(14, 16);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(246, 36);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "Browse Image...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.BtnBrowse_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(14, 60);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(246, 36);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save to Disk";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(14, 104);
            this.btnReset.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(246, 36);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "Reset to Original";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.BtnReset_Click);
            // 
            // lblColorSystemTitle
            // 
            this.lblColorSystemTitle.AutoSize = true;
            this.lblColorSystemTitle.Location = new System.Drawing.Point(14, 144);
            this.lblColorSystemTitle.Name = "lblColorSystemTitle";
            this.lblColorSystemTitle.Size = new System.Drawing.Size(151, 19);
            this.lblColorSystemTitle.TabIndex = 3;
            this.lblColorSystemTitle.Text = "Active Color System";
            // 
            // cmbColorSystem
            // 
            this.cmbColorSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbColorSystem.FormattingEnabled = true;
            this.cmbColorSystem.Items.AddRange(new object[] {
            "RGB",
            "CMYK",
            "HSV",
            "YUV",
            "LAB",
            "YCbCr"});
            this.cmbColorSystem.Location = new System.Drawing.Point(14, 167);
            this.cmbColorSystem.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbColorSystem.Name = "cmbColorSystem";
            this.cmbColorSystem.Size = new System.Drawing.Size(246, 27);
            this.cmbColorSystem.TabIndex = 4;
            this.cmbColorSystem.SelectedIndexChanged += new System.EventHandler(this.CmbColorSystem_SelectedIndexChanged);
            // 
            // grpColorReduction
            // 
            this.grpColorReduction.Controls.Add(this.btnReduceColors);
            this.grpColorReduction.Controls.Add(this.numColors);
            this.grpColorReduction.Location = new System.Drawing.Point(14, 202);
            this.grpColorReduction.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpColorReduction.Name = "grpColorReduction";
            this.grpColorReduction.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpColorReduction.Size = new System.Drawing.Size(246, 95);
            this.grpColorReduction.TabIndex = 3;
            this.grpColorReduction.TabStop = false;
            this.grpColorReduction.Text = "Color Reduction";
            // 
            // btnReduceColors
            // 
            this.btnReduceColors.Location = new System.Drawing.Point(7, 58);
            this.btnReduceColors.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnReduceColors.Name = "btnReduceColors";
            this.btnReduceColors.Size = new System.Drawing.Size(212, 27);
            this.btnReduceColors.TabIndex = 1;
            this.btnReduceColors.Text = "Reduce";
            this.btnReduceColors.UseVisualStyleBackColor = true;
            this.btnReduceColors.Click += new System.EventHandler(this.btnReduceColors_Click);
            // 
            // numColors
            // 
            this.numColors.Location = new System.Drawing.Point(7, 25);
            this.numColors.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numColors.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.numColors.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numColors.Name = "numColors";
            this.numColors.Size = new System.Drawing.Size(212, 27);
            this.numColors.TabIndex = 0;
            this.numColors.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numColors.ValueChanged += new System.EventHandler(this.numColors_ValueChanged);
            // 
            // grpChannels
            // 
            this.grpChannels.Controls.Add(this.chkLuminance);
            this.grpChannels.Location = new System.Drawing.Point(14, 305);
            this.grpChannels.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpChannels.Name = "grpChannels";
            this.grpChannels.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpChannels.Size = new System.Drawing.Size(246, 71);
            this.grpChannels.TabIndex = 6;
            this.grpChannels.TabStop = false;
            this.grpChannels.Text = "Channels";
            // 
            // chkLuminance
            // 
            this.chkLuminance.AutoSize = true;
            this.chkLuminance.Location = new System.Drawing.Point(8, 26);
            this.chkLuminance.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkLuminance.Name = "chkLuminance";
            this.chkLuminance.Size = new System.Drawing.Size(111, 23);
            this.chkLuminance.TabIndex = 0;
            this.chkLuminance.Text = "Luminance";
            this.chkLuminance.UseVisualStyleBackColor = true;
            // 
            // grpRGB
            // 
            this.grpRGB.Controls.Add(this.lblCh1);
            this.grpRGB.Controls.Add(this.trkR);
            this.grpRGB.Controls.Add(this.lblCh2);
            this.grpRGB.Controls.Add(this.trkG);
            this.grpRGB.Controls.Add(this.lblCh3);
            this.grpRGB.Controls.Add(this.trkB);
            this.grpRGB.Controls.Add(this.lblCh4);
            this.grpRGB.Controls.Add(this.trk4);
            this.grpRGB.Location = new System.Drawing.Point(14, 384);
            this.grpRGB.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpRGB.Name = "grpRGB";
            this.grpRGB.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grpRGB.Size = new System.Drawing.Size(246, 398);
            this.grpRGB.TabIndex = 7;
            this.grpRGB.TabStop = false;
            this.grpRGB.Text = "Color System Controls";
            // 
            // lblCh1
            // 
            this.lblCh1.AutoSize = true;
            this.lblCh1.Location = new System.Drawing.Point(9, 21);
            this.lblCh1.Name = "lblCh1";
            this.lblCh1.Size = new System.Drawing.Size(19, 19);
            this.lblCh1.TabIndex = 0;
            this.lblCh1.Text = "R";
            // 
            // trkR
            // 
            this.trkR.Location = new System.Drawing.Point(3, 44);
            this.trkR.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.trkR.Maximum = 255;
            this.trkR.Name = "trkR";
            this.trkR.Size = new System.Drawing.Size(212, 69);
            this.trkR.TabIndex = 1;
            this.trkR.Scroll += new System.EventHandler(this.trkR_Scroll);
            // 
            // lblCh2
            // 
            this.lblCh2.AutoSize = true;
            this.lblCh2.Location = new System.Drawing.Point(7, 112);
            this.lblCh2.Name = "lblCh2";
            this.lblCh2.Size = new System.Drawing.Size(20, 19);
            this.lblCh2.TabIndex = 2;
            this.lblCh2.Text = "G";
            // 
            // trkG
            // 
            this.trkG.Location = new System.Drawing.Point(10, 134);
            this.trkG.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.trkG.Maximum = 255;
            this.trkG.Name = "trkG";
            this.trkG.Size = new System.Drawing.Size(212, 69);
            this.trkG.TabIndex = 3;
            this.trkG.Scroll += new System.EventHandler(this.trkG_Scroll);
            // 
            // lblCh3
            // 
            this.lblCh3.AutoSize = true;
            this.lblCh3.Location = new System.Drawing.Point(11, 215);
            this.lblCh3.Name = "lblCh3";
            this.lblCh3.Size = new System.Drawing.Size(18, 19);
            this.lblCh3.TabIndex = 4;
            this.lblCh3.Text = "B";
            this.lblCh3.Click += new System.EventHandler(this.lblCh3_Click);
            // 
            // trkB
            // 
            this.trkB.Location = new System.Drawing.Point(15, 238);
            this.trkB.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.trkB.Maximum = 255;
            this.trkB.Name = "trkB";
            this.trkB.Size = new System.Drawing.Size(212, 69);
            this.trkB.TabIndex = 5;
            // 
            // lblCh4
            // 
            this.lblCh4.AutoSize = true;
            this.lblCh4.Location = new System.Drawing.Point(11, 308);
            this.lblCh4.Name = "lblCh4";
            this.lblCh4.Size = new System.Drawing.Size(0, 19);
            this.lblCh4.TabIndex = 6;
            // 
            // trk4
            // 
            this.trk4.Location = new System.Drawing.Point(15, 331);
            this.trk4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.trk4.Maximum = 100;
            this.trk4.Name = "trk4";
            this.trk4.Size = new System.Drawing.Size(212, 69);
            this.trk4.TabIndex = 7;
            this.trk4.Visible = false;
            this.trk4.Scroll += new System.EventHandler(this.trk4_Scroll);
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabDisplay);
            this.tabControlMain.Controls.Add(this.tab3DColorSpace);
            this.tabControlMain.Controls.Add(this.tabMetadata);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(880, 784);
            this.tabControlMain.TabIndex = 0;
            // 
            // tabDisplay
            // 
            this.tabDisplay.Controls.Add(this.picDisplay);
            this.tabDisplay.Location = new System.Drawing.Point(4, 28);
            this.tabDisplay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabDisplay.Name = "tabDisplay";
            this.tabDisplay.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabDisplay.Size = new System.Drawing.Size(872, 752);
            this.tabDisplay.TabIndex = 0;
            this.tabDisplay.Text = "Display";
            this.tabDisplay.UseVisualStyleBackColor = true;
            // 
            // picDisplay
            // 
            this.picDisplay.AllowDrop = true;
            this.picDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picDisplay.Location = new System.Drawing.Point(3, 4);
            this.picDisplay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picDisplay.Name = "picDisplay";
            this.picDisplay.Size = new System.Drawing.Size(866, 744);
            this.picDisplay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picDisplay.TabIndex = 0;
            this.picDisplay.TabStop = false;
            this.picDisplay.Click += new System.EventHandler(this.picDisplay_Click);
            // 
            // tab3DColorSpace
            // 
            this.tab3DColorSpace.Location = new System.Drawing.Point(4, 28);
            this.tab3DColorSpace.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tab3DColorSpace.Name = "tab3DColorSpace";
            this.tab3DColorSpace.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tab3DColorSpace.Size = new System.Drawing.Size(840, 753);
            this.tab3DColorSpace.TabIndex = 1;
            this.tab3DColorSpace.Text = "3D Color Space";
            this.tab3DColorSpace.UseVisualStyleBackColor = true;
            // 
            // tabMetadata
            // 
            this.tabMetadata.Controls.Add(this.lstMetadata);
            this.tabMetadata.Location = new System.Drawing.Point(4, 28);
            this.tabMetadata.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabMetadata.Name = "tabMetadata";
            this.tabMetadata.Size = new System.Drawing.Size(840, 753);
            this.tabMetadata.TabIndex = 2;
            this.tabMetadata.Text = "Metadata";
            this.tabMetadata.UseVisualStyleBackColor = true;
            // 
            // lstMetadata
            // 
            this.lstMetadata.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colProperty,
            this.colValue});
            this.lstMetadata.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstMetadata.FullRowSelect = true;
            this.lstMetadata.GridLines = true;
            this.lstMetadata.HideSelection = false;
            this.lstMetadata.Location = new System.Drawing.Point(0, 0);
            this.lstMetadata.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstMetadata.Name = "lstMetadata";
            this.lstMetadata.Size = new System.Drawing.Size(840, 753);
            this.lstMetadata.TabIndex = 0;
            this.lstMetadata.UseCompatibleStateImageBehavior = false;
            this.lstMetadata.View = System.Windows.Forms.View.Details;
            // 
            // colProperty
            // 
            this.colProperty.Text = "Property";
            this.colProperty.Width = 150;
            // 
            // colValue
            // 
            this.colValue.Text = "Value";
            this.colValue.Width = 300;
            // 
            // statusStripMain
            // 
            this.statusStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatusPixel});
            this.statusStripMain.Location = new System.Drawing.Point(0, 784);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStripMain.Size = new System.Drawing.Size(1134, 32);
            this.statusStripMain.TabIndex = 1;
            this.statusStripMain.Text = "statusStripMain";
            // 
            // lblStatusPixel
            // 
            this.lblStatusPixel.Name = "lblStatusPixel";
            this.lblStatusPixel.Size = new System.Drawing.Size(227, 25);
            this.lblStatusPixel.Text = "RGB: (0, 0, 0) | HSV: (0, 0, 0)";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 816);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.statusStripMain);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "PixelLab 2026";
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.flowLayoutPanelControls.ResumeLayout(false);
            this.flowLayoutPanelControls.PerformLayout();
            this.grpColorReduction.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numColors)).EndInit();
            this.grpChannels.ResumeLayout(false);
            this.grpChannels.PerformLayout();
            this.grpRGB.ResumeLayout(false);
            this.grpRGB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trk4)).EndInit();
            this.tabControlMain.ResumeLayout(false);
            this.tabDisplay.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picDisplay)).EndInit();
            this.tabMetadata.ResumeLayout(false);
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelControls;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lblColorSystemTitle;
        private System.Windows.Forms.ComboBox cmbColorSystem;
        private System.Windows.Forms.GroupBox grpColorReduction;
        private System.Windows.Forms.NumericUpDown numColors;
        private System.Windows.Forms.Button btnReduceColors;
        private System.Windows.Forms.GroupBox grpChannels;
        private System.Windows.Forms.CheckBox chkLuminance;
        private System.Windows.Forms.GroupBox grpRGB;
        private System.Windows.Forms.Label lblCh1;
        private System.Windows.Forms.TrackBar trkR;
        private System.Windows.Forms.Label lblCh2;
        private System.Windows.Forms.TrackBar trkG;
        private System.Windows.Forms.Label lblCh3;
        private System.Windows.Forms.TrackBar trkB;
        private System.Windows.Forms.Label lblCh4;
        private System.Windows.Forms.TrackBar trk4;

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabDisplay;
        private System.Windows.Forms.PictureBox picDisplay;
        private System.Windows.Forms.TabPage tab3DColorSpace;
        private System.Windows.Forms.TabPage tabMetadata;
        private System.Windows.Forms.ListView lstMetadata;
        private System.Windows.Forms.ColumnHeader colProperty;
        private System.Windows.Forms.ColumnHeader colValue;

        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusPixel;
    }
}

