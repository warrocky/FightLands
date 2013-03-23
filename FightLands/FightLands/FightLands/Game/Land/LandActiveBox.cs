using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class LandActiveBox : ActiveBox
    {
        DrawableTexture border;
        public LandActiveBox(Land land, World world, Point size)
            : base(world, new Camera(size.X - 10, size.Y - 10, land))
        {
            border = new DrawableTexture("whiteSquare", this);
            border.size.X = size.X;
            border.size.Y = size.Y;
            border.filter = Color.Black;
            border.layer = 0.501f;
            texture.layer = 0.5f;
        }
        public override void Draw(DrawState state)
        {
            border.Draw(state);
            base.Draw(state);
        }
    }
}
