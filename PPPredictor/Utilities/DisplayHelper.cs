using PPPredictor.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Utilities
{
    class DisplayHelper
    {
        internal static readonly string ColorCountryRankDisabled = "grey";
        internal static readonly string ColorWhite = "white";
        internal static readonly string ColorGreen = "green";
        internal static readonly string ColorYellow = "yellow";
        internal static readonly string ColorRed = "red";
        public static string GetDisplayColor(double value, bool invert, bool isPPGainWithVergeOption = false)
        {
            if (invert) value *= -1;
            if (value > 0)
            {
                return ColorGreen;
            }
            else if (isPPGainWithVergeOption && Plugin.ProfileInfo.PpGainCalculationType == PPGainCalculationType.Raw && value < 0 && value >= Plugin.ProfileInfo.RawPPLossHighlightThreshold)
            {
                return ColorYellow;
            }
            else if (value < 0)
            {
                return ColorRed;
            }
            return ColorWhite;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        /// Taken from https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
        public static byte[] ResizeImage(byte[] data, int width, int height)
        {
            byte[] output = null;
            try
            {
                using (var ms = new MemoryStream(data))
                {
                    Image img = Image.FromStream(ms);
                    var destRect = new Rectangle(0, 0, width, height);
                    var destImage = new Bitmap(width, height);

                    destImage.SetResolution(img.HorizontalResolution, img.VerticalResolution);

                    using (var graphics = System.Drawing.Graphics.FromImage(destImage))
                    {
                        graphics.CompositingMode = CompositingMode.SourceCopy;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        using (var wrapMode = new ImageAttributes())
                        {
                            wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                            graphics.DrawImage(img, destRect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, wrapMode);
                        }
                    }

                    using (var msOut = new MemoryStream())
                    {
                        destImage.Save(msOut, destImage.RawFormat);
                        output = msOut.ToArray();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Plugin.ErrorPrint($"ResizeImage error {ex.Message}");
            }
            return output;
        }
    }

    class MenuPositionHelper
    {
        internal static readonly SVector3 UnderScoreboardPosition = new SVector3(2.5f, 0.05f, 2.0f);
        internal static readonly SVector3 UnderScoreboardEulerAngles = new SVector3(88, 60, 0);
        internal static readonly SVector3 RightOfScoreboardPosition = new SVector3(4.189056f, 1.14293063f, -0.5054281f);
        internal static readonly SVector3 RightOfScoreboardEulerAngles = new SVector3(1f, 90.1f, 359.437439f);
    }
}
