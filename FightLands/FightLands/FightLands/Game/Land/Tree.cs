using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    class Tree : GameObject
    {
        int seed;
        DrawableTexture texture;
        public float radius;

        public Tree(int seed,  float radius, Land world)
            :base(world)
        {
            Random rdm = new Random(seed);
            texture = new DrawableTexture("whiteSquare",this);
            texture.filter = Color.ForestGreen;
            texture.size = Vector2.One * radius*2f;
            this.radius = radius;

        }
        public override void Draw(DrawState state)
        {
            texture.Draw(state);
        }

    }
}
