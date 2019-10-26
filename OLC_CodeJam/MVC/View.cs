using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace OLC_CodeJam
{
    public partial class View : UserControl
    {
        Model model;

        public View(Model model)
        {
            InitializeComponent();

            this.model = model;
            this.Size = model.Canvas.Size;
            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            lock (model.Canvas)
            {
                if(model.Alive)
                    e.Graphics.DrawImage(model.Canvas, 0, 0);
            }

            this.Invalidate();
        }
    }
}
