using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands.GraphicsComponent
{
    public abstract class DrawCall
    {
        internal abstract void Render(RenderState state);
    }
}
