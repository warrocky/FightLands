using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
