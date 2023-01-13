using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NavBall2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread NavBallThread = new Thread(Navball);
            NavBallThread.Start();
            pictureBox1.Controls.Add(pictureBox2);
            pictureBox2.Location = new Point((pictureBox1.Width / 2) - (pictureBox2.Width / 2), (pictureBox1.Height / 2) - (pictureBox2.Height / 2) + 8);
        }

        float pitch, Roll, Heading = 0F;

        private void Navball()
        {
            float FiveHundredOverNintey = 500f / 90F;

            while (true)
            {
                pitch = GetPitchValue();//(float)Math.Cos(((float)DateTime.Now.Second + (float)(DateTime.Now.Millisecond / 1000)) * 6.0F * (Math.PI/180)) * 90.0F;
                Roll = -GetRollValue();
                Heading = Heading - (Roll / 90);//GetHeadingValue();
                int PitchIsNegative = 180;
                Int16 HeadingIsReciprocal = 0;

                if(Heading < 0)
                {
                    Heading += 360;
                }
                else if (Heading > 360)
                {
                    Heading -= 360;
                }

                

                Bitmap bitmap = new Bitmap(500 , 500, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                Graphics graphics = Graphics.FromImage(bitmap);

                //SetLabel(pitch.ToString());

                if (pitch < 0)
                {
                    PitchIsNegative = 0;
                    pitch = Math.Abs(pitch);
                }
                else
                {
                    PitchIsNegative = 180;
                }

                Pen pen = new Pen(Color.Blue, 2);


                graphics.FillEllipse(new SolidBrush(Color.Blue), new Rectangle(0, 0, 500, 500));

                GraphicsPath gp = new GraphicsPath();
                gp.AddArc(new Rectangle(0, 0, 500, 500), 0, 180);
                gp.AddArc(new Rectangle(0, (int)(250 - (1 + (FiveHundredOverNintey) * pitch) / 2F), 500, (int)(1 + (FiveHundredOverNintey) * pitch)), PitchIsNegative, 180);

                Region ground = new Region(gp);
                graphics.FillRegion(new SolidBrush(Color.Brown), ground);

                //Draw the horizon
                pen.Color = Color.White;
                graphics.DrawArc(pen, 0, 250F - (1 + FiveHundredOverNintey * pitch) / 2, 500, 1F + (FiveHundredOverNintey) * pitch, PitchIsNegative, 180);

                float BoundedHeading = Heading;

                while(BoundedHeading > 90)
                {
                    BoundedHeading -= 90;
                }

                //North
                if(Heading < 90)
                {
                    pen.Color = Color.Red;
                }

                //Heading Line Render
                graphics.DrawArc(pen, 250F - (1 + FiveHundredOverNintey * BoundedHeading) / 2, 0F, 1F + (FiveHundredOverNintey) * BoundedHeading, 500,  90, 180);
                pen.Color = Color.White;

                //North
                if (Heading > 270)
                {
                    pen.Color = Color.Red;
                }

                graphics.DrawArc(pen, +(FiveHundredOverNintey * BoundedHeading) / 2, 0F, 500F - (FiveHundredOverNintey) * BoundedHeading, 500, 270, 180);

                SetPicture(RotateImage(bitmap,Roll));

            }
        }

        private void SetPicture(Image img)
        {
            if (pictureBox1.InvokeRequired)
            {
                pictureBox1.Invoke(new MethodInvoker(
                delegate ()
                {
                    pictureBox1.Image = img;
                }));
            }
            else
            {
                pictureBox1.Image = img;
            }
        }

        //private void SetLabel(string text)
        //{
        //    if (label1.InvokeRequired)
        //    {
        //        label1.Invoke(new MethodInvoker(
        //        delegate ()
        //        {
        //            label1.Text = text;
        //        }));
        //    }
        //    else
        //    {
        //        label1.Text = text;
        //    }
        //}

        delegate int GetSliderValueCallback();

        private int GetPitchValue()
        {
            int sliderValue;
            if (PitchSlider.InvokeRequired)
            {
                GetSliderValueCallback cb = new GetSliderValueCallback(GetPitchValue);
                return (int)PitchSlider.Invoke(cb);
            }
            else
            {
                return (int)PitchSlider.Value;
            }
        }

        private int GetRollValue()
        {
            if (RollSlider.InvokeRequired)
            {
                GetSliderValueCallback cb = new GetSliderValueCallback(GetRollValue);
                return (int)RollSlider.Invoke(cb);
            }
            else
            {
                return (int)RollSlider.Value;
            }
        }

        //private int GetHeadingValue()
        //{
        //    if (RollSlider.InvokeRequired)
        //    {
        //        GetSliderValueCallback cb = new GetSliderValueCallback(GetHeadingValue);
        //        return (int)HeadingSlider.Invoke(cb);
        //    }
        //    else
        //    {
        //        return (int)HeadingSlider.Value;
        //    }
        //}

        private Bitmap RotateImage(Bitmap bmp, float angle)
        {
            Bitmap rotatedImage = new Bitmap(bmp.Width, bmp.Height);
            rotatedImage.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                // Set the rotation point to the center in the matrix
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                // Rotate
                g.RotateTransform(angle);
                // Restore rotation point in the matrix
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
                // Draw the image on the bitmap
                g.DrawImage(bmp, new Point(0, 0));
            }

            return rotatedImage;
        }
    }
}
