using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    class LandPhysicalProperties
    {
        public float radius;
        public bool activelyColliding;
        public LandCollisionTypes collisionType;
    }

    public enum LandCollisionTypes
    {
        Static,
        Solid,
        Ethereal
    }
}
