using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImageUtils
{
    public class BlowoutIndicator
    {
        string _source;
        


        public BlowoutIndicator(string SourceFile)
        {
            _source = SourceFile;
            
        }

        public void CreateBlowoutGif(string FilePath)
        {
            Bitmap[] images = CreateImages();

            GifBitmapEncoder gEnc = new GifBitmapEncoder();

            foreach (Bitmap bmpImage in images)
            {
                var src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bmpImage.GetHbitmap(),
                    IntPtr.Zero,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                gEnc.Frames.Add(BitmapFrame.Create(src));
            }

            gEnc.Save(new FileStream(FilePath, FileMode.Create));
        }


        Bitmap[] CreateImages()
        {
            Bitmap original = GetSmallImage();
            Bitmap black = DrawBlowouts(original, Color.Black);
            Bitmap white = DrawBlowouts(original, Color.White);
            return new Bitmap[] { black, white };
        }

        Bitmap DrawBlowouts(Bitmap bm, Color color)
        {
            // source: http://stackoverflow.com/questions/7964839/determine-image-overall-lightness
            double lum = 0;
            var tmpBmp = new Bitmap(bm);
            var width = bm.Width;
            var height = bm.Height;
            var bppModifier = bm.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;

            var srcData = tmpBmp.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, bm.PixelFormat);
            var stride = srcData.Stride;
            var scan0 = srcData.Scan0;

            //Luminance (standard, objective): (0.2126*R) + (0.7152*G) + (0.0722*B)
            //Luminance (perceived option 1): (0.299*R + 0.587*G + 0.114*B)
            //Luminance (perceived option 2, slower to calculate): sqrt( 0.241*R^2 + 0.691*G^2 + 0.068*B^2 )
            Pen pen = new Pen(color, 1f);
            Graphics gfx = Graphics.FromImage(bm);
            unsafe
            {
                byte* p = (byte*)(void*)scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = (y * stride) + x * bppModifier;
                        //                 Red                  Green                   Blue
                        lum = (0.2126 * p[idx + 2] + 0.7152 * p[idx + 1] + 0.0722 * p[idx]); // standard
                        if (lum > 250)
                        {
                            Point point = new Point(x, y);
                            gfx.DrawLine(pen, point, point);
                        }
                    }
                }
            }

            tmpBmp.UnlockBits(srcData);
            tmpBmp.Dispose();

            return bm;
        }


        /// <summary>
        /// Scales the original image down
        /// </summary>
        /// <returns></returns>
        private Bitmap GetSmallImage()
        {
            float width = 150;
            float height = 100;
            var brush = new SolidBrush(Color.Black);

            Bitmap image = new Bitmap(_source);

            float scale = Math.Min(width / image.Width, height / image.Height);

            var bmp = new Bitmap((int)width, (int)height);
            var graph = Graphics.FromImage(bmp);

            // uncomment for higher quality output
            graph.InterpolationMode = InterpolationMode.High;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.AntiAlias;

            var scaleWidth = (int)(image.Width * scale);
            var scaleHeight = (int)(image.Height * scale);

            graph.FillRectangle(brush, new RectangleF(0, 0, width, height));
            graph.DrawImage(image, new Rectangle(((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight));

            return image;

        }


    }
}
