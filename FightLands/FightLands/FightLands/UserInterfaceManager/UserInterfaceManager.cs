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

        public static List<UserInterfaceArea> userInterfaceAreas = new List<UserInterfaceArea>();

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
        public static Vector2 getDiagonalOnWorld()
        {
            return baseCamera.diagonal;
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

        public static void createNewUserInterfaceArea(Vector2 size, Vector2 center, String name)
        {
            if(existsUserInterfaceArea(name))
                throw new Exception("The name: \"" + name + "\" is already in use in the userInterfaceManager");
 
            UserInterfaceArea newArea = new UserInterfaceArea(center, size, name);
            userInterfaceAreas.Add(newArea);
        }
        public static void createNewUserInterfaceArea(Rectangle rectangle, String name)
        {
            Vector2 center = new Vector2(rectangle.X + ((float)rectangle.Width)/2f, rectangle.Y + ((float)rectangle.Height)/2f);
            center += getCurrentUpperLeftCorner();

            createNewUserInterfaceArea(new Vector2(rectangle.Width,rectangle.Height),center, name);
        }
        public static bool existsUserInterfaceArea(String name)
        {
            for (int i = 0; i < userInterfaceAreas.Count; i++)
                if (userInterfaceAreas[i].name == name)
                    return true;

            return false;
        }
        public static UserInterfaceArea getUserInterfaceArea(String name)
        {
            for (int i = 0; i < userInterfaceAreas.Count; i++)
                if (name == userInterfaceAreas[i].name)
                    return userInterfaceAreas[i];

            throw new Exception("UserInterfaceArea : \"" + name + "\" not found.");
        }
    }
}
