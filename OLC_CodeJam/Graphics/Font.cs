using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLC_CodeJam
{
    public static class Font
    {
        static Bitmap FontImages;
        public static Bitmap[] All = new Bitmap[127];
        public static Bitmap[] Alphabet = new Bitmap[28];
        public static Bitmap[] Number = new Bitmap[10];
        public static Bitmap Comma;
        public static Bitmap DoubleDot;
        public static Bitmap Apostrophe;
        public static Bitmap Dot;
        public static Bitmap Minus;

        static Font()
        {
            FontImages = Assets.SpriteAtlas.getSpriteRegion(0, 19, 26 * 8, 8 * 2);

            for (int i = 0; i < Alphabet.Length; i++)
            {
                Alphabet[i] = new Bitmap(Assets.TILE_WIDTH, Assets.TILE_HEIGHT);

                using (Graphics gfx = Graphics.FromImage(Alphabet[i]))
                {
                    gfx.DrawImage(FontImages, 0, 0, new Rectangle(
                        i * Assets.TILE_WIDTH,
                        0,
                        Assets.TILE_WIDTH,
                        Assets.TILE_HEIGHT), GraphicsUnit.Pixel);
                }
            }
            for (int i = 0; i < Number.Length; i++)
            {
                Number[i] = new Bitmap(Assets.TILE_WIDTH, Assets.TILE_HEIGHT);

                using (Graphics gfx = Graphics.FromImage(Number[i]))
                {
                    gfx.DrawImage(FontImages, 0, 0, new Rectangle(
                        i * Assets.TILE_WIDTH,
                        8,
                        Assets.TILE_WIDTH,
                        Assets.TILE_HEIGHT), GraphicsUnit.Pixel);
                }
            }

            Comma = new Bitmap(Assets.TILE_WIDTH, Assets.TILE_HEIGHT);
            using (Graphics gfx = Graphics.FromImage(Comma))
            {
                gfx.DrawImage(FontImages, 0, 0, new Rectangle(
                    11 * Assets.TILE_WIDTH,
                    8,
                    Assets.TILE_WIDTH,
                    Assets.TILE_HEIGHT), GraphicsUnit.Pixel);
            }

            DoubleDot = new Bitmap(Assets.TILE_WIDTH, Assets.TILE_HEIGHT);
            using (Graphics gfx = Graphics.FromImage(DoubleDot))
            {
                gfx.DrawImage(FontImages, 0, 0, new Rectangle(
                    12 * Assets.TILE_WIDTH,
                    8,
                    Assets.TILE_WIDTH,
                    Assets.TILE_HEIGHT), GraphicsUnit.Pixel);
            }

            Apostrophe = new Bitmap(Assets.TILE_WIDTH, Assets.TILE_HEIGHT);
            using (Graphics gfx = Graphics.FromImage(Apostrophe))
            {
                gfx.DrawImage(FontImages, 0, 0, new Rectangle(
                    13 * Assets.TILE_WIDTH,
                    8,
                    Assets.TILE_WIDTH,
                    Assets.TILE_HEIGHT), GraphicsUnit.Pixel);
            }

            Dot = new Bitmap(Assets.TILE_WIDTH, Assets.TILE_HEIGHT);
            using (Graphics gfx = Graphics.FromImage(Dot))
            {
                gfx.DrawImage(FontImages, 0, 0, new Rectangle(
                    14 * Assets.TILE_WIDTH,
                    8,
                    Assets.TILE_WIDTH,
                    Assets.TILE_HEIGHT), GraphicsUnit.Pixel);
            }

            Minus = new Bitmap(Assets.TILE_WIDTH, Assets.TILE_HEIGHT);
            using (Graphics gfx = Graphics.FromImage(Minus))
            {
                gfx.DrawImage(FontImages, 0, 0, new Rectangle(
                    15 * Assets.TILE_WIDTH,
                    8,
                    Assets.TILE_WIDTH,
                    Assets.TILE_HEIGHT), GraphicsUnit.Pixel);
            }


            for (int i = 0; i < All.Length; i++)
            {
                All[i] = new Bitmap(Assets.TILE_WIDTH, Assets.TILE_HEIGHT);
                using (Graphics gfx = Graphics.FromImage(All[i]))
                {
                    gfx.DrawImage(Assets.Unknown.Image, 0, 0);
                }
            }
            for (int i = 65; i < Alphabet.Length + 65; i++)
            {
                All[i] = Alphabet[i - 65];
            }
            for (int i = 48; i < Number.Length + 48; i++)
            {
                All[i] = Number[i - 48];
            }
            All[','] = Comma;
            All[':'] = DoubleDot;
            All['\''] = Apostrophe;
            All['.'] = Dot;
            All['-'] = Minus;
            All[' '] = Assets.Empty.Image;
        }
        public static Bitmap GetText(string text)
        {
            text = text.ToUpper();

            int newLines = 0;
            int currentWidth = 0;
            int maxWidth = 0;
            for (int i = 0; i < text.Length; i++)
            {

                if (text[i] == '\n')
                {
                    newLines++;
                    if (maxWidth < currentWidth)
                        maxWidth = currentWidth;
                    currentWidth = 0;
                    continue;
                }
                currentWidth++;
            }
            if (maxWidth == 0) maxWidth = text.Length;

            Bitmap textImage = new Bitmap(maxWidth * Assets.TILE_WIDTH, Assets.TILE_HEIGHT + newLines * Assets.TILE_HEIGHT);

            int newLine = 0;
            int xIntend = 0;
            using (Graphics gfx = Graphics.FromImage(textImage))
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == '\n')
                    {
                        newLine++;
                        xIntend = 0;
                        continue;
                    }

                    gfx.DrawImage(All[text[i]], xIntend * Assets.TILE_WIDTH, newLine * Assets.TILE_HEIGHT);

                    xIntend++;
                }
            }

            return textImage;
        }

        public static void DrawText(Graphics gfx, string text, int x, int y)
        {
            Bitmap textImage = GetText(text);
            gfx.DrawImage(textImage, x, y, textImage.Width, textImage.Height);
            textImage.Dispose();
        }
        public static void DrawText(Graphics gfx, string text,bool centerAlongWidth, int width, int y)
        {
            Bitmap textImage = GetText(text);
            gfx.DrawImage(textImage, width / 2 - textImage.Width / 2, y, textImage.Width, textImage.Height);
            textImage.Dispose();
        }
        public static void DrawText(Graphics gfx, string text, bool centerAlongWidth, int width, int y, Color one, Color two)
        {
            ColorMap[] colorMap = new ColorMap[2];
            colorMap[0] = new ColorMap();
            colorMap[1] = new ColorMap();

            colorMap[0].OldColor = Color.FromArgb(0xE5, 0xE5, 0xE5);
            colorMap[0].NewColor = one;
            colorMap[1].OldColor = Color.FromArgb(0x00, 0x00, 0x00);
            colorMap[1].NewColor = two;

            ImageAttributes attr = new ImageAttributes();
            attr.SetRemapTable(colorMap);

            Bitmap textImage = GetText(text);

            Rectangle rect = new Rectangle(width / 2 - textImage.Width / 2, y, textImage.Width  , textImage.Height  );

            gfx.DrawImage(textImage, rect, 0, 0,
                textImage.Width,
                textImage.Height, GraphicsUnit.Pixel, attr);

            textImage.Dispose();
        }
        public static void DrawText(Graphics gfx, string text, bool centerAlongWidth, int width, int y, int fontWidth, int fontHeight)
        {
            Bitmap textImage = GetText(text);
            gfx.DrawImage(textImage, 
                width / 2 - (textImage.Width * (fontWidth/ Assets.TILE_WIDTH)) / 2, 
                y,
                textImage.Width / Assets.TILE_WIDTH * fontWidth,
                textImage.Height / Assets.TILE_HEIGHT * fontHeight);
            textImage.Dispose();
        }
        public static void DrawText(Graphics gfx, string text, bool centerAlongWidth, int width, int y, int fontWidth, int fontHeight, Color one, Color two)
        {
            ColorMap[] colorMap = new ColorMap[2];
            colorMap[0] = new ColorMap();
            colorMap[1] = new ColorMap();

            colorMap[0].OldColor = Color.FromArgb(0xE5, 0xE5, 0xE5);
            colorMap[0].NewColor = one;
            colorMap[1].OldColor = Color.FromArgb(0x00, 0x00, 0x00);
            colorMap[1].NewColor = two;

            ImageAttributes attr = new ImageAttributes();
            attr.SetRemapTable(colorMap);

            Bitmap textImage = GetText(text);

            Rectangle rect = new Rectangle(
                width / 2 - (textImage.Width * (fontWidth / Assets.TILE_WIDTH)) / 2,
                y, 
                textImage.Width / Assets.TILE_WIDTH * fontWidth,
                textImage.Height / Assets.TILE_HEIGHT * fontHeight);

            gfx.DrawImage(textImage, rect, 0, 0,
                textImage.Width,
                textImage.Height, GraphicsUnit.Pixel, attr);

            textImage.Dispose();
        }
        public static void DrawText(Graphics gfx, string text, int x, int y, int fontWidth, int fontHeight)
        {
            Bitmap textImage = GetText(text);

            gfx.DrawImage(textImage, x, y, 
                textImage.Width / Assets.TILE_WIDTH * fontWidth ,
                textImage.Height / Assets.TILE_HEIGHT * fontHeight);

            textImage.Dispose();
        }

        public static void DrawText(Graphics gfx, string text, int x, int y, int fontWidth, int fontHeight, Color one, Color two)
        {
            ColorMap[] colorMap = new ColorMap[2];
            colorMap[0] = new ColorMap();
            colorMap[1] = new ColorMap();

            colorMap[0].OldColor = Color.FromArgb(0xE5, 0xE5, 0xE5);
            colorMap[0].NewColor = one;
            colorMap[1].OldColor = Color.FromArgb(0x00, 0x00, 0x00);
            colorMap[1].NewColor = two;

            ImageAttributes attr = new ImageAttributes();
            attr.SetRemapTable(colorMap);

            Bitmap textImage = GetText(text);

            Rectangle rect = new Rectangle(x, y, textImage.Width / Assets.TILE_WIDTH * fontWidth, textImage.Height / Assets.TILE_HEIGHT * fontHeight);

            gfx.DrawImage(textImage,rect, 0, 0,
                textImage.Width,
                textImage.Height, GraphicsUnit.Pixel, attr);

            textImage.Dispose();
        }
    }
}
