using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    class FightObject : GameObject
    {
        public FightWorld fight
        {
            get { return (FightWorld)world; }
        }

        public FightObject(FightWorld world)
            : base(world)
        {
        }
    }
}
