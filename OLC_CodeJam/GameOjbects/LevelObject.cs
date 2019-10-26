using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLC_CodeJam
{
    class LevelObject: GameObject
    {
        public bool IsPassable { get; private set; }
        Asset currentAsset;
        public new Asset CurrentAsset
        {
            get { return currentAsset; }

            set
            {
                currentAsset = value;
                if ((value.ColorID.ToArgb() & Assets.INTERACTIVE_FILTER) == Assets.INTERACTIVE_WALL ||
                    (value.ColorID.ToArgb() & Assets.INTERACTIVE_FILTER) == Assets.INTERACTIVE_IRON ||
                    (value.ColorID.ToArgb() & Assets.INTERACTIVE_FILTER) == Assets.INTERACTIVE_WOOD ||
                    (value.ColorID.ToArgb() & Assets.INTERACTIVE_FILTER) == Assets.INTERACTIVE_BRICK ||
                    (value.ColorID.ToArgb() & Assets.INTERACTIVE_FILTER) == Assets.INTERACTIVE_WOODLONG)
                    IsPassable = false;
                else
                    IsPassable = true;
            }
        }
        public new Bitmap Image
        {
            get { return CurrentAsset.Image; }
        }

        public LevelObject() : base() { }
        public LevelObject(PointF point, Size size, Asset asset, bool isPassable) : base(point, size, asset)
        {
            this.IsPassable = isPassable;
            this.currentAsset = asset;
        }
    }
}
