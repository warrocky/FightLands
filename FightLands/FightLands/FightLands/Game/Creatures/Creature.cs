using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    abstract class Creature : GameObject
    {
        public Creature(World world)
            : base(world)
        {

        }
    }
}
