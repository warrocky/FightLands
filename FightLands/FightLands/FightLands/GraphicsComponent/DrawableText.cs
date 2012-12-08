using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    class DrawableText
    {
        GameObject parent;
        AssetSpriteFont font;

        String _text;
        public String text
        {
            get { return _text; }
            set { _text = value; measure = font.font.MeasureString(_text); }
        }
        Vector2 measure;

        public Color filter;
        public Vector2 position;
        public float rotation;
        public float layer;
        public Vector2 scale;

        public float sizeX
        {
            get { return scale.X * measure.X; }
            set { scale.X = value / measure.X; }
        }

        public float sizeY
        {
            get { return scale.Y * measure.Y; }
            set { scale.Y = value / measure.Y; }
        }

        public DrawableText(AssetSpriteFont font, GameObject parent, String text, Color color)
        {
            this.font = font;
            this.parent = parent;
            this.text = text;
            this.filter = color;
            scale = Vector2.One;
        }
        public void Draw(DrawState state)
        {
            Vector2 drawPosition = parent.position + MathHelper.rotateVector2(parent.rotation, position);
            drawPosition = drawPosition - state.currentCamera.position;
            drawPosition *= state.currentCamera.zoom;
            drawPosition = MathHelper.rotateVector2(-state.currentCamera.rotation, drawPosition);
            drawPosition += state.currentCamera.diagonal / 2f;

            state.currentCamera.addDrawCall(new DrawCallSpriteFontText(font, text, filter, drawPosition, rotation + parent.rotation - state.currentCamera.rotation, layer, scale*state.currentCamera.zoom));
        }

        public class DrawCallSpriteFontText : GraphicsComponent.DrawCall
        {
            AssetSpriteFont font;
            String text;
            Color filter;
            Vector2 position;
            float rotation;
            float layer;
            Vector2 scale;

            public DrawCallSpriteFontText(AssetSpriteFont font, String text, Color filter, Vector2 position, float rotation, float layer, Vector2 scale)
            {
                this.font = font;
                this.text = text;
                this.filter = filter;
                this.position = position;
                this.rotation = rotation;
                this.layer = layer;
                this.scale = scale;
            }
            internal override void Render(GraphicsComponent.RenderState state)
            {
                state.spriteBatch.DrawString(font.font, text, position, filter, rotation, font.font.MeasureString(text) / 2f, scale, SpriteEffects.None, layer);
            }
        }
    }
}
