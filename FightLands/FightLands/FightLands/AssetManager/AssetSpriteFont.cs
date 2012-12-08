using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    class AssetSpriteFont
    {
        public readonly String name;
        public readonly SpriteFont font;

        public AssetSpriteFont(String label, SpriteFont font)
        {
            this.name = label;
            this.font = font;
        }
    }
}
