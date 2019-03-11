using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using SOFT152SteeringLibrary;

namespace SOFT152Steering
{
    public class AntNest
    {
        /// <summary>
        /// This vector determines the position of the nest.
        /// </summary>
        public SOFT152Vector nestPosition;
        public AntNest(SOFT152Vector position)
        {
            nestPosition = new SOFT152Vector(position.X, position.Y);
        }

    }
}