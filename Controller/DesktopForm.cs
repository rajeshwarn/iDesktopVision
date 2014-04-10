using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controller.Handlers;

namespace Controller
{
    public partial class DesktopForm : Form
    {
        private readonly Desktop _desktop;
        private Bitmap _buffer;
        private Graphics _bufferGraphics;

        public DesktopForm(Desktop desktop)
        {
            InitializeComponent();

            pictureBox.KeyDown += pictureBox_KeyDown;
            pictureBox.KeyUp += pictureBox_KeyUp;

            _desktop = desktop;
            _desktop.FrameUpdated += FrameUpdated;
            _desktop.Acknowledged += Acknowledged;
            _desktop.Quality = trackBarQuality.Value; 
            _desktop.Start();

            timerFPS.Start();
        }

        private void pictureBox_KeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            
        }

        private void pictureBox_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            
        }

        private void Acknowledged(object sender, EventArgs eventArgs)
        {
            if (_buffer != null)
                _buffer.Dispose();

            _buffer = new Bitmap(_desktop.Resolution.Width, _desktop.Resolution.Height);
            _bufferGraphics = Graphics.FromImage(_buffer);
        }

        private void FrameUpdated(object sender, DesktopEventArgs eventArgs)
        {;
            var position =
                new Rectangle(
                    new Point(eventArgs.Block.X*_desktop.BlockSize.Width, eventArgs.Block.Y*_desktop.BlockSize.Height),
                    _desktop.BlockSize);

            
           _bufferGraphics.DrawImage(eventArgs.Image, position);

            pictureBox.Image = _buffer;
        }

        private void trackBarQuality_ValueChanged(object sender, EventArgs e)
        {
            _desktop.Quality = trackBarQuality.Value;
            _desktop.Synchronize(_desktop.Blocks);
        }

        private void timerFPS_Tick(object sender, EventArgs e)
        {
            labelFPS.Text = String.Format("FPS: {0:#.##}", _desktop.FPS);
        }

        private void DesktopForm_SizeChanged(object sender, EventArgs e)
        {
        }

        private void textBlocks_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            int width, height;
            if (Int32.TryParse(textWidth.Text, out width) == false ||
                Int32.TryParse(textHeight.Text, out height) == false)
                return;

            _desktop.ChangeBlocks(new Size(width, height));
        }

        private void DesktopForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                _desktop.Stop();
            else
                _desktop.ChangeBlocks(_desktop.Blocks); // resume
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (buttonPause.Text == "Pause")
            {
                labelFPS.Text = "FPS: ";
                timerFPS.Stop();
                _desktop.Stop();
                buttonPause.Text = "Resume";
            }
            else
            {
                timerFPS.Start();
                _desktop.ChangeBlocks(_desktop.Blocks); // resume
                buttonPause.Text = "Pause";
            }
        }

        private void DesktopForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _desktop.Stop();
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            var x = e.X * _desktop.Resolution.Width / pictureBox.Width;
            var y = e.Y * _desktop.Resolution.Height / pictureBox.Height;
            
            _desktop.MouseDown(new Point(x, y), e.Button);
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            var x = e.X * _desktop.Resolution.Width / pictureBox.Width;
            var y = e.Y * _desktop.Resolution.Height / pictureBox.Height;

            _desktop.MouseUp(new Point(x, y), e.Button);
        }
    }
}
