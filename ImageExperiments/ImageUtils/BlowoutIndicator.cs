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

        public void CreateBlowoutImages()
        {
            string folder = _source.Replace(Path.GetFileName(_source), "");
            string noExt = Path.GetFileNameWithoutExtension(_source);
            string ext = Path.GetExtension(_source);
            string white = folder +  noExt + ".white." + ext;
            string black = folder + noExt + ".black." + ext;

            Bitmap[] images = CreateImages();
            images[0].Save(white);
            images[1].Save(black);

            // animated gif code below isn't working
            /* 
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
            */
        }


        Bitmap[] CreateImages()
        {
            List<Bitmap> imag = new List<Bitmap>();
            Bitmap original = GetSmallImage();
            imag.Add(original);
            var clone = original.Clone();
            Bitmap black = DrawBlowouts((Bitmap)clone, Color.Black);
            imag.Add(black);
            return imag.ToArray();
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
                            gfx.DrawLine(pen, x, y, x + 1, y + 1);
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
            


            Bitmap image = new Bitmap(_source);

            //float max = 150;
            float width = 150;
            float height = 100;
            var brush = new SolidBrush(Color.Black);

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

            return bmp;

        }


    }
}
