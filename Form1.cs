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
        private string currentImagePath;
        private Bitmap originalBitmap;
        private Bitmap workingBitmap;

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
                // load image into a Bitmap to avoid file lock and to allow processing
                using (var img = Image.FromFile(path))
                {
                    originalBitmap?.Dispose();
                    workingBitmap?.Dispose();
                    originalBitmap = new Bitmap(img);
                    workingBitmap = (Bitmap)originalBitmap.Clone();
                    picDisplay.Image = (Image)workingBitmap.Clone();
                    currentImagePath = path;
                    UpdateMetadata(path, originalBitmap);
                }
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
            // Format (use extension)
            lstMetadata.Items.Add(new ListViewItem(new[] { "Format", System.IO.Path.GetExtension(path).TrimStart('.').ToUpper() }));
            lstMetadata.Items.Add(new ListViewItem(new[] { "Dimensions", $"{img.Width} x {img.Height}" }));
            // File size on disk
            try
            {
                var fi = new System.IO.FileInfo(path);
                lstMetadata.Items.Add(new ListViewItem(new[] { "File Size (bytes)", fi.Length.ToString() }));
            }
            catch { }
            // Pixel format and color depth
            if (img is Bitmap bmp)
            {
                lstMetadata.Items.Add(new ListViewItem(new[] { "Pixel Format", bmp.PixelFormat.ToString() }));
                lstMetadata.Items.Add(new ListViewItem(new[] { "Color Depth (bits)", Image.GetPixelFormatSize(bmp.PixelFormat).ToString() }));
                // unique color count (may be slow for very large images)
                int unique = GetUniqueColorCount(bmp);
                lstMetadata.Items.Add(new ListViewItem(new[] { "Unique Colors", unique.ToString() }));
            }
        }

        private int GetUniqueColorCount(Bitmap bmp)
        {
            try
            {
                var set = new System.Collections.Generic.HashSet<int>();
                // limit work for extremely large images to avoid UI freeze
                long maxPixels = (long)bmp.Width * bmp.Height;
                if (maxPixels > 4000 * 4000)
                {
                    return -1; // indicate too large to compute
                }

                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        set.Add(bmp.GetPixel(x, y).ToArgb());
                    }
                }
                return set.Count;
            }
            catch
            {
                return -1;
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (originalBitmap != null)
            {
                workingBitmap?.Dispose();
                workingBitmap = (Bitmap)originalBitmap.Clone();
                picDisplay.Image = (Image)workingBitmap.Clone();
                lblStatusPixel.Text = "Image reset to original.";
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (picDisplay.Image == null) return;
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg;*.jpeg|Bitmap Image|*.bmp|GIF Image|*.gif";
                sfd.FileName = System.IO.Path.GetFileName(currentImagePath) ?? "image";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var fmt = System.Drawing.Imaging.ImageFormat.Png;
                    var ext = System.IO.Path.GetExtension(sfd.FileName).ToLower();
                    switch (ext)
                    {
                        case ".jpg":
                        case ".jpeg": fmt = System.Drawing.Imaging.ImageFormat.Jpeg; break;
                        case ".bmp": fmt = System.Drawing.Imaging.ImageFormat.Bmp; break;
                        case ".gif": fmt = System.Drawing.Imaging.ImageFormat.Gif; break;
                        default: fmt = System.Drawing.Imaging.ImageFormat.Png; break;
                    }
                    try
                    {
                        // save current displayed image
                        using (var toSave = new Bitmap(picDisplay.Image))
                        {
                            toSave.Save(sfd.FileName, fmt);
                        }
                        lblStatusPixel.Text = $"Saved to {sfd.FileName}";
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error saving image: {ex.Message}");
                    }
                }
            }
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
                    lblCh1.Text = "R"; lblCh2.Text = "G"; lblCh3.Text = "B"; lblCh4.Text = "";
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
                    lblCh1.Text = "H"; lblCh2.Text = "S"; lblCh3.Text = "V"; lblCh4.Text = "";
                    trkR.Minimum = 0; trkR.Maximum = 360; // Hue
                    trkG.Minimum = 0; trkG.Maximum = 100; // Saturation %
                    trkB.Minimum = 0; trkB.Maximum = 100; // Value %
                    trk4.Visible = false;
                    chkLuminance.Enabled = false;
                    break;
                case "YUV":
                    lblCh1.Text = "Y"; lblCh2.Text = "U"; lblCh3.Text = "V"; lblCh4.Text = "";
                    trkR.Minimum = 0; trkR.Maximum = 255; // Y
                    trkG.Minimum = -128; trkG.Maximum = 127; // U
                    trkB.Minimum = -128; trkB.Maximum = 127; // V
                    trk4.Visible = false;
                    chkLuminance.Enabled = true;
                    break;
                case "LAB":
                    lblCh1.Text = "L"; lblCh2.Text = "A"; lblCh3.Text = "B"; lblCh4.Text = "";
                    trkR.Minimum = 0; trkR.Maximum = 100; // L
                    trkG.Minimum = -128; trkG.Maximum = 127; // a
                    trkB.Minimum = -128; trkB.Maximum = 127; // b
                    trk4.Visible = false;
                    chkLuminance.Enabled = true;
                    break;
                case "YCbCr":
                    lblCh1.Text = "Y"; lblCh2.Text = "Cb"; lblCh3.Text = "Cr"; lblCh4.Text = "";
                    trkR.Minimum = 0; trkR.Maximum = 255; // Y
                    trkG.Minimum = -128; trkG.Maximum = 127; // Cb
                    trkB.Minimum = -128; trkB.Maximum = 127; // Cr
                    trk4.Visible = false;
                    chkLuminance.Enabled = true;
                    break;
            }
        }

        

        private void trk4_Scroll(object sender, EventArgs e)
        {

        }

        private void trkG_Scroll(object sender, EventArgs e)
        {

        }

        private void lblCh3_Click(object sender, EventArgs e)
        {

        }

        private void trkR_Scroll(object sender, EventArgs e)
        {

        }

        private void picDisplay_Click(object sender, EventArgs e)
        {

        }
    }
}
