using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLC_CodeJam
{
    class Level
    {
        public List<LevelObject> Objects { get; set; } = new List<LevelObject>();
        public int Width { get; set; }
        public int Height { get; set; }
        public Point PlayerLocation { get; set; }

        public Level()
        {

        }
        public Level(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public LevelObject this[int index]
        {
            get { return Objects[index]; }
            set { Objects[index] = value; }
        }

        static int levelIndex = 0;
        public static int LevelIndex { get { return levelIndex; } }
        public static Level Level1 { get; private set; }
        public static Level Level2 { get; private set; }
        public static Level Level3 { get; private set; }
        public static Level Level4 { get; private set; }
        public static Level Level5 { get; private set; }
        public static Level Level6 { get; private set; }
        public static Level Level7 { get; private set; }
        public static List<Level> Levels { get; private set; }
        public static List<Bitmap> RawLevels { get; private set; }  

        static Level()
        {
            Level1 = Loader.LoadLevel(Properties.Resources.lvl_01);
            Level2 = Loader.LoadLevel(Properties.Resources.lvl_02);
            Level3 = Loader.LoadLevel(Properties.Resources.lvl_03);
            Level4 = Loader.LoadLevel(Properties.Resources.lvl_04);
            Level5 = Loader.LoadLevel(Properties.Resources.lvl_05);
            Level6 = Loader.LoadLevel(Properties.Resources.lvl_06);
            Level7 = Loader.LoadLevel(Properties.Resources.lvl_07);

            Levels = new List<Level>();
            Levels.Add(Level1);
            Levels.Add(Level2);
            Levels.Add(Level3);
            Levels.Add(Level4);
            Levels.Add(Level5);
            Levels.Add(Level6);
            Levels.Add(Level7);

            RawLevels = new List<Bitmap>();
            RawLevels.Add(Properties.Resources.lvl_01);
            RawLevels.Add(Properties.Resources.lvl_02);
            RawLevels.Add(Properties.Resources.lvl_03);
            RawLevels.Add(Properties.Resources.lvl_04);
            RawLevels.Add(Properties.Resources.lvl_05);
            RawLevels.Add(Properties.Resources.lvl_06);
            RawLevels.Add(Properties.Resources.lvl_07);

            if (levelIndex > 5)
                Assets.Background = new Asset(Properties.Resources.Background2, Assets.Background.ColorID);
            else
                Assets.Background = new Asset(Properties.Resources.Background, Assets.Background.ColorID);

        }
        public static Level GetCurrentLevel()
        {
            return Levels[levelIndex];
        }
        public static void NextLevel(Player p)
        {
            levelIndex++;
            if (levelIndex > RawLevels.Count-1)
            { 
                levelIndex = 0;
                Levels[0] = Loader.LoadLevel(Properties.Resources.lvl_01);
                Levels[1] = Loader.LoadLevel(Properties.Resources.lvl_02);
                Levels[2] = Loader.LoadLevel(Properties.Resources.lvl_03);
                Levels[3] = Loader.LoadLevel(Properties.Resources.lvl_04);
                Levels[4] = Loader.LoadLevel(Properties.Resources.lvl_05);
                Levels[5] = Loader.LoadLevel(Properties.Resources.lvl_06);
                Levels[6] = Loader.LoadLevel(Properties.Resources.lvl_07);
            }

            if (levelIndex > 5)
                Assets.Background = new Asset(Properties.Resources.Background2, Assets.Background.ColorID);
            else
                Assets.Background = new Asset(Properties.Resources.Background, Assets.Background.ColorID);

            p.Reset();
            p.Position = new PointF(
                GetCurrentLevel().PlayerLocation.X * Assets.TILE_WIDTH + GetCurrentLevel().Objects[0].Position.X,
                GetCurrentLevel().PlayerLocation.Y * Assets.TILE_HEIGHT + GetCurrentLevel().Objects[0].Position.Y);
        }
        public static void Copy(Level from, Level to)
        {
            to = new Level(from.Width, from.Height);
            for (int i = 0; i < from.Objects.Count; i++)
            {
                to.Objects.Add(new LevelObject(from.Objects[i].Position, from.Objects[i].Size, from.Objects[i].CurrentAsset, from.Objects[i].IsPassable));
            }
        }
        public void Restart(Player p)
        {
            Levels[levelIndex] = Loader.LoadLevel(RawLevels[levelIndex]);

            p.Reset();
            p.Position = new PointF(
                GetCurrentLevel().PlayerLocation.X * Assets.TILE_WIDTH + GetCurrentLevel().Objects[0].Position.X,
                GetCurrentLevel().PlayerLocation.Y * Assets.TILE_HEIGHT + GetCurrentLevel().Objects[0].Position.Y);
        }
    }
}