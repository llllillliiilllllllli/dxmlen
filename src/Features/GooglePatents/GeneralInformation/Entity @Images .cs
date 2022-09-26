using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DxMLEngine.Features.GooglePatents
{
    internal class Images
    {
        internal class ImageHref
        {
            public string ThumbnailHref { set; get; }
            public string FullImageHref { set; get; }

            public ImageHref(string thumbnailHref, string fullImageHref)
            {
                this.ThumbnailHref = thumbnailHref;
                this.FullImageHref = fullImageHref;
            }
        }

        public ImageHref[] ImageHrefs { set; get; }

        public Images(ImageHref[] imageHrefs)
            => this.ImageHrefs = imageHrefs;
    }
}
