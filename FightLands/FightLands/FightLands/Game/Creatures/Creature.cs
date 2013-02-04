using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    abstract class Creature : GameObject
    {
        public int strength;
        public int dexterity;
        public int inteligence;

        public Creature(World world)
            : base(world)
        {

        }
    }
}
