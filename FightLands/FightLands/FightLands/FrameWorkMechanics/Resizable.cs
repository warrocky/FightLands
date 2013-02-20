using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    interface Resizable
    {
        Vector2 getSize();
        void setSize(Vector2 newSize);
    }
}
