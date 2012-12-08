using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    class Camera : GraphicsComponent.RenderBatch
    {
        /// <summary>
        /// The width associated with this camera backbuffer.
        /// </summary>
        public readonly int width;
        /// <summary>
        /// The height associated with this camera backbuffer.
        /// </summary>
        public readonly int height;

        public Vector2 position;
        public float rotation;
        float _zoom;
        /// <summary>
        /// A property that represents the zoom factor of this camera.
        /// </summary>
        public float zoom
        {
            get { return _zoom; }
            set 
            { 
                _zoom = value;
                _diagonal = new Vector2(width, height) / value;
            }
        }

        Vector2 _diagonal;
        /// <summary>
        /// A property that returns the rendering diagonal of this camera.
        /// </summary>
        public Vector2 diagonal
        {
            get { return _diagonal; }
        }

        public World world;

        public Camera(int width, int height, World world)
            :base(width, height)
        {
            this.width = width;
            this.height = height;
            zoom = 1f;
            this.world = world;
        }

        public void Draw(DrawState state)
        {
            Camera previousCamera = state.currentCamera;
            if(previousCamera != null)
                previousCamera.addSubBatch(this);
            state.currentCamera = this;

            world.Draw(state);

            state.currentCamera = previousCamera;
        }
    }
}
