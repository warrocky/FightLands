using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    class Dummy : GameObject
    {
        public DrawableTexture texture;

        public Dummy(World world, String texture)
            : base(world)
        {
            this.texture = new DrawableTexture(AssetManager.getAssetTexture(texture), this);
        }
        public Dummy(World world, AssetTexture texture)
            : base(world)
        {
            this.texture = new DrawableTexture(texture, this);
        }
        public override void Draw(DrawState state)
        {
            texture.Draw(state);
        }
    }
}
