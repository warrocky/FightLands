using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class Town : LandObject
    {
        int ID;
        public String name;
        DrawableTexture texture;
        DrawableText label;

        public Town(Land land, int ID)
            : base(land)
        {
            this.ID = ID;
            texture = new DrawableTexture("whiteSquare", this);
            texture.filter = Color.Orange;
            texture.layer = 0.95f;
            label = new DrawableText(AssetManager.getAssetSpriteFont("townLabelFont"), this, "Town " + ID, Color.White);
            label.layer = 0.949f;
        }
        public override void Draw(DrawState state)
        {
            texture.Draw(state);
            label.Draw(state);
        }

        public enum TownResources
        {
            Stone,
            Water,
            Wood
        }
    }
}
