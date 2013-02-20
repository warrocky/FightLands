using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    class MapPreviewer : GameObject, Resizable
    {
        DrawableTexture minimap;

        public MapPreviewer(World world, Land land, Vector2 size)
            :base(world)
        {
            Texture2D textureBase = land.getMinimap(new Point(200,200));

            AssetTexture text = new AssetTexture(textureBase, "minimapTexture");

            minimap = new DrawableTexture(text, this);
            minimap.size = size;
        }

        public override void Draw(DrawState state)
        {
            minimap.Draw(state);
        }

        public Vector2 getSize()
        {
            return minimap.size;
        }
        public void setSize(Vector2 size)
        {
            minimap.size = size;
        }
    }
}
