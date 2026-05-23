using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace homeworkMaltimedia
{
    public class ColorSpaceVisualizer : UserControl
    {
        public event Action<Color> OnColorPicked;
        private string _activeSystem = "RGB";
        private float _yaw = 0.6f;
        private float _pitch = 0.5f;
        private float _zoom = 1.0f;
        private bool _dragging;
        private Point _lastMouse;
        private const int RgbSamplesPerAxis = 8;

        public ColorSpaceVisualizer()
        {
            DoubleBuffered = true;
            BackColor = Color.FromArgb(30, 30, 30);
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint
                     | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        }

        public string ActiveSystem
        {
            get { return _activeSystem; }
            set
            {
                var v = string.IsNullOrEmpty(value) ? "RGB" : value;
                if (_activeSystem != v)
                {
                    _activeSystem = v;
                    Invalidate();
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _lastMouse = e.Location;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left) _dragging = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_dragging)
            {
                _yaw += (e.X - _lastMouse.X) * 0.01f;
                _pitch += (e.Y - _lastMouse.Y) * 0.01f;
                if (_pitch > 1.5f) _pitch = 1.5f;
                if (_pitch < -1.5f) _pitch = -1.5f;
                _lastMouse = e.Location;
                Invalidate();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            _zoom *= (e.Delta > 0) ? 1.1f : 0.9f;
            if (_zoom < 0.3f) _zoom = 0.3f;
            if (_zoom > 4.0f) _zoom = 4.0f;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            switch ((_activeSystem ?? "RGB").ToUpperInvariant())
            {
                case "RGB": DrawRgbCube(g); break;
                case "HSV": DrawHsvPlaceholder(g); break;
                case "CMYK": DrawCmykPlaceholder(g); break;
                case "YUV": DrawYuvSpace(g, false); break;
                case "YCBCR": DrawYuvSpace(g, true); break;
                case "LAB": DrawLabSpace(g); break;
                default: DrawRgbCube(g); break;
            }

            using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.White))
                g.DrawString("Color Space: " + _activeSystem, font, brush, 8, 8);

            using (var font = new Font("Segoe UI", 8))
            using (var brush = new SolidBrush(Color.LightGray))
                g.DrawString("Drag = rotate, Wheel = zoom", font, brush, 8, Height - 22);
        }

        private PointF Project(float x, float y, float z)
        {
            // Center coordinates around origin (cube is [0..1])
            float cx = x - 0.5f, cy = y - 0.5f, cz = z - 0.5f;

            // Yaw around Y, pitch around X
            float cosY = (float)Math.Cos(_yaw), sinY = (float)Math.Sin(_yaw);
            float cosP = (float)Math.Cos(_pitch), sinP = (float)Math.Sin(_pitch);

            float x1 = cx * cosY + cz * sinY;
            float z1 = -cx * sinY + cz * cosY;
            float y2 = cy * cosP - z1 * sinP;
            // z2 unused for orthographic projection

            float scale = Math.Min(Width, Height) * 0.55f * _zoom;
            float px = Width / 2f + x1 * scale;
            float py = Height / 2f + y2 * scale;
            return new PointF(px, py);
        }

        private float Depth(float x, float y, float z)
        {
            float cx = x - 0.5f, cy = y - 0.5f, cz = z - 0.5f;
            float cosY = (float)Math.Cos(_yaw), sinY = (float)Math.Sin(_yaw);
            float cosP = (float)Math.Cos(_pitch), sinP = (float)Math.Sin(_pitch);
            float z1 = -cx * sinY + cz * cosY;
            float z2 = cy * sinP + z1 * cosP;
            return z2;
        }

        private void DrawRgbCube(Graphics g)
        {
            int n = RgbSamplesPerAxis;
            var points = new List<Sample>(n * n * n);

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (int k = 0; k < n; k++)
                    {
                        float fx = i / (float)(n - 1);
                        float fy = j / (float)(n - 1);
                        float fz = k / (float)(n - 1);

                        points.Add(new Sample
                        {
                            X = fx,
                            Y = fy,
                            Z = fz,
                            D = Depth(fx, fy, fz),

                            Color = ToColor(fx, fy, fz)
                        });
                    }

            DrawCubeFrame(g, false);
            DrawSortedSamples(g, points, n);
            DrawAxes(g, "R", "G", "B",
                Color.Red, Color.LimeGreen, Color.DeepSkyBlue);
        }
        private void DrawSortedSamples(Graphics g, List<Sample> points, int density)
        {
            points.Sort((a, b) => a.D.CompareTo(b.D));
            float size = Math.Max(3f, Math.Min(Width, Height) / (density * 3f) * _zoom);
            foreach (var s in points)
            {
                var p = Project(s.X, s.Y, s.Z);
                using (var br = new SolidBrush(s.Color))
                    g.FillRectangle(br, p.X - size / 2f, p.Y - size / 2f, size, size);
            }
        }

        private void DrawCubeFrame(Graphics g, bool front)
        {
            var corners = new PointF[8];
            for (int i = 0; i < 8; i++)
            {
                float x = (i & 1);
                float y = ((i >> 1) & 1);
                float z = ((i >> 2) & 1);
                corners[i] = Project(x, y, z);
            }
            int[,] edges = {
                {0,1},{2,3},{4,5},{6,7},
                {0,2},{1,3},{4,6},{5,7},
                {0,4},{1,5},{2,6},{3,7}
            };
            using (var pen = new Pen(Color.FromArgb(120, Color.White), 1f))
            {
                for (int i = 0; i < edges.GetLength(0); i++)
                    g.DrawLine(pen, corners[edges[i, 0]], corners[edges[i, 1]]);
            }
        }

        private void DrawAxes(Graphics g, string xLabel, string yLabel, string zLabel,
                              Color xCol, Color yCol, Color zCol)
        {
            var origin = Project(0, 0, 0);
            var xEnd = Project(1.15f, 0, 0);
            var yEnd = Project(0, 1.15f, 0);
            var zEnd = Project(0, 0, 1.15f);
            using (var penX = new Pen(xCol, 2f))
            using (var penY = new Pen(yCol, 2f))
            using (var penZ = new Pen(zCol, 2f))
            {
                g.DrawLine(penX, origin, xEnd);
                g.DrawLine(penY, origin, yEnd);
                g.DrawLine(penZ, origin, zEnd);
            }
            using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
            {
                using (var br = new SolidBrush(xCol)) g.DrawString(xLabel, font, br, xEnd);
                using (var br = new SolidBrush(yCol)) g.DrawString(yLabel, font, br, yEnd);
                using (var br = new SolidBrush(zCol)) g.DrawString(zLabel, font, br, zEnd);
            }
        }

        private void DrawHsvPlaceholder(Graphics g)
        {
            // HSV cylinder: Hue = angle in XZ plane, Saturation = radius, Value = vertical (Y)
            const int hueSteps = 24;
            const int satSteps = 5;
            const int valSteps = 6;
            var points = new List<Sample>(hueSteps * satSteps * valSteps);
            for (int vi = 0; vi < valSteps; vi++)
            {
                float v = vi / (float)(valSteps - 1);
                for (int si = 0; si < satSteps; si++)
                {
                    float s = si / (float)(satSteps - 1);
                    int hCount = (si == 0) ? 1 : hueSteps;
                    for (int hi = 0; hi < hCount; hi++)
                    {
                        double h = (hi * 360.0) / hueSteps;
                        double rad = h * Math.PI / 180.0;
                        float fx = 0.5f + (float)Math.Cos(rad) * 0.5f * s;
                        float fz = 0.5f + (float)Math.Sin(rad) * 0.5f * s;
                        float fy = v;
                        points.Add(new Sample
                        {
                            X = fx,
                            Y = fy,
                            Z = fz,
                            D = Depth(fx, fy, fz),
                            Color = HsvToColor(h, s, Math.Max(0.05, v))
                        });
                    }
                }
            }
            DrawCubeFrame(g, false);
            DrawSortedSamples(g, points, 7);
            // Vertical V axis through center, H ring at top, S radius marker
            using (var penV = new Pen(Color.White, 2f))
            {
                g.DrawLine(penV, Project(0.5f, 0f, 0.5f), Project(0.5f, 1.15f, 0.5f));
            }
            using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
            using (var brW = new SolidBrush(Color.White))
                g.DrawString("V", font, brW, Project(0.5f, 1.15f, 0.5f));
            DrawSystemLabel(g, "HSV (Hue = angle, Saturation = radius, Value = vertical)");
        }

        private void DrawCmykPlaceholder(Graphics g)
        {
            // Subtractive cube: axes = C, M, Y. K shown as a smaller inset darker cube.
            int n = 7;
            var points = new List<Sample>(n * n * n);
            float kOuter = 0f;
            AddCmykLayer(points, n, kOuter, 0f, 1f);
            // Inner layer with K=0.5 to visualize the K dimension
            AddCmykLayer(points, 5, 0.5f, 0.15f, 0.85f);

            DrawCubeFrame(g, false);
            DrawSortedSamples(g, points, n);
            DrawAxes(g, "C", "M", "Y", Color.Cyan, Color.Magenta, Color.Yellow);
            DrawSystemLabel(g, "CMYK (C/M/Y axes; inner darker cube shows K=0.5 layer)");
        }

        private void AddCmykLayer(List<Sample> points, int n, float k, float lo, float hi)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (int m = 0; m < n; m++)
                    {
                        float c = i / (float)(n - 1);
                        float mg = j / (float)(n - 1);
                        float y = m / (float)(n - 1);
                        float fx = lo + (hi - lo) * c;
                        float fy = lo + (hi - lo) * mg;
                        float fz = lo + (hi - lo) * y;
                        int rr = (int)((1 - c) * (1 - k) * 255);
                        int gg = (int)((1 - mg) * (1 - k) * 255);
                        int bb = (int)((1 - y) * (1 - k) * 255);
                        points.Add(new Sample
                        {
                            X = fx,
                            Y = fy,
                            Z = fz,
                            D = Depth(fx, fy, fz),
                            Color = Color.FromArgb(Clamp255(rr), Clamp255(gg), Clamp255(bb))
                        });
                    }
        }

        private void DrawYuvSpace(Graphics g, bool ycbcr)
        {
            // X = U/Cb, Y = Y (luma vertical), Z = V/Cr
            int n = 7;
            var points = new List<Sample>(n * n * n);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (int m = 0; m < n; m++)
                    {
                        float u = i / (float)(n - 1);
                        float yl = j / (float)(n - 1);
                        float v = m / (float)(n - 1);
                        Color col = ycbcr
                            ? YCbCrToRgb(yl, u, v)
                            : YuvToRgb(yl, u - 0.5f, v - 0.5f);
                        points.Add(new Sample
                        {
                            X = u,
                            Y = yl,
                            Z = v,
                            D = Depth(u, yl, v),
                            Color = col
                        });
                    }
            DrawCubeFrame(g, false);
            DrawSortedSamples(g, points, n);
            if (ycbcr)
            {
                DrawAxes(g, "Cb", "Y", "Cr", Color.DeepSkyBlue, Color.Gainsboro, Color.OrangeRed);
                DrawSystemLabel(g, "YCbCr (Y vertical; Cb/Cr chroma plane)");
            }
            else
            {
                DrawAxes(g, "U", "Y", "V", Color.DeepSkyBlue, Color.Gainsboro, Color.OrangeRed);
                DrawSystemLabel(g, "YUV (Y vertical; U/V chroma plane)");
            }
        }

        private void DrawLabSpace(Graphics g)
        {
            // X = a (-128..127), Y = L (0..100), Z = b (-128..127)
            int n = 7;
            var points = new List<Sample>(n * n * n);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (int m = 0; m < n; m++)
                    {
                        float ax = i / (float)(n - 1);
                        float ly = j / (float)(n - 1);
                        float bz = m / (float)(n - 1);
                        double L = ly * 100.0;
                        double a = (ax - 0.5) * 200.0;
                        double b = (bz - 0.5) * 200.0;
                        points.Add(new Sample
                        {
                            X = ax,
                            Y = ly,
                            Z = bz,
                            D = Depth(ax, ly, bz),
                            Color = LabToRgb(L, a, b)
                        });
                    }
            DrawCubeFrame(g, false);
            DrawSortedSamples(g, points, n);
            DrawAxes(g, "a", "L", "b", Color.HotPink, Color.Gainsboro, Color.Gold);
            DrawSystemLabel(g, "LAB (L vertical; a = green↔red, b = blue↔yellow)");
        }

        private static int Clamp255(int v) { return v < 0 ? 0 : (v > 255 ? 255 : v); }

        private static Color YuvToRgb(float y, float u, float v)
        {
            float r = y + 1.13983f * v;
            float gC = y - 0.39465f * u - 0.58060f * v;
            float b = y + 2.03211f * u;
            return Color.FromArgb(Clamp255((int)(r * 255)), Clamp255((int)(gC * 255)), Clamp255((int)(b * 255)));
        }

        private static Color YCbCrToRgb(float y, float cb, float cr)
        {
            float r = y + 1.402f * (cr - 0.5f);
            float gC = y - 0.344136f * (cb - 0.5f) - 0.714136f * (cr - 0.5f);
            float b = y + 1.772f * (cb - 0.5f);
            return Color.FromArgb(Clamp255((int)(r * 255)), Clamp255((int)(gC * 255)), Clamp255((int)(b * 255)));
        }

        private static Color LabToRgb(double L, double a, double b)
        {
            double fy = (L + 16.0) / 116.0;
            double fx = fy + a / 500.0;
            double fz = fy - b / 200.0;
            double xn = 0.95047, yn = 1.0, zn = 1.08883;
            double X = xn * LabFInv(fx);
            double Y = yn * LabFInv(fy);
            double Z = zn * LabFInv(fz);
            double r = 3.2406 * X - 1.5372 * Y - 0.4986 * Z;
            double g = -0.9689 * X + 1.8758 * Y + 0.0415 * Z;
            double bl = 0.0557 * X - 0.2040 * Y + 1.0570 * Z;
            r = SrgbCompand(r);
            g = SrgbCompand(g);
            bl = SrgbCompand(bl);
            return Color.FromArgb(
                Clamp255((int)(r * 255)),
                Clamp255((int)(g * 255)),
                Clamp255((int)(bl * 255)));
        }

        private static double LabFInv(double t)
        {
            double d = 6.0 / 29.0;
            return (t > d) ? t * t * t : 3 * d * d * (t - 4.0 / 29.0);
        }

        private static double SrgbCompand(double t)
        {
            if (t <= 0) return 0;
            if (t >= 1) return 1;
            return (t > 0.0031308) ? 1.055 * Math.Pow(t, 1.0 / 2.4) - 0.055 : 12.92 * t;
        }

        private void DrawSystemLabel(Graphics g, string text)
        {
            using (var font = new Font("Segoe UI", 9, FontStyle.Italic))
            using (var br = new SolidBrush(Color.Gainsboro))
                g.DrawString(text, font, br, 8, Height - 40);
        }

        private static Color HsvToColor(double h, double s, double v)
        {
            double c = v * s;
            double hp = h / 60.0;
            double x = c * (1 - Math.Abs(hp % 2 - 1));
            double r = 0, gC = 0, b = 0;
            if (hp < 1) { r = c; gC = x; }
            else if (hp < 2) { r = x; gC = c; }
            else if (hp < 3) { gC = c; b = x; }
            else if (hp < 4) { gC = x; b = c; }
            else if (hp < 5) { r = x; b = c; }
            else { r = c; b = x; }
            double m = v - c;
            return Color.FromArgb(
                (int)Math.Max(0, Math.Min(255, (r + m) * 255)),
                (int)Math.Max(0, Math.Min(255, (gC + m) * 255)),
                (int)Math.Max(0, Math.Min(255, (b + m) * 255)));
        }

        private struct Sample
        {
            public float X, Y, Z, D;
            public Color Color;
        }

         // 5//
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            Color picked = PickColorFromScreen(e.Location);
            OnColorPicked?.Invoke(picked);
        }


        


        private Color MapToRGB(float x, float y, float z)
        {
            switch (_activeSystem.ToUpper())
            {
                case "RGB":
                    return Color.FromArgb(
                        (int)(x * 255),
                        (int)(y * 255),
                        (int)(z * 255));

                case "HSV":
                    return HsvToColor(x * 360, y, z);

                case "CMYK":
                    return CmykToRgb(x, y, z, 0f);

                case "YUV":
                    return YuvToRgb(y, x - 0.5f, z - 0.5f);

                case "YCbCr":
                    return YCbCrToRgb(y, x, z);

                case "LAB":
                    double L = y * 100;
                    double a = (x - 0.5) * 200;
                    double b = (z - 0.5) * 200;
                    return LabToRgb(L, a, b);

                default:
                    return Color.White;
            }
        }

       
        private Color CmykToRgb(float c, float m, float y, float k)
        {
            int r = (int)(255 * (1 - c) * (1 - k));
            int g = (int)(255 * (1 - m) * (1 - k));
            int b = (int)(255 * (1 - y) * (1 - k));

            return Color.FromArgb(Clamp255(r), Clamp255(g), Clamp255(b));
        }

        private Color ToColor(float x, float y, float z)
        {
            return Color.FromArgb(
                (int)(x * 255),
                (int)(y * 255),
                (int)(z * 255));
        }


        private Color PickColorFromScreen(Point p)
        {
            int n = RgbSamplesPerAxis;

            float bestScore = float.MaxValue;
            Color bestColor = Color.Black;

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (int k = 0; k < n; k++)
                    {
                        float x = i / (float)(n - 1);
                        float y = j / (float)(n - 1);
                        float z = k / (float)(n - 1);

                        var projected = Project(x, y, z);

                        float dx = projected.X - p.X;
                        float dy = projected.Y - p.Y;

                       
                        float dz = Depth(x, y, z);

                        float score = dx * dx + dy * dy + (dz * dz * 0.3f);

                        if (score < bestScore)
                        {
                            bestScore = score;
                            bestColor = MapToRGB(x, y, z);
                        }
                    }

            return bestColor;
        }
    }
}
