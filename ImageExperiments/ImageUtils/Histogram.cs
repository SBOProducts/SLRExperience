using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageUtils
{
    public class Histogram
    {

        Bitmap bitmap = null;
        private long myMaxValue;
        private long[] myValues;
        private bool myIsDrawing;
        private float myYUnit; //this gives the vertical unit used to scale our values
        private float myXUnit; //this gives the horizontal unit used to scale our values
        private int myOffset = 0; //the offset, in pixels, from the control margins.
        private Color myColor = Color.Yellow;
        private Font myFont = new Font("Tahoma", 10);
        private int Offset
        {
            set
            {
                if (value > 0)
                    myOffset = value;
            }
            get
            {
                return myOffset;
            }
        }
        private Color DisplayColor
        {
            set
            {
                myColor = value;
            }
            get
            {
                return myColor;
            }
        }
        string _source;

        /// <summary>
        /// Creates a histogram from an image
        /// </summary>
        /// <param name="SourcePath"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public Histogram(string SourcePath)
        {
            _source = SourcePath;
        }


        /// <summary>
        /// Draws the histogram to a bitmap 
        /// </summary>
        /// <returns></returns>
        public Bitmap DrawHistogram(int Width, int Height)
        {
            Image image = Image.FromFile(_source);
            Bitmap picture = new Bitmap(image);
            bitmap = new Bitmap(Width, Height);

            long[] histogram = GetHistogram(picture);
            ProcessHistogram(histogram);
            return bitmap;
        }


        /// <summary>
        /// Draws the histogram bitmap to a file
        /// </summary>
        /// <param name="FilePath"></param>
        public void SaveHistogram(string FilePath, int Width, int Height)
        {
            Bitmap hist = DrawHistogram(Width, Height);
            hist.Save(FilePath);
        }


        /// <summary>
        /// We draw the histogram on the control
        /// </summary>
        /// <param name="myValues">The values beeing draw</param>
        private void ProcessHistogram(long[] Values)
        {
            myValues = new long[Values.Length];
            Values.CopyTo(myValues, 0);

            myIsDrawing = true;
            myMaxValue = getMaxim(myValues);

            ComputeXYUnitValues();

            DrawBitmap();
        }


        /// <summary>
        /// Reads the histogram data from the image
        /// </summary>
        /// <param name="picture"></param>
        /// <returns></returns>
        private long[] GetHistogram(Bitmap picture)
        {
            long[] myHistogram = new long[256];

            for (int i = 0; i < picture.Size.Width; i++)
                for (int j = 0; j < picture.Size.Height; j++)
                {
                    System.Drawing.Color c = picture.GetPixel(i, j);

                    long Temp = 0;
                    Temp += c.R;
                    Temp += c.G;
                    Temp += c.B;

                    Temp = (int)Temp / 3;
                    myHistogram[Temp]++;
                }

            return myHistogram;
        }


        /// <summary>
        /// Draws the histogram to a bitmap
        /// </summary>
        private void DrawBitmap()
        {
            
            Graphics g = Graphics.FromImage(bitmap);
            g.FillRectangle(new Pen(Color.Black).Brush, 0, 0, bitmap.Width, bitmap.Height);

            Pen myPen = new Pen(new SolidBrush(myColor), myXUnit);
            //The width of the pen is given by the XUnit for the control.
            for (int i = 0; i < myValues.Length; i++)
            {

                //We draw each line 
                g.DrawLine(myPen,
                    new PointF(myOffset + (i * myXUnit), bitmap.Height - myOffset),
                    new PointF(myOffset + (i * myXUnit), bitmap.Height - myOffset - myValues[i] * myYUnit));

                //We plot the coresponding index for the maximum value.
                /*
                if (myValues[i] == myMaxValue)
                {
                    SizeF mySize = g.MeasureString(i.ToString(), myFont);

                    g.DrawString(i.ToString(), myFont, new SolidBrush(myColor),
                        new PointF(myOffset + (i * myXUnit) - (mySize.Width / 2), bitmap.Height - myFont.Height),
                        System.Drawing.StringFormat.GenericDefault);
                }*/
            }

            //We draw the indexes for 0 and for the length of the array beeing plotted
            /*
            g.DrawString("0", myFont, new SolidBrush(myColor), new PointF(myOffset, bitmap.Height - myFont.Height), System.Drawing.StringFormat.GenericDefault);
            g.DrawString((myValues.Length - 1).ToString(), myFont,
                new SolidBrush(myColor),
                new PointF(myOffset + (myValues.Length * myXUnit) - g.MeasureString((myValues.Length - 1).ToString(), myFont).Width,
                bitmap.Height - myFont.Height),
                System.Drawing.StringFormat.GenericDefault);
            */

            //We draw a rectangle surrounding the control.
            g.DrawRectangle(new System.Drawing.Pen(new SolidBrush(Color.Black), 1), 0, 0, bitmap.Width - 1, bitmap.Height - 1);
            
            
        }
        

        /// <summary>
        /// We get the highest value from the array
        /// </summary>
        /// <param name="Vals">The array of values in which we look</param>
        /// <returns>The maximum value</returns>
        private long getMaxim(long[] Vals)
        {
            if (myIsDrawing)
            {
                long max = 0;
                for (int i = 0; i < Vals.Length; i++)
                {
                    if (Vals[i] > max)
                        max = Vals[i];
                }
                return max;
            }
            return 1;
        }


        /// <summary>
        /// Computes the unit values for the x axis
        /// </summary>
        private void ComputeXYUnitValues()
        {
            myYUnit = (float)(bitmap.Height - (2 * myOffset)) / myMaxValue;
            myXUnit = (float)(bitmap.Width - (2 * myOffset)) / (myValues.Length - 1);
        }


    }
}
