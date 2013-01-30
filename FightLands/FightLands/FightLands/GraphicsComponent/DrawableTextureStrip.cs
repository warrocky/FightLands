using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    class DrawableTextureStrip
    {
        GameObject parent;
        AssetTextureStrip texture;

        public Color filter;
        public Vector2 position;
        public float rotation;
        public float layer;
        public Vector2 size;


        private float _phase;
        public float period;
        public float Phase
        {
            get { return _phase*period; }
            set { _phase = value/period; }
        }

        public Vector2 textureScale
        {
            set
            {
                size.X = value.X * texture.width;
                size.Y = value.Y * texture.height;
            }
            get { return new Vector2(size.X / (float)texture.width, size.Y / (float)texture.height); }
        }

        public DrawableTextureStrip(AssetTextureStrip texture, GameObject parent)
        {
            this.texture = texture;
            this.parent = parent;
            this.size = new Vector2(texture.width, texture.height);
            this.filter = Color.White;
            this.layer = 0f;
            this.period = 1f;
        }
        public DrawableTextureStrip(String textureLabel, GameObject parent)
        {
            this.texture = AssetManager.getAssetTextureStrip(textureLabel);
            this.parent = parent;
            this.size = new Vector2(texture.width, texture.height);
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
            if (texture.length != 0)
            {
                phase = (((int)(this._phase * (texture.length))) % texture.length + texture.length) % texture.length;
            }
            else
            {
                phase = 0;
            }

            state.currentCamera.addDrawCall(new DrawCallTextureStrip(texture, drawPosition, rotation + parent.rotation - state.currentCamera.rotation, filter, textureScale * state.currentCamera.zoom, layer, texture.getRectangleFrame((int)phase)));
        }

        public class DrawCallTextureStrip : GraphicsComponent.DrawCall
        {
            AssetTextureStrip texture;
            Vector2 position;
            float rotation;
            Color filter;
            Vector2 scale;
            float layer;
            Rectangle source;

            public DrawCallTextureStrip(AssetTextureStrip texture, Vector2 position, float rotation, Color filter, Vector2 scale, float layer, Rectangle source)
            {
                this.texture = texture;
                this.position = position;
                this.rotation = rotation;
                this.filter = filter;
                this.scale = scale;
                this.layer = layer;
                this.source = source;
            }

            internal override void Render(GraphicsComponent.RenderState state)
            {
                state.spriteBatch.Draw(texture.texture, position, source, filter, rotation, texture.center, scale, SpriteEffects.None, layer);
            }
        }
    }
}
