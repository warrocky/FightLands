using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    class MapPreviewer : GameObject
    {
        DrawableTexture minimap;

        public MapPreviewer(World world, Land land)
            :base(world)
        {
            Texture2D textureBase = land.getMinimap();

            AssetTexture text = new AssetTexture(textureBase, "minimapTexture");

            minimap = new DrawableTexture(text, this);
        }

        public override void Draw(DrawState state)
        {
            minimap.Draw(state);
        }
    }
}
