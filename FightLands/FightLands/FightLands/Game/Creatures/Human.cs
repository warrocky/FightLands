using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    class Human : Creature
    {
        public Human(Land land)
            :base(land)
        {

        }
    }
    class LandHuman : LandCreature
    {
        public LandHuman(Land land, Creature human)
            : base(human, land)
        {

        }
    }
}
