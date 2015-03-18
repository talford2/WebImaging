using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using Dataway.WebImaging.Extensions;
using Dataway.ImageLibrary;

namespace Dataway.WebImaging
{
    public class WebImagingController : Controller
    {
        #region Private Properties

        private string ScratchDirectory
        {
            get
            {
                string dir = null;
                if (string.IsNullOrWhiteSpace(WebImagingCommon.ScratchDirectory))
                {
                    dir = System.Web.HttpContext.Current.Request.MapPath("~/Sratch");
                }
                else
                {
                    dir = System.Web.HttpContext.Current.Request.MapPath(WebImagingCommon.ScratchDirectory);
                }
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                return dir;
            }
        }

        #endregion

        #region Public Actions

        public string GetProcessedImage(string imageUrl, string scratchUrl, int w, int h, int? q, string f, string u, string r, string g, string i, string missingImageUrl, string t, string rp, string cp)
        {
            if (string.IsNullOrEmpty(r))
            {
                if (WebImagingCommon.Resampling)
                {
                    r = "t";
                }
            }
            ImageProcessingFullSettings settings = new ImageProcessingFullSettings
            {
                Quality = q,
                IsCropped = f == "t",
                IsUpscale = u == "t",
                UseResampling = r == "t",
                IsGreyScale = g == "t",
                IsInverted = i == "t",
                MissingImageUrl = missingImageUrl,
                IsTrimmed = t == "t",
                TrimRepadding = !string.IsNullOrWhiteSpace(rp) ? int.Parse(rp) : 0,
                ScratchUrl = scratchUrl
            };

            if (!string.IsNullOrWhiteSpace(cp))
            {
                switch (cp)
                {
                    case "tl":
                        settings.CropPosition = CropPosition.TopLeft;
                        break;
                    case "tc":
                        settings.CropPosition = CropPosition.TopCentre;
                        break;
                }
            }

            return GetProcessedImageFilename(imageUrl, w, h, settings);
            //return GetProcessedImageResult(imageUrl, w, h, settings);
        }
                
        #endregion

        #region Public Static Methods

        public static ActionResult GetProcessedImageResult(string imageUrl, int width, int height, ImageProcessingFullSettings settings)
        {
            var file = GetProcessedImageFilename(imageUrl, width, height, settings);
            if (!string.IsNullOrWhiteSpace(file))
            {
                return new FilePathResult(file, "image/jpeg");
            }
            return null;
        }

        public static string GetProcessedImageFilename(string imageUrl, int width, int height, ImageProcessingFullSettings settings)
        {
            if (settings == null)
            {
                settings = new ImageProcessingFullSettings();
            }

            string sourceImagePath = System.Web.HttpContext.Current.Request.MapPath("~/" + imageUrl);

            string scratchDir = System.Web.HttpContext.Current.Request.MapPath(WebImagingCommon.ScratchDirectory);
            
            if (settings.ScratchUrl != null)
            {
                scratchDir = System.Web.HttpContext.Current.Request.MapPath("~/" + settings.ScratchUrl.TrimEnd('/').TrimStart('/'));
            }
            if (!Directory.Exists(scratchDir))
            {
                Directory.CreateDirectory(scratchDir);
            }

            string specificImgVersionPath = string.Format("{0}\\{1}", scratchDir, GetFilename(imageUrl, width, height, settings));

            bool imageHasChanged = false;
            bool specifiedFileExists = false;

            if (System.IO.File.Exists(specificImgVersionPath))
            {
                specifiedFileExists = true;
                FileInfo specificFileInfo = new FileInfo(specificImgVersionPath);
                FileInfo origFileInfo = new FileInfo(sourceImagePath);
                if (specificFileInfo.LastWriteTime < origFileInfo.LastWriteTime)
                {
                    imageHasChanged = true;
                }
            }

            if (System.IO.File.Exists(sourceImagePath) && (!specifiedFileExists || imageHasChanged))
            {
                Image img = new Bitmap(sourceImagePath);

                if (!settings.CustomCrop.IsEmpty)
                {
                    img = ImageManipulator.GetCropped(img, settings.CustomCrop);
                }

                // Sizing
                if (width == 0 || height == 0)
                {
                }
                else if (settings.IsCropped)
                {
                    img = ImageManipulator.GetFittedImage(img, new Size(width, height), settings.IsUpscale, settings.UseResampling, settings.CropPosition);
                }
                else
                {
                    img = ImageManipulator.GetScaledAspectImage(img, new Size(width, height), settings.IsUpscale, settings.UseResampling);
                }

                // Trim white space first if trimming is on
                //if (settings.IsTrimmed)
                //{
                //    img = ImageManipulator.GetTrimmedImage(img, settings.TrimRepadding, settings.TrimColor, settings.TrimThreashold);
                //}

                //if (settings.TrimRepadding)
                //{
                //    img = ImageManipulator.GetPaddedImage(img, settings.TrimRepadding, 
                //}

                try
                {
                    int qualityOut = 75;
                    if (settings.Quality.HasValue)
                    {
                        qualityOut = settings.Quality.Value;
                    }
                    if (settings.IsGreyScale)
                    {
                        ImageManipulator.GreyScale(img as Bitmap);
                    }

                    if (settings.IsInverted)
                    {
                        ImageManipulator.Invert(img as Bitmap);
                    }

                    ImageManipulator.SaveImage(img, specificImgVersionPath, qualityOut);
                }
                catch (ExternalException ex)
                {
                    throw ex;
                }
                finally
                {
                    img.Dispose();
                    img = null;
                }
            }

            if (System.IO.File.Exists(specificImgVersionPath))
            {
                return specificImgVersionPath;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(settings.MissingImageUrl))
                {
                    string i = settings.MissingImageUrl;
                    settings.MissingImageUrl = null;
                    return GetProcessedImageFilename(i, width, height, settings);
                }
            }

            return null;
        }

        #endregion

        #region Private Static Methods

        private static string GetFilename(string url, int width, int height, ImageProcessSettings settings)
        {
            if (settings == null)
            {
                settings = new ImageProcessSettings();
            }

            string fittedStr = "";
            string upsacelStr = "";
            string resampleStr = "";
            string greyStr = "";
            string invertStr = "";
            string rePadStr = "";
            string cropPos = "";

            if (settings.IsCropped)
            {
                fittedStr = "c";
                if (settings.TrimRepadding > 0)
                {
                    rePadStr = "rp" + settings.TrimRepadding;
                }
            }
            if (settings.IsUpscale)
            {
                upsacelStr = "u";
            }
            if (settings.UseResampling)
            {
                resampleStr = "r";
            }
            if (settings.IsGreyScale)
            {
                greyStr = "g";
            }
            if (settings.IsInverted)
            {
                invertStr = "i";
            }

            switch (settings.CropPosition)
            {
                case CropPosition.TopCentre:
                    cropPos = "tc";
                    break;
                case CropPosition.TopLeft:
                    cropPos = "tl";
                    break;
            }

            string customCrop = "";
            if (!settings.CustomCrop.IsEmpty)
            {
                customCrop += "(" + settings.CustomCrop.X + "," + settings.CustomCrop.Y + "," + settings.CustomCrop.Width + "x" + settings.CustomCrop.Height + ")";
            }

            string pathFilename = url.Replace("/", "_");
            string filename = string.Format("{0}_{1}x{2}_{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}",
                Path.GetFileNameWithoutExtension(pathFilename),
                width,
                height,
                fittedStr,
                upsacelStr,
                resampleStr,
                greyStr,
                invertStr,
                settings.Quality,
                rePadStr,
                cropPos,
                customCrop,
                System.IO.Path.GetExtension(url));

            return filename;

            //			return string.Format("{0}_{1}x{2}_{3}{4}{5}{6}{7}{8}{9}{10}", Path.GetFileNameWithoutExtension(url), width, height, fittedStr, upsacelStr, resampleStr, greyStr, invertStr, settings.Quality, rePadStr, System.IO.Path.GetExtension(url));
        }

        #endregion
    }
}

