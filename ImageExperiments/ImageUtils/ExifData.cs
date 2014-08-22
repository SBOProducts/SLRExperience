// Source of ExifLibrary: http://www.codeproject.com/Articles/43665/ExifLibrary-for-NET

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExifLibrary; 


namespace ImageUtils
{
    /// <summary>
    /// provides methods for reading exif data from an image
    /// </summary>
    public static class ExifData
    {
        /// <summary>
        /// Gets a list of all exif values for an image
        /// </summary>
        /// <param name="ImagePath"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetExifInfo(string ImagePath)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();

            ExifFile file = ExifFile.Read(ImagePath);
            foreach (ExifProperty item in file.Properties.Values)
            {
                properties.Add(item.Tag.ToString(), item.Value.ToString());
            }


            return properties;
        }


        /// <summary>
        /// Gets the exposure info for an image
        /// </summary>
        /// <param name="ImagePath"></param>
        /// <returns></returns>
        public static Exposure GetExposure(string ImagePath)
        {
            ExifFile file = ExifFile.Read(ImagePath);
            object time = file.Properties[ExifTag.ExposureTime].Value;
            object FNumber = file.Properties[ExifTag.FNumber].Value;
            object ISO = file.Properties[ExifTag.ISOSpeedRatings].Value;
            return new Exposure(time, FNumber, ISO);
        }
    }
}
