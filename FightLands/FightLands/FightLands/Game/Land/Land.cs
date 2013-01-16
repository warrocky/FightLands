using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class Land : World
    {
        List<MapPreviewer> minimaps;
        Random rdm;
        float counter;
        bool wait;

        public Land()
        {
            for(int i=0;i<10;i++)
                for (int j = 0; j < 10; j++)
                {
                    new TerrainTile(TerrainTile.TerrainType.Grassland, new Vector2(i, j) * 100f, this);
                }

            rdm = new Random();
            minimaps = new List<MapPreviewer>();
            //minimaps.Add(new MapPreviewer(this,rdm.Next()));
            wait = false;
        }

        public override void Update(UpdateState state)
        {
            if (!wait)
            {
                counter += state.elapsedTime;

                if(counter > 1f)
                for(int i=0;i<minimaps.Count;i++)
                    minimaps[i].position.X = ((counter-1f)/3f)*400f + 400f*(minimaps.Count-1 - i) - 400f;
            }
            else
                wait = false;

            if (counter > 4)
            {
                minimaps.Add(new MapPreviewer(this, rdm.Next()));
                minimaps[minimaps.Count - 1].position.X = -400;
                counter = 0;
                wait = true;
            }
            base.Update(state);
        }
    }
}
