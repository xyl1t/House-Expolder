using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLC_CodeJam
{
    class SpriteAtlas
    {
        public Bitmap SpriteSheet { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        public SpriteAtlas(Bitmap sprite, int tileWidth, int tileHeight, int transparency)
        {
            this.SpriteSheet = sprite;
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            SpriteSheet.MakeTransparent(Color.FromArgb(transparency));
        }

        public Bitmap getSprite(int x, int y)
        {
            if (x * TileWidth >= SpriteSheet.Width || y * TileHeight >= SpriteSheet.Height)
                throw new ArgumentException("The x or y coordinate is greater then the maximum sprite size.");

            Bitmap sprite = new Bitmap(TileWidth, TileHeight);
            using (Graphics gfx = Graphics.FromImage(sprite))
            {
                //gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                gfx.DrawImage(SpriteSheet, 0, 0, new Rectangle(x * TileWidth, y * TileHeight, TileWidth, TileHeight), GraphicsUnit.Pixel);
            }
            return sprite;
        }
        public Bitmap getSpriteRegion(int x, int y, int width, int height)
        {
            if (x * TileWidth >= SpriteSheet.Width || y * TileHeight >= SpriteSheet.Height ||
                width + x * TileWidth > SpriteSheet.Width || height + y * TileHeight > SpriteSheet.Height)
                throw new ArgumentException("The x or y coordinate is greater then the maximum sprite size.");

            Bitmap sprite = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(sprite))
            {
                //gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                gfx.DrawImage(SpriteSheet, 0, 0, new Rectangle(
                    x * TileWidth,
                    y * TileHeight,
                    width,
                    height), GraphicsUnit.Pixel);
            }
            return sprite;
        }
    }
}
