using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OLC_CodeJam
{
    public partial class Controller : Form
    {
        Model model;
        View view;

        const int WIDTH = 800, HEIGHT = 600;

        public Controller()
        {
            InitializeComponent();
            this.ClientSize = new Size(WIDTH, HEIGHT);
            this.FormClosed += MainForm_FormClosed;
            this.DoubleBuffered = true;
            this.PreviewKeyDown += keyPress;
            this.KeyUp += keyUp;
            this.MouseDown += mouseDown;
            this.MouseUp += mouseUp;
            this.MouseMove += mouseMove;
            this.StartPosition = FormStartPosition.CenterScreen;


            model = new Model(WIDTH, HEIGHT);
            view = new View(model);
            view.PreviewKeyDown += keyPress;
            view.KeyUp += keyUp;
            view.MouseDown += mouseDown;
            view.MouseUp += mouseUp;
            view.MouseMove += mouseMove;
            view.GotFocus += GotFocus;
            view.LostFocus += LostFocus;
            view.Location = new Point(0, 0);
            this.Controls.Add(view);
            model.start();
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            model.Cursor = e.Location;
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            model.Cursor = e.Location;
            model.Mouse = false;
            model.MouseButton = e.Button;
        }
        private void mouseDown(object sender, MouseEventArgs e)
        {
            model.Cursor = e.Location;
            model.Mouse = true;
            model.MouseButton = e.Button;
        }

        private new void GotFocus(object sender, EventArgs e)
        {
            if(!model.ShowHelp)
                model.Pause = false;
            base.OnGotFocus(e);
        }
        private new void LostFocus(object sender, EventArgs e)
        {
            model.Pause = true;
            base.OnLostFocus(e);
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Space:
                    model.Space = false;
                    break;
                case Keys.Up:
                    model.Up = false;
                    break;
                case Keys.Down:
                    model.Down = false;
                    break;
                case Keys.Right:
                    model.Right = false;
                    break;
                case Keys.Left:
                    model.Left = false;
                    break;
                case Keys.D1:
                    model.One = false;
                    break;
                case Keys.D2:
                    model.Two = false;
                    break;
                case Keys.H:
                    model.H = false;
                    break;
                case Keys.S:
                    model.S = false;
                    break;
                case Keys.R:
                    model.R = false;
                    break;
            }
        }

        private void keyPress(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Space:
                    model.Space = true;
                    break;
                case Keys.Up:
                    model.Up = true;
                    break;
                case Keys.Down:
                    model.Down = true;
                    break;
                case Keys.Right:
                    model.Right = true;
                    break;
                case Keys.Left:
                    model.Left = true;
                    break;
                case Keys.D1:
                    model.One = true;
                    break;
                case Keys.D2:
                    model.Two = true;
                    break;
                case Keys.Z:
                    model.Z = true;
                    break;
                case Keys.H:
                    model.H = true;
                    break;
                case Keys.S:
                    model.S = true;
                    break;
                case Keys.R:
                    model.R = true;
                    break;

                case Keys.P:
                    model.Pause = !model.Pause;
                    break;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            model.stop();
        }
    }
}
