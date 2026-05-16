using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;

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
            ApplyColorTransform();
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
        private int Clamp(int value)
        {
            if (value < 0) return 0;
            if (value > 255) return 255;
            return value;
        }

        private Color RGBToHSV(Color c)
        {
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double h = 0;
            if (delta != 0)
            {
                if (max == r) h = 60 * (((g - b) / delta) % 6);
                else if (max == g) h = 60 * (((b - r) / delta) + 2);
                else if (max == b) h = 60 * (((r - g) / delta) + 4);
            }

            double s = (max == 0) ? 0 : (delta / max);
            double v = max;

            return Color.FromArgb(Clamp((int)(h * 255 / 360)), Clamp((int)(s * 255)), Clamp((int)(v * 255)));
        }

        private Color RGBToYUV(Color c)
        {
            double y = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
            double u = -0.14713 * c.R - 0.28886 * c.G + 0.436 * c.B;
            double v = 0.615 * c.R - 0.51499 * c.G - 0.10001 * c.B;

            return Color.FromArgb(Clamp((int)y), Clamp((int)(u + 128)), Clamp((int)(v + 128)));
        }

        private Color RGBToLAB(Color c)
        {
            double l = (0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B) / 2.55;
            double a = (c.R - c.G) + 128;
            double b = (c.G - c.B) + 128;

            return Color.FromArgb(Clamp((int)(l * 2.55)), Clamp((int)a), Clamp((int)b));
        }

        private Color RGBToYCbCr(Color c)
        {
            double Y = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
            double Cb = -0.168736 * c.R - 0.331264 * c.G + 0.5 * c.B + 128;
            double Cr = 0.5 * c.R - 0.418688 * c.G - 0.081312 * c.B + 128;

            return Color.FromArgb(Clamp((int)Y), Clamp((int)Cb), Clamp((int)Cr));
        }

        private Color RGBToCMYK(Color c)
        {
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            double k = 1 - Math.Max(r, Math.Max(g, b));

            double cyan = (1 - r - k) / (1 - k + 0.000001);
            double magenta = (1 - g - k) / (1 - k + 0.000001);
            double yellow = (1 - b - k) / (1 - k + 0.000001);

            return Color.FromArgb(
                Clamp((int)(cyan * 255)),
                Clamp((int)(magenta * 255)),
                Clamp((int)(yellow * 255))
            );
        }

        private void ApplyColorTransform()
        {
            if (originalBitmap == null) return;

            Bitmap bmp = new Bitmap(originalBitmap);
            string selectedSystem = cmbColorSystem.SelectedItem?.ToString() ?? "RGB";

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),ImageLockMode.ReadWrite,PixelFormat.Format32bppArgb); 

            int bytesCount = data.Stride * bmp.Height;
            byte[] pixels = new byte[bytesCount];

            Marshal.Copy(data.Scan0, pixels, 0, bytesCount);

            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte b = pixels[i];
                byte g = pixels[i + 1];
                byte r = pixels[i + 2];

                Color currentPixelColor = Color.FromArgb(r, g, b);
                Color resultColor = currentPixelColor;

                switch (selectedSystem)
                {
                    case "YCbCr": resultColor = RGBToYCbCr(currentPixelColor); break;
                    case "HSV": resultColor = RGBToHSV(currentPixelColor); break;
                    case "CMYK": resultColor = RGBToCMYK(currentPixelColor); break;
                    case "YUV": resultColor = RGBToYUV(currentPixelColor); break;
                    case "LAB": resultColor = RGBToLAB(currentPixelColor); break;
                    case "RGB": resultColor = currentPixelColor; break;
                }

                pixels[i] = resultColor.B; 
                pixels[i + 1] = resultColor.G; 
                pixels[i + 2] = resultColor.R; 
            }

            Marshal.Copy(pixels, 0, data.Scan0, bytesCount);

            bmp.UnlockBits(data);

            picDisplay.Image?.Dispose();
            picDisplay.Image = bmp;
        }

        private void numColors_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnReduceColors_Click(object sender, EventArgs e)
        {
            if (originalBitmap == null) return;

            int levels = (int)numColors.Value;
            if (levels < 2) levels = 2;

            Bitmap bmp = new Bitmap(originalBitmap);
            int width = bmp.Width;
            int height = bmp.Height;

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height),ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            int bytes = data.Stride * height;
            byte[] pixels = new byte[bytes];
            Marshal.Copy(data.Scan0, pixels, 0, bytes);

            int step = 255 / (levels - 1);

            for (int i = 0; i < pixels.Length; i += 4)
            {
                pixels[i] = (byte)((pixels[i] / step) * step);    
                pixels[i + 1] = (byte)((pixels[i + 1] / step) * step); 
                pixels[i + 2] = (byte)((pixels[i + 2] / step) * step); 
            }

            Marshal.Copy(pixels, 0, data.Scan0, bytes);
            bmp.UnlockBits(data); 

            picDisplay.Image = bmp;
        }
    }
}
