using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLC_CodeJam
{
    class ExplodingObject : GameObject, IMoveAble, IRotateAble
    {
        private Vector velocity;
        public Vector Velocity
        {
            get
            {
                return velocity;
            }

            set
            {
                velocity = value;
            }
        }
        public new PointF Position
        {
            get
            {
                return position;
            }
            set
            {
                drawPos = new PointF(Position.X - (BoundingBox.Width - this.Size.Width) / 2, Position.Y - (BoundingBox.Height - this.Size.Height) / 2);
                position = value;
            }
        }
        public PointF drawPos;
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

                drawPos = new PointF(Position.X - (BoundingBox.Width - this.Size.Width) / 2, Position.Y - (BoundingBox.Height - this.Size.Height) / 2);

            }
        }
        public Size BoundingBox { get; set; }
        public float IntitialAngle { get; private set; }



        public ExplodingObject() : base()
        { this.IntitialAngle = this.Angle = 0; }
        public ExplodingObject(PointF point, Size size, Asset asset) : base(point, size, asset)
        { this.IntitialAngle = this.Angle = 0; }
        public ExplodingObject(PointF point, float angle, Size size, Asset asset) : base(point, size, asset)
        { this.IntitialAngle = this.Angle = angle; }
        public ExplodingObject(PointF point, float angle, Vector velocity, Size size, Asset asset) : base(point, size, asset)
        { this.IntitialAngle = this.Angle = angle; this.Velocity = velocity; }


        public Bitmap GetRotatedImage()
        {
            Bitmap bmp = new Bitmap(BoundingBox.Width, BoundingBox.Height);

            using (Graphics _gfx = Graphics.FromImage(bmp))
            {
                //_gfx.Clear(Color.Black);
                _gfx.PixelOffsetMode = PixelOffsetMode.Half;
                _gfx.InterpolationMode = InterpolationMode.NearestNeighbor;

                _gfx.TranslateTransform((BoundingBox.Width) / 2f, (BoundingBox.Height) / 2f);
                _gfx.RotateTransform(Angle);

                _gfx.DrawImage(Image, (-Size.Width) / 2f, (-Size.Height) / 2f);
            }

            return bmp;
        }
    }
}
