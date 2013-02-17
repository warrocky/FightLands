using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class Rat : LandCreature
    {

        DrawableTexture texture;

        public Rat(Land land, int seed)
            : base(land)
        {
            Random rdm = new Random(seed);
            texture = new DrawableTexture("whiteSquare", this);
            texture.size = new Vector2(10f, 10f);
            texture.filter = Color.Black;
            texture.rotation = (float)(rdm.NextDouble() * Math.PI * 2f);

            mobProperties = new MobProperties(20f, true);
        }
        public override void Draw(DrawState state)
        {
            texture.Draw(state);
        }
    }
}
