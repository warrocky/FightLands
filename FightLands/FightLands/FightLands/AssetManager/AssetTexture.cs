using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    class AssetTexture
    {
        public readonly String name;

        Texture2D textureContent;
        public Texture2D texture
        {
            get { return textureContent; }
        }

        Vector2 _center;
        public Vector2 center
        {
            get { return _center; }
        }




        public AssetTexture(Texture2D texture, String name)
        {
            this.name = name;
            this.textureContent = texture;
            _center = new Vector2(texture.Width, texture.Height)/2f;
        }
    }
}
