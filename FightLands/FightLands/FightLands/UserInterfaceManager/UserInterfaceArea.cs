using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class UserInterfaceArea
    {
        public readonly String name;

        Vector2 center;
        Vector2 size;

        public UserInterfaceArea(Vector2 center, Vector2 size, String name)
        {
            this.center = center;
            this.size = size;
            this.name = name;
        }
        public Vector2 getCenter()
        {
            return center;
        }
        public Vector2 getSize()
        {
            return size;
        }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)(center.X - size.X / 2f), (int)(center.Y + size.Y / 2f), (int)size.X, (int)size.Y);
        }

        public Vector2 getUpperLeftCorner()
        {
            return center + new Vector2(-size.X, size.Y)/2f;
        }
        public Vector2 getUpperRightCorner()
        {
            return center + new Vector2(size.X, size.Y) / 2f;
        }
        public Vector2 getLowerLeftCorner()
        {
            return center + new Vector2(-size.X, -size.Y) / 2f;
        }
        public Vector2 getLowerRightCorner()
        {
            return center + new Vector2(size.X, -size.Y) / 2f;
        }
    }
}
