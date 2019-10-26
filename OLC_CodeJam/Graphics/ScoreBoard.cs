using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace OLC_CodeJam
{
    static class ScoreBoard
    {
        static Bitmap board;

        static ScoreBoard()
        {
            board = new Bitmap(96 * 4, 80 * 4);

            //using (Graphics boardGfx = Graphics.FromImage(board))
            //{
            //    boardGfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            //    boardGfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            //    boardGfx.DrawImage(Assets.SpriteAtlas.getSpriteRegion(0, 11, 80, 64), 0, 0, 80 * 4, 64 * 4);

            //    Font.DrawText(boardGfx, "press space to continue", true, board.Width, board.Height - 64);

            //    Font.DrawText(boardGfx, "time:",
            //        32, 80,
            //        8 * 2, 8 * 2);
            //    Font.DrawText(boardGfx, "score:",
            //        32, 80 + 48,
            //        8 * 2, 8 * 2);
            //}
        }

        public static void Print(Graphics gfx, float time, int score, int width, int height)
        {
            using (Graphics boardGfx = Graphics.FromImage(board))
            {
                boardGfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                boardGfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

                boardGfx.DrawImage(Assets.SpriteAtlas.getSpriteRegion(0, 9, 96, 80), 0, 0, 96 * 4, 80 * 4);

                Font.DrawText(boardGfx, "press space to continue", true, board.Width, board.Height - 64,
                    Color.Black,
                    Color.Transparent);

                Font.DrawText(boardGfx, "time:",
                    32, 80,
                    8 * 3, 8 * 3,
                    Color.Black,
                    Color.Transparent);
                Font.DrawText(boardGfx, "score:",
                    32, 80 + 48,
                    8 * 3, 8 * 3,
                    Color.Black,
                    Color.Transparent);

                Font.DrawText(boardGfx, time.ToString("0.000"),
                    board.Width-32 - (time.ToString("0.000").Length * 8 * 3), 80,
                    8 * 3, 8 * 3,
                    Color.Black,
                    Color.Transparent);
                Font.DrawText(boardGfx, score.ToString(),
                    board.Width - 32 - (score.ToString().Length * 8 * 3), 80 + 48,
                    8 * 3, 8 * 3,
                    Color.Black,
                    Color.Transparent);
            }

            gfx.DrawImage(board, width / 2 - (board.Width / 2), height / 2 - (board.Height / 2), board.Width, board.Height);
        }
    }
}
