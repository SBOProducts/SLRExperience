using ImageUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //Photo();
            //ExposureReadings();
            //Exif();
            //ExposureValuesFromImageExifData();
            //ExposureValueEquvalents();
            //DrawHistogram();
            Blowouts();
            Console.WriteLine();
            Console.WriteLine("DONE");
            Console.ReadLine();
        }

        static void Blowouts()
        {
            string src = @"C:\Temp\images\test.jpg";
            string dst = @"C:\Temp\images\blowout.gif";
            BlowoutIndicator ind = new BlowoutIndicator(src);
            ind.CreateBlowoutImages();
        }


        /// <summary>
        /// Draws a histogram to a file
        /// </summary>
        static void DrawHistogram()
        {
            string src = @"C:\Temp\images\motion.jpg";
            string dst = @"C:\Temp\images\histogram.jpg";
            Histogram hist = new Histogram(src);
            hist.SaveHistogram(dst, 150, 100);
        }


        /// <summary>
        /// Shows a table of shutter speeds across the top and aperture settings down the left side and their exposure value for the specified ISO
        /// </summary>
        static void ExposureValueEquvalents()
        {
            int padding = 6;
            int iso = 100;

            // Validate against table 1.Table 1. Exposure times, in seconds or minutes (m), for various exposure values and f-numbers  http://en.wikipedia.org/wiki/Exposure_value
            string[] shutters = new string[] { "1/500", "1/250", "1/125", "1/60", "1/30", "1/15", "1/8", "1/4", "1/2", "1/1", "2/1", "4/1" };
            string[] apertures = new string[] { "1.0", "1.4", "2.0", "2.8", "4.0", "5.6", "8.0", "11", "16", "22", "32", "45", "64" };

            // show settings
            Console.WriteLine("The table below is for ISO " + iso.ToString());

            // creat the header row of shutter speeds
            Console.Write(string.Empty.PadRight(padding));
            foreach (string s in shutters)
                Console.Write(s.PadRight(padding));
            Console.WriteLine();

            // write out the table with aperture along the Y-axis and Shutter along the X-axis
            foreach (string a in apertures) 
            {
                Fraction aperture = new Fraction(Convert.ToDouble(a));

                Console.Write(a.PadRight(padding));
                foreach (string s in shutters)
                {
                    Fraction shutter = new Fraction(s);
                    Exposure exp = new Exposure(shutter, aperture, iso);
                    Console.Write(exp.ExposureValueThirds.ToString().PadRight(padding));
                }
                Console.WriteLine();
            }

        }


        /// <summary>
        /// shows the exposure data from the exif data of an image
        /// </summary>
        static void ExposureValuesFromImageExifData()
        {
            string[] exposures = new string[] { "N+4", "N+3", "N+2", "N+1", "N", "N-1", "N-2", "N-3", "N-4" };
            foreach (string exposure in exposures)
            {
                string path = string.Format(@"C:\Temp\images\{0}.JPG", exposure);
                Exposure exp = ExifData.GetExposure(path);
                Console.WriteLine(exp);    
            }
        }


        /// <summary>
        /// displays all the exif values from an image
        /// </summary>
        static void Exif()
        {
            string path = @"C:\Temp\images\N.JPG";
            Dictionary<string, string> data = ExifData.GetExifInfo(path);
            foreach (KeyValuePair<string, string> item in data)
            {
                Console.WriteLine(item.Key.PadRight(30, '.') + ": " + item.Value);
            }
            
        }


        /// <summary>
        /// draws the metering areas on an image
        /// </summary>
        static void DrawMeteringAreas()
        {
            string path = @"C:\Temp\images\motion.jpg";
            Metering.DrawMeteringAreas(path, @"C:\Temp\images\motion-metered.jpg");
            Console.WriteLine("done");
        }


        /// <summary>
        /// Compares the imageUtils Meter readings to known camera meter readings
        /// </summary>
        static void ExposureReadings()
        {
            string[] exposures = new string[] { "N+4", "N+3", "N+2", "N+1", "N", "N-1", "N-2", "N-3", "N-4" };
            foreach (string exposure in exposures)
            {
                string path = string.Format(@"C:\Temp\images\{0}.JPG", exposure);
                double meter = Metering.MeterReading(MeteringAreas.Center, path) * 100;
                Console.WriteLine(exposure.PadRight(3, ' ') + ": " + Convert.ToInt32( meter));
            }
            
        }


        /// <summary>
        /// Measures the ligtness of areas within a gradient image
        /// </summary>
        static void Gradient()
        {

            string path = @"C:\Temp\test-image.png";
            double value = Metering.CalculateAverageLightness(path);
            Console.WriteLine(value);

            int[] xs = new int[] { 0, 130, 170, 220, 260, 300, 340, 370, 415, 455, 500 };
            foreach (int x in xs)
            {
                Rectangle area = new Rectangle(x, 0, 10, 10);
                value = Metering.MeterReading(area, path);
                Console.WriteLine(value);
            }
        }
    }
}
