using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class Land : World
    {
        public Land()
        {
            for(int i=0;i<10;i++)
                for (int j = 0; j < 10; j++)
                {
                    new TerrainTile(TerrainTile.TerrainType.Grassland, new Vector2(i, j) * 100f, this);
                }
        }
    }
}
