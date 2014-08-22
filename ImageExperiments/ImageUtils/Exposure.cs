using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageUtils
{
    /// <summary>
    /// Represents a fraction such as 1/100 or 9/2
    /// </summary>
    public struct Fraction
    {
        double _top, _bottom, _value;

        /// <summary>
        /// Creates a new fraction from an exif data value
        /// </summary>
        /// <param name="Value"></param>
        public Fraction(object Value) : this(Value.ToString()) { }


        /// <summary>
        /// Creates a new fraction using the value only
        /// </summary>
        /// <param name="Value"></param>
        public Fraction(double Value) : this(FracMethod.GetFraction(Value)) { }


        /// <summary>
        /// Creates a new fraction from a string data value (eg "9/2")
        /// </summary>
        /// <param name="Value"></param>
        public Fraction(string Value)
        {
            _top = Convert.ToDouble(Value.Split('/')[0]);
            _bottom = Convert.ToDouble(Value.Split('/')[1]);
            _value = (double)(_top / _bottom);
        }


        /// <summary>
        /// The top number in the fraction
        /// </summary>
        public int TopNumber { get { return (int)_top; } }


        /// <summary>
        /// The bottom number of a fraction
        /// </summary>
        public int BottomNumber { get { return (int)_bottom; } }


        /// <summary>
        /// The value of the top value divided by the bottom value represented as a double
        /// </summary>
        public double Value { get { return _value; } }


        /// <summary>
        /// The string representation of the fraction i.e. Top / Bottom
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}/{1}", TopNumber, BottomNumber);
        }


        /// <summary>
        /// used to get a fraction from a double value
        /// </summary>
        private class FracMethod
        {
            // source: http://www.codeproject.com/Articles/165320/Easy-Way-of-Converting-a-Decimal-to-a-Fraction?rp=/KB/cs/Fraction_Conversion/Frac.zip

            public static string GetFraction(double Value)
            {
                string val = dec2frac(Value);
                if (val.Contains('/'))
                    return val;
                else
                    return val + "/1";
            }


            private static string dec2frac(double dbl)
            {
                char neg = ' ';
                double dblDecimal = dbl;
                if (dblDecimal == (int)dblDecimal) return dblDecimal.ToString(); //return no if it's not a decimal
                if (dblDecimal < 0)
                {
                    dblDecimal = Math.Abs(dblDecimal);
                    neg = '-';
                }
                var whole = (int)Math.Truncate(dblDecimal);
                string decpart = dblDecimal.ToString().Replace(Math.Truncate(dblDecimal) + ".", "");
                double rN = Convert.ToDouble(decpart);
                double rD = Math.Pow(10, decpart.Length);

                string rd = recur(decpart);
                int rel = Convert.ToInt32(rd);
                if (rel != 0)
                {
                    rN = rel;
                    rD = (int)Math.Pow(10, rd.Length) - 1;
                }
                //just a few prime factors for testing purposes
                var primes = new[] { 47, 43, 37, 31, 29, 23, 19, 17, 13, 11, 7, 5, 3, 2 };
                foreach (int i in primes) reduceNo(i, ref rD, ref rN);

                rN = rN + (whole * rD);
                return string.Format("{0}{1}/{2}", neg, rN, rD);
            }


            /// <summary>
            /// Finds out the recurring decimal in a specified number
            /// </summary>
            /// <param name="db">Number to check</param>
            /// <returns></returns>
            private static string recur(string db)
            {
                if (db.Length < 13) return "0";
                var sb = new StringBuilder();
                for (int i = 0; i < 7; i++)
                {
                    sb.Append(db[i]);
                    int dlength = (db.Length / sb.ToString().Length);
                    int occur = occurence(sb.ToString(), db);
                    if (dlength == occur || dlength == occur - sb.ToString().Length)
                    {
                        return sb.ToString();
                    }
                }
                return "0";
            }


            /// <summary>
            /// Checks for number of occurence of specified no in a number
            /// </summary>
            /// <param name="s">The no to check occurence times</param>
            /// <param name="check">The number where to check this</param>
            /// <returns></returns>
            private static int occurence(string s, string check)
            {
                int i = 0;
                int d = s.Length;
                string ds = check;
                for (int n = (ds.Length / d); n > 0; n--)
                {
                    if (ds.Contains(s))
                    {
                        i++;
                        ds = ds.Remove(ds.IndexOf(s), d);
                    }
                }
                return i;
            }


            /// <summary>
            /// Reduces a fraction given the numerator and denominator
            /// </summary>
            /// <param name="i">Number to use in an attempt to reduce fraction</param>
            /// <param name="rD">the Denominator</param>
            /// <param name="rN">the Numerator</param>
            private static void reduceNo(int i, ref double rD, ref double rN)
            {
                //keep reducing until divisibility ends
                while ((rD % i) == 0 && (rN % i) == 0)
                {
                    rN = rN / i;
                    rD = rD / i;
                }
            }
        }

    }



    /// <summary>
    /// Provides exposure information including shutter speed, aperture, and iso
    /// </summary>
    public struct Exposure
    {
        Fraction _shutterSpeed, _aperture;
        int _iso;


        /// <summary>
        /// Creates a new instance of Exposure from exif data values
        /// </summary>
        /// <param name="ShutterSpeed">The object value read from exif data</param>
        /// <param name="Aperture">The object value read from exif data</param>
        /// <param name="ISO">The object value read from exif data</param>
        public Exposure(object ShutterSpeed, object Aperture, object ISO)
        {
            _shutterSpeed = new Fraction(ShutterSpeed);
            _aperture = new Fraction(Aperture);
            _iso = Convert.ToInt32(ISO);
        }


        /// <summary>
        /// Creates a new instance of Exposure from cammera setting values
        /// </summary>
        /// <param name="ShutterSpeed"></param>
        /// <param name="Aperture"></param>
        /// <param name="ISO"></param>
        public Exposure(Fraction ShutterSpeed, Fraction Aperture, int ISO)
        {
            _shutterSpeed = ShutterSpeed;
            _aperture = Aperture;
            _iso = ISO;
        }


        /// <summary>
        /// The shutter speed for the image
        /// </summary>
        public Fraction ShutterSpeed { get { return _shutterSpeed; } }


        /// <summary>
        /// The aperture (FNumber) for the image
        /// </summary>
        public Fraction Aperture { get { return _aperture; } }


        /// <summary>
        /// The ISO speed rating for the image
        /// </summary>
        public int ISO { get { return _iso; } }


        /// <summary>
        /// Returns the Exposure Value based on the Aperture, Shutter, and ISO
        /// </summary>
        /// <remarks>
        /// Source: http://en.wikipedia.org/wiki/Exposure_value
        /// </remarks>
        public double ExposureValue
        {
            get
            {
                //return ShutterSpeed.Value * Aperture.Value * ISO;
                double N = Aperture.Value * Aperture.Value;
                double t = ShutterSpeed.Value;
                double i = Math.Log(Convert.ToDouble(ISO) / 100d, 2d);
                return Math.Log(N / t, 2d) + i;
            }
        }


        /// <summary>
        /// Returns the Exposure Value rounded to the nearest third of a stop
        /// </summary>
        public double ExposureValueThirds
        {
            get
            {
                double rounded = Math.Round(ExposureValue, 2);
                string str = rounded.ToString() + ".0";
                double whole = Convert.ToDouble(str.Split('.')[0]);
                int dec = Convert.ToInt32(str.Split('.')[1].PadRight(2, '0'));
                if (dec > 17 && dec < 50)
                    whole = whole + .3d;
                if (dec >= 50 && dec < 83)
                    whole = whole + .6d;
                if (dec >= 83)
                    whole = whole + 1d;

                return whole;
            }
        }


        /// <summary>
        /// Returns the Exposure Value rounded to the nearest half of a stop
        /// </summary>
        public double ExposureValueHalves
        {
            get
            {
                double rounded = Math.Round(ExposureValue, 2);
                string str = rounded.ToString() + ".0";
                double whole = Convert.ToDouble(str.Split('.')[0]);
                int dec = Convert.ToInt32(str.Split('.')[1].PadRight(2, '0'));
                if (dec > 25 && dec < 75)
                    whole = whole + .5d;
                if (dec >= 75)
                    whole = whole + 1d;

                return whole;
            }
        }


        /// <summary>
        /// Returns the string representation of the exposure
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Shutter {0} Aperture {1} ISO {2} Exposure Value {3}", ShutterSpeed.ToString().PadRight(10), Aperture.Value.ToString().PadRight(10), ISO.ToString().PadRight(10), ExposureValueThirds);
        }
        
    }
}
