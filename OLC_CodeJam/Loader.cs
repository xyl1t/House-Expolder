using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace OLC_CodeJam
{
    static class Loader
    {
        public static Level LoadLevel(Bitmap level)
        {
            Model.random = new Random(Model.startedDate.Second);

            int pointX = Assets.Background.Width / 2 - (level.Width * Assets.TILE_WIDTH) / 2;
            int pointY = Assets.Background.Height - (level.Height * Assets.TILE_HEIGHT);

            Level lvl = new Level(level.Width, level.Height);

            for (int i = 0; i < level.Height; i++)
            {
                for (int j = 0; j < level.Width; j++)
                {
                    Color pixel = level.GetPixel(j, i);

                    try
                    {
                        if ((Assets.All[pixel].ColorID.ToArgb() == Assets.Bomb.ColorID.ToArgb()))
                        {
                            lvl.PlayerLocation = new Point(j, i);
                            lvl.Objects.Add(
                            new LevelObject(
                            new PointF(pointX + j * Assets.TILE_WIDTH, pointY + i * Assets.TILE_HEIGHT), new Size(Assets.TILE_WIDTH, Assets.TILE_HEIGHT), 
                            Assets.All[level.GetPixel(j,i-1)], true));
                            continue;
                        }
                        if (Assets.All[pixel].ColorID.ToArgb() == Assets.Walls[0, 0].ColorID.ToArgb())
                        {
                            lvl.Objects.Add(
                            new LevelObject(
                            new PointF(pointX + j * Assets.TILE_WIDTH, pointY + i * Assets.TILE_HEIGHT), new Size(Assets.TILE_WIDTH, Assets.TILE_HEIGHT), 
                            Assets.Walls[Model.random.Next(0, 4), Model.random.Next(0, 3)], false));

                            continue;
                        }
                        if (Assets.All[pixel].ColorID.ToArgb() == Assets.WallsInside[0, 0].ColorID.ToArgb())
                        {
                            lvl.Objects.Add(
                            new LevelObject(
                            new PointF(pointX + j * Assets.TILE_WIDTH, pointY + i * Assets.TILE_HEIGHT), new Size(Assets.TILE_WIDTH, Assets.TILE_HEIGHT),
                            Assets.WallsInside[Model.random.Next(0, 4), Model.random.Next(0, 3)], true));

                            continue;
                        }
                        if (Assets.All[pixel].ColorID.ToArgb() == Assets.IronBars[0].ColorID.ToArgb())
                        {
                            lvl.Objects.Add(
                            new LevelObject(
                            new PointF(pointX + j * Assets.TILE_WIDTH, pointY + i * Assets.TILE_HEIGHT), new Size(Assets.TILE_WIDTH, Assets.TILE_HEIGHT), 
                            Assets.IronBars[Model.random.Next(0, 4)], true));

                            continue;
                        }

                        if (Assets.All[pixel].ColorID.ToArgb() == Assets.WoodVertical[0].ColorID.ToArgb())
                        {
                            lvl.Objects.Add(
                            new LevelObject(
                            new PointF(pointX + j * Assets.TILE_WIDTH, pointY + i * Assets.TILE_HEIGHT), new Size(Assets.TILE_WIDTH, Assets.TILE_HEIGHT),
                            Assets.WoodVertical[Model.random.Next(0, 3)], false));

                            continue;
                        }
                        if (Assets.All[pixel].ColorID.ToArgb() == Assets.WoodHorizontal[0].ColorID.ToArgb())
                        {
                            lvl.Objects.Add(
                            new LevelObject(
                            new PointF(pointX + j * Assets.TILE_WIDTH, pointY + i * Assets.TILE_HEIGHT), new Size(Assets.TILE_WIDTH, Assets.TILE_HEIGHT),
                            Assets.WoodHorizontal[Model.random.Next(0, 3)], false));

                            continue;
                        }
                        if (Assets.All[pixel].ColorID.ToArgb() == Assets.WoodHorizontalInside[0].ColorID.ToArgb())
                        {
                            lvl.Objects.Add(
                            new LevelObject(
                            new PointF(pointX + j * Assets.TILE_WIDTH, pointY + i * Assets.TILE_HEIGHT), new Size(Assets.TILE_WIDTH, Assets.TILE_HEIGHT),
                            Assets.WoodHorizontalInside[Model.random.Next(0, 3)], true));

                            continue;
                        }
                        if (Assets.All[pixel].ColorID.ToArgb() == Assets.WoodVerticalInside[0].ColorID.ToArgb())
                        {
                            lvl.Objects.Add(
                            new LevelObject(
                            new PointF(pointX + j * Assets.TILE_WIDTH, pointY + i * Assets.TILE_HEIGHT), new Size(Assets.TILE_WIDTH, Assets.TILE_HEIGHT),
                            Assets.WoodVerticalInside[Model.random.Next(0, 3)], true));

                            continue;
                        }
                        if (Assets.All[pixel].ColorID.ToArgb() == Assets.Bricks[0,0].ColorID.ToArgb())
                        {
                            lvl.Objects.Add(
                            new LevelObject(
                            new PointF(pointX + j * Assets.TILE_WIDTH, pointY + i * Assets.TILE_HEIGHT), new Size(Assets.TILE_WIDTH, Assets.TILE_HEIGHT),
                            Assets.Bricks[Model.random.Next(0, 3), Model.random.Next(0, 3)], false));

                            continue;
                        }
                        if (Assets.All[pixel].ColorID.ToArgb() == Assets.BricksInside[0, 0].ColorID.ToArgb())
                        {
                            lvl.Objects.Add(
                            new LevelObject(
                            new PointF(pointX + j * Assets.TILE_WIDTH, pointY + i * Assets.TILE_HEIGHT), new Size(Assets.TILE_WIDTH, Assets.TILE_HEIGHT),
                            Assets.BricksInside[Model.random.Next(0, 3), Model.random.Next(0, 3)], true));

                            continue;
                        }

                        if (Assets.All[pixel].ColorID.ToArgb() == Assets.WoodInside[0, 0].ColorID.ToArgb())
                        {
                            lvl.Objects.Add(
                            new LevelObject(
                            new PointF(pointX + j * Assets.TILE_WIDTH, pointY + i * Assets.TILE_HEIGHT), new Size(Assets.TILE_WIDTH, Assets.TILE_HEIGHT),
                            Assets.WoodInside[Model.random.Next(0, 3), Model.random.Next(0, 3)], true));

                            continue;
                        }
                        if (Assets.All[pixel].ColorID.ToArgb() == Assets.Wood[0, 0].ColorID.ToArgb())
                        {
                            lvl.Objects.Add(
                            new LevelObject(
                            new PointF(pointX + j * Assets.TILE_WIDTH, pointY + i * Assets.TILE_HEIGHT), new Size(Assets.TILE_WIDTH, Assets.TILE_HEIGHT),
                            Assets.Wood[Model.random.Next(0, 3), Model.random.Next(0, 3)], false));

                            continue;
                        }

                        if(Assets.All[pixel].ColorID.ToArgb() == Assets.Iron.ColorID.ToArgb())
                        {
                            lvl.Objects.Add(
                               new LevelObject(
                               new PointF(pointX + j * Assets.TILE_WIDTH, pointY + i * Assets.TILE_HEIGHT), new Size(Assets.TILE_WIDTH, Assets.TILE_HEIGHT), Assets.All[pixel], false));

                            continue;
                        }

                            lvl.Objects.Add(
                            new LevelObject(
                            new PointF(pointX + j * Assets.TILE_WIDTH, pointY + i * Assets.TILE_HEIGHT), new Size(Assets.TILE_WIDTH, Assets.TILE_HEIGHT), Assets.All[pixel], true));
                    }
                    catch
                    {
                        lvl.Objects.Add(
                        new LevelObject(
                        new PointF(pointX + j * Assets.TILE_WIDTH, pointY + i * Assets.TILE_HEIGHT), new Size(Assets.TILE_WIDTH, Assets.TILE_HEIGHT), Assets.Unknown, true));
                    }
                }        
            }

            return lvl;
        }

        public static List<GameObject> GetObstacles(Level source)
        {
            List<GameObject> obstacles = new List<GameObject>();

            for (int i = 0; i < source.Objects.Count; i++)
            {
                if ((source[i].CurrentAsset.ColorID.ToArgb() & 0x00FF0000) >>16 == 0xAA)
                {
                    obstacles.Add(source[i]);
                }
            }

            return obstacles;
        }
    }
}
