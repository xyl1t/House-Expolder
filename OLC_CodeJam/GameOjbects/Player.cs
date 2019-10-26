using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace OLC_CodeJam
{
    class Player : GameObject, IMoveAble, IRotateAble
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
        public bool OnGround { get; set; }

        public new Bitmap Image { get { return Animation_Fire.GetImage(); } }

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


                BoundingBox = new Size(max_x + Math.Abs(min_x) + 1, max_y + Math.Abs(min_y) + 1 + Assets.TILE_HEIGHT);

                drawPos = new PointF(Position.X - (BoundingBox.Width - this.Size.Width) / 2, Position.Y - (BoundingBox.Height - this.Size.Height - Assets.TILE_HEIGHT) / 2);

            }
        }
        public Size BoundingBox { get; set; }

        public const int MAX_X = 100;
        public int ExplosionRadius = BOMB_EXPLOSION_RADIUS;
        public const int BOMB_EXPLOSION_RADIUS = 1;
        public const int SUPERBOMB_EXPLOSION_RADIUS = 12;
        public int Timer = 4;
        public Animation Animation_Explosion;
        public Animation Animation_BigExplosion;
        public Animation Animation_Fire;

        public PointF ExplosionPosition = new PointF(-64, -64);
        public bool HasSuperBomb { get; set; }
        private bool superBombEquiped;
        public bool SuperBombEquiped
        {
            get { return superBombEquiped; }
            set
            {
                superBombEquiped = value;

                if (value)
                    ExplosionRadius = SUPERBOMB_EXPLOSION_RADIUS;
                else
                    ExplosionRadius = BOMB_EXPLOSION_RADIUS;
            }
        }
        public SoundPlayer[] Explosions { get; set; }
        public SoundPlayer[] BigExplosions { get; set; }
        public PointF ExplosionShake { get; private set; }
        public bool Exploding { get; set; }
        public Stopwatch timer = new Stopwatch();

        public Player() : base()
        {
            Animation_Explosion = new Animation(
                Assets.SpriteAtlas.getSpriteRegion(0, 3, 15 * Assets.TILE_WIDTH, 9 * Assets.TILE_HEIGHT),
                Assets.TILE_WIDTH * 3,
                Assets.TILE_HEIGHT * 3,
                25);

            Animation_BigExplosion = new Animation(
                Assets.SpriteAtlas.getSpriteRegion(0, 12, 27 * Assets.TILE_WIDTH, 18 * Assets.TILE_HEIGHT),
                Assets.TILE_WIDTH * 9,
                Assets.TILE_HEIGHT * 9,
                100);

            Animation_Fire = new Animation(
                Assets.SpriteAtlas.getSpriteRegion(0, 3, 7 * Assets.TILE_WIDTH, 2 * Assets.TILE_HEIGHT),
                Assets.TILE_WIDTH,
                Assets.TILE_HEIGHT * 2,
                250);

            Explosions = Assets.Explosions;
            BigExplosions = Assets.BigExplosions;
        }
        public Player(PointF point, Size size, Asset asset) : base(point, size, asset)
        {

            Animation_Explosion = new Animation(
                Assets.AnimationSpriteAtlas.getSpriteRegion(0, 0, 15 * Assets.TILE_WIDTH, 9 * Assets.TILE_HEIGHT),
                Assets.TILE_WIDTH * 3,
                Assets.TILE_HEIGHT * 3,
                25);

            Animation_BigExplosion = new Animation(
                Assets.AnimationSpriteAtlas.getSpriteRegion(0, 9, 27 * Assets.TILE_WIDTH, 18 * Assets.TILE_HEIGHT),
                Assets.TILE_WIDTH * 9,
                Assets.TILE_HEIGHT * 9,
                100);

            Animation_Fire = new Animation(
                Assets.AnimationSpriteAtlas.getSpriteRegion(0, 30, 7 * Assets.TILE_WIDTH, 2 * Assets.TILE_HEIGHT),
                Assets.TILE_WIDTH,
                Assets.TILE_HEIGHT * 2,
                250);

            Explosions = Assets.Explosions;
            BigExplosions = Assets.BigExplosions;
            timer.Start();
        }


        long current;
        long expected;

        public bool explosionFinished = false;
        public void tick()
        {
            ExplosionShake = new PointF(0, 0);

            if (!SuperBombEquiped && current < expected)
            {
                if (!Animation_Explosion.Finished)
                    Animation_Explosion.tick();

                int shakeValue = (int)(expected - current) / 200;
                ExplosionShake = new PointF(ExplosionShake.X + Model.random.Next(-shakeValue, +shakeValue), ExplosionShake.Y + Model.random.Next(-shakeValue, +shakeValue));
                //ExplosionShake = new PointF(ExplosionShake.X + Model.random.Next(-2, +3), ExplosionShake.Y + Model.random.Next(-2, +3));

            }
            else if (SuperBombEquiped && current < expected)
            {
                if(!Animation_BigExplosion.Finished)
                    Animation_BigExplosion.tick();


                int shakeValue = (int)((expected - current) / 200d);
                ExplosionShake = new PointF(ExplosionShake.X + Model.random.Next(-shakeValue, +shakeValue), ExplosionShake.Y + Model.random.Next(-shakeValue, +shakeValue));

            }
            else
            { 
                explosionFinished = true;
                Animation_Explosion.Reset();
                Animation_BigExplosion.Reset();
            }


            current = timer.ElapsedMilliseconds;
        }

        public void Explode(Level currentLevel, List<ExplodingObject> explodingObjects)
        {
            for (int i = -ExplosionRadius; i <= ExplosionRadius; i++)
            {
                for (int j = -ExplosionRadius; j <= ExplosionRadius; j++)
                {
                    if (i * i + j * j <= ExplosionRadius * ExplosionRadius)
                    {
                        if (SuperBombEquiped)
                        {
                            if (Model.random.Next(Math.Abs(i) + 1) >= ExplosionRadius / 3d * 2d ||
                                Model.random.Next(Math.Abs(j) + 1) >= ExplosionRadius / 3d * 2d)
                                continue;
                        }

                        int playerX = (int)(Math.Round((Position.X + Size.Width / 2) - currentLevel[0].Position.X) / 8);
                        int playerY = (int)(Math.Round((Position.Y + Size.Height / 2) - currentLevel[0].Position.Y) / 8);
                        int index = (playerX + j) + (playerY + i) * currentLevel.Width;

                        if (((playerX + j) > -1 && (playerX + j) < currentLevel.Width && (playerY + i) >= 0 && (playerY + i) < currentLevel.Height))
                        {
                            if ((currentLevel[index].CurrentAsset.ColorID.ToArgb() & Assets.INTERACTIVE_FILTER) == Assets.INTERACTIVE_IRON && !SuperBombEquiped) continue;
                            if ((currentLevel[index].CurrentAsset.ColorID.ToArgb()) == Assets.Empty.ColorID.ToArgb()) continue;
                            if ((currentLevel[index].CurrentAsset.ColorID.ToArgb()) == Assets.SuperBomb.ColorID.ToArgb()) continue;


                            for (int repeat = 0; repeat < Model.random.Next(1, 5); repeat++)
                                explodingObjects.Add(
                                    new ExplodingObject(
                                          currentLevel[index].Position,
                                        j * 10 + Model.random.Next(-10, 10),
                                        new Vector(
                                            (j + Model.random.Next(-5, 5)) * 10,
                                            (i + Model.random.Next(-5, 5)) * 10 - 100),
                                        new Size(Model.random.Next(3, 9), Model.random.Next(3, 9)),
                                        currentLevel[index].CurrentAsset));

                            //for (int repeat = 0; repeat < Model.random.Next(1, 5); repeat++)
                            //    explodingObjects.Add(
                            //        new ExplodingObject(
                            //              currentLevel[index].Position,
                            //            j * 10 + Model.random.Next(-10, 10),
                            //            new Vector(j * 15 + Model.random.Next(-20, 20), -115 + Model.random.Next(-80, 80)),
                            //            new Size(Model.random.Next(3, 9), Model.random.Next(3, 9)),
                            //            currentLevel[index].CurrentAsset));



                            currentLevel[index].CurrentAsset = Assets.Empty;

                        }
                    }
                }
            }

            explosionFinished = false;

            Animation_Explosion.Reset();
            Animation_BigExplosion.Reset();

            ExplosionPosition = Position;
            //Exploding = true;


            if (!SuperBombEquiped)
            {
                Animation_Explosion.tick();
                current = timer.ElapsedMilliseconds;
                expected = current + (int)(1000 * 1);
            }
            else
            {
                Animation_BigExplosion.tick();
                current = timer.ElapsedMilliseconds;
                expected = current + (int)(1000 * 2);
            }

            PlayRandomExplosion();
        }

        public void Reset()
        {
            Velocity = new Vector(0, 0);
            HasSuperBomb = false;
            SuperBombEquiped = false;
            Angle = 0;
            ExplosionPosition = new PointF(-64, -64);
            ExplosionRadius = BOMB_EXPLOSION_RADIUS;
            Animation_Explosion.Reset();
            Animation_Fire.Reset();

            Position = new PointF(0, 0);
        }
        public Bitmap GetRotatedImage()
        {
            Bitmap bmp = new Bitmap(BoundingBox.Width, BoundingBox.Height);

            using (Graphics _gfx = Graphics.FromImage(bmp))
            {
                //_gfx.Clear(Color.Black);
                _gfx.PixelOffsetMode = PixelOffsetMode.Half;
                _gfx.InterpolationMode = InterpolationMode.NearestNeighbor;

                _gfx.TranslateTransform((BoundingBox.Width) / 2f, (BoundingBox.Height + Assets.TILE_HEIGHT) / 2);
                _gfx.RotateTransform(Angle);

                _gfx.DrawImage(Animation_Fire.GetImage(), (-Size.Width) / 2f, (-Size.Height) / 2f - Assets.TILE_HEIGHT);
            }

            return bmp;
        }

        public void DrawRotated(Graphics gfx, float zoom, float x, float y, float width, float height)
        {
            float tX =(x + width / 2f);
            float tY =(y + height - (Assets.TILE_HEIGHT * zoom / 2f));

            float a = angle;

            gfx.TranslateTransform(tX, tY);
            gfx.RotateTransform(a);

            gfx.DrawImage(this.Image, -width / 2f, -height + (Assets.TILE_HEIGHT * zoom / 2f), width, height);

            gfx.RotateTransform(-a);
            gfx.TranslateTransform(-tX, -tY);

        }
        public void PlayRandomExplosion()
        {
            if(!SuperBombEquiped)
                Explosions[Model.random.Next(Explosions.Length)].Play();
            else
                BigExplosions[Model.random.Next(BigExplosions.Length)].Play();

        }
    }
}
