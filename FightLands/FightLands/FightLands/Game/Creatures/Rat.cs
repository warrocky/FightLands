using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class Rat : Creature
    {
        public Rat(Land land)  
            :base(land)
        {

        }
    }
    class LandRat : LandCreature
    {
        DrawableTexture texture;

        public LandRat(Land land, Rat rat, int seed)
            : base(rat, land)
        {
            Random rdm = new Random(seed);

            texture = new DrawableTexture("whiteSquare", this);
            texture.size = new Vector2(10f, 10f);
            texture.filter = Color.Black;
            texture.rotation = (float)(rdm.NextDouble() * Math.PI * 2f);
        }
        public LandRat(Land land, int seed, Rat creature)
            :base(creature, land)
        {
            Random rdm = new Random(seed);

            texture = new DrawableTexture("whiteSquare", this);
            texture.size = new Vector2(10f, 10f);
            texture.filter = Color.Black;
            texture.rotation = (float)(rdm.NextDouble() * Math.PI * 2f);
        }
        public override void Draw(DrawState state)
        {
            texture.Draw(state);
        }
    }
}
