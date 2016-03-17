using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace FooBlog
{
    public class FooFileHelper
    {
        public enum ImageFormat
        {
            bmp,
            jpeg,
            gif,
            tiff,
            png,
            unknown
        }

        public static ImageFormat GetImageFormat(byte[] bytes)
        {
            byte[] bmp = Encoding.ASCII.GetBytes("BM");
            byte[] gif = Encoding.ASCII.GetBytes("GIF");
            var png = new byte[] {137, 80, 78, 71};
            var tiff = new byte[] {73, 73, 42};
            var tiff2 = new byte[] {77, 77, 42};
            var jpeg = new byte[] {255, 216, 255, 224};
            var jpeg2 = new byte[] {255, 216, 255, 225};

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
                return ImageFormat.bmp;

            if (gif.SequenceEqual(bytes.Take(gif.Length)))
                return ImageFormat.gif;

            if (png.SequenceEqual(bytes.Take(png.Length)))
                return ImageFormat.png;

            if (tiff.SequenceEqual(bytes.Take(tiff.Length)))
                return ImageFormat.tiff;

            if (tiff2.SequenceEqual(bytes.Take(tiff2.Length)))
                return ImageFormat.tiff;

            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
                return ImageFormat.jpeg;

            if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
                return ImageFormat.jpeg;

            return ImageFormat.unknown;
        }

        public static byte[] GetFileBytesFromHttpStream(HttpPostedFile file)
        {
            byte[] fileData = null;

            using (var binaryReader = new BinaryReader(file.InputStream))
            {
                fileData = binaryReader.ReadBytes(file.ContentLength);
            }

            return fileData;
        }

        public static bool IsImage(byte[] fileBytes)
        {
            ImageFormat fileType = GetImageFormat(fileBytes);

            return fileType != ImageFormat.unknown;
        }
    }
}