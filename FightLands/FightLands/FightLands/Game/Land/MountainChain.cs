using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class MountainChain
    {
        List<Mountain> mountains;

        public MountainChain(List<Vector2> chainSpline, Land land, int seed)
        {
            mountains = new List<Mountain>();
            Random rdm = new Random(seed);


        }
    }
}
