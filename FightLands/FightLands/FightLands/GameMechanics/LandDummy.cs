using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    class LandDummy : LandObject
    {
        public DrawableTexture texture;

        public LandDummy(Land world, String texture)
            : base(world)
        {
            this.texture = new DrawableTexture(AssetManager.getAssetTexture(texture), this);
        }
        public LandDummy(Land world, AssetTexture texture)
            : base(world)
        {
            this.texture = new DrawableTexture(texture, this);
        }
        public override void Draw(DrawState state)
        {
            texture.Draw(state);
        }
    }

    class LandStripDummy : LandObject
    {
        public DrawableTextureStrip texture;

        public LandStripDummy(Land world, String textureStrip)
            : base(world)
        {
            this.texture = new DrawableTextureStrip(AssetManager.getAssetTextureStrip(textureStrip), this);
        }
        public LandStripDummy(Land world, AssetTextureStrip textureStrip)
            : base(world)
        {
            this.texture = new DrawableTextureStrip(textureStrip, this);
        }
        public override void Update(UpdateState state)
        {
            texture.Phase += state.elapsedTime;
            base.Update(state);
        }
        public override void Draw(DrawState state)
        {
            texture.Draw(state);
        }
    }
}
