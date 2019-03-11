using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using SOFT152SteeringLibrary;

namespace SOFT152Steering
{
    public class FoodPile
    {
        public SOFT152Vector foodPosition;
        public int Quantity { set; get; }

        public FoodPile(SOFT152Vector position)
        {
            //the units of the food object
            Quantity = 250;

            //the position of the food object
            foodPosition = new SOFT152Vector(position.X, position.Y);
        }

    }
}