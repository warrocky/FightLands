using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    abstract class Creature : LandObject
    {
        public int strength;
        public int dexterity;
        public int inteligence;

        public Creature(Land world)
            : base(world)
        {

        }
    }
}
