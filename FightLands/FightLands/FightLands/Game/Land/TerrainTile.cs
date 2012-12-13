using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    class TerrainTile : GameObject
    {

        DrawableTexture texture;

        public enum TerrainType { Grassland };

        public TerrainTile(TerrainType type, Vector2 position, Land land)
            :base(land)
        {
            this.position = position;

            texture = new DrawableTexture("whiteSquare", this);
            texture.filter = Color.ForestGreen;
        }
        public override void Draw(DrawState state)
        {
            texture.Draw(state);
        }
    }
}
