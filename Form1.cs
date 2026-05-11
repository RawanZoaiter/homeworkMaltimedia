using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace homeworkMaltimedia
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            picDisplay.DragEnter += PicDisplay_DragEnter;
            picDisplay.DragDrop += PicDisplay_DragDrop;
            picDisplay.MouseMove += PicDisplay_MouseMove;
            btnBrowse.Click += BtnBrowse_Click;
            cmbColorSystem.SelectedIndexChanged += CmbColorSystem_SelectedIndexChanged;
            chkLuminance.CheckedChanged += ChkLuminance_CheckedChanged;

            // initialize UI per default selection
            UpdateColorSystemUI();
        }

        private void PicDisplay_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void PicDisplay_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                LoadImage(files[0]);
            }
        }

        private void BtnBrowse_Click(object sender, System.EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    LoadImage(ofd.FileName);
                }
            }
        }

        private void LoadImage(string path)
        {
            try
            {
                var img = Image.FromFile(path);
                picDisplay.Image = img;
                UpdateMetadata(path, img);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");
            }
        }

        private void UpdateMetadata(string path, Image img)
        {
            lstMetadata.Items.Clear();
            lstMetadata.Items.Add(new ListViewItem(new[] { "File Name", System.IO.Path.GetFileName(path) }));
            lstMetadata.Items.Add(new ListViewItem(new[] { "Format", img.RawFormat.ToString() }));
            lstMetadata.Items.Add(new ListViewItem(new[] { "Size", $"{img.Width} x {img.Height}" }));
            // Unique color count computation goes here ideally
        }

        private void PicDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            if (picDisplay.Image != null)
            {
                // Simple implementation, not accounting for zoom/stretch in real-time unless computed
                lblStatusPixel.Text = $"RGB -> (R, G, B) and HSV -> (H, S, V) at X:{e.X} Y:{e.Y}";
            }
        }

        private void CmbColorSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateColorSystemUI();
        }

        private void ChkLuminance_CheckedChanged(object sender, EventArgs e)
        {
            // Luminance checkbox affects only color systems that expose a luminance channel (Y or L)
            var sys = cmbColorSystem.SelectedItem?.ToString() ?? "RGB";
            if (sys == "YUV" || sys == "LAB" || sys == "YCbCr")
            {
                // For now, we only update status text to reflect the toggle; image processing logic should respond to this flag.
                lblStatusPixel.Text = chkLuminance.Checked ? $"Luminance channel enabled for {sys}" : $"Luminance channel disabled for {sys}";
            }
            else
            {
                // ensure unchecked for systems without luminance
                if (chkLuminance.Checked) chkLuminance.Checked = false;
            }
        }

        private void UpdateColorSystemUI()
        {
            var sys = cmbColorSystem.SelectedItem?.ToString() ?? "RGB";
            // reset defaults
            trkR.Minimum = 0; trkR.Maximum = 255;
            trkG.Minimum = 0; trkG.Maximum = 255;
            trkB.Minimum = 0; trkB.Maximum = 255;
            trk4.Minimum = 0; trk4.Maximum = 100;
            trk4.Visible = false;
            chkLuminance.Enabled = false;

            switch (sys)
            {
                case "RGB":
                    lblCh1.Text = "R"; lblCh2.Text = "G"; lblCh3.Text = "B";
                    trkR.Minimum = 0; trkR.Maximum = 255;
                    trkG.Minimum = 0; trkG.Maximum = 255;
                    trkB.Minimum = 0; trkB.Maximum = 255;
                    trk4.Visible = false;
                    chkLuminance.Enabled = false;
                    break;
                case "CMYK":
                    lblCh1.Text = "C"; lblCh2.Text = "M"; lblCh3.Text = "Y"; lblCh4.Text = "K";
                    trkR.Minimum = 0; trkR.Maximum = 100;
                    trkG.Minimum = 0; trkG.Maximum = 100;
                    trkB.Minimum = 0; trkB.Maximum = 100;
                    trk4.Minimum = 0; trk4.Maximum = 100;
                    trk4.Visible = true;
                    chkLuminance.Enabled = false;
                    break;
                case "HSV":
                    lblCh1.Text = "H"; lblCh2.Text = "S"; lblCh3.Text = "V";
                    trkR.Minimum = 0; trkR.Maximum = 360; // Hue
                    trkG.Minimum = 0; trkG.Maximum = 100; // Saturation %
                    trkB.Minimum = 0; trkB.Maximum = 100; // Value %
                    trk4.Visible = false;
                    chkLuminance.Enabled = false;
                    break;
                case "YUV":
                    lblCh1.Text = "Y"; lblCh2.Text = "U"; lblCh3.Text = "V";
                    trkR.Minimum = 0; trkR.Maximum = 255; // Y
                    trkG.Minimum = -128; trkG.Maximum = 127; // U
                    trkB.Minimum = -128; trkB.Maximum = 127; // V
                    trk4.Visible = false;
                    chkLuminance.Enabled = true;
                    break;
                case "LAB":
                    lblCh1.Text = "L"; lblCh2.Text = "a"; lblCh3.Text = "b";
                    trkR.Minimum = 0; trkR.Maximum = 100; // L
                    trkG.Minimum = -128; trkG.Maximum = 127; // a
                    trkB.Minimum = -128; trkB.Maximum = 127; // b
                    trk4.Visible = false;
                    chkLuminance.Enabled = true;
                    break;
                case "YCbCr":
                    lblCh1.Text = "Y"; lblCh2.Text = "Cb"; lblCh3.Text = "Cr";
                    trkR.Minimum = 0; trkR.Maximum = 255; // Y
                    trkG.Minimum = -128; trkG.Maximum = 127; // Cb
                    trkB.Minimum = -128; trkB.Maximum = 127; // Cr
                    trk4.Visible = false;
                    chkLuminance.Enabled = true;
                    break;
            }
        }
    }
}
