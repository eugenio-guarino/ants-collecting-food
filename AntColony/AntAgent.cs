using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using SOFT152SteeringLibrary;

namespace SOFT152Steering
{
    class AntAgent
    {

        /// <summary>
        /// The speed of the agent as used in all three movment methods 
        /// Ideal value depends on timer tick interval and realistic motion of
        /// agents needed. Suggest though in range 0 ... 2
        /// </summary>
        public double AgentSpeed { set; get; }


        /// <summary>
        /// If the agent is using the the ApproachAgent() method, this property defines
        /// at what point the agent will reduce the speed of approach to miminic a 
        /// more relistic approach behaviour
        /// </summary>
        public double ApproachRadius { set; get; }

        public double AvoidDistance { set; get; }

        /// <summary>
        /// Property defines how 'random' the agent movement is whilst 
        /// the agent is using the Wander() method
        /// Suggest range of WanderLimits is 0 ... 1
        /// </summary>
        public double WanderLimits { set; get; }


        /// <summary>
        /// Used in conjunction worldBounds to determine if
        /// the agents position will stay within the world bounds 
        /// </summary>
        public bool ShouldStayInWorldBounds { set; get; }

        /// <summary>
        /// Current postion of the agent, updated by the three
        /// movment methods
        /// </summary>
        public SOFT152Vector agentPosition;

        /// <summary>
        /// used in conjunction with the Wander() method
        /// to detemin the next position an agent should be in 
        /// Should remain a private field and do not edit within this class
        /// </summary>
        private SOFT152Vector wanderPosition;

        /// <summary>
        /// this stores the previous position of the agent and it's
        /// used only by the 'robber' ant to come back to the last position
        /// he saw the ant that it stoled the food from
        /// </summary>
        public SOFT152Vector previousAntAgentPosition { set; get; }

        /// <summary>
        /// Used by the regular ants, this is the vector that allows the ants
        /// to come back to the location where the food is depleted and check if
        /// there's something left.
        /// </summary>
        public SOFT152Vector previousFoodPilePosition {set ; get;}

        /// <summary>
        /// this bool is set only if the specific ant is close to another ant
        /// </summary>
        public bool isCloseToAnt { set; get; }

        /// <summary>
        /// If this boolean is set to true it means that the 'regular'
        /// ant is close to his nest and vice vers if false.
        /// </summary>
        public bool isCloseToNest { set; get; }

        /// <summary>
        /// If the 'regular' ant is close to the food, then this will be set to true,
        /// if it's not, it will be set to false.
        /// </summary>
        public bool isCloseToFood { set; get; }

        /// <summary>
        /// The boolean below is used by the 'robber' ant to come back to the location of the 
        /// ant it stole the food from. It is used in conjunction with the previousAntAgentPosition
        /// vector.
        /// </summary>
        public bool isAntLocationKnown { set; get; }

        /// <summary>
        /// it will be set to true if the ant knows where the food is/was. 
        /// </summary>
        public bool isFoodKnown { set; get; }

        /// <summary>
        /// If the ants has got or stolen food it will be set to true. This boolean also determines
        /// wether the little piece of food carried should be drawn or not.
        /// </summary>
        public bool isCarrying { set; get; }

        /// <summary>
        /// The boolean is used by the 'regular' ants and it will be set to true if they know the
        /// position of the nest and to false if they don't.
        /// </summary>
        public bool isNestKnown { set; get; }

        /// <summary>
        /// This int stores the index of the closest food to the ant.
        /// </summary>
        public int foodClosest { set; get; }

        /// <summary>
        /// This int stores the index of the closest nest to the ant.
        /// </summary>
        public int nestClosest { set; get; }

        /// <summary>
        /// This int stores the index of the closest ant to the another ant.
        /// </summary>
        public int closestAnt { set; get; }

        /// <summary>
        /// This int determines how forgetful an ant is.
        /// </summary>
        public int Forgetfulness { set; get; }


        /// <summary>
        /// The size of the world the agent lives on as a Rectangle object.
        /// Used in conjunction with ShouldStayInWorldBounds, which if true
        /// will mean the agents position will be kept within the world bounds 
        /// (i.e. the  world width or the world height)
        /// </summary>
        private Rectangle worldBounds;   // To keep track of the obejcts bounds i.e. ViewPort dimensions

        /// <summary>
        /// The random object passed to the agent. 
        /// Used only in the Wander() method to generate a 
        /// random direction to move in
        /// </summary>
        private Random randomNumberGenerator;         


        public AntAgent(SOFT152Vector position, Random random, Rectangle bounds )
        {
            agentPosition = new SOFT152Vector(position.X, position.Y);

            worldBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);

            randomNumberGenerator = random;

            InitialiseAgent();
        }

        /// <summary>
        /// Initialises the Agents various fields
        /// with default values
        /// </summary>
        private void InitialiseAgent()
        {
            wanderPosition = new SOFT152Vector();

            ShouldStayInWorldBounds = true;

            ApproachRadius = 10;

            AvoidDistance = 25;

            AgentSpeed = 1.0;

            WanderLimits = 0.5;

            Forgetfulness = 20; // out of 10000
        }

        /// <summary>
        /// Causes the agent to make one step towards the object at objectPosition
        /// The speed of approach will reduce one this agent is within
        /// an ApproachRadius of the objectPosition
        /// </summary>
        /// <param name="agentToApproach"></param>
        public void Approach(SOFT152Vector objectPosition)
        {

            Steering.MoveTo(agentPosition, objectPosition, AgentSpeed, ApproachRadius);

            StayInWorld();
        }


        /// <summary>
        /// Causes the agent to make one random step.
        /// The size of the step determined by the value of WanderLimits
        /// and the agents speed
        /// </summary>
        public void Wander()
        {
            Steering.Wander(agentPosition, wanderPosition, WanderLimits, AgentSpeed, randomNumberGenerator);

            StayInWorld();
        }



        private void StayInWorld()
        {
            // if the agent should stay with in the world
            if (ShouldStayInWorldBounds == true)
            {
                // and the world has a positive width and height
                if (worldBounds.Width >= 0 && worldBounds.Height >= 0)
                {
                    // now adjust the agents position if outside the limits of the world
                    if (agentPosition.X < 0)
                        agentPosition.X = worldBounds.Width;

                    else if (agentPosition.X > worldBounds.Width)
                        agentPosition.X = 0;

                    if (agentPosition.Y < 0)
                        agentPosition.Y = worldBounds.Height;

                    else if (agentPosition.Y > worldBounds.Height)
                        agentPosition.Y = 0;
                }
            }
        }


        public void DepositFood()
        {
            isCarrying = false;

        }


    }  // end class AntAgent
}
