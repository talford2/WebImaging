using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;

namespace Dataway.WebImaging
{
    public static class HelperExtensions
    {
        public static IHtmlString ProcessedImage(this HtmlHelper helper, string imagePath, int width, int height)
        {
            return ProcessedImage(helper, imagePath, width, height, null);
        }

        public static IHtmlString ProcessedImage(this HtmlHelper helper, string imagePath, int width, int height, ImageProcessSettings settings)
        {
            return new HtmlString(string.Format("<img src=\"{0}\" />", HelperExtensions.ProcessedImageUrl(helper, imagePath, width, height, settings)));
        }

        public static IHtmlString ProcessedImageUrl(this HtmlHelper helper, string imagePath, int width, int height)
        {
            return ProcessedImageUrl(helper, imagePath, width, height, null);
        }

        public static IHtmlString ProcessedImageUrl(this HtmlHelper helper, string imagePath, int width, int height, ImageProcessSettings settings)
        {

            //string encodedPath = HttpContext.Current.Server.UrlEncode(imagePath);

            QueryStringBuilder qsb = new QueryStringBuilder("/" + WebImagingCommon.RoutePath.TrimEnd('/') + "/" + imagePath.TrimStart('/'));

            if (settings == null)
            {
                settings = new ImageProcessSettings();
            }

            qsb.Add("w", width);
            qsb.Add("h", height);

            if (settings.Quality.HasValue)
            {
                qsb.Add("q", settings.Quality.Value);
            }
            if (settings.IsCropped)
            {
                qsb.Add("f", "t");
            }
            if (settings.IsUpscale)
            {
                qsb.Add("u", "t");
            }
            if (settings.UseResampling)
            {
                qsb.Add("r", "t");
            }
            if (settings.IsGreyScale)
            {
                qsb.Add("g", "t");
            }

            switch (settings.CropPosition)
            {
                case ImageLibrary.CropPosition.TopLeft:
                    qsb.Add("cp", "tl");
                    break;
                case ImageLibrary.CropPosition.TopCentre:
                    qsb.Add("cp", "tc");
                    break;
            }

            //return new HtmlString(qsb.ToString());

            //string url = qsb.ToString();
            //url = url.Replace(" ", "%20");
            //return new HtmlString(url);


            return new HtmlString(qsb.ToString());
        }
    }
}
