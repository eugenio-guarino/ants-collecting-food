using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SOFT152SteeringLibrary;

namespace SOFT152Steering
{
    public partial class AntFoodForm : Form
    {

        private List<FoodPile> foodPileList;

        private List<AntAgent> antList;

        private List<AntAgent> robberAntList;

        private List<AntNest> antNestList;

        private List<AntNest> robberAntNestList;

        // the random object given to each Ant agent
        private Random randomGenerator;

        // A bitmap image used for double buffering
        private Bitmap backgroundImage;

        /// <summary>
        /// This is the initial method called at the start of the program. Here, the list for the food
        /// and nests are created. 
        /// </summary>
        public AntFoodForm()
        {
            antNestList = new List<AntNest>();

            robberAntNestList = new List<AntNest>();

            foodPileList = new List<FoodPile>();

            InitializeComponent();

            CreateBackgroundImage();

            CreateAnts();
        }

        /// <summary>
        /// this method creates the ants and put them into their lists. Thanks to the for-loops inside
        /// we can set the amount of ants that will wander on the graphic panel.
        /// </summary>
        private void CreateAnts()
        {
            Rectangle worldLimits;

            AntAgent tempAnt;

            antList = new List<AntAgent>();

            robberAntList = new List<AntAgent>();

            worldLimits = new Rectangle(0, 0, drawingPanel.Width, drawingPanel.Height);

            randomGenerator = new Random();

            //Inside the for-loop below, two coordinates get created randomly. They are then used to create
            //the vector object of a single ant. The vector object is used inside the AntAgent constructor method
            //to give the AntAgent a position on the graphic panel. The AntAgent object is assigned to the variable
            //"tempAnt". After it is added to the list the loop goes back to create another AntAgent.

            //The loop below is used to create the 'civilian' ants.

            for (int i = 0; i < 700; i++)
            {
                int X = Convert.ToInt32(randomGenerator.Next(0, drawingPanel.Width - 1));
                int Y = Convert.ToInt32(randomGenerator.Next(0, drawingPanel.Height - 1));
                tempAnt = new AntAgent(new SOFT152Vector(X, Y), randomGenerator, worldLimits);
                antList.Add(tempAnt);
            }

            //The loop below creates the 'robber' ants.

            for (int i = 0; i < 0; i++)
            {
                int X = Convert.ToInt32(randomGenerator.Next(0, drawingPanel.Width - 1));
                int Y = Convert.ToInt32(randomGenerator.Next(0, drawingPanel.Height - 1));
                tempAnt = new AntAgent(new SOFT152Vector(X, Y), randomGenerator, worldLimits);
                robberAntList.Add(tempAnt);
            }

        }

        private void CreateBackgroundImage()
        {
            //This method is called to create the background image.

            int imageWidth;
            int imageHeight;

            imageWidth = drawingPanel.Width;
            imageHeight = drawingPanel.Height;

            backgroundImage = new Bitmap(drawingPanel.Width, drawingPanel.Height);
        }

        /// <summary>
        /// timer_Tick is the main method of AntFoodForm. It is where all the
        /// behaviours of the ants are. It contains many for-loops that set 
        /// the booleans related to the movements of the ants.
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            //This is the method called to draw all the ants, nest and food piles on the panel.
            DrawDoubleBuffering();

            //Inside the for-loop below, all the behaviours related to the 'civilian' ants will take place.
            for (int i = 0; i < antList.Count; i++)
            {
                var ant = antList.ElementAt(i);

                if (randomGenerator.Next(0, 10000) < ant.Forgetfulness)
                {
                    // This if-statement is what makes the ants forgetful. A number between 0 and 20000
                    // gets picked and id the number is less than the Forgetfulness parameter, all the booleans of
                    // the ant are reset
                    ant.isFoodKnown = false;

                    ant.isNestKnown = false;

                    ant.foodClosest = 0;

                    ant.nestClosest = 0;

                    ant.closestAnt = 0;
                }

                ant.AgentSpeed = 0.95;

                ant.isCloseToNest = false;

                ant.isCloseToFood = false;

                ant.isCloseToAnt = false;



                for (int j = 0; j < foodPileList.Count; j++)
                {
                    if (foodPileList.ElementAt(j) != null)
                    {
                        if (ant.agentPosition.Distance(foodPileList.ElementAt(j).foodPosition) <= 11)
                        {
                            ant.isCloseToFood = true;
                            ant.isFoodKnown = true;
                            ant.foodClosest = j;
                            ant.previousFoodPilePosition = foodPileList.ElementAt(j).foodPosition;
                        }
                    }
                }


                for (int k = 0; k < antNestList.Count; k++)
                {
                    if (ant.agentPosition.Distance(antNestList.ElementAt(k).nestPosition) <= 11)
                    {
                        ant.isCloseToNest = true;
                        ant.isNestKnown = true;
                        ant.nestClosest = k;

                    }
                }


                for (int l = 0; l < antList.Count; l++)
                //loop through all ants again
                {
                    if (ant.agentPosition.Distance(antList.ElementAt(l).agentPosition) <= 5)
                    {
                        ant.isCloseToAnt = true;
                        ant.closestAnt = l;
                    }
                }

                // Below, the set of if-statements that determines the movements of the ants in the list.
                // Inside them, if the requirements are satisfied, the behaviour-related methods are called.

                if (ant.isCarrying)
                {
                    if (ant.isCloseToNest)
                    {
                        //The ant will deposit the food only if it's carrying and it's close to the nest.
                        ant.DepositFood();
                    }
                    else
                    {
                        if (ant.isNestKnown)
                        {
                            //If the ants it's not close to the nest and it's carrying the food it then will approach the nest location.
                            ant.Approach(antNestList.ElementAt(ant.nestClosest).nestPosition);
                        }
                        else
                        {
                            //If non of the above it's satisfied, the ant will wander asking for information.
                            ant.Wander();
                        }
                    }
                }
                else
                {

                    if (ant.isCloseToFood)
                    {
                        //If the ant is close to the food, it will then pick up the food.
                        PickUpFood(i);

                    }
                    else
                    {
                        if (ant.isFoodKnown)
                        {
                            if (foodPileList.ElementAt(ant.foodClosest) != null)
                            {
                                //The ant will go to the food only if its location it's known and the food 
                                // it's still active (not null).
                                ant.Approach(foodPileList.ElementAt(ant.foodClosest).foodPosition);
                            }
                            else
                            {
                                //When the food depletes, the ant will go to check if there's still food left
                                // by going to the previous location of the food and setting the "isFoodKnown"
                                // boolean to false.
                                ant.Approach(ant.previousFoodPilePosition);

                                if (ant.agentPosition.Distance(ant.previousFoodPilePosition) <= 11)
                                {
                                    ant.isFoodKnown = false;
                                    ant.previousFoodPilePosition = null;
                                }

                            }

                        }

                        else
                        {
                            ant.Wander();
                        }
                    }
                }
                if (ant.isCloseToAnt)
                {
                    AskLocations(i);
                }
            }


            //Inside the for-loop below, all the behaviour related to the 
            // 'robber' ants will take place.

            for (int i = 0; i < robberAntList.Count; i++)
            {
                var robberant = robberAntList.ElementAt(i);

                robberant.isCloseToAnt = false;

                robberant.isCloseToNest = false;

                //'robber' ant speed is slightly slower to give the 'civilian' ants some advantage
                robberant.AgentSpeed = 0.85;



                for (int j = 0; j < antList.Count; j++)
                {
                    if (robberant.agentPosition.Distance(antList.ElementAt(j).agentPosition) <= 5)
                    {
                        robberant.isCloseToAnt = true;
                        robberant.closestAnt = j;
                    }
                }


                for (int r = 0; r < robberAntNestList.Count; r++)
                {
                    if (robberant.agentPosition.Distance(robberAntNestList.ElementAt(r).nestPosition) < 11)
                    {
                        robberant.isCloseToNest = true;
                        robberant.nestClosest = r;
                        robberant.isNestKnown = true;
                    }
                }

                if (robberant.isCarrying)
                {
                    if (robberant.isCloseToNest)
                    {
                        robberant.DepositFood();
                    }
                    else
                    {
                        if (robberant.isNestKnown)
                        {
                            robberant.Approach(robberAntNestList.ElementAt(robberant.nestClosest).nestPosition);
                        }
                        else
                        {
                            robberant.Wander();
                        }
                    }
                }

                else
                {
                    if (robberant.isCloseToAnt)
                    {
                        if (antList.ElementAt(robberant.closestAnt).isCarrying)
                        {
                            //If the "robber" ant is close to a "civilian" ant carrying food
                            // the method to steal the food will be called.
                            StealFood(i);
                        }
                        else
                        {
                            robberant.Wander();
                        }
                    }
                    else
                    {
                        if (robberant.isAntLocationKnown)
                        {
                            //the 'robber' ant will chase down the ant from which the food was stealed.
                            robberant.Approach(robberant.previousAntAgentPosition);

                            if (robberant.agentPosition.Distance(robberant.previousAntAgentPosition) <= 1)
                            {
                                //when the 'robber' ant comes back to the location it thoughts the food was
                                //it will acknowledge the other ant is now gone and the boolean "isAntLocationKnown"
                                // will be set to false.
                                robberant.isAntLocationKnown = false;
                            }
                        }
                        else
                        {
                            robberant.Wander();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// The method below is used by the 'civilian' ants to ask for locations. If the ant it's close to another ant
        /// and doesn't know where the food is, but the other does, the "isFoodKnown" boolean will be set to true and thus
        /// the information is passed. Same with the location of the nests.
        /// </summary>
        private void AskLocations(int antID)
        {
            int closestAnt;
            closestAnt = antList.ElementAt(antID).closestAnt;

            if (antList.ElementAt(antID).isNestKnown == false && antList.ElementAt(closestAnt).isNestKnown == true)
            {
                antList.ElementAt(antID).nestClosest = antList.ElementAt(closestAnt).nestClosest;
                antList.ElementAt(antID).isNestKnown = true;
            }

            if (antList.ElementAt(antID).isFoodKnown == false && antList.ElementAt(closestAnt).isFoodKnown == true)
            {
                antList.ElementAt(antID).foodClosest = antList.ElementAt(closestAnt).foodClosest;
                antList.ElementAt(antID).previousFoodPilePosition = antList.ElementAt(closestAnt).previousFoodPilePosition;
                antList.ElementAt(antID).isFoodKnown = true;
            }

        }

        /// <summary>
        /// The method below is used by the 'civilian' ants to pick up the food from the FoodPile.
        /// Each time they pick up the food, the quantity of the food pile will decrease and the 
        /// "isCarrying" and "isFoodKnown" booleans will be both set to true. When the quantity of
        /// the food is equal to 0, the EraseFood method will be called.
        /// </summary>
        private void PickUpFood(int antID)
        {
            int foodClosest;

            foodClosest = antList.ElementAt(antID).foodClosest;

            foodPileList.ElementAt(foodClosest).Quantity--;

            antList.ElementAt(antID).isCarrying = true;

            antList.ElementAt(antID).isFoodKnown = true;

            //when the food depletes the DeleteFood method is called
            if (foodPileList.ElementAt(foodClosest).Quantity == 0)
            {
                DeleteFood(foodClosest);
            }
        }

        /// <summary>
        /// This is the method used by the 'robber' ants to steal the food from the 'civilian' ants.
        /// When the 'robber' ant is in close proximity of a 'civilian' ant, the boolean "isCarrying"
        /// will be set to true for the 'robber' ant and to false for the 'civilian' ant.
        /// </summary>
        private void StealFood(int antID)
        {
            int closestAnt;

            closestAnt = robberAntList.ElementAt(antID).closestAnt;

            robberAntList.ElementAt(antID).isCarrying = true;

            antList.ElementAt(closestAnt).isCarrying = false;

            robberAntList.ElementAt(antID).isAntLocationKnown = true;

            robberAntList.ElementAt(antID).previousAntAgentPosition = antList.ElementAt(closestAnt).agentPosition;

        }

        /// <summary>
        /// Thanks to this method we can erase the food from the background image and remove it from the list.
        /// </summary>
        private void DeleteFood(int foodID)
        {
            double posX, posY;

            using (Graphics backgroundGraphics = Graphics.FromImage(backgroundImage))
            using (Brush brushClearFood = new SolidBrush(Color.White))
            {
                posX = foodPileList.ElementAt(foodID).foodPosition.X;
                posY = foodPileList.ElementAt(foodID).foodPosition.Y;

                backgroundGraphics.FillEllipse(brushClearFood, (float)posX, (float)posY, 15, 15);
            }

            //the next two strings will remove the foodPile object from the list
            foodPileList.RemoveAt(foodID);
            foodPileList.Insert(foodID, null);

        }

        /// <summary>
        /// Draws the ants and any stationary objects using 'double buffering', a technique that 
        /// prevents flickering. In this method, all the graphic aspects of the program are handled by using several
        /// foreach loops.
        /// </summary>
        private void DrawDoubleBuffering()
        {
            float tempXPosition;

            float tempYPosition;

            float antSize;

            antSize = 5f;

            using (Graphics backgroundGraphics = Graphics.FromImage(backgroundImage))
            {
                backgroundGraphics.Clear(Color.SandyBrown);

                //this loop will draw the 'civilian' ants
                foreach (AntAgent agent in antList)
                {
                    // by using the keyword "using" there's no need to dispose of the brush afterwards
                    using (Brush antBrush = new SolidBrush(Color.Black))
                    {
                        // get the 1st agent position
                        tempXPosition = (float)agent.agentPosition.X;
                        tempYPosition = (float)agent.agentPosition.Y;

                        // draw the 1st agent on the backgroundImage
                        backgroundGraphics.FillRectangle(antBrush, tempXPosition, tempYPosition, antSize, antSize);
                    }

                    //If the ant is carrying something, then the unit of food will be drawn
                    if (agent.isCarrying)
                    {
                        using (Brush brushCarrying = new SolidBrush(Color.FloralWhite))
                        backgroundGraphics.FillRectangle(brushCarrying, tempXPosition + 1, tempYPosition + 1, 3, 3);
                    }

                }

                // this loop will draw the 'robber' ants
                foreach (AntAgent robberAgent in robberAntList)
                {
                    using (Brush robberBrush = new SolidBrush(Color.DarkRed))
                    {
                        // get the 1st agent position
                        tempXPosition = (float)robberAgent.agentPosition.X;
                        tempYPosition = (float)robberAgent.agentPosition.Y;

                        // draw the 1st agent on the backgroundImage
                        backgroundGraphics.FillRectangle(robberBrush, tempXPosition, tempYPosition, antSize, antSize);
                    }

                    if (robberAgent.isCarrying)
                    {
                        using (Brush brushCarrying = new SolidBrush(Color.FloralWhite))
                        {
                            backgroundGraphics.FillRectangle(brushCarrying, tempXPosition + 1, tempYPosition + 1, 3, 3);
                        }
                    }
                }
                
                //this loop will draw all the 'civilian' nests
                foreach (AntNest nest in antNestList)
                {
                    using (Brush brushNest1 = new SolidBrush(Color.Black))
                    using (Brush brushNest2 = new SolidBrush(Color.Gray))
                    {
                        tempXPosition = (float)nest.nestPosition.X;
                        tempYPosition = (float)nest.nestPosition.Y;

                        backgroundGraphics.FillEllipse(brushNest2, tempXPosition - 2, tempYPosition - 2, 20, 20);
                        backgroundGraphics.FillEllipse(brushNest1, tempXPosition, tempYPosition, 15, 15);

                    }
                }

                //this loop will draw all the piles of food
                foreach (FoodPile food in foodPileList)
                {
                    if (food != null)
                    {
                        using (Brush brushFood = new SolidBrush(Color.FloralWhite))
                        {
                            tempXPosition = (float)food.foodPosition.X;
                            tempYPosition = (float)food.foodPosition.Y;

                            backgroundGraphics.FillEllipse(brushFood, tempXPosition, tempYPosition, 15, 15);
                        }
                    }
                }

                //this loop will draw all the 'robber' ants
                foreach (AntNest robberNest in robberAntNestList)
                {
                    using (Brush brushRobberNest1 = new SolidBrush(Color.DarkRed))
                    using (Brush brushRobberNest2 = new SolidBrush(Color.Gray))
                    {
                        tempXPosition = (float)robberNest.nestPosition.X;
                        tempYPosition = (float)robberNest.nestPosition.Y;

                        backgroundGraphics.FillEllipse(brushRobberNest2, tempXPosition - 2, tempYPosition - 2, 20, 20);
                        backgroundGraphics.FillEllipse(brushRobberNest1, tempXPosition, tempYPosition, 15, 15);

                    }
                }

            }

            using (Graphics g = drawingPanel.CreateGraphics())
            {
                g.DrawImage(backgroundImage, 0, 0, drawingPanel.Width, drawingPanel.Height);
            }

        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            //this is what makes the timer stop
            timer.Stop();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            //this is what makes the timer start
            timer.Start();
        }

        /// <summary>
        /// With this event handler is possible to reset the program without turning it off and on
        /// back again. It deletes the old lists and create new ones. All nests and food piles will
        /// be lost and the ants will not longer carry anything.
        /// </summary>
        private void resetButton_Click(object sender, EventArgs e)
        {
            CreateBackgroundImage();

            CreateAnts();

            antNestList = new List<AntNest>();

            robberAntNestList = new List<AntNest>();

            foodPileList = new List<FoodPile>();

        }

        /// <summary>
        /// This event handler is used to determine the item you're placing.
        /// </summary>
        private void drawingPanel_MouseClick(object sender, MouseEventArgs e)
        {
            Point mouseClickPoint;

            FoodPile tempFood;

            AntNest tempNest;

            AntNest tempRobberNest;

            mouseClickPoint = e.Location;

            if (timer.Enabled)
            {

                if (foodRadioButton.Checked)
                {
                    tempFood = new FoodPile(new SOFT152Vector(mouseClickPoint.X - 10, mouseClickPoint.Y - 10));
                    foodPileList.Add(tempFood);
                }

                else if (nestRadioButton.Checked)
                {
                    tempNest = new AntNest(new SOFT152Vector(mouseClickPoint.X - 10, mouseClickPoint.Y - 10));
                    antNestList.Add(tempNest);
                }

                else if (RobberNestRadioButton.Checked)
                {
                    tempRobberNest = new AntNest(new SOFT152Vector(mouseClickPoint.X - 10, mouseClickPoint.Y - 10));
                    robberAntNestList.Add(tempRobberNest);
                }

            }


            if (!timer.Enabled)
            {
                //This error message box prevents people from placing anything before the timer is started.
                MessageBox.Show("Cannot place items when simulation is stopped.");

            }
        }
    }
} 
