using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    static class Graphics
    {
        static GraphicsDevice _device;
        public static GraphicsDevice device
        {
            get { return _device; }
        }

        static Point _resolution;
        public static Point resolution
        {
            get { return _resolution; }
        }

        static SpriteBatch spriteBatch;

        public static void Initialize(GraphicsDevice graphicsDevice,Point resolution)
        {
            _device = graphicsDevice;
            graphicsDevice.PresentationParameters.BackBufferWidth = resolution.X;
            graphicsDevice.PresentationParameters.BackBufferHeight = resolution.Y;
            _resolution = resolution;
            spriteBatch = new SpriteBatch(_device);
        }

        public static Camera baseCamera;

        public static void Draw()
        {
            if (baseCamera != null)
            {
                DrawState state = new DrawState();
                baseCamera.Draw(state);
            }
        }
        public static void Render()
        {
            if (baseCamera != null)
            {
                device.Clear(Color.Red);

                GraphicsComponent.RenderState state = new GraphicsComponent.RenderState();
                state.spriteBatch = spriteBatch;

                baseCamera.Render(state);

                device.SetRenderTarget(null);
                spriteBatch.Begin();
                spriteBatch.Draw(baseCamera.backbuffer, new Rectangle(0, 0, resolution.X, resolution.Y), Color.White);
                spriteBatch.End();
            }
        }

    }
}
