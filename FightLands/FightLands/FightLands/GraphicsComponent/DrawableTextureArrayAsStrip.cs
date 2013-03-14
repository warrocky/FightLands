using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{

    class DrawableTextureArrayAsStrip
    {
        GameObject parent;
        AssetTexture[] textures;

        public Color filter;
        public Vector2 position;
        public float rotation;
        public float layer;
        public Vector2 size;


        private float _phase;
        public float period;
        public float Phase
        {
            get { return _phase * period; }
            set { _phase = value / period; }
        }

        public Vector2 textureScale
        {
            set
            {
                AssetTexture texture = currentTexture;
                size.X = value.X * currentTexture.width;
                size.Y = value.Y * currentTexture.height;
            }
            get
            {
                AssetTexture texture = currentTexture;
                return new Vector2(size.X / (float)texture.width, size.Y / (float)texture.height);
            }
        }

        public AssetTexture currentTexture
        {
            get
            {
                return textures[(((int)(this._phase * (textures.Length))) % textures.Length + textures.Length) % textures.Length];
            }
        }

        public DrawableTextureArrayAsStrip(AssetTexture[] textures, GameObject parent)
        {
            if (textures.Length == 0)
                throw new Exception("Attempt at creating a DrawableTexstureStripAsArray with an empty AssetTexture array");

            this.textures = textures;
            this.parent = parent;
            this.size = new Vector2(textures[0].width, textures[0].height);
            this.filter = Color.White;
            this.layer = 0f;
            this.period = 1f;
        }

        public void Draw(DrawState state)
        {
            Vector2 drawPosition = parent.position + MathHelper.rotateVector2(parent.rotation, position);
            drawPosition = drawPosition - state.currentCamera.position;
            drawPosition *= state.currentCamera.zoom;
            drawPosition = MathHelper.rotateVector2(-state.currentCamera.rotation, drawPosition);
            drawPosition += state.currentCamera.diagonal / 2f;

            int phase;

            phase = (((int)(this._phase * (textures.Length))) % textures.Length + textures.Length) % textures.Length;
            Vector2 scale = new Vector2(size.X / (float)textures[phase].width, size.Y / (float)textures[phase].height);

            state.currentCamera.addDrawCall(new DrawableTexture.DrawCallTexture(textures[phase], drawPosition, rotation + parent.rotation - state.currentCamera.rotation, filter, textureScale * state.currentCamera.zoom, layer));
        }
    }

}
