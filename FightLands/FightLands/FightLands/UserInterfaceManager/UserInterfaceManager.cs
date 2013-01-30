using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    static class UserInterfaceManager
    {
        public static World baseWorld;
        public static Camera baseCamera;

        public static void Initialize()
        {
            baseWorld = new World();
            baseCamera = new Camera(Graphics.resolution.X, Graphics.resolution.Y, baseWorld);

            UpdateManager.addUpdateRegister(new UpdateRegister(baseWorld, "UIBaseWorld"));
            Graphics.baseCamera = baseCamera;
        }
        public static Point getCurrentResolution()
        {
            return new Point(baseCamera.width, baseCamera.height);
        }
        public static Vector2 getCurrentUpperLeftCorner()
        {
            return baseCamera.position - baseCamera.diagonal / 2f;
        }
        public static Vector2 getCurrentLowerRightCorner()
        {
            return baseCamera.position + baseCamera.diagonal / 2f;
        }
        public static Vector2 getCurrentUpperRightCorner()
        {
            Vector2 diagonal = baseCamera.diagonal / 2f;
            diagonal.X = -diagonal.X;
            return baseCamera.position - baseCamera.diagonal / 2f;
        }
        public static Vector2 getCurrentLowerLeftCorner()
        {
            Vector2 diagonal = baseCamera.diagonal / 2f;
            diagonal.Y = -diagonal.Y;
            return baseCamera.position - baseCamera.diagonal / 2f;
        }
        public static Vector2 getDiagonalOnWorld()
        {
            return baseCamera.diagonal;
        }
    }
}
