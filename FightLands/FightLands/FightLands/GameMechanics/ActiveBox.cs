using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    /// <summary>
    /// A class which draws and displays the texture of a camera.
    /// </summary>
    class ActiveBox : GameObject
    {
        static int currentID;
        int ID;

        public readonly Camera camera;
        public readonly DrawableTexture texture;

        /// <summary>
        /// The size in pixels of the visual element of the ActiveBox.
        /// </summary>
        public Vector2 size
        {
            set { texture.sizeX = value.X; texture.sizeY = value.Y; }
        }

        /// <summary>
        /// A constructor which takes the world the ActiveBox will be displayed in and the Camera which will be displayed.
        /// </summary>
        /// <param name="world">The world the ActiveBox is going to be displayed im.</param>
        /// <param name="camera">The camera the ActiveBox will display the texture of.</param>
        public ActiveBox(World world, Camera camera)
            :base(world)
        {
            currentID++;
            ID = currentID;
            this.camera = camera;
            AssetTexture textureAsset = AssetManager.CreateAssetTexture("ActiveBoxText" + ID, camera.backbuffer);
            texture = new DrawableTexture(textureAsset , this);
        }
        /// <summary>
        /// A constructor which takes the world the ActiveBox will be displayed in plus Camera specifications.
        /// </summary>
        /// <param name="activeBoxWorld">The world the ActiveBox will be displayed in.</param>
        /// <param name="targetWorld">The world the camera displayed will draw.</param>
        /// <param name="cameraWidth">The width of the backbuffer of the camera.</param>
        /// <param name="cameraHeight">The heifht of the backbuffer of the camera.</param>
        public ActiveBox(World activeBoxWorld, World targetWorld, int cameraWidth, int cameraHeight)
            :base(activeBoxWorld)
        {
            currentID++;
            ID = currentID;
            this.camera = new Camera(cameraWidth, cameraHeight, targetWorld);
            AssetTexture textureAsset = AssetManager.CreateAssetTexture("ActiveBoxText" + ID, camera.backbuffer);
            texture = new DrawableTexture(textureAsset, this);
        }
        public override void Draw(DrawState state)
        {
            camera.Draw(state);
        }
    }
}
