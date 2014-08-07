using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageUtils
{

    public enum MeteringAreas
    {
        Top,
        TopLeft,
        TopRight,
        FarLeft,
        Left,
        Center,
        Right,
        FarRight,
        BottomLeft,
        BottomRight,
        Bottom
    }

    

    public class Metering
    {
        /// <summary>
        /// Gets the rectangle representing the metering area 
        /// </summary>
        /// <param name="Area"></param>
        /// <returns></returns>
        /// <remarks>
        /// The accuracy of this is questionable because I used a 300x200 image from the internet to measure metering positions
        /// These values are then applied to the image size, so there is room for error. 
        /// </remarks>
        private static Rectangle GetMeteringArea(MeteringAreas Area, int ImageWidth, int ImageHeight)
        {
            // Measured areas taken from a 300x200 image of the metering areas downloaded from the internet
            Rectangle model;

            // lookup the correct are based on the enum
            switch (Area)
            {
                case MeteringAreas.Bottom:
                    model = new Rectangle(145, 141, 7, 6);
                    break;
                case MeteringAreas.BottomLeft:
                    model =  new Rectangle(90, 121, 6, 7);
                    break;
                case MeteringAreas.BottomRight:
                    model = new Rectangle(201, 121, 6, 7);
                    break;
                case MeteringAreas.Center:
                    model = new Rectangle(145, 97, 7, 6);
                    break;
                case MeteringAreas.FarLeft:
                    model = new Rectangle(56, 96, 6, 7);
                    break;
                case MeteringAreas.FarRight:
                    model = new Rectangle(235, 96, 6, 7);
                    break;
                case MeteringAreas.Left:
                    model = new Rectangle(90, 96, 6, 7);
                    break;
                case MeteringAreas.Right:
                    model = new Rectangle(201, 96, 6, 7);
                    break;
                case MeteringAreas.Top:
                    model = new Rectangle(145, 53, 7, 6);
                    break;
                case MeteringAreas.TopLeft:
                    model = new Rectangle(90, 71, 6, 7);
                    break;
                case MeteringAreas.TopRight:
                    model = new Rectangle(201, 71, 6, 7);
                    break;
                default:
                    throw new ArgumentException("The MeteringArea was not found");
            }

            // calculate the rectangle for the full size image based on the measured areas on the 300 x 200 image 
            int x = Convert.ToInt32((model.X / 300d) * ImageWidth);
            int y = Convert.ToInt32((model.Y / 200d) * ImageHeight);
            int w = Convert.ToInt32((model.Width / 300d) * ImageWidth);
            int h = Convert.ToInt32((model.Height / 200d) * ImageHeight);

            // return the calculated areas
            return new Rectangle(x, y, w, h);

        }


        /// <summary>
        /// Draws the metering areas on the source image and saves it to the destination image
        /// </summary>
        /// <param name="SourceFilePath"></param>
        /// <param name="DestFilePath"></param>
        public static void DrawMeteringAreas(string SourceFilePath, string DestFilePath)
        {
            using (Image img = Image.FromFile(SourceFilePath))
            {
                using (Bitmap bmp = new Bitmap(img))
                {

                    using (Graphics gfx = Graphics.FromImage(img))
                    {
                        gfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;



                        using (Pen pen = new Pen(Color.Black, 4F))
                        {

                            Font font = new Font(FontFamily.GenericSansSerif, 18f);

                            DrawMeteringArea(gfx, bmp, MeteringAreas.Bottom, pen, font);
                            DrawMeteringArea(gfx, bmp, MeteringAreas.BottomLeft, pen, font);
                            DrawMeteringArea(gfx, bmp, MeteringAreas.BottomRight, pen, font);
                            DrawMeteringArea(gfx, bmp, MeteringAreas.Center, pen, font);
                            DrawMeteringArea(gfx, bmp, MeteringAreas.FarLeft, pen, font);
                            DrawMeteringArea(gfx, bmp, MeteringAreas.FarRight, pen, font);
                            DrawMeteringArea(gfx, bmp, MeteringAreas.Left, pen, font);
                            DrawMeteringArea(gfx, bmp, MeteringAreas.Right, pen, font);
                            DrawMeteringArea(gfx, bmp, MeteringAreas.Top, pen, font);
                            DrawMeteringArea(gfx, bmp, MeteringAreas.TopLeft, pen, font);
                            DrawMeteringArea(gfx, bmp, MeteringAreas.TopRight, pen, font);
                        }

                        img.Save(DestFilePath);

                    }
                }
            }

        }


        /// <summary>
        /// Draws a metering area rectangle with the exposure of that meter
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="img"></param>
        /// <param name="Area"></param>
        /// <param name="pen"></param>
        /// <param name="font"></param>
        private static void DrawMeteringArea(Graphics gfx, Bitmap img, MeteringAreas Area, Pen pen, Font font)
        {
            Rectangle area = GetMeteringArea(Area, img.Width, img.Height);
            int value = Convert.ToInt32(MeterReading(Area, img) * 100);
            gfx.DrawRectangle(pen, area);
            //gfx.DrawString(value.ToString(), font, pen.Brush, area.X + area.Width + 5, area.Y );
        }


        /// <summary>
        /// Gets the meter reading from a metering area within an image
        /// </summary>
        /// <param name="MeteredArea"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static double MeterReading(MeteringAreas MeteredArea, string FilePath)
        {
            using (Image img = Image.FromFile(FilePath))
            {
                using (Bitmap bmp = new Bitmap(img))
                {
                    return MeterReading(MeteredArea, bmp);   
                }
            }
        }


        /// <summary>
        /// Gets the meter reading from a metering area within an image
        /// </summary>
        /// <param name="MeteredArea"></param>
        /// <param name="Image"></param>
        /// <returns></returns>
        public static double MeterReading(MeteringAreas MeteredArea, Bitmap Image)
        {
            Rectangle area = GetMeteringArea(MeteredArea, Image.Width, Image.Height);
            return MeterReading(area, Image);
        }


        /// <summary>
        /// Gets the lightness of an area witin an image
        /// </summary>
        /// <param name="MeteredArea"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static double MeterReading(Rectangle MeteredArea, string FilePath)
        {
            using (Image img = Image.FromFile(FilePath))
            {
                using (Bitmap bmp = new Bitmap(img))
                {
                    return MeterReading(MeteredArea, bmp);
                }
            }
        }

        
        /// <summary>
        /// Gets the lightness of an area within an image
        /// </summary>
        /// <param name="MeteredArea"></param>
        /// <param name="Image"></param>
        /// <returns></returns>
        public static double MeterReading(Rectangle MeteredArea, Bitmap Image)
        {
            using (Bitmap meteredArea = Image.Clone(MeteredArea, Image.PixelFormat))
            {
                return CalculateAverageLightness(meteredArea);
            }
        }
        

        /// <summary>
        /// Returns the lightness value of an image from file
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static double CalculateAverageLightness(string FilePath)
        {
            using (Image img = Image.FromFile(FilePath))
            {
                using (Bitmap bmp = new Bitmap(img))
                {
                    return Metering.CalculateAverageLightness(bmp);
                }
            }
        }


        /// <summary>
        /// Returns the lightness value of an image from a bitmap
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        public static double CalculateAverageLightness(Bitmap bm)
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

            unsafe
            {
                byte* p = (byte*)(void*)scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = (y * stride) + x * bppModifier;
                        //                 Red                  Green                   Blue
                        lum += (0.2126 * p[idx + 2] + 0.7152 * p[idx + 1] + 0.0722 * p[idx]); // standard
                        //lum += (0.299 * p[idx + 2] + 0.587 * p[idx + 1] + 0.114 * p[idx]); // option 1
                    }
                }
            }

            tmpBmp.UnlockBits(srcData);
            tmpBmp.Dispose();
            var avgLum = lum / (width * height);


            return avgLum / 255.0;
        }
    }
}
