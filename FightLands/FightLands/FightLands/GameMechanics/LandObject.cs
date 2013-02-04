using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    class LandObject : GameObject
    {

        public float objectRadius;
        LandPhysicalProperties physicalProperties;

        public Land land
        {
            get { return (Land)world; }
        }

        public LandObject(Land land)
            : base(land)
        {
            physicalProperties = new LandPhysicalProperties();
        }

        public bool AuthorizeCollision(LandObject collider)
        {
            return false;
        }
    }
}
