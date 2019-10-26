using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace OLC_CodeJam
{
    class Animation
    {
        int speed;
        int index;
        public int GetIndex { get { return index; } }
        double lastTime;
        double timer;
        Bitmap[] images;

        public bool Finished { get; private set; } = true;

        public Animation(Bitmap sprite, int width, int height, int speed)
        {
            this.speed = speed;
            int spriteWidth = sprite.Width / width;
            int spriteHeight = sprite.Height / height;

            images = new Bitmap[spriteWidth * spriteHeight];

            for (int i = 0; i < spriteHeight; i++)
            {
                for (int j = 0; j < spriteWidth; j++)
                {
                    images[j + i * spriteWidth] = new Bitmap(width, height);

                    using (Graphics gfx = Graphics.FromImage(images[j + i * spriteWidth]))
                    {
                        gfx.DrawImage(sprite, 0, 0, new Rectangle(j * width, i * height, width, height), GraphicsUnit.Pixel);
                    }
                }
            }

            lastTime = Environment.TickCount;
        }

        public void tick()
        {
            timer += Environment.TickCount - lastTime;
            lastTime = Environment.TickCount;

            Finished = false;
            if (timer > speed)
            {
                index++;
                timer = 0;
                if (index >= images.Length)
                {
                    index = 0;
                    Finished = true;
                }
            }
        }

        public Bitmap GetImage()
        {
            return images[index];
        }

        public void Reset()
        {
            lastTime = Environment.TickCount;
            Finished = true;
            index = 0;
        }
    }
}
