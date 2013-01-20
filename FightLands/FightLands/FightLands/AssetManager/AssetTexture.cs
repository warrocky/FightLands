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
            set { setContent(value); }
            get { return textureContent; }
        }

        Vector2 _center;
        public Vector2 center
        {
            get { return _center; }
        }

        public int width;
        public int height;

        public List<AssetTextureContentChangeHandler> contentChangedHandlers;

        public AssetTexture(Texture2D texture, String name)
        {
            this.name = name;
            contentChangedHandlers = new List<AssetTextureContentChangeHandler>();

            setContent(texture);
        }

        public void setContent(Texture2D newContent)
        {
            textureContent = newContent;
            _center = new Vector2(textureContent.Width, textureContent.Height)/2f;
            width = textureContent.Width;
            height = textureContent.Height;

            for (int i = 0; i < contentChangedHandlers.Count; i++)
                contentChangedHandlers[i].TextureContentChanged(this);
        }

        public AssetTexture createAssetCopy(String name)
        {
            Texture2D contentCopy = new Texture2D(textureContent.GraphicsDevice, textureContent.Width, textureContent.Height);

            Color[] content = new Color[textureContent.Width*textureContent.Height];
            textureContent.GetData<Color>(content);
            contentCopy.SetData<Color>(content);

            AssetTexture copy = new AssetTexture(contentCopy, name);
            return copy;
        }
    }

    interface AssetTextureContentChangeHandler
    {
        void TextureContentChanged(AssetTexture sender);
    }
}
