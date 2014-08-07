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

            Photo();
            //ExposureReadings();

            Console.ReadLine();
        }

        static void Photo()
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
