using OLC_CodeJam.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;

namespace OLC_CodeJam
{
    static class Assets
    {
        public static SpriteAtlas SpriteAtlas { get; private set; }
        public static SpriteAtlas AnimationSpriteAtlas { get; private set; }

        public static Dictionary<Color, Asset> All = new Dictionary<Color, Asset>();
        public static Asset[,] Shadows = new Asset[3,3];
        public static Asset[,] Lights = new Asset[3, 3];
        public static Asset[] ShadowStraight = new Asset[4];
        public static Asset[] ShadowDiagonal = new Asset[4];
        public static Asset[] LightStraight = new Asset[4];
        public static Asset[] LightDiagonal = new Asset[4];
        public static Asset[,] Walls = new Asset[4, 3];
        public static Asset[,] WallsInside = new Asset[4, 3];
        public static Asset[,] Bricks = new Asset[3, 3];
        public static Asset[,] BricksInside = new Asset[3, 3];
        public static Asset[,] Wood = new Asset[3, 3];
        public static Asset[,] WoodInside = new Asset[3, 3];
        public static Asset[] IronBars = new Asset[4];
        public static Asset[] WoodVertical = new Asset[3];
        public static Asset[] WoodHorizontal = new Asset[3];
        public static Asset[] WoodVerticalInside = new Asset[3];
        public static Asset[] WoodHorizontalInside = new Asset[3];
        public static Asset White;
        public static Asset Iron;
        public static Asset Glass;
        public static Asset Bomb;
        public static Asset SuperBomb;
        public static Asset Background;
        public static Asset Empty;
        public static Asset Unknown;
        

        public const int TRANSPARENCY_COLOR = 0xFF88FF;
        public const int TILE_WIDTH = 8;
        public const int TILE_HEIGHT = 8;

        public const int INTERACTIVE_FILTER = 0x00FF0000;
        public const int INTERACTIVE_WALL   = 0x00AA0000;
        public const int INTERACTIVE_IRON   = 0x00110000;
        public const int INTERACTIVE_WOOD   = 0x00660000;
        public const int INTERACTIVE_BRICK  = 0x00770000;
        public const int INTERACTIVE_WOODLONG = 0x00cc0000;
        public const int IRONBARS = 0x00B00000;
        public const int LIGHT_FILTER = 0x000000FF;
        public const int LIGHT = 0x000000BD;


        public static SoundPlayer Explosion1;
        public static SoundPlayer Explosion2;
        public static SoundPlayer Explosion3;
        public static SoundPlayer[] Explosions = new SoundPlayer[3];
        public static SoundPlayer BigExplosion1;
        public static SoundPlayer BigExplosion2;
        public static SoundPlayer BigExplosion3;
        public static SoundPlayer[] BigExplosions = new SoundPlayer[3];


        static Assets()
        {
            SpriteAtlas = new SpriteAtlas(Resources.tiles, TILE_WIDTH, TILE_HEIGHT, TRANSPARENCY_COLOR);
            AnimationSpriteAtlas = new SpriteAtlas(Resources.animationTiles, TILE_WIDTH, TILE_HEIGHT, TRANSPARENCY_COLOR);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Shadows[j, i] = new Asset(SpriteAtlas.getSprite(i+29, j), Color.FromArgb(0x5AD011));
                }
            }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Lights[j, i] = new Asset(SpriteAtlas.getSprite(i + 29, j+3), Color.FromArgb(0x11651111));
                }
            }
            for (int i = 0; i < 4; i++)
            {
                //                                                                               SADOww
                ShadowStraight[i] = new Asset(SpriteAtlas.getSprite(14 + i, 0), Color.FromArgb(0x5AD011)); 
            }
            for (int i = 0; i < 4; i++)
            {
                //                                                                               SADOww
                ShadowDiagonal[i] = new Asset(SpriteAtlas.getSprite(14 + i, 1), Color.FromArgb(0x5AD011));
            }
            for (int i = 0; i < 4; i++)
            {
                //                                                                              liGhtttt
                LightStraight[i] = new Asset(SpriteAtlas.getSprite(18 + i, 0), Color.FromArgb(0x11651111));
            }
            for (int i = 0; i < 4; i++)
            {
                //                                                                              liGhtttt
                LightDiagonal[i] = new Asset(SpriteAtlas.getSprite(18 + i, 1), Color.FromArgb(0x11651111));
            }

            Background = new Asset(Resources.Background2, Color.FromArgb(0xBACC00));
            All.Add(Background.ColorID, Background);

            Empty = new Asset(SpriteAtlas.getSprite(31, 31), Color.FromArgb(0xFF,0x88,0xFF));
            All.Add(Empty.ColorID, Empty);

            Unknown = new Asset(SpriteAtlas.getSprite(30, 31), Color.FromArgb(0xB0, 0x00, 0x00));
            All.Add(Unknown.ColorID, Unknown);
            
            White = new Asset(SpriteAtlas.getSprite(29, 31), Color.FromArgb(0x80, 0x12, 0x2F));
            All.Add(White.ColorID, White);

            Bomb = new Asset(SpriteAtlas.getSpriteRegion(0, 28, 8, 16), Color.FromArgb(0x00, 0xFF, 0x00));
            All.Add(Bomb.ColorID, Bomb);

            SuperBomb = new Asset(SpriteAtlas.getSpriteRegion(1, 30, 8, 16), Color.FromArgb(0xFF, 0x00, 0x00));
            All.Add(SuperBomb.ColorID, SuperBomb);

            Iron = new Asset(SpriteAtlas.getSprite(8, 2), Color.FromArgb(0x11, 0x20, 0x11));
            All.Add(Iron.ColorID, Iron);

            Glass = new Asset(SpriteAtlas.getSprite(10, 2), Color.FromArgb(0x61, 0xA5, 0xBD));
            All.Add(Glass.ColorID, Glass);


            int woodVerticalValue = 0x0000;
            for(int i = 0; i< 3; i++)
            {
                WoodVertical[i] = new Asset(SpriteAtlas.getSprite(8 + i, 0), Color.FromArgb(0x66, woodVerticalValue, 0x11));
                All.Add(WoodVertical[i].ColorID, WoodVertical[i]);
                woodVerticalValue += 0x0033;
            }
            int woodHorizontalValue = 0x0099;
            for (int i = 0; i < 3; i++)
            {
                WoodHorizontal[i] = new Asset(SpriteAtlas.getSprite(8 + i, 1), Color.FromArgb(0x66, woodHorizontalValue, 0x11));
                All.Add(WoodHorizontal[i].ColorID, WoodHorizontal[i]);
                woodHorizontalValue += 0x0033;
            }
            int woodVerticalInsideValue = 0x0000;
            for (int i = 0; i < 3; i++)
            {
                WoodVerticalInside[i] = new Asset(SpriteAtlas.getSprite(11 + i, 0), Color.FromArgb(0x22, woodVerticalInsideValue, 0x11));
                All.Add(WoodVerticalInside[i].ColorID, WoodVerticalInside[i]);
                woodVerticalInsideValue += 0x0033;
            }
            int woodHorizontalInsideValue = 0x0099;
            for (int i = 0; i < 3; i++)
            {
                WoodHorizontalInside[i] = new Asset(SpriteAtlas.getSprite(11 + i, 1), Color.FromArgb(0x22, woodHorizontalInsideValue, 0x11));
                All.Add(WoodHorizontalInside[i].ColorID, WoodHorizontalInside[i]);
                woodHorizontalInsideValue += 0x0033;
            }

            int brickVlue = 0x0011;
            for(int i = 0; i < 4; i++)
                for (int j = 0; j < 3; j++)
                {
                    Walls[i,j] = new Asset(SpriteAtlas.getSprite(i, j), Color.FromArgb(0xAA, brickVlue, 0x00));
                    All.Add(Walls[i, j].ColorID, Walls[i, j]);
                    brickVlue += 0x11;
                }

            int darkBrickValue = 0x0011;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 3; j++)
                {
                    WallsInside[i, j] = new Asset(SpriteAtlas.getSprite(i+4, j), Color.FromArgb(0x55, darkBrickValue, 0x00));
                    All.Add(WallsInside[i, j].ColorID, WallsInside[i, j]);

                    darkBrickValue += 0x11;
                }
            int stoneBirck = 0x0077;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    Bricks[i, j] = new Asset(SpriteAtlas.getSprite(i, j+3), Color.FromArgb(0x77, stoneBirck, 0x77));
                    All.Add(Bricks[i, j].ColorID, Bricks[i, j]);

                    stoneBirck += 0x01;
                }
            int stoneBrickInside = 0x0033;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    BricksInside[i, j] = new Asset(SpriteAtlas.getSprite(i+3, j+3), Color.FromArgb(0x33, stoneBrickInside, 0x33));
                    All.Add(BricksInside[i, j].ColorID, BricksInside[i, j]);

                    stoneBrickInside += 0x01;
                }


            int wood = 0x00ee;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    Wood[i, j] = new Asset(SpriteAtlas.getSprite(i + 6, j + 3), Color.FromArgb(0xcc, wood, 0xdd));
                    All.Add(Wood[i, j].ColorID, Wood[i, j]);

                    wood += 0x01;
                }

            int woodInside = 0x00ee;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    WoodInside[i, j] = new Asset(SpriteAtlas.getSprite(i + 9, j + 3 ), Color.FromArgb(0x55, woodInside, 0xdd));
                    All.Add(WoodInside[i, j].ColorID, WoodInside[i, j]);

                    woodInside += 0x01;
                }

            int ironBarValue = 0x00AF;
            for (int i = 0; i < 4; i++)
            {
                IronBars[i] = new Asset(SpriteAtlas.getSprite(i + 11, 2), Color.FromArgb(0xB0, ironBarValue, 0xBD));
                All.Add(IronBars[i].ColorID, IronBars[i]);

                ironBarValue += 0x1;
            }

            Explosion1 = new SoundPlayer(Resources.Explosion);
            Explosion2 = new SoundPlayer(Resources.Explosion2);
            Explosion3 = new SoundPlayer(Resources.Explosion3);
            Explosions[0] = Explosion1;
            Explosions[1] = Explosion2;
            Explosions[2] = Explosion3;

            BigExplosion1 = new SoundPlayer(Resources.BigExplosion);
            BigExplosion2 = new SoundPlayer(Resources.BigExplosion2);
            BigExplosion3 = new SoundPlayer(Resources.BigExplosion3);
            BigExplosions[0] = BigExplosion1;
            BigExplosions[1] = BigExplosion2;
            BigExplosions[2] = BigExplosion3;
        }
    }
}
