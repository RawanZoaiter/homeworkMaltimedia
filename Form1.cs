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
//using Emgu.CV.CvEnum;

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
            if (colorSpaceVisualizer != null)
                colorSpaceVisualizer.ActiveSystem = cmbColorSystem.SelectedItem?.ToString() ?? "RGB";
            colorSpaceVisualizer.OnColorPicked += ColorPickedFrom3D;
        }
        private void SetupEventHandlers()
        {
            picDisplay.DragEnter += PicDisplay_DragEnter;
            picDisplay.DragDrop += PicDisplay_DragDrop;
            picDisplay.MouseMove += PicDisplay_MouseMove;
            picDisplay.MouseClick += PicDisplay_MouseClick;

            trkR.Scroll += (s, e) => { UpdateLabels(); ApplyColorTransform(); };
            trkG.Scroll += (s, e) => { UpdateLabels(); ApplyColorTransform(); };
            trkB.Scroll += (s, e) => { UpdateLabels(); ApplyColorTransform(); };
            trk4.Scroll += (s, e) => { UpdateLabels(); ApplyColorTransform(); };

            checkBox1.CheckedChanged += (s, e) => { trkR.Enabled = checkBox1.Checked; ApplyColorTransform(); };
            checkBox2.CheckedChanged += (s, e) => { trkG.Enabled = checkBox2.Checked; ApplyColorTransform(); };
            checkBox3.CheckedChanged += (s, e) => { trkB.Enabled = checkBox3.Checked; ApplyColorTransform(); };
            checkBox4.CheckedChanged += (s, e) => { trk4.Enabled = checkBox4.Checked; ApplyColorTransform(); };

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
            if (colorSpaceVisualizer != null)
                colorSpaceVisualizer.ActiveSystem = cmbColorSystem.SelectedItem?.ToString() ?? "RGB";
        }


        private void UpdateColorSystemUI()
        {
            var sys = cmbColorSystem.SelectedItem?.ToString() ?? "RGB";
            // reset defaults
            checkBox1.Checked = true;
            checkBox2.Checked = true;
            checkBox3.Checked = true;
            checkBox4.Checked = true;

            // Set wide minimums and maximums temporarily to allow Value to reset to 0 without errors
            trkR.Minimum = -1000; trkR.Maximum = 1000;
            trkG.Minimum = -1000; trkG.Maximum = 1000;
            trkB.Minimum = -1000; trkB.Maximum = 1000;
            trk4.Minimum = -1000; trk4.Maximum = 1000;

            trkR.Value = 0; trkG.Value = 0; trkB.Value = 0; trk4.Value = 0;
            trk4.Visible = false;

            switch (sys)
            {
                case "RGB":
                    lblCh1.Tag = "R"; lblCh2.Tag = "G"; lblCh3.Tag = "B"; lblCh4.Tag = "";
                    trkR.Minimum = -255; trkR.Maximum = 255;
                    trkG.Minimum = -255; trkG.Maximum = 255;
                    trkB.Minimum = -255; trkB.Maximum = 255;
                    trk4.Visible = false;

                    break;
                case "CMYK":
                    lblCh1.Tag = "C"; lblCh2.Tag = "M"; lblCh3.Tag = "Y"; lblCh4.Tag = "K";
                    trkR.Minimum = -255; trkR.Maximum = 255;
                    trkG.Minimum = -255; trkG.Maximum = 255;
                    trkB.Minimum = -255; trkB.Maximum = 255;
                    trk4.Minimum = -255; trk4.Maximum = 255;
                    trk4.Visible = true;

                    break;
                case "HSV":
                    lblCh1.Tag = "H"; lblCh2.Tag = "S"; lblCh3.Tag = "V"; lblCh4.Tag = "";
                    trkR.Minimum = -360; trkR.Maximum = 360; // Hue
                    trkG.Minimum = -100; trkG.Maximum = 100; // Saturation %
                    trkB.Minimum = -100; trkB.Maximum = 100; // Value %
                    trk4.Visible = false;
                    break;
                case "YUV":
                    lblCh1.Tag = "Y"; lblCh2.Tag = "U"; lblCh3.Tag = "V"; lblCh4.Tag = "";
                    trkR.Minimum = -255; trkR.Maximum = 255; // Y
                    trkG.Minimum = -255; trkG.Maximum = 255; // U
                    trkB.Minimum = -255; trkB.Maximum = 255; // V
                    trk4.Visible = false;

                    break;
                case "LAB":
                    lblCh1.Tag = "L"; lblCh2.Tag = "A"; lblCh3.Tag = "B"; lblCh4.Tag = "";
                    trkR.Minimum = -255; trkR.Maximum = 255; // L
                    trkG.Minimum = -255; trkG.Maximum = 255; // a
                    trkB.Minimum = -255; trkB.Maximum = 255; // b
                    trk4.Visible = false;

                    break;
                case "YCbCr":
                    lblCh1.Tag = "Y"; lblCh2.Tag = "Cb"; lblCh3.Tag = "Cr"; lblCh4.Tag = "";
                    trkR.Minimum = -255; trkR.Maximum = 255; // Y
                    trkG.Minimum = -255; trkG.Maximum = 255; // Cb
                    trkB.Minimum = -255; trkB.Maximum = 255; // Cr
                    trk4.Visible = false;

                    break;
            }

            checkBox4.Visible = trk4.Visible;
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            if (lblCh1.Tag != null) lblCh1.Text = $"{lblCh1.Tag}: {trkR.Value}";
            if (lblCh2.Tag != null) lblCh2.Text = $"{lblCh2.Tag}: {trkG.Value}";
            if (lblCh3.Tag != null) lblCh3.Text = $"{lblCh3.Tag}: {trkB.Value}";
            if (lblCh4.Tag != null && trk4.Visible) lblCh4.Text = $"{lblCh4.Tag}: {trk4.Value}";
            else lblCh4.Text = "";
        }

        private void ApplyColorTransform()
        {
            if (originalBitmap == null) return;

            Bitmap bmp = new Bitmap(originalBitmap);
            string selectedSystem = cmbColorSystem.SelectedItem?.ToString() ?? "RGB";

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            int bytesCount = data.Stride * bmp.Height;
            byte[] pixels = new byte[bytesCount];

            Marshal.Copy(data.Scan0, pixels, 0, bytesCount);

            int val1 = trkR.Value;
            int val2 = trkG.Value;
            int val3 = trkB.Value;
            int val4 = trk4.Value;

            bool chk1 = checkBox1.Checked;
            bool chk2 = checkBox2.Checked;
            bool chk3 = checkBox3.Checked;
            bool chk4 = checkBox4.Checked;

            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte b = pixels[i];
                byte g = pixels[i + 1];
                byte r = pixels[i + 2];

                Color currentPixelColor = Color.FromArgb(r, g, b);
                int outR = r, outG = g, outB = b;

                switch (selectedSystem)
                {
                    case "RGB":
                        outR = chk1 ? ColorHelper.Clamp(r + val1) : 0;
                        outG = chk2 ? ColorHelper.Clamp(g + val2) : 0;
                        outB = chk3 ? ColorHelper.Clamp(b + val3) : 0;
                        break;
                    case "HSV":
                        Color hsv = ColorHelper.RGBToHSV(currentPixelColor);
                        double H = (hsv.R / 255.0) * 360;
                        double S = hsv.G / 255.0;
                        double V = hsv.B / 255.0;

                        if (chk1)
                        {
                            H += val1;
                            if (H < 0) H += 360;
                            if (H >= 360) H %= 360;
                        }
                        else S = 0;

                        if (chk2) S = Math.Max(0, Math.Min(1, S + (val2 / 100.0)));
                        else S = 0;

                        if (chk3) V = Math.Max(0, Math.Min(1, V + (val3 / 100.0)));
                        else V = 0;

                        Color resHSV = ColorHelper.HSVToRGB(H, S, V);
                        outR = resHSV.R; outG = resHSV.G; outB = resHSV.B;
                        break;
                    case "CMYK":
                        double C, M, Y_c, K;
                        ColorHelper.RGBToCMYK_Ext(currentPixelColor, out C, out M, out Y_c, out K);

                        if (chk1) C = Math.Max(0, Math.Min(1, C + val1 / 255.0)); else C = 0;
                        if (chk2) M = Math.Max(0, Math.Min(1, M + val2 / 255.0)); else M = 0;
                        if (chk3) Y_c = Math.Max(0, Math.Min(1, Y_c + val3 / 255.0)); else Y_c = 0;
                        if (chk4) K = Math.Max(0, Math.Min(1, K + val4 / 255.0)); else K = 0;

                        Color resCMYK = ColorHelper.CMYKToRGB(C, M, Y_c, K);
                        outR = resCMYK.R; outG = resCMYK.G; outB = resCMYK.B;
                        break;
                    case "YUV":
                        Color yuv = ColorHelper.RGBToYUV(currentPixelColor);
                        int yy = yuv.R;
                        int uu = yuv.G - 128;
                        int vv = yuv.B - 128;

                        if (chk1) yy = ColorHelper.Clamp(yy + val1); else yy = 128;
                        if (chk2) uu = ColorHelper.Clamp(uu + val2 + 128) - 128; else uu = 0;
                        if (chk3) vv = ColorHelper.Clamp(vv + val3 + 128) - 128; else vv = 0;

                        Color resYUV = ColorHelper.YUVToRGB(yy, uu, vv);
                        outR = resYUV.R; outG = resYUV.G; outB = resYUV.B;
                        break;
                    case "LAB":
                        Color lab = ColorHelper.RGBToLAB(currentPixelColor);
                        double l = lab.R / 2.55;
                        int aa = lab.G - 128;
                        int bb = lab.B - 128;

                        if (chk1) l = Math.Max(0, Math.Min(100, l + val1 / 2.55)); else l = 50;
                        if (chk2) aa = ColorHelper.Clamp(aa + val2 + 128) - 128; else aa = 0;
                        if (chk3) bb = ColorHelper.Clamp(bb + val3 + 128) - 128; else bb = 0;

                        Color resLAB = ColorHelper.LABToRGB(l, aa, bb);
                        outR = resLAB.R; outG = resLAB.G; outB = resLAB.B;
                        break;
                    case "YCbCr":
                        Color ycbcr = ColorHelper.RGBToYCbCr(currentPixelColor);
                        int y_cbcr = ycbcr.R;
                        int cb = ycbcr.G - 128;
                        int cr = ycbcr.B - 128;

                        if (chk1) y_cbcr = ColorHelper.Clamp(y_cbcr + val1); else y_cbcr = 128;
                        if (chk2) cb = ColorHelper.Clamp(cb + val2 + 128) - 128; else cb = 0;
                        if (chk3) cr = ColorHelper.Clamp(cr + val3 + 128) - 128; else cr = 0;

                        Color resYCbCr = ColorHelper.YCbCrToRGB(y_cbcr, cb, cr);
                        outR = resYCbCr.R; outG = resYCbCr.G; outB = resYCbCr.B;
                        break;
                }

                pixels[i] = (byte)outB;
                pixels[i + 1] = (byte)outG;
                pixels[i + 2] = (byte)outR;
            }

            Marshal.Copy(pixels, 0, data.Scan0, bytesCount);

            bmp.UnlockBits(data);

            picDisplay.Image?.Dispose();
            picDisplay.Image = bmp;
        }


        private void btnReduceColors_Click(object sender, EventArgs e)
        {
            if (originalBitmap == null) return;

            int levels = (int)numColors.Value;
            if (levels < 2) levels = 2;

            Bitmap bmp = new Bitmap(originalBitmap);
            int width = bmp.Width;
            int height = bmp.Height;

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

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
        // 5
        private void PicDisplay_MouseClick(object sender, MouseEventArgs e)
        {
            if (picDisplay.Image == null) return;

            Bitmap bmp = new Bitmap(picDisplay.Image);

            int x = e.X;
            int y = e.Y;

            if (x < 0 || y < 0 || x >= bmp.Width || y >= bmp.Height)
                return;

            Color pixel = bmp.GetPixel(x, y);

            ShowPixelValues(pixel);
        }

        private void ShowPixelValues(Color pixel)
        {
            int r = pixel.R;
            int g = pixel.G;
            int b = pixel.B;

            // ===== HSV =====
            var hsv = ColorHelper.RGBToHSV(pixel);
            double H = (hsv.R / 255.0) * 360;
            double S = hsv.G / 255.0;
            double V = hsv.B / 255.0;

            // ===== CMYK =====
            double C, M, Y, K;
            ColorHelper.RGBToCMYK_Ext(pixel, out C, out M, out Y, out K);

            // ===== YUV =====
            Color yuv = ColorHelper.RGBToYUV(pixel);
            int Yyuv = yuv.R;
            int U = yuv.G - 128;
            int Vv = yuv.B - 128;

            // ===== LAB =====
            Color lab = ColorHelper.RGBToLAB(pixel);
            double L = lab.R / 2.55;
            int A = lab.G - 128;
            int B = lab.B - 128;

            // ===== YCbCr =====
            Color ycbcr = ColorHelper.RGBToYCbCr(pixel);
            int Y2 = ycbcr.R;
            int Cb = ycbcr.G;
            int Cr = ycbcr.B;


            lblStatusPixel.Text =
                $"RGB → ({r}, {g}, {b})\n" +
                $"HSV → ({Math.Round(H)}°, {Math.Round(S * 100)}%, {Math.Round(V * 100)}%)\n" +
                $"CMYK → ({Math.Round(C * 100)}%, {Math.Round(M * 100)}%, {Math.Round(Y * 100)}%, {Math.Round(K * 100)}%)\n" +
                $"YUV → ({Yyuv}, {U}, {Vv})\n" +
                $"LAB → ({Math.Round(L)}, {A}, {B})\n" +
                $"YCbCr → ({Y2}, {Cb}, {Cr})";
        }

        private void ColorPickedFrom3D(Color pixel)
        {
            ShowPixelValues(pixel);
        }
    }
}
