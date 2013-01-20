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
        public LandActiveBox(Land land, World world)
            : base(world, new Camera(500, 500, land))
        {
            border = new DrawableTexture("whiteSquare", this);
            border.size.X = 512;
            border.size.Y = 512;
            border.filter = Color.Black;
        }
        public override void Draw(DrawState state)
        {
            border.Draw(state);
            base.Draw(state);
        }
    }
}
