using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using OLC_CodeJam.Properties;

namespace OLC_CodeJam
{
    public class Model
    {
        public static Random random = new Random();
        public static DateTime startedDate = DateTime.Now;

        public bool Up { get; set; }
        public bool Down { get; set; }
        public bool Right { get; set; }
        public bool Left {get;set; }
        public bool Space { get; set; }
        public bool One { get; set; }
        public bool Two { get; set; }
        public bool Z { get; set; }
        public bool H { get; set; } = true;
        public bool S { get; set; } = false;
        public bool R { get; set; } = false;
        public bool Mouse { get;  set; }
        public Point Cursor { get;  set; }
        public MouseButtons MouseButton { get;  set; }
        private Asset currentSelectedAsset = Assets.Walls[0, 0];

        public bool Pause { get; set; } = false;
        public bool ShowHelp { get; set; } = false;
        public bool DisplayShadow { get; set; } = true;

        private Bitmap canvas;
        public Bitmap Canvas
        {
            get
            {
                return canvas;
            }
        }
        private Graphics _gfx;
        public Graphics gfx
        {
            get
            {
                if (_gfx == null)
                {
                    _gfx = Graphics.FromImage(canvas);
                    _gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    _gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    _gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                }
                return _gfx;
            }
        }
        public Size CanvasSize;
        bool alive = false;
        public bool Alive { get { return alive; } }


        public bool allowNextLevel = false;

        Stopwatch timeSpeed = new Stopwatch();


        Thread gameThread;

        public Model(int width, int height)
        {
            CanvasSize = new Size(width, height);
            canvas = new Bitmap(width, height);
        }

        Vector g; // gravity
        Vector f; // friction
        Level level;
        //List<GameObject> obstacles;
        Player player;

        List<ExplodingObject> explodingObjects;
        int destroyedObjectsAmount;


        void init()
        {
            this.g = new Vector(0, 400);
            this.f = new Vector(2.5d, 0);

            level = Level.GetCurrentLevel();

            player = new Player(new PointF(level.PlayerLocation.X * Assets.TILE_WIDTH + level.Objects[0].Position.X, level.PlayerLocation.Y * Assets.TILE_HEIGHT + level.Objects[0].Position.Y), new Size(8, 8), Assets.Bomb);
            player.Angle = 0;
            player.Velocity = new Vector(0, 0);

            explodingObjects = new List<ExplodingObject>();



            zoomImage = new Bitmap((int)zoom * 400, (int)zoom * 300);
            zoomGfx = Graphics.FromImage(zoomImage);
            zoomGfx.InterpolationMode = InterpolationMode.NearestNeighbor;
            zoomGfx.PixelOffsetMode = PixelOffsetMode.Half;

            timeSpeed.Start();
            //obstacles = Loader.GetObstacles(level1);
        }


        int fps;
        void run()
        {
            init();


            //int fps = 60;
            //double timePerTick = 1000d / fps;
            //double delta = 0;
            //int now = 0;
            //int lastTime = Environment.TickCount;
            //int timer = 0;
            //int ticks = 0;

            //while (alive)
            //{
            //    now = Environment.TickCount;
            //    delta += (double)now - lastTime;
            //    timer += (int)now - lastTime;
            //    lastTime = now;

            //    if (delta >= timePerTick)
            //    {
            //        double dTime = delta / 1000d;
            //        if (dTime > 0.16d)
            //            dTime = 0.16d;
            //        else if (dTime <= 0.01)
            //            dTime = 0.01d;

            //        tick(dTime);
            //        render();

            //        ticks++;
            //        delta -= timePerTick;

            //    }
            //    if (timer >= 1000)
            //    {
            //        Debug.Print("fps: " + (ticks));
            //        ticks = 0;
            //        timer = 0;
            //    }
            //}

            //double prev_frame_tick;
            //double curr_frame_tick = Environment.TickCount;
            //double dTime = 0;


            //while (alive)
            //{

            //    prev_frame_tick = curr_frame_tick;
            //    curr_frame_tick = Environment.TickCount;

            //    Debug.Print((curr_frame_tick - prev_frame_tick).ToString());

            //    dTime = (curr_frame_tick - prev_frame_tick) / 1000d;

            //    tick(dTime);

            //    render();

            //    Thread.Sleep(0);
            //};



            Stopwatch timer = new Stopwatch();
            long old = 0;
            long now = 0;

            timer.Start();

            while (alive)
            {
                old = now;
                now = timer.ElapsedMilliseconds;

                
                tick((now - old) / 1000f);
                render();

                fps = (int)(1 / ((now - old) / 1000f));
                Debug.Print(fps.ToString());


                //Thread.Sleep(50);
            };
        }

        void tick(double elapsedTime)
        {

            if (H)
            {
                ShowHelp = !ShowHelp;
                H = false;
                Pause = !Pause;
            }

            if (Pause)
            {
                timeSpeed.Stop();
                player.timer.Stop();
                return;
            }

            else if (!timeSpeed.IsRunning && !allowNextLevel)
            {
                timeSpeed.Start();
            }
            player.timer.Start();

            

            if (Space)
            {

                if (allowNextLevel && player.explosionFinished)
                {
                    Level.NextLevel(player);

                    level = Level.GetCurrentLevel();
                    allowNextLevel = false;

                    destroyedObjectsAmount = 0;
                    timeSpeed.Restart();
                    timeSpeed.Start();

                    Space = false;

                }

                else if (!allowNextLevel && player.SuperBombEquiped)
                {
                    player.Explode(level, explodingObjects);
                    destroyedObjectsAmount += explodingObjects.Count;

                    allowNextLevel = true;
                    timeSpeed.Stop();
                }
                else if (!player.SuperBombEquiped && player.Animation_Explosion.Finished)
                {
                    player.Explode(level, explodingObjects);
                    destroyedObjectsAmount += explodingObjects.Count;
                    player.Animation_Fire.Reset();
                }

            }
            if (!(allowNextLevel /*&& player.SuperBombEquiped*/ && player.explosionFinished))
            {
                if (Up && player.OnGround)
                {
                    player.Velocity = new Vector(player.Velocity.X, -192);
                }
                //if (Down)
                //{
                //    player.Velocity = new Vector(player.Velocity.X, +192);
                //}
                if (Right)
                {
                    player.Velocity = new Vector(player.Velocity.X + 300 * elapsedTime, player.Velocity.Y);
                }
                if (Left)
                {
                    player.Velocity = new Vector(player.Velocity.X - 300 * elapsedTime, player.Velocity.Y);

                }
                if(!allowNextLevel)
                { 
                if (One)
                {
                    player.SuperBombEquiped = false;
                }
                if (Two && player.HasSuperBomb)
                {
                    player.SuperBombEquiped = true;
                }
                }
                if (Z)
                {
                    zoom = (zoom + 1) % 5;
                    if (zoom < 2)
                        zoom = 2;

                    Z = false;
                }
                if (S && DisplayShadow)
                {
                    DisplayShadow = false;

                    S = false;
                }
                if (S && !DisplayShadow)
                {
                    DisplayShadow = true;

                    S = false;
                }

                if (R )
                {
                    allowNextLevel = false;
                    level.Restart(player);
                    level = Level.GetCurrentLevel();
                    timeSpeed.Restart();
                    destroyedObjectsAmount = 0;

                    R = false;
                }

                //if (Mouse)
                //{
                //    int x = (int)((Cursor.X + Camera.X - (level[0].Position.X) * zoom) / zoom / Assets.TILE_WIDTH);
                //    int y = (int)((Cursor.Y + Camera.Y - (level[0].Position.Y) * zoom) / zoom / Assets.TILE_HEIGHT);

                //    if (MouseButton == MouseButtons.Left)
                //    {
                //        if (x >= 0 && x < level.Width && y >= 0 && y < level.Height)
                //        {
                //            level[x + y * level.Width].CurrentAsset = currentSelectedAsset;
                //        }
                //    }
                //    else if (MouseButton == MouseButtons.Right)
                //    {
                //        if (x >= 0 && x < level.Width && y >= 0 && y < level.Height)
                //        {
                //            currentSelectedAsset = level[x + y * level.Width].CurrentAsset;
                //        }
                //    }
                //}

                player.Animation_Fire.tick();
            }


            if (player.Animation_Fire.Finished && !player.SuperBombEquiped)
            {
                player.Explode(level, explodingObjects);
                destroyedObjectsAmount += explodingObjects.Count;

                //explode();
                //player.Explosions[random.Next(player.Explosions.Length)].Play();
                //player.Timer = 4;
            }

            if (player.Velocity.X > Player.MAX_X)
                player.Velocity = new Vector(Player.MAX_X, player.Velocity.Y);
            if (player.Velocity.X < -Player.MAX_X)
                player.Velocity = new Vector(-Player.MAX_X, player.Velocity.Y);

            if (player.OnGround)
                player.Velocity = new Vector(player.Velocity.X + player.Velocity.X * -f.X * elapsedTime, player.Velocity.Y);


            if (Math.Abs(player.Velocity.X) < 1)
                player.Velocity = new Vector(0, player.Velocity.Y);


            player.Velocity = new Vector(player.Velocity.X, player.Velocity.Y + g.Y * elapsedTime);

            if (allowNextLevel && player.SuperBombEquiped && player.explosionFinished)
                player.Velocity = new Vector(0, 0);


            for (int i = 0; i < explodingObjects.Count; i++)
            {
                explodingObjects[i].Velocity = new Vector(explodingObjects[i].Velocity.X, explodingObjects[i].Velocity.Y + g.Y * elapsedTime);
                explodingObjects[i].Position = explodingObjects[i].Position + explodingObjects[i].Velocity * elapsedTime;
                explodingObjects[i].Angle = explodingObjects[i].Angle + explodingObjects[i].IntitialAngle % 360;
            }

            checkCollision(elapsedTime);


            if (!player.SuperBombEquiped)
                player.Angle = (player.Angle + (float)player.Velocity.X * (8 * (float)elapsedTime)) % 360;
            else
                player.Angle = 0;
            player.Position = player.Position + player.Velocity * elapsedTime;


            keepWithinBorders(elapsedTime);

            Camera = player.Position;


            //if (!player.Animation_Explosion.Finished)
            //{
            //    player.Animation_Explosion.tick();

            //    Camera.X += random.Next(-3, 3);
            //    Camera.Y += random.Next(-3, 3);
            //}

            //if (!player.Animation_Explosion.Finished)
            //player.ExplosionShake = new PointF(Camera.X + random.Next(-2, 2), Camera.Y + random.Next(-2, 2));
            if (zoom > 2)
            {
                Camera = new PointF(
                    ((player.Position.X + player.Size.Width / 2) - CanvasSize.Width / 2 / zoom) * zoom,
                    (((player.Position.Y + player.Size.Height / 2)) - CanvasSize.Height / 2 / zoom) * zoom);

                if (Camera.X < 0)
                    Camera = new PointF(0, Camera.Y);
                if (Camera.Y < 0)
                    Camera = new PointF(Camera.X, 0);
                if (Camera.X > Assets.Background.Width * zoom - CanvasSize.Width)
                    Camera = new PointF(Assets.Background.Width * zoom - CanvasSize.Width, Camera.Y);
                if (Camera.Y > Assets.Background.Height * zoom - CanvasSize.Height)
                    Camera = new PointF(Camera.X, Assets.Background.Height * zoom - CanvasSize.Height);
            }
            else
                Camera = new PointF();

            // adding explosion shakes if there are any
            Camera = new PointF(Camera.X + player.ExplosionShake.X * (zoom -1), Camera.Y + player.ExplosionShake.Y * (zoom - 1));


            player.tick();
        }


        //private void explode()
        //{
        //    for (int i = -player.ExplosionRadius; i <= player.ExplosionRadius; i++)
        //    {
        //        for (int j = -player.ExplosionRadius; j <= player.ExplosionRadius; j++)
        //        {
        //            if (i * i + j * j <= player.ExplosionRadius * player.ExplosionRadius)
        //            {
        //                int playerX = (int)(Math.Round((player.Position.X) - level[0].Position.X) / 8);
        //                int playerY = (int)(Math.Round((player.Position.Y) - level[0].Position.Y) / 8);
        //                int index = (playerX + j) + (playerY + i) * level.Width;

        //                if (((playerX + j) > -1 && (playerX + j) < level.Width && (playerY + i) >= 0 && (playerY + i) < level.Height))
        //                {
        //                    if (index == -1) Debugger.Break();
        //                    if ((level[index].CurrentAsset.ColorID.ToArgb() & Assets.INTERACTIVE_FILTER) == Assets.INTERACTIVE_IRON) continue;

        //                    explodingObjects.Add(
        //                        new ExplodingObject(
        //                            level[index].Position,
        //                            j * 10 + random.Next(-10, 10),
        //                            new Vector(j * 15 + random.Next(-20, 20), -115 + random.Next(-80, 80)),
        //                            level[index].Size,
        //                            level[index].CurrentAsset));

        //                    level[index].CurrentAsset = Assets.Empty;

        //                }
        //            }
        //        }
        //    }
        //    if (player.Animation_Fire.Finished || !player.Animation_Explosion.Finished)
        //    {
        //        player.Animation_Explosion.tick();
        //    }

        //}
        private void checkCollision(double elapsedTime)
        {
            player.OnGround = false;
            for (int i = 0; i < level.Objects.Count; i++)
            {
                if (!level.Objects[i].IsPassable)
                {

                    if (player.Position.X + player.Velocity.X * elapsedTime + player.Size.Width  - 1> level.Objects[i].Position.X &&
                        (int)player.Position.Y + player.Size.Height  -1> level.Objects[i].Position.Y &&
                        player.Position.X + player.Velocity.X * elapsedTime +1< level.Objects[i].Position.X + level.Objects[i].Size.Width &&
                        (int)player.Position.Y +1< level.Objects[i].Position.Y + level.Objects[i].Size.Height)
                    {
                        if (player.Velocity.X > 0)
                        {
                            player.Velocity = new Vector(0, player.Velocity.Y);
                            player.Position = new PointF(level.Objects[i].Position.X - player.Size.Width+1, player.Position.Y);

                        }
                        else if (player.Velocity.X < 0)
                        {
                            player.Velocity = new Vector(0, player.Velocity.Y);
                            player.Position = new PointF(level.Objects[i].Position.X + level.Objects[i].Size.Width-1, player.Position.Y);
                        }
                    }

                    if (player.Position.X + player.Size.Width - 1 > level.Objects[i].Position.X &&
                        player.Position.Y + player.Velocity.Y * elapsedTime + player.Size.Height-1 > level.Objects[i].Position.Y &&
                        player.Position.X +1< level.Objects[i].Position.X + level.Objects[i].Size.Width &&
                        player.Position.Y + player.Velocity.Y * elapsedTime +1< level.Objects[i].Position.Y + level.Objects[i].Size.Height)
                    {
                        if (player.Velocity.Y > 0)
                        {
                            player.Velocity = new Vector(player.Velocity.X, 0);
                            player.Position = new PointF(player.Position.X, level.Objects[i].Position.Y - player.Size.Height+1);

                            player.OnGround = true;

                        }
                        else if (player.Velocity.Y < 0)
                        {
                            player.Velocity = new Vector(player.Velocity.X, 0);
                            player.Position = new PointF(player.Position.X, level.Objects[i].Position.Y + level.Objects[i].Size.Height-1);
                        }
                    }
                }
                else if (level.Objects[i].CurrentAsset.ColorID.ToArgb() == Assets.SuperBomb.ColorID.ToArgb())
                {
                    if (player.Position.X + player.Size.Width > level.Objects[i].Position.X &&
                        player.Position.Y + player.Size.Height > level.Objects[i].Position.Y &&
                        player.Position.X < level.Objects[i].Position.X + level.Objects[i].Size.Width &&
                        player.Position.Y < level.Objects[i].Position.Y + level.Objects[i].Size.Height)
                    {
                        /*
                         * 258 = x + y * 32;
                        */
                        player.HasSuperBomb = true;
                        if (i - level.Width >= 0 && level.Objects[i - level.Width].IsPassable)
                            level.Objects[i].CurrentAsset = level.Objects[i - level.Width].CurrentAsset;
                        else if (i - 1 >= 0 && level.Objects[i - 1].IsPassable)
                            level.Objects[i].CurrentAsset = level.Objects[i - 1].CurrentAsset;
                        else if (i + 1 < level.Objects.Count && level.Objects[i + 1].IsPassable)
                            level.Objects[i].CurrentAsset = level.Objects[i + 1].CurrentAsset;
                        else if (i + level.Width < level.Objects.Count && level.Objects[i + level.Width].IsPassable)
                            level.Objects[i].CurrentAsset = level.Objects[i + level.Width].CurrentAsset;
                        else
                            level.Objects[i].CurrentAsset = Assets.WoodVerticalInside[0];
                    }
                }
            }
        }
        private void keepWithinBorders(double elapsedTime)
        {
            if (player.Position.Y + player.Size.Height >= Assets.Background.Height && !allowNextLevel)
            {
                level.Restart(player);
                level = Level.GetCurrentLevel();
                timeSpeed.Restart();
                destroyedObjectsAmount = 0;
            }
            //if (bricks[i].Position.Y <= 0)
            //{
            //    bricks[i].Velocity = new Vector(0, 0);
            //    bricks[i].Position = new PointF(bricks[i].Position.X, 0);
            //}

            for (int i = explodingObjects.Count-1; i >=0; i--)
            {
                if (explodingObjects[i].Position.Y + explodingObjects[i].Size.Height >= Assets.Background.Height + explodingObjects[i].Size.Height)
                {
                    explodingObjects.RemoveAt(i);
                }
            }
        }


        float zoom = 3;
        PointF Camera;
        Bitmap zoomImage;
        Graphics zoomGfx;

        bool top = false, bottom = false, right = false, left = false;
        bool topRight = false, bottomRight = false, topLeft = false, bottomLeft = false;
        void render()
        {
            lock (canvas)
            {
                //gfx.Clear(Color.Black);
                
                if (ShowHelp)
                { 
                    for (int i = 0; i < CanvasSize.Width / (Assets.TILE_WIDTH * 2); i++)
                    {
                        for (int j = 0; j < (CanvasSize.Height+8) / (Assets.TILE_HEIGHT * 2); j++)
                        {

                            gfx.DrawImage(Assets.WallsInside[(i)*7%3, j*7%3].Image, new RectangleF(
                                    i * (Assets.TILE_WIDTH * 1) * 2,
                                    j * (Assets.TILE_HEIGHT * 1) * 2,
                                    Assets.WallsInside[0, 0].Width * 2,
                                    Assets.WallsInside[0, 0].Height * 2));
                        }
                    }

                    Font.DrawText(gfx, "House Exploder",
                        true, CanvasSize.Width, 32,
                        32, 32, Color.Gold, Color.DarkRed);
                    Font.DrawText(gfx,
                        "\n\n\n\n\n" +
                        "how to play:\n\n" +
                        " youre a bomb that explodes every few\n" +
                        " seconds and destroys everything except\n" +
                        " the iron block. the iron block can only\n" +
                        " be destroyed by the 'Super Bomb'.\n" +
                        " Your goal is to get the 'Super Bomb' and\n" +
                        " destroy as much as you can.\n" +
                        " If you jump out of the building you lose.\n\n" +

                        "Controls:\n\n" +
                        " Move:Arrow keys\n" +
                        " z:Toggle zoom\n" +
                        " s:Toggle shadows\n" +
                        " r:Restart level\n" +
                        " 1,2 switch between normal and super bomb\n" +
                        " space:Explode bomb or superbomb\n" +
                        " p:Pause\n\n" +
                        
                        "Credits:\n\n" +
                        " Developer: Marat Isaw\n" +
                        " Technologies used:\n" +
                        "  Overall game: Csharp WinForms\n" +
                        "  Graphics: Paint.NET\n" +
                        "  Sound: BFXR, audacity" +

                        "\n\n\n",

                        true, CanvasSize.Width, 16,
                        16, 16);

                    Font.DrawText(gfx, "Press 'H' to continue",
                                  true, CanvasSize.Width, CanvasSize.Height - 16*2,
                                  16, 16, Color.Gold, Color.Black);

                    return;
                }


                gfx.DrawImage(Assets.Background.Image, new RectangleF(-(int)Camera.X, -(int)Camera.Y, Assets.Background.Width * zoom, Assets.Background.Height * zoom));

                for (int i = 0; i < level.Objects.Count; i++)
                {
                    if (level[i].CurrentAsset.ColorID == Assets.Empty.ColorID) continue;

                    if (level[i].Position.X + 8 < Camera.X / zoom ||
                        level[i].Position.Y + 8 < Camera.Y / zoom ||
                        level[i].Position.X > Camera.X / zoom + CanvasSize.Width / zoom ||
                        level[i].Position.Y > Camera.Y / zoom + CanvasSize.Height / zoom)
                        continue;


                    float x = level[i].Position.X * zoom - (int)Camera.X;
                    float y = level[i].Position.Y * zoom - (int)Camera.Y;
                    float width = level[i].Image.Width * zoom;
                    float height = level[i].Image.Height * zoom;

                    if (level[i].Image.Height == Assets.TILE_HEIGHT * 2)
                    {
                        gfx.DrawImage(level[i].Image, new RectangleF(
                            x,
                            y - Assets.TILE_HEIGHT * zoom,
                            width,
                            height));

                        continue;
                    }

                    gfx.DrawImage(level[i].Image, new RectangleF(
                            x,
                            y,
                            width,
                            height));

                    if (DisplayShadow)
                    {
                        top = bottom = left = right = topRight = topLeft = bottomLeft = bottomRight = false;



                        if (level.Objects[i].IsPassable &&
                            (level.Objects[i].CurrentAsset.ColorID != Assets.Empty.ColorID) &&
                            (level.Objects[i].CurrentAsset.ColorID != Assets.Glass.ColorID) &&
                            (level.Objects[i].CurrentAsset.ColorID.ToArgb() & Assets.INTERACTIVE_FILTER) != Assets.IRONBARS)
                        {
                            if (i - level.Width >= 0)
                                if (!level.Objects[i - level.Width].IsPassable)
                                    top = true;
                            if (i + level.Width < level.Objects.Count)
                                if (!level.Objects[i + level.Width].IsPassable)
                                    bottom = true;
                            if (i - 1 >= 0)
                                if (!level.Objects[i - 1].IsPassable)
                                    left = true;
                            if (i + 1 < level.Objects.Count)
                                if (!level.Objects[i + 1].IsPassable)
                                    right = true;

                            if (i - level.Width - 1 >= 0)
                                if (!level.Objects[i - level.Width - 1].IsPassable)
                                    topLeft = true;
                            if (i - level.Width + 1 >= 0)
                                if (!level.Objects[i - level.Width + 1].IsPassable)
                                    topRight = true;
                            if (i + level.Width - 1 < level.Objects.Count)
                                if (!level.Objects[i + level.Width - 1].IsPassable)
                                    bottomLeft = true;
                            if (i + level.Width + 1 < level.Objects.Count)
                                if (!level.Objects[i + level.Width + 1].IsPassable)
                                    bottomRight = true;

                            if (top)
                            {
                                gfx.DrawImage(Assets.ShadowStraight[0].Image, new RectangleF(
                                    x,
                                    y,
                                    width,
                                    height));
                            }
                            if (bottom)
                            {
                                gfx.DrawImage(Assets.ShadowStraight[1].Image, new RectangleF(
                                    x,
                                    y,
                                    width,
                                    height));
                            }
                            if (left)
                            {
                                gfx.DrawImage(Assets.ShadowStraight[2].Image, new RectangleF(
                                    x,
                                    y,
                                    width,
                                    height));
                            }
                            if (right)
                            {
                                gfx.DrawImage(Assets.ShadowStraight[3].Image, new RectangleF(
                                    x,
                                    y,
                                    width,
                                    height));
                            }

                            if (topLeft && !top && !left)
                            {
                                gfx.DrawImage(Assets.ShadowDiagonal[0].Image, new RectangleF(
                                        x,
                                        y,
                                        width,
                                        height));
                            }
                            if (topRight && !top && !right)
                            {
                                gfx.DrawImage(Assets.ShadowDiagonal[1].Image, new RectangleF(
                                      x,
                                      y,
                                      width,
                                      height));
                            }
                            if (bottomLeft && !bottom && !left)
                            {
                                gfx.DrawImage(Assets.ShadowDiagonal[2].Image, new RectangleF(
                                      x,
                                      y,
                                      width,
                                      height));
                            }
                            if (bottomRight && !bottom && !right)
                            {
                                gfx.DrawImage(Assets.ShadowDiagonal[3].Image, new RectangleF(
                                      x,
                                      y,
                                      width,
                                      height));
                            }
                        }

                        topRight = topLeft = bottomLeft = bottomRight = false;

                        if (level.Objects[i].IsPassable &&
                              level.Objects[i].CurrentAsset.ColorID != Assets.Empty.ColorID &&
                              (level.Objects[i].CurrentAsset.ColorID.ToArgb() & Assets.LIGHT_FILTER) != Assets.LIGHT)
                        {
                            if (i - level.Width - 1 > 0)
                                if ((level.Objects[i - level.Width - 1].CurrentAsset.ColorID.ToArgb() & Assets.LIGHT_FILTER) == Assets.LIGHT || level.Objects[i - level.Width - 1].CurrentAsset.ColorID == Assets.Empty.ColorID)
                                    topLeft = true;
                            if (i - level.Width + 1 > 0)
                                if ((level.Objects[i - level.Width + 1].CurrentAsset.ColorID.ToArgb() & Assets.LIGHT_FILTER) == Assets.LIGHT || level.Objects[i - level.Width + 1].CurrentAsset.ColorID == Assets.Empty.ColorID)
                                    topRight = true;
                            if (i + level.Width < level.Objects.Count)
                                if ((level.Objects[i + level.Width - 1].CurrentAsset.ColorID.ToArgb() & Assets.LIGHT_FILTER) == Assets.LIGHT || level.Objects[i + level.Width - 1].CurrentAsset.ColorID == Assets.Empty.ColorID)
                                    bottomLeft = true;
                            if (i + level.Width + 1 < level.Objects.Count)
                                if ((level.Objects[i + level.Width + 1].CurrentAsset.ColorID.ToArgb() & Assets.LIGHT_FILTER) == Assets.LIGHT || level.Objects[i + level.Width + 1].CurrentAsset.ColorID == Assets.Empty.ColorID)
                                    bottomRight = true;

                            if (topLeft && !top && !left)
                            {
                                gfx.DrawImage(Assets.LightDiagonal[0].Image, new RectangleF(
                                        x,
                                        y,
                                        width,
                                        height));
                            }
                            if (topRight && (!top || !right))
                            {
                                gfx.DrawImage(Assets.LightDiagonal[1].Image, new RectangleF(
                                        x,
                                        y,
                                        width,
                                        height));
                            }
                            if (bottomLeft && (!bottom || !left))
                            {
                                gfx.DrawImage(Assets.LightDiagonal[2].Image, new RectangleF(
                                        x,
                                        y,
                                        width,
                                        height));
                            }
                            if (bottomRight && (!bottom || !right))
                            {
                                gfx.DrawImage(Assets.LightDiagonal[3].Image, new RectangleF(
                                        x,
                                        y,
                                        width,
                                        height));
                            }

                            top = bottom = left = right = false;

                            if (i - level.Width >= 0)
                                if ((level.Objects[i - level.Width].CurrentAsset.ColorID.ToArgb() & Assets.LIGHT_FILTER) == Assets.LIGHT || level.Objects[i - level.Width].CurrentAsset.ColorID == Assets.Empty.ColorID)
                                    top = true;
                            if (i + level.Width < level.Objects.Count)
                                if ((level.Objects[i + level.Width].CurrentAsset.ColorID.ToArgb() & Assets.LIGHT_FILTER) == Assets.LIGHT || level.Objects[i + level.Width].CurrentAsset.ColorID == Assets.Empty.ColorID)
                                    bottom = true;
                            if (i - 1 >= 0)
                                if ((level.Objects[i - 1].CurrentAsset.ColorID.ToArgb() & Assets.LIGHT_FILTER) == Assets.LIGHT || level.Objects[i - 1].CurrentAsset.ColorID == Assets.Empty.ColorID)
                                    left = true;
                            if (i + 1 < level.Objects.Count)
                                if ((level.Objects[i + 1].CurrentAsset.ColorID.ToArgb() & Assets.LIGHT_FILTER) == Assets.LIGHT || level.Objects[i + 1].CurrentAsset.ColorID == Assets.Empty.ColorID)
                                    right = true;


                            if (top)
                            {
                                gfx.DrawImage(Assets.LightStraight[0].Image, new RectangleF(
                                    x,
                                    y,
                                    width,
                                    height));
                            }
                            if (bottom)
                            {
                                gfx.DrawImage(Assets.LightStraight[1].Image, new RectangleF(
                                    x,
                                    y,
                                    width,
                                    height));
                            }
                            if (left)
                            {
                                gfx.DrawImage(Assets.LightStraight[2].Image, new RectangleF(
                                    x,
                                    y,
                                    width,
                                    height));
                            }
                            if (right)
                            {
                                gfx.DrawImage(Assets.LightStraight[3].Image, new RectangleF(
                                    x,
                                    y,
                                    width,
                                    height));
                            }

                        }
                    }



                    //GameObject.DrawBoundaryBox(Color.Red, gfx, x,
                    //           y,
                    //           width,
                    //           height);
                }

                //if (MouseButton == MouseButtons.Right && Mouse)
                //{
                //    int xx = (int)((Cursor.X + Camera.X - (level[0].Position.X) * zoom) / zoom / Assets.TILE_WIDTH);
                //    int yy = (int)((Cursor.Y + Camera.Y - (level[0].Position.Y) * zoom) / zoom / Assets.TILE_HEIGHT);

                //    if (xx >= 0 && xx < level.Width && yy >= 0 && yy < level.Height)
                //    {

                //        float x = level[xx + yy * level.Height].Position.X * zoom - (int)Camera.X;
                //        float y = level[xx + yy * level.Height].Position.Y * zoom - (int)Camera.Y;
                //        float width = level[xx + yy * level.Height].Image.Width * zoom;
                //        float height = level[xx + yy * level.Height].Image.Height * zoom;
                //        if (level[xx + yy * level.Height].CurrentAsset.Height == Assets.TILE_HEIGHT * 2)
                //            y -= Assets.TILE_HEIGHT * zoom;

                //        for (int i = 0; i < 2; i++)
                //            for (int x1 = -1; x1 < 2; x1++)
                //            {
                //                for (int y1 = -1; y1 < 2; y1++)
                //                {
                //                    if (x1 == 0 && y1 == 0)
                //                        continue;

                //                    gfx.DrawImage(Assets.Shadows[y1 + 1, x1 + 1].Image, new RectangleF(
                //                            x + x1 * zoom * Assets.TILE_WIDTH,
                //                            y + y1 * zoom * Assets.TILE_HEIGHT,
                //                            width,
                //                            height));
                //                }
                //            }
                //    }
                //}

                //for (int i = 0; i < level.Objects.Count; i++)
                //{

                //    float x = level[i].Position.X * zoom - (int)Camera.X;
                //    float y = level[i].Position.Y * zoom - (int)Camera.Y;
                //    float width = level[i].Image.Width * zoom;
                //    float height = level[i].Image.Height * zoom;

                //    if ((level.Objects[i].CurrentAsset.ColorID.ToArgb() & Assets.LIGHT_FILTER) == Assets.LIGHT ||
                //             level.Objects[i].CurrentAsset.ColorID == Assets.Empty.ColorID)
                //    {
                //        if (i + level.Width < level.Objects.Count && level.Objects[i + level.Width].IsPassable && level.Objects[i + level.Width].CurrentAsset.ColorID != Assets.Empty.ColorID)
                //        {
                //            gfx.DrawImage(Assets.LightStraight[0].Image, new RectangleF(
                //                level[i + level.Width].Position.X * zoom - (int)Camera.X,
                //                level[i + level.Width].Position.Y * zoom - (int)Camera.Y,
                //                width,
                //                height));
                //        }
                //        if (i - level.Width >= 0 && level.Objects[i - level.Width].IsPassable && level.Objects[i - level.Width].CurrentAsset.ColorID != Assets.Empty.ColorID)
                //        {
                //            gfx.DrawImage(Assets.LightStraight[1].Image, new RectangleF(
                //                level[i - level.Width].Position.X * zoom - (int)Camera.X,
                //                level[i - level.Width].Position.Y * zoom - (int)Camera.Y,
                //                width,
                //                height));
                //        }
                //        if (i + 1 < level.Objects.Count && level.Objects[i + 1].IsPassable && level.Objects[i +1].CurrentAsset.ColorID != Assets.Empty.ColorID)
                //        {
                //            gfx.DrawImage(Assets.LightStraight[2].Image, new RectangleF(
                //                level[i+1].Position.X * zoom - (int)Camera.X,
                //                level[i+1].Position.Y * zoom - (int)Camera.Y,
                //                width,
                //                height));
                //        }
                //        if (i - 1 >= 0 && level.Objects[i - 1].IsPassable && level.Objects[i - 1].CurrentAsset.ColorID != Assets.Empty.ColorID)
                //        {
                //            gfx.DrawImage(Assets.LightStraight[3].Image, new RectangleF(
                //                level[i - 1].Position.X * zoom - (int)Camera.X,
                //                level[i - 1].Position.Y * zoom - (int)Camera.Y,
                //                width,
                //                height));
                //        }



                //        if (i + level.Width + 1< level.Objects.Count && level.Objects[i + level.Width + 1].IsPassable && level.Objects[i + level.Width + 1].CurrentAsset.ColorID != Assets.Empty.ColorID)
                //        {
                //            gfx.DrawImage(Assets.LightDiagonal[0].Image, new RectangleF(
                //                level[i + level.Width + 1].Position.X * zoom - (int)Camera.X,
                //                level[i + level.Width + 1].Position.Y * zoom - (int)Camera.Y,
                //                width,
                //                height));
                //        }
                //        if (i + level.Width - 1 < level.Objects.Count && level.Objects[i + level.Width - 1].IsPassable && level.Objects[i + level.Width - 1].CurrentAsset.ColorID != Assets.Empty.ColorID)
                //        {
                //            gfx.DrawImage(Assets.LightDiagonal[1].Image, new RectangleF(
                //                level[i + level.Width - 1].Position.X * zoom - (int)Camera.X,
                //                level[i + level.Width - 1].Position.Y * zoom - (int)Camera.Y,
                //                width,
                //                height));
                //        }

                //        if (i - level.Width  - 1>= 0 && level.Objects[i - level.Width - 1].IsPassable && level.Objects[i - level.Width - 1].CurrentAsset.ColorID != Assets.Empty.ColorID)
                //        {
                //            gfx.DrawImage(Assets.LightDiagonal[3].Image, new RectangleF(
                //                level[i - level.Width - 1].Position.X * zoom - (int)Camera.X,
                //                level[i - level.Width - 1].Position.Y * zoom - (int)Camera.Y,
                //                width,
                //                height));
                //        }
                //        if (i - level.Width + 1 >= 0 && level.Objects[i - level.Width + 1].IsPassable && level.Objects[i - level.Width +1].CurrentAsset.ColorID != Assets.Empty.ColorID)
                //        {
                //            gfx.DrawImage(Assets.LightDiagonal[2].Image, new RectangleF(
                //                level[i - level.Width + 1].Position.X * zoom - (int)Camera.X,
                //                level[i - level.Width + 1].Position.Y * zoom - (int)Camera.Y,
                //                width,
                //                height));
                //        }


                //    }
                //}

                //gfx.TranslateTransform((int)(player.Position.X + player.Size.Width / 2f), (int)(player.Position.Y + player.Size.Height - (Assets.TILE_HEIGHT * zoom / 2f)));
                //gfx.RotateTransform(player.Angle);

                if (!player.SuperBombEquiped)
                    player.DrawRotated(gfx, zoom, player.Position.X * zoom - (int)Camera.X,
                    player.Position.Y * zoom - Assets.TILE_HEIGHT * zoom - (int)Camera.Y,
                    player.Size.Width * zoom,
                    player.Size.Height * 2 * zoom);
                //gfx.DrawImage(player.GetRotatedImage(),
                //    player.Position.X * zoom - (int)Camera.X,
                //    player.Position.Y * zoom - Assets.TILE_HEIGHT * zoom - (int)Camera.Y,
                //    player.Size.Width * zoom,
                //    player.Size.Height * 2 * zoom);
                else
                    gfx.DrawImage(Assets.SuperBomb.Image, player.Position.X * zoom - (int)Camera.X, player.Position.Y * zoom - Assets.TILE_HEIGHT * zoom - (int)Camera.Y, Assets.SuperBomb.Image.Width * zoom, Assets.SuperBomb.Image.Height * zoom);
                gfx.ResetTransform();

                //GameObject.DrawBoundaryBox(Color.Green, gfx,
                //    player.Angle, 
                //    (int)zoom, 
                //    player.Position.X * zoom - (int)Camera.X, 
                //    player.Position.Y * zoom - Assets.TILE_HEIGHT * zoom - (int)Camera.Y,
                //    player.Size.Width * zoom, 
                //    player.Size.Height * 2 * zoom);


                for (int i = 0; i < explodingObjects.Count; i++)
                    gfx.DrawImage(explodingObjects[i].GetRotatedImage(),
                                explodingObjects[i].drawPos.X * zoom - Camera.X,
                                explodingObjects[i].drawPos.Y * zoom - Camera.Y,
                                explodingObjects[i].BoundingBox.Width * zoom,
                                explodingObjects[i].BoundingBox.Height * zoom);

                //gfx.DrawImage(player.Fire.GetImage(), player.Position.X * zoom - Camera.X, player.Position.Y * zoom - Assets.TILE_HEIGHT*zoom - Camera.Y, player.Fire.GetImage().Width* zoom, player.Fire.GetImage().Height * zoom);

                if (!player.Animation_Explosion.Finished && !player.SuperBombEquiped)
                {
                    player.Animation_BigExplosion.Reset();
                    gfx.DrawImage(
                      player.Animation_Explosion.GetImage(),
                      player.ExplosionPosition.X * zoom - Assets.TILE_WIDTH * zoom - Camera.X,
                      player.ExplosionPosition.Y * zoom - Assets.TILE_HEIGHT * zoom - Camera.Y,
                      player.Animation_Explosion.GetImage().Width * zoom,
                      player.Animation_Explosion.GetImage().Height * zoom);

                }
                else
                    player.Animation_Explosion.Reset();
                if (!player.Animation_BigExplosion.Finished && player.SuperBombEquiped)
                {
                    player.Animation_Explosion.Reset();
                    gfx.DrawImage(
                      player.Animation_BigExplosion.GetImage(),
                      player.ExplosionPosition.X * (zoom) - Assets.TILE_WIDTH*27/2 * zoom - Camera.X,
                      player.ExplosionPosition.Y * zoom - Assets.TILE_HEIGHT*27/2 * zoom - Camera.Y,
                      player.Animation_BigExplosion.GetImage().Width * zoom*3,
                      player.Animation_BigExplosion.GetImage().Height * zoom*3);
                }
                else
                    player.Animation_BigExplosion.Reset();

                string time = "time: " + timeSpeed.ElapsedMilliseconds/1000d;
                string score = "score: " + destroyedObjectsAmount;
                string FPS = "fps: " + fps;
                //string mousePosition = String.Format("X: {0}, Y: {1}",
                //    (int)((Cursor.X + Camera.X - (level[0].Position.X + Assets.TILE_WIDTH) * zoom) / zoom / Assets.TILE_WIDTH), (int)((Cursor.Y + Camera.Y - (level[0].Position.Y + Assets.TILE_HEIGHT) * zoom) / zoom / +Assets.TILE_HEIGHT));

                Font.DrawText(gfx, time, 8, 8, Assets.TILE_WIDTH * 2, Assets.TILE_HEIGHT * 2);
                Font.DrawText(gfx, score, 8, Assets.TILE_HEIGHT * 2 + 16, Assets.TILE_WIDTH * 2, Assets.TILE_HEIGHT * 2);
                Font.DrawText(gfx, FPS, 8, Assets.TILE_HEIGHT * 2 + 40, Assets.TILE_WIDTH * 2, Assets.TILE_HEIGHT * 2);
                //Font.DrawText(gfx, mousePosition, 8, Assets.TILE_HEIGHT * 2 + 60, Assets.TILE_WIDTH * 2, Assets.TILE_HEIGHT * 2);

                if (allowNextLevel /*&& player.SuperBombEquiped*/ && player.explosionFinished)
                    ScoreBoard.Print(gfx, timeSpeed.ElapsedMilliseconds / 1000f, destroyedObjectsAmount, CanvasSize.Width, CanvasSize.Height);


                if (Pause)
                    Font.DrawText(gfx, "paused", true, CanvasSize.Width, Assets.TILE_WIDTH *3, Assets.TILE_WIDTH*4, Assets.TILE_HEIGHT * 4, 
                        Color.FromArgb(0xFF,0xFF,0x00), Color.Black);

                //Font.DrawText(gfx, "Press space to continue", 0, Assets.TILE_HEIGHT * (int)zoom*2);
                //gfx.DrawImage(Font.GetText(text), 0, 0 , Font.GetText(text).Width * 2, Font.GetText(text).Height*2);

                /*
                //gfx.DrawImage(zoomImage, new RectangleF(-Camera.X * zoom + CanvasSize.Width/2, -Camera.Y*zoom + CanvasSize.Height / 2, Assets.Background.Width*4, Assets.Background.Height*4));
                //gfx.DrawImage(zoomImage, new RectangleF((-Camera.X + Assets.Background.Width / zoom) * zoom, (-Camera.Y + Assets.Background.Height / zoom) * zoom, Assets.Background.Width * zoom, Assets.Background.Height * zoom));
                //gfx.DrawImage(Assets.Background.Image, new Rectangle(((int)-Camera.X + Assets.Background.Width / zoom) * zoom, ((int)-Camera.Y + Assets.Background.Height / zoom) * zoom, Assets.Background.Width * zoom, Assets.Background.Height * zoom));
                //Bitmap bmp = new Bitmap(player.Image.Width, player.Image.Height);

                //using (Graphics _gfx = Graphics.FromImage(bmp))
                //{
                //    //_gfx.Clear(Color.White);
                //    _gfx.PixelOffsetMode = PixelOffsetMode.Half;
                //    _gfx.InterpolationMode = InterpolationMode.NearestNeighbor;

                //    _gfx.TranslateTransform(4, 12);
                //    _gfx.RotateTransform(player.Angle);

                //    _gfx.DrawImage(player.CurrentAsset.Image, -4, -12);

                //    _gfx.RotateTransform(-player.Angle);
                //    _gfx.TranslateTransform(-8, -16);

                //    //_gfx.FillRectangle(Brushes.Black, (player.Size.Width - 1) / 2f, (player.Size.Height - 1) / 2f, 1, 1);
                //}

                //gfx.DrawImage(bmp, player.Position.X*2, player.Position.Y*2 - Assets.TILE_HEIGHT * 2, player.Image.Width * 2, player.Image.Height * 2);
                //bmp.Dispose();
                */
            }
        }

        public void start()

        {
            if (alive) return;

            alive = true;
            gameThread = new Thread(run);
            gameThread.Start();
        }
        public void stop()
        {
            if (gameThread != null && alive)
            {
                alive = false;
                canvas.Dispose();

                gameThread.Abort();
            }
        }

        public static PointF GetRotatedPoint(Point p, double angle)
        {
            PointF pPrime = p;
            double radianAngle = angle * Math.PI / 180d;
            double sinAngle = Math.Sin(radianAngle);
            double cosAngle = Math.Cos(radianAngle);

            pPrime.X = (float)Math.Round(cosAngle * p.X - sinAngle * p.Y);
            pPrime.Y = (float)Math.Round(sinAngle * p.X + cosAngle * p.Y);

            return pPrime;
        }
        public static PointF GetRotatedPoint(int x, int y, double angle)
        {
            PointF pPrime = new PointF(x, y);
            double radianAngle = angle * Math.PI / 180d;
            double sinAngle = Math.Sin(radianAngle);
            double cosAngle = Math.Cos(radianAngle);

            pPrime.X = (float)Math.Round(cosAngle * x - sinAngle * y);
            pPrime.Y = (float)Math.Round(sinAngle * x + cosAngle * y);

            return pPrime;
        }

    }
}


/*
    OLD COLLISION!       
    
float diffX = (bricks[i].Position.X + bricks[i].BoundingBox.Width) - (bricks[j].Position.X + bricks[j].BoundingBox.Width);
float diffY = (bricks[i].Position.Y + bricks[i].BoundingBox.Height) - (bricks[j].Position.Y + bricks[j].BoundingBox.Height);


//if (bricks[i].Velocity.X < 0)
//{


//    bricks[i].Velocity = new Vector(diffX, bricks[i].Velocity.Y + diffY + g.Y * elapsedTime);
//    bricks[j].Velocity = new Vector(-diffX, bricks[j].Velocity.Y - diffY + g.Y * elapsedTime);
//    //bricks[i].Position = new PointF(bricks[j].Position.X + bricks[j].BoundingBox.Width - 1, bricks[i].Position.Y);
//}
// if (bricks[i].Velocity.X > 0)
//{


//    bricks[i].Velocity = new Vector(diffX, bricks[i].Velocity.Y + diffY + g.Y * elapsedTime);
//    bricks[j].Velocity = new Vector(-diffX, bricks[j].Velocity.Y - diffY + g.Y * elapsedTime);
//    //bricks[i].Position = new PointF(bricks[j].Position.X - bricks[i].BoundingBox.Width, bricks[i].Position.Y);
//}

//if (bricks[i].Velocity.Y > 0)
//{


//    bricks[i].Velocity = new Vector(diffX, bricks[i].Velocity.Y + diffY + g.Y * elapsedTime);
//    bricks[j].Velocity = new Vector(diffX, bricks[j].Velocity.Y - diffY + g.Y * elapsedTime);
//    //bricks[i].Position = new PointF(bricks[i].Position.X, bricks[j].Position.Y - bricks[i].BoundingBox.Height);
//}
// if (bricks[i].Velocity.Y < 0)
//{


//    bricks[i].Velocity = new Vector(diffX, bricks[i].Velocity.Y + diffY + g.Y * elapsedTime);
//    bricks[j].Velocity = new Vector(diffX, bricks[j].Velocity.Y - diffY + g.Y * elapsedTime);
//    //bricks[i].Position = new PointF(bricks[i].Position.X, bricks[j].Position.Y + bricks[i].BoundingBox.Height);

//}

/* OLD */
//if(bricks[i].Velocity.Y >= 0)
//{
//    bricks[i].Position = new PointF(bricks[i].Position.X, bricks[j].Position.Y - bricks[j].BoundingBox.Height - 1);
//    bricks[i].Velocity = new Vector(bricks[i].Velocity.X, -bricks[i].Velocity.Y);
//    bricks[j].Velocity = new Vector(bricks[j].Velocity.X, -bricks[j].Velocity.Y);
//    continue;
//}
//else if (bricks[i].Velocity.Y < 0)
//{
//    bricks[i].Position = new PointF(bricks[i].Position.X, bricks[j].Position.Y + bricks[j].BoundingBox.Height + 1);
//    bricks[i].Velocity = new Vector(bricks[i].Velocity.X, -bricks[i].Velocity.Y);
//    bricks[j].Velocity = new Vector(bricks[j].Velocity.X, -bricks[j].Velocity.Y);
//    continue;
//}
//if (bricks[i].Velocity.X >= 0)
//{
//    bricks[i].Position = new PointF(bricks[j].Position.X - bricks[j].BoundingBox.Width - 1, bricks[i].Position.Y);
//    bricks[i].Velocity = new Vector(+bricks[i].Velocity.X, bricks[i].Velocity.Y);
//    bricks[j].Velocity = new Vector(+bricks[j].Velocity.X, bricks[j].Velocity.Y);
//    continue;
//}
//else if (bricks[i].Velocity.X < 0)
//{
//    bricks[i].Position = new PointF(bricks[j].Position.X + bricks[j].BoundingBox.Width + 1, bricks[i].Position.Y);
//    bricks[i].Velocity = new Vector(bricks[i].Velocity.X, bricks[i].Velocity.Y);
//    bricks[j].Velocity = new Vector(+bricks[j].Velocity.X, bricks[j].Velocity.Y);
//    continue;
//}

//bricks[i].Velocity = new Vector(-bricks[i].Velocity.X, bricks[i].Velocity.Y);
//bricks[j].Velocity = new Vector(-bricks[j].Velocity.X, bricks[j].Velocity.Y);*/