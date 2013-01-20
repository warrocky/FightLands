using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    class TerrainTile : GameObject
    {
        Land land;
        int seed;
        DrawableTexture texture;
        AssetTexture terrainTexture;
        Vector2 size;

        bool textureLoaded;

        public enum TerrainType { Grassland };

        public TerrainTile(TerrainType type, Vector2 position, Vector2 size, Land land, int seed)
            :base(land)
        {
            this.position = position;
            this.land = land;
            this.seed = seed;

            terrainTexture = AssetManager.getAssetTexture("whiteSquare").createAssetCopy("terrain");
            texture = new DrawableTexture(terrainTexture , this);
            texture.size = size;
            this.size = size;

            textureLoaded = false;
        }
        public void loadTexture()
        {
            if (!textureLoaded)
            {
                terrainTexture.setContent(createTexture());
                textureLoaded = true;
            }
        }
        private Texture2D createTexture()
        {
            Texture2D texture = new Texture2D(Graphics.device, (int)size.X,(int)size.Y);
            Random rdm = new Random(seed);

            float[,] noiseData = land.grassLandGreeness.getValues(new Point(texture.Width, texture.Height), position - size/2f, size);
            float[,] noiseDirtData = land.dirtPatches.getValues(new Point(texture.Width, texture.Height), position - size / 2f, size);

            Color[] colorArray = new Color[texture.Width*texture.Height];

            float dirtMergeStart = 0.5f;
            float dirtStart = 0.3f;

            int x, y;
            float grassRoughness, dirtRoughness, noise;
            Color grassColor;
            Color dirtColor;
            for(int i=0;i<texture.Width * texture.Height;i++)
            {
                x = i % texture.Width;
                y = i / texture.Width;

                grassRoughness = (((float)rdm.NextDouble() - 1f) * 0.3f)*(noiseData[x,y]);
                grassColor = Color.Lerp(Color.Lerp(Color.Black,Color.DarkGreen, (float)Math.Sqrt(noiseData[x,y])), Color.LawnGreen, noiseData[x, y] + grassRoughness - noiseDirtData[x,y]*0.3f);

                dirtRoughness = (((float)rdm.NextDouble() - 1f) * 0.3f);
                dirtColor = Color.Lerp(Color.Black,Color.Lerp(Color.SaddleBrown,Color.Black,0.5f),noiseDirtData[x,y] + dirtRoughness);

                noise = noiseData[x, y]*0.7f + noiseDirtData[x, y]*0.3f;

                if (noise < dirtMergeStart)
                    if (noise > dirtStart)
                        colorArray[i] = Color.Lerp(dirtColor, grassColor, (noise - dirtStart) / (dirtMergeStart - dirtStart));
                    else
                        colorArray[i] = dirtColor;
                else
                    colorArray[i] = grassColor;

                if (x == texture.Width/2|| y == texture.Height/2)
                    colorArray[i] = Color.Black;
            }

            texture.SetData<Color>(colorArray);

            return texture;
        }
        public override void Draw(DrawState state)
        {
            texture.Draw(state);
        }
    }
}
