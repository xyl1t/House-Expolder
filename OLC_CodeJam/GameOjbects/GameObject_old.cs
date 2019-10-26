using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace OLC_CodeJam
{
    class GameObject_old
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
                drawPos = new PointF(Position.X - (BoundingBox.Width - this.Size.Width) / 2, Position.Y - (BoundingBox.Height - this.Size.Height) / 2);
            }
        }
        protected PointF drawPos;
        protected float angle;
        public float Angle
        {
            get
            {
                return angle;
            }

            set
            {
                angle = value;
                
                int max_x = (int)Math.Round(Math.Max(Math.Max(Model.GetRotatedPoint(0, 0, Angle).X, Model.GetRotatedPoint(Size.Width, 0, Angle).X),
                                                     Math.Max(Model.GetRotatedPoint(Size.Width, Size.Height, Angle).X, Model.GetRotatedPoint(0, Size.Height, Angle).X)));
                int max_y = (int)Math.Round(Math.Max(Math.Max(Model.GetRotatedPoint(0, 0, Angle).Y, Model.GetRotatedPoint(Size.Width, 0, Angle).Y),
                                                     Math.Max(Model.GetRotatedPoint(Size.Width, Size.Height, Angle).Y, Model.GetRotatedPoint(0, Size.Height, Angle).Y)));

                int min_x = (int)Math.Round(Math.Min(Math.Min(Model.GetRotatedPoint(0, 0, Angle).X, Model.GetRotatedPoint(Size.Width, 0, Angle).X),
                                                     Math.Min(Model.GetRotatedPoint(Size.Width, Size.Height, Angle).X, Model.GetRotatedPoint(0, Size.Height, Angle).X)));
                int min_y = (int)Math.Round(Math.Min(Math.Min(Model.GetRotatedPoint(0, 0, Angle).Y, Model.GetRotatedPoint(Size.Width, 0, Angle).Y),
                                                     Math.Min(Model.GetRotatedPoint(Size.Width, Size.Height, Angle).Y, Model.GetRotatedPoint(0, Size.Height, Angle).Y)));

                BoundingBox = new Size(max_x + Math.Abs(min_x) + 1, max_y + Math.Abs(min_y) + 1);

                drawPos = new PointF(Position.X - (BoundingBox.Width - this.Size.Width)/2, Position.Y - (BoundingBox.Height - this.Size.Height) / 2);

            }
        }
        public Size BoundingBox { get; private set; }

        Vector velocity;
        public Vector Velocity { get { return velocity; } set { velocity = value; } }
        public Size Size { get; set; }
        public Rectangle DrawingRectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Size.Width-1, Size.Height-1);
            }
        }
        public Vector CollisionFix { get; set; }

        public GameObject_old()
        { }
        public GameObject_old(PointF point, Size size, float angle)
        {
            this.Position = point;
            this.Size = size;
            this.Angle = angle; // Bounding Box gets automatically initialized because of property setter
        }

        public virtual void drawBoundaryBox(Color tint, Graphics gfx)
        {
            //Angle = 90;

            int alpha = 32;
            Pen drawingPen = new Pen(tint);
            Brush drawingBrush = new SolidBrush(Color.FromArgb(alpha, tint));

 
            Bitmap bmp = new Bitmap(BoundingBox.Width, BoundingBox.Height);

            using (Graphics _gfx = Graphics.FromImage(bmp))
            {
                _gfx.Clear(Color.FromArgb(64, tint));
                _gfx.PixelOffsetMode = PixelOffsetMode.Half;

                _gfx.TranslateTransform((BoundingBox.Width) / 2f, (BoundingBox.Height) / 2f);
                _gfx.RotateTransform(Angle);

                _gfx.DrawRectangle(drawingPen, (-Size.Width) / 2f, (-Size.Height) / 2f, Size.Width, Size.Height);
                _gfx.FillRectangle(drawingBrush, (-Size.Width) / 2f, (-Size.Height) / 2f, Size.Width, Size.Height);

                _gfx.RotateTransform(-Angle);
                _gfx.TranslateTransform(-(BoundingBox.Width) / 2f, -(BoundingBox.Height) / 2f);

                _gfx.FillRectangle(Brushes.White, (BoundingBox.Width-1) / 2f, (BoundingBox.Height - 1) / 2f, 1, 1);
            }

            gfx.DrawImage(bmp, Position.X, Position.Y);

            drawingBrush.Dispose();
            drawingPen.Dispose();
            bmp.Dispose();

        }
    }
}
