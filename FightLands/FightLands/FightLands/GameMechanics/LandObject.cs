using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    class LandObject : GameObject
    {

        public float objectRadius;
        
        public LandPhysicalProperties physicalProperties;


        public Land land
        {
            get { return (Land)world; }
        }

        public LandObject(Land land)
            : base(land)
        {

        }

        public virtual bool AuthorizeCollision(LandObject collider)
        {
            return false;
        }
        public virtual void CollideEffect(LandObject collider)
        {

        }
    }
}
