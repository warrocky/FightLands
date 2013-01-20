using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    /// <summary>
    /// A class responsible for handling texture drawing associated with GameObjects.
    /// </summary>
    class DrawableTexture
    {
        GameObject parent;
        AssetTexture texture;

        public Color filter;
        public Vector2 position;
        public float rotation;
        public float layer;
        public Vector2 size;

        public Vector2 textureScale
        {
            set 
            { 
                size.X = value.X * texture.width; 
                size.Y = value.Y * texture.height; 
            }
            get { return new Vector2(size.X / (float)texture.width, size.Y / (float)texture.height); }
        }

        public DrawableTexture(AssetTexture texture, GameObject parent)
        {
            this.texture = texture;
            this.parent = parent;
            this.size = new Vector2(texture.width, texture.height);
            this.filter = Color.White;
            this.layer = 0f;
        }
        public DrawableTexture(String textureLabel, GameObject parent)
        {
            this.texture = AssetManager.getAssetTexture(textureLabel);
            this.parent = parent;
            this.size = new Vector2(texture.width, texture.height);
            this.filter = Color.White;
            this.layer = 0f;
        }

        public void Draw(DrawState state)
        {
            Vector2 drawPosition = parent.position + MathHelper.rotateVector2(parent.rotation, position);
            drawPosition = drawPosition - state.currentCamera.position;
            drawPosition *= state.currentCamera.zoom;
            drawPosition = MathHelper.rotateVector2(-state.currentCamera.rotation, drawPosition);
            drawPosition += state.currentCamera.diagonal / 2f;

            state.currentCamera.addDrawCall(new DrawCallTexture(texture, drawPosition, rotation + parent.rotation - state.currentCamera.rotation, filter, textureScale*state.currentCamera.zoom, layer));
        }

        public class DrawCallTexture : GraphicsComponent.DrawCall
        {
            AssetTexture texture;
            Vector2 position;
            float rotation;
            Color filter;
            Vector2 scale;
            float layer;

            public DrawCallTexture(AssetTexture texture, Vector2 position, float rotation, Color filter, Vector2 scale, float layer)
            {
                this.texture = texture;
                this.position = position;
                this.rotation = rotation;
                this.filter = filter;
                this.scale = scale;
                this.layer = layer;
            }

            internal override void Render(GraphicsComponent.RenderState state)
            {
                state.spriteBatch.Draw(texture.texture, position, null, filter, rotation, texture.center, scale, SpriteEffects.None, layer);
            }
        }
    }
}
