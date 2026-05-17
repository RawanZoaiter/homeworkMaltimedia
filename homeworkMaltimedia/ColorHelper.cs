using System;
using System.Drawing;

namespace homeworkMaltimedia
{
    public static class ColorHelper
    {
        public static int Clamp(int value)
        {
            if (value < 0) return 0;
            if (value > 255) return 255;
            return value;
        }

        public static Color RGBToHSV(Color c)
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

        public static Color RGBToYUV(Color c)
        {
            double y = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
            double u = -0.14713 * c.R - 0.28886 * c.G + 0.436 * c.B;
            double v = 0.615 * c.R - 0.51499 * c.G - 0.10001 * c.B;

            return Color.FromArgb(Clamp((int)y), Clamp((int)(u + 128)), Clamp((int)(v + 128)));
        }

        public static Color RGBToLAB(Color c)
        {
            double l = (0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B) / 2.55;
            double a = (c.R - c.G) + 128;
            double b = (c.G - c.B) + 128;

            return Color.FromArgb(Clamp((int)(l * 2.55)), Clamp((int)a), Clamp((int)b));
        }

        public static Color RGBToYCbCr(Color c)
        {
            double Y = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
            double Cb = -0.168736 * c.R - 0.331264 * c.G + 0.5 * c.B + 128;
            double Cr = 0.5 * c.R - 0.418688 * c.G - 0.081312 * c.B + 128;

            return Color.FromArgb(Clamp((int)Y), Clamp((int)Cb), Clamp((int)Cr));
        }

        public static Color RGBToCMYK(Color c)
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

        public static void RGBToCMYK_Ext(Color c, out double cyan, out double magenta, out double yellow, out double black)
        {
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            black = 1.0 - Math.Max(r, Math.Max(g, b));
            cyan = (1 - r - black) / (1 - black + 0.000001);
            magenta = (1 - g - black) / (1 - black + 0.000001);
            yellow = (1 - b - black) / (1 - black + 0.000001);
        }

        public static Color CMYKToRGB(double c, double m, double y, double k)
        {
            int r = (int)(255 * (1 - c) * (1 - k));
            int g = (int)(255 * (1 - m) * (1 - k));
            int b = (int)(255 * (1 - y) * (1 - k));
            return Color.FromArgb(Clamp(r), Clamp(g), Clamp(b));
        }

        public static Color HSVToRGB(double h, double s, double v)
        {
            int hi = Convert.ToInt32(Math.Floor(h / 60)) % 6;
            double f = h / 60 - Math.Floor(h / 60);

            v = v * 255;
            int r = Clamp((int)v);
            int p = Clamp((int)(v * (1 - s)));
            int q = Clamp((int)(v * (1 - f * s)));
            int t = Clamp((int)(v * (1 - (1 - f) * s)));

            if (hi == 0) return Color.FromArgb(r, t, p);
            else if (hi == 1) return Color.FromArgb(q, r, p);
            else if (hi == 2) return Color.FromArgb(p, r, t);
            else if (hi == 3) return Color.FromArgb(p, q, r);
            else if (hi == 4) return Color.FromArgb(t, p, r);
            else return Color.FromArgb(r, p, q);
        }

        public static Color YUVToRGB(double y, double u, double v)
        {
            int r = (int)(y + 1.13983 * v);
            int g = (int)(y - 0.39465 * u - 0.58060 * v);
            int b = (int)(y + 2.03211 * u);
            return Color.FromArgb(Clamp(r), Clamp(g), Clamp(b));
        }

        public static Color YCbCrToRGB(double y, double cb, double cr)
        {
            int r = (int)(y + 1.402 * cr);
            int g = (int)(y - 0.344136 * cb - 0.714136 * cr);
            int b = (int)(y + 1.772 * cb);
            return Color.FromArgb(Clamp(r), Clamp(g), Clamp(b));
        }

        public static Color LABToRGB(double l, double a, double b)
        {
            double g = l * 2.55 - 0.2126 * a + 0.0722 * b;
            double r = g + a;
            double bb = g - b;
            return Color.FromArgb(Clamp((int)r), Clamp((int)g), Clamp((int)bb));
        }
    }
}