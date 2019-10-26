using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLC_CodeJam
{
    class GameObject
    {
        public Color DebugTint { get; set; }
        protected PointF position;
        public PointF Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
        public Size Size { get; set; }
        public Bitmap Image
        {
            get { return CurrentAsset.Image; }
        }
        public Asset CurrentAsset { get; set; }
        
        

        public GameObject()
        { }
        public GameObject(PointF point, Size size, Asset asset)
        {
            this.Position = point;
            this.Size = size;
            this.CurrentAsset = asset;
        }

        public virtual void DrawBoundaryBox(Color tint, Graphics gfx)
        {
            int alpha = 32;
            Pen drawingPen = new Pen(tint);
            Brush drawingBrush = new SolidBrush(Color.FromArgb(alpha, tint));

            gfx.DrawRectangle(drawingPen, Position.X,Position.Y, Size.Width, Size.Height);
            gfx.FillRectangle(drawingBrush, Position.X, Position.Y, Size.Width, Size.Height);

            drawingBrush.Dispose();
            drawingPen.Dispose();
        }
        public static void DrawBoundaryBox(Color tint, Graphics gfx, float x, float y, float width, float height)
        {
            int alpha = 32;
            Pen drawingPen = new Pen(tint);
            Brush drawingBrush = new SolidBrush(Color.FromArgb(alpha, tint));
            
            gfx.DrawRectangle(drawingPen, x, y, width, height);
            gfx.FillRectangle(drawingBrush, x, y, width, height);

            drawingBrush.Dispose();
            drawingPen.Dispose();
        }
        public static void DrawBoundaryBox(Color tint, Graphics gfx, float angle, int zoom, float x, float y, float width, float height)
        {
            int alpha = 32;
            Pen drawingPen = new Pen(tint);
            Brush drawingBrush = new SolidBrush(Color.FromArgb(alpha, tint));

            int tX = (int)(x + width/2f);
            int tY = (int)(y + height - (Assets.TILE_HEIGHT*zoom / 2f));

            gfx.TranslateTransform(tX, tY);
            gfx.RotateTransform(angle);

            gfx.DrawRectangle(drawingPen,   -width / 2f, -height + (Assets.TILE_HEIGHT * zoom / 2f), width, height);
            gfx.FillRectangle(drawingBrush, -width / 2f, -height + (Assets.TILE_HEIGHT * zoom / 2f), width, height);

            gfx.RotateTransform(-angle);
            gfx.TranslateTransform(-tX, -tY);

            drawingBrush.Dispose();
            drawingPen.Dispose();
        }
        public virtual void Draw(Graphics gfx)
        {
            gfx.DrawImage(this.Image, this.Position.X, this.Position.Y);
        }
    }
}
