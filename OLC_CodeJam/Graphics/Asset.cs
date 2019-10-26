using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLC_CodeJam
{
    struct Asset
    {
        private Bitmap image;
        public Bitmap Image { get { return image; } private set { image = value; } }
        public Color ColorID { get; private set; }
        public int Width { get { return image.Width; } }
        public int Height { get { return image.Height; } }

        public Asset(Bitmap img, Color id)
        {
            image = img;
            ColorID = id;
        }
    }
}
