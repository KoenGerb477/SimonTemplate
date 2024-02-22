using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Diagnostics;

namespace SimonSays
{
    public partial class MenuScreen : UserControl
    {
        //variables for animation
        PointF origin = new PointF(100, 100);
        float leftX;
        float rightX;
        float topY;
        float bottomY;
        float size;
        float startAngle;
        float spinSpeed = 3;
        int colour = 0;
        const int gift = 400; // gift from saahil patel :)

        Pen greenPen = new Pen(Color.DarkGreen, 100);
        Pen redPen = new Pen(Color.DarkRed, 100);
        Pen bluePen = new Pen(Color.DarkBlue, 100);
        Pen yellowPen = new Pen(Color.Goldenrod, 100);

        Pen[] pens = new Pen[4];

        List<Point> cloudList = new List<Point>();
        List<int> cloudSpeedList = new List<int>();

        public MenuScreen()
        {
            InitializeComponent();
            size = 200;
            startAngle = 1;

            pens[0] = greenPen;
            pens[1] = redPen;
            pens[2] = bluePen;
            pens[3] = yellowPen;
        }

        //button press runs animation then sends to gamescreen
        private void newButton_Click(object sender, EventArgs e)
        {
            //TODO: remove this screen and start the GameScreen
            Animation(360, 125, new Point(72, 68), 70, false, true);

            Form1.ChangeScreen(this, new GameScreen());
        }

        //runs animation then exits
        private void exitButton_Click(object sender, EventArgs e)
        {
            //TODO: end the application
            Animation(100000, 300, new Point(-5, -5), 200, true, true);

            Application.Exit();
        }

        private void MenuScreen_Paint(object sender, PaintEventArgs e)
        {
            //clouds
            SolidBrush cloudColor = new SolidBrush(Color.LightGray);
            int cloudSize = 50;
            for (int i = 0; i < cloudList.Count; i++)
            {
                e.Graphics.FillEllipse(cloudColor, cloudList[i].X, cloudList[i].Y, cloudSize, cloudSize);
                e.Graphics.FillEllipse(cloudColor, cloudList[i].X + 40, cloudList[i].Y, cloudSize + 20, cloudSize + 20);
                e.Graphics.FillEllipse(cloudColor, cloudList[i].X + 90, cloudList[i].Y, cloudSize, cloudSize);
                e.Graphics.FillEllipse(cloudColor, cloudList[i].X + 50, cloudList[i].Y + 20, cloudSize + 20, cloudSize + 20);
                e.Graphics.FillEllipse(cloudColor, cloudList[i].X + 80, cloudList[i].Y + 20, cloudSize, cloudSize);
                e.Graphics.FillEllipse(cloudColor, cloudList[i].X + 30, cloudList[i].Y + 40, cloudSize, cloudSize);
                e.Graphics.FillEllipse(cloudColor, cloudList[i].X + 60, cloudList[i].Y + 40, cloudSize + 20, cloudSize + 20);
                e.Graphics.FillEllipse(cloudColor, cloudList[i].X + 90, cloudList[i].Y + 40, cloudSize, cloudSize);
            }

            //simon things
            leftX = origin.X + 5;
            rightX = origin.X + 25;
            topY = origin.Y + 5;
            bottomY = origin.Y + 25;

            //back color
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(colour,0,0,0)), new Rectangle(0,0,this.Width,this.Height));

            //simon things
            leftX = origin.X + 5;
            rightX = origin.X + 25;
            topY = origin.Y + 5;
            bottomY = origin.Y + 25;
            
            e.Graphics.DrawArc(greenPen, leftX, topY, size, size, startAngle + 178, 94);
            e.Graphics.DrawArc(redPen, rightX, topY, size, size, startAngle + 268, 94);
            e.Graphics.DrawArc(bluePen, rightX, bottomY, size, size, startAngle + 358, 94);
            e.Graphics.DrawArc(yellowPen, leftX, bottomY, size, size, startAngle + 88, 94);
        }

        public void Animation(int endAngle, int endSize, Point endPoint, int endPenWidth, bool exit, bool colourChange)
        {
            //continue movement until it spins enough
            while (startAngle != endAngle && startAngle < 10000)
            {
                //rotation
                if (startAngle < endAngle)
                {
                    startAngle += spinSpeed;
                }
                else
                {
                    startAngle--;
                }
                //size
                if (size > endSize)
                {
                    size--;
                }
                else if (size < endSize)
                {
                    size++;
                }
                //location x
                if (origin.X >= endPoint.X)
                {
                    origin.X--;
                }
                else
                {
                    origin.X++;
                }
                //location y
                if(origin.Y >= endPoint.Y)
                {
                    origin.Y--;
                }
                else
                {
                    origin.Y++;
                }
                //pen size
                for(int i = 0; i < 4; i++)
                {
                    if (pens[i].Width > endPenWidth)
                    {
                        pens[i].Width--;
                    }
                    else if (pens[i].Width <  endPenWidth)
                    {
                        pens[i].Width++;
                    }
                }
                //background colour
                if(colour < 254 && colourChange)
                {
                    colour+=2;
                }
                //for exit animation accelerate
                if (exit)
                {
                    spinSpeed += 0.5f;
                }
                this.Refresh();
                Thread.Sleep(20);
            }
        }

        private void cloudTimer_Tick(object sender, EventArgs e)
        {
            Random random = new Random();
            //make new clouds
            int randNum = random.Next(1, 201);
            if (randNum > 199)
            {
                int y = random.Next(-100, this.Height);
                int x = -200;

                Point cloud = new Point(x, y);
                cloudList.Add(cloud);
                cloudSpeedList.Add(random.Next(1, 3));
            }

            //move stars
            for (int i = 0; i < cloudList.Count; i++)
            {
                int x = cloudList[i].X + cloudSpeedList[i];
                int y = cloudList[i].Y;
                cloudList[i] = new Point(x, y);
            }

            //delete stars
            for (int i = 0; i < cloudList.Count; i++)
            {
                if ((cloudList[i].X > this.Width))
                {
                    cloudList.RemoveAt(i);
                    cloudSpeedList.RemoveAt(i);
                }
            }

            this.Refresh();
        }
    }
}
