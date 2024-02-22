using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Security.Cryptography;
using System.Xml.Schema;
using System.Drawing.Drawing2D;

namespace SimonSays
{
    public partial class GameScreen : UserControl
    {
        int arrayIndex;
        bool computerTurn = true;
        bool playerTurn = false;
        int counter = 0;

        //TODO: create an int guess variable to track what part of the pattern the user is at
        int guess;

        const int DELAY_TIME = 30;

        //arrays
        Color[] secondColorArray = { Color.LawnGreen, Color.Red, Color.Blue, Color.Yellow };
        Color[] firstColorArray = { Color.ForestGreen, Color.DarkRed, Color.DarkBlue, Color.Goldenrod };
        SoundPlayer[] soundArray = { new SoundPlayer(Properties.Resources.green), new SoundPlayer(Properties.Resources.red), new SoundPlayer(Properties.Resources.blue), new SoundPlayer(Properties.Resources.yellow), new SoundPlayer(Properties.Resources.mistake) };
        Button[] buttonArray = new Button[4];
        int[] blinkTimer = { 0, 0, 0, 0 };

        //star movement lists
        List<PointF> starPoints = new List<PointF>();
        List<int> starXSpeed = new List<int>();
        List<int> starYSpeed = new List<int>();

        public GameScreen()
        {
            InitializeComponent();

            buttonArray[0] = greenButton;
            buttonArray[1] = redButton;
            buttonArray[2] = blueButton;
            buttonArray[3] = yellowButton;
        }

        private void GameScreen_Load(object sender, EventArgs e)
        {
            //button regions for arc shape
            GraphicsPath buttonPath = new GraphicsPath();
            buttonPath.AddArc(5, 5, 200, 200, 180, 90);
            buttonPath.AddArc(75, 75, 62, 62, 270, -90);

            for (int i = 0; i < buttonArray.Length; i++)
            {
                buttonArray[i].Region = new Region(buttonPath);

                Matrix transformMatrix = new Matrix();
                transformMatrix.RotateAt(90, new PointF(55, 55));
                buttonPath.Transform(transformMatrix);
            }

            //TODO: clear the pattern list from form1
            Form1.pattern.Clear();
            //TODO: refresh
            Refresh();
            //TODO: pause for a bit
            Thread.Sleep(DELAY_TIME);
            //TODO: run ComputerTurn()
            ComputerTurn();
        }

        //get new values for pattern
        private void ComputerTurn()
        {

            //TODO: get rand num between 0 and 4  (0, 1, 2, 3) and add to pattern list. Each number represents a button. For example, 0 may be green, 1 may be blue, etc.
            Random randGen = new Random();
            int randNum = randGen.Next(0, 4);
            Form1.pattern.Add(randNum);

            //TODO: set guess value back to 0
            guess = 0;
        }

        public void GameOver()
        {
            //TODO: Play a game over sound
            soundArray[4].Play();
            //TODO: close this screen and open the GameOverScreen
            Form1.ChangeScreen(this, new GameOverScreen());
        }

        public void ClickEvent(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            //find which button was clicked and store it
            for (int i = 0; i < buttonArray.Length; i++)
            {
                if (buttonArray[i] == button)
                {
                    arrayIndex = i;
                }
            }

            //if button that flashed is the button that was pressed
            if (Form1.pattern[guess] == arrayIndex)
            {
                //blink the button and play a sound
                soundArray[arrayIndex].Play();
                buttonArray[arrayIndex].BackColor = secondColorArray[arrayIndex];

                guess++;
            }
            //else lose
            else
            {
                GameOver();
            }
        }

        private void starTimer_Tick(object sender, EventArgs e)
        {
            Random random = new Random();

            //make new stars
            int randNum = random.Next(1, 101);
            if (randNum > 99)
            {
                int y = random.Next(-this.Height, 0);
                int x = random.Next(this.Width, this.Width * 2);

                Point star = new Point(x, y);
                starPoints.Add(star);
                starXSpeed.Add(random.Next(-10, -1));
                starYSpeed.Add(random.Next(1, 10));
            }

            //move stars
            for (int i = 0; i < starPoints.Count; i++)
            {
                float x = starPoints[i].X + starXSpeed[i];
                float y = starPoints[i].Y + starYSpeed[i];
                starPoints[i] = new PointF(x, y);
            }

            //delete stars
            for (int i = 0; i < starPoints.Count; i++)
            {
                if ((starPoints[i].X < -30))
                {
                    starPoints.RemoveAt(i);
                    starXSpeed.RemoveAt(i);
                    starYSpeed.RemoveAt(i);
                }
            }

            //player turn
            if (playerTurn == true)
            {
                for (int i = 0; i < blinkTimer.Length; i++)
                {
                    //if button is lit up increase its timer
                    if (buttonArray[i].BackColor == secondColorArray[i])
                    {
                        blinkTimer[i]++;
                    }

                    //if buttons timer is above limit change it back to original colour
                    if (blinkTimer[i] >= DELAY_TIME)
                    {
                        buttonArray[i].BackColor = firstColorArray[i];
                        blinkTimer[i] = 0;
                    }

                    //if blink timer is reset and the guess number and pattern size are equal go to computer turn
                    if (guess == Form1.pattern.Count && blinkTimer[i] == 0)
                    {
                        //blinking lights to show round over
                        for (int g = 0; g < 4; g++)
                        {
                            for (int f = 0; f < buttonArray.Length; f++)
                            {
                                buttonArray[f].BackColor = secondColorArray[f];
                            }
                            this.Refresh();
                            Thread.Sleep(300);
                            for (int f = 0; f < buttonArray.Length; f++)
                            {
                                buttonArray[f].BackColor = firstColorArray[f];
                            }
                            this.Refresh();
                            Thread.Sleep(300);
                        }

                        //reset and change to computer turn
                        ComputerTurn();
                        counter = 0;

                        computerTurn = true;
                        playerTurn = false;
                    }
                }
            }

            //computer turn
            if (computerTurn == true)
            {
                //go one by one through the pattern lighting up buttons

                arrayIndex = Form1.pattern[counter];

                //play sound only once
                if (blinkTimer[arrayIndex] == 0)
                {
                    soundArray[arrayIndex].Play(); ;
                }
                //wait while light is on
                if (blinkTimer[arrayIndex] < DELAY_TIME)
                {
                    buttonArray[arrayIndex].BackColor = secondColorArray[arrayIndex];
                    blinkTimer[arrayIndex]++;
                }
                //wait while light is off
                else if (blinkTimer[arrayIndex] < DELAY_TIME * 2)
                {
                    buttonArray[arrayIndex].BackColor = firstColorArray[arrayIndex];
                    blinkTimer[arrayIndex]++;
                }
                //reset timer and move on to next part of the pattern
                else
                {
                    blinkTimer[arrayIndex] = 0;
                    counter++;
                }

                //if at the end of the pattern switch to player turn
                if (counter == Form1.pattern.Count)
                {
                    computerTurn = false;
                    playerTurn = true;
                }
            }

            this.Refresh();
        }

        //star method from draw star assignment
        public void DrawOrFillStar(object drawTool, float x, float y, float pixels)
        {
            PointF point1 = new PointF(0, 80);
            PointF point2 = new PointF(85, 80);
            PointF point3 = new PointF(110, 0);
            PointF point4 = new PointF(140, 80);
            PointF point5 = new PointF(225, 80);
            PointF point6 = new PointF(160, 130);
            PointF point7 = new PointF(180, 210);
            PointF point8 = new PointF(110, 160);
            PointF point9 = new PointF(40, 210);
            PointF point10 = new PointF(70, 130);
            PointF[] points = new PointF[] { point1, point2, point3, point4, point5, point6, point7, point8, point9, point10 };

            if (drawTool is Pen)
            {
                //draw
                Pen starPen;
                starPen = (Pen)drawTool;

                Graphics g = this.CreateGraphics();

                float scaleFactor = pixels / 225;

                for (int i = 0; i < points.Length; i++)
                {
                    points[i].X *= scaleFactor;
                    points[i].Y *= scaleFactor;

                    points[i].X += x;
                    points[i].Y += y;
                }

                g.DrawPolygon(starPen, points);
            }
            else
            {
                //fill
                SolidBrush starBrush;
                starBrush = (SolidBrush)drawTool;

                Graphics g = this.CreateGraphics();

                float scaleFactor = pixels / 225;

                for (int i = 0; i < points.Length; i++)
                {
                    points[i].X *= scaleFactor;
                    points[i].Y *= scaleFactor;

                    points[i].X += x;
                    points[i].Y += y;
                }

                g.FillPolygon(starBrush, points);
            }
        }

        private void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush starBrush = new SolidBrush(Color.Yellow);

            //draw stars
            for (int i = 0; i < starPoints.Count; i++)
            {
                DrawOrFillStar(starBrush, starPoints[i].X, starPoints[i].Y, 20);
            }
        }
    }
}
