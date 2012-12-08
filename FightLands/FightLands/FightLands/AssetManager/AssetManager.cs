using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    /// <summary>
    /// A class representing a static resource responsible for content handling and labeling.
    /// </summary>
    static class AssetManager
    {
        static ContentManager content;

        static List<AssetTexture> textures;
        static List<AssetTextureStrip> textureStrips;
        static List<AssetSpriteFont> fonts;

        /// <summary>
        /// Method responsible for Initializing the AssetManager (called by Game1.cs preferably).
        /// </summary>
        /// <param name="_content">The ContentManager to be used in the AssetManager</param>
        public static void Initialize(ContentManager _content)
        {
            content = _content;
            textures = new List<AssetTexture>();
            textureStrips = new List<AssetTextureStrip>();
            fonts = new List<AssetSpriteFont>();
        }

        /// <summary>
        /// Method that creates an AssetTexture from a texture project file and registers it on the AssetManager.
        /// </summary>
        /// <param name="label">The label the asset is to be given.</param>
        /// <param name="fileName">The filename to get the content associated with the asset.</param>
        /// <returns>A pointer to the created asset.</returns>
        public static AssetTexture CreateAssetTexture(String label, String fileName)
        {
            Texture2D texture = content.Load<Texture2D>(fileName);
            AssetTexture asset = new AssetTexture(texture, label);
            textures.Add(asset);
            return asset;
        }
        /// <summary>
        /// Method that creates an AssetTexture from a Texture2D type and registers it on the AssetManager.
        /// </summary>
        /// <param name="label">The label the asset is to be given.</param>
        /// <param name="texture">The texture content of the asset.</param>
        /// <returns>A pointer to the created asset.</returns>
        public static AssetTexture CreateAssetTexture(String label, Texture2D texture)
        {
            AssetTexture asset = new AssetTexture(texture, label);
            textures.Add(asset);
            return asset;
        }
        /// <summary>
        /// A method to retrieve a AssetTexture stored in the AssetManager.
        /// </summary>
        /// <param name="label">The label associated with the AssetTexture.</param>
        /// <returns>The AssetTexture in the AssetManager with the requested label or throws an Exception if there isn't such a match.</returns>
        public static AssetTexture getAssetTexture(String label)
        {
            for (int i = 0; i < textures.Count; i++)
                if (textures[i].name == label)
                    return textures[i];

            throw new Exception("No such AssetTexture in the AssetManager, label: \"" + label + "\".");
        }
        /// <summary>
        /// A method that checks if there is a AssetTexture registered in the AssetManager with the specified label.
        /// </summary>
        /// <param name="label">The label to look for.</param>
        /// <returns>True if an AssetTexture exists with the specified label, false otherwise.</returns>
        public static bool checkIfAssetTextureExists(String label)
        {
            for (int i = 0; i < textures.Count; i++)
                if (textures[i].name == label)
                    return true;

            return false;
        }

        /// <summary>
        /// A method that creates an AssetTextureStrip from a texture project file and registers it on the AssetManager.
        /// </summary>
        /// <param name="label">The label the asset is to be given.</param>
        /// <param name="fileName">The filename to get the content associated with the asset. </param>
        /// <param name="frameSize">The size in (width,length) of a single frame in the strip.</param>
        /// <param name="orientation">The orientation the content is to be loaded in.</param>
        public static void CreateAssetTextureStrip(String label, String fileName, Point frameSize, AssetTextureStrip.StripOrientation orientation)
        {
            Texture2D texture = content.Load<Texture2D>(fileName);
            textureStrips.Add(new AssetTextureStrip(label, texture, orientation, frameSize));
        }
        /// <summary>
        /// A method to retrieve an AssetTextureStrip from the AssetManager.
        /// </summary>
        /// <param name="label">The label associated with the AssetTextureStrip to retrieve.</param>
        /// <returns>The AssetTextureStrip in the AssetManager with the requested label or throwns an Exception if there isn't such a match.</returns>
        public static AssetTextureStrip getAssetTextureStrip(String label)
        {
            for (int i = 0; i < textureStrips.Count; i++)
                if (textureStrips[i].name == label)
                    return textureStrips[i];

            throw new Exception("No such AssetTextureStrip in the AssetManager, label: \"" + label + "\".");
        }

        /// <summary>
        /// A method that creates an AssetSpriteFont from a spritefont project file and registers it on the AssetManager.
        /// </summary>
        /// <param name="label">The label the asset is to be given.</param>
        /// <param name="fileName">The filename of the spritefont file in the project this asset will be associated with.</param>
        /// <returns>A pointer to the created asset.</returns>
        public static AssetSpriteFont CreateAssetSpriteFont(String label, String fileName)
        {
            SpriteFont font = content.Load<SpriteFont>(fileName);
            AssetSpriteFont asset = new AssetSpriteFont(label, font);
            fonts.Add(asset);
            return asset;
        }
        /// <summary>
        /// A method to retrieve and AssetSpriteFont from the AssetManager.
        /// </summary>
        /// <param name="label">The label associated with the AssetSpriteFont to retrieve.</param>
        /// <returns>The AssetSpriteFont in the AssetManager with the requested label or throws an Exception if there isn't such a match.</returns>
        public static AssetSpriteFont getAssetSpriteFont(String label)
        {
            for (int i = 0; i < fonts.Count; i++)
                if (fonts[i].name == label)
                    return fonts[i];

            throw new Exception("No such AssetSpriteFont in the AssetManager, label: \"" + label + "\".");
        }
    }
}
