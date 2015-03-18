using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;
using System.Web.Hosting;
using Dataway.ImageLibrary;
using System.Drawing;

namespace Dataway.WebImaging
{
    public static class WebImagingCommon
    {
        private static string scratchDir = "Scratch";

        public static string ScratchDirectory
        {
            get
            {
                return scratchDir;
            }
            private set
            {
                scratchDir = value;
            }

            //get;
            //private set;
        }

        private static string routePath = "image-processing";

        public static string RoutePath
        {
            get
            {
                return routePath;
            }
            private set
            {
                routePath = value;
            }
        }

        public static bool Resampling { get; set; }

        /// <summary>
        /// Add the image processing route used to render images.
        /// </summary>
        /// <param name="routes">Current routes collection to add route to.</param>
        /// <param name="url">Url of the routing.</param>
        public static void AddRoute(RouteCollection routes, string url, string scratchDirectory, string routeName)
        {
            WebImagingCommon.RoutePath = url;

            scratchDirectory = "~/" + scratchDirectory.TrimStart('~').TrimStart('/');

            WebImagingCommon.ScratchDirectory = scratchDirectory;

            string[] namespaces = { "Dataway.WebImaging" };
            routes.MapRoute(routeName, WebImagingCommon.RoutePath + "/{*imageUrl}", new { Controller = "WebImaging", Action = "GetProcessedImage", imageUrl = UrlParameter.Optional }, namespaces);
        }

        public static void AddRoute(RouteCollection routes, string url)
        {
            WebImagingCommon.AddRoute(routes, url, "Scratch", "Image Processing");
        }

        public static void AddImageProcessingRoute(this RouteCollection routes)
        {
            WebImagingCommon.AddRoute(routes, RoutePath, ScratchDirectory, "Image Processing");
        }
    }

    public class ImageProcessSettings
    {
        #region Public Properties

        public int? Quality { get; set; }

        public bool IsCropped { get; set; }

        public bool IsUpscale { get; set; }

        public bool UseResampling { get; set; }

        public bool IsGreyScale { get; set; }

        public bool IsInverted { get; set; }

        public int TrimRepadding { get; set; }

        public CropPosition CropPosition { get; set; }

        public Rectangle CustomCrop { get; set; }

        #endregion
    }

    public class ImageProcessingFullSettings : ImageProcessSettings
    {
        #region Public Properties

        public string ScratchUrl { get; set; }

        public string MissingImageUrl { get; set; }

        public bool IsTrimmed { get; set; }

        public int TrimThreashold { get; set; }

        #endregion
    }
}
