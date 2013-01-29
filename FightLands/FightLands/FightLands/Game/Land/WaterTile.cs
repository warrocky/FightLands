using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    class WaterTile : GameObject
    {
        Land land;
        int seed;
        DrawableTexture texture;
        AssetTexture terrainTexture;
        Vector2 size;

        bool textureLoaded;

        public WaterTile(Vector2 position, Vector2 size, Land land, int seed)
            : base(land)
        {
            this.position = position;
            this.land = land;
            this.seed = seed;

            terrainTexture = AssetManager.getAssetTexture("whiteSquare").createAssetCopy("terrain");
            texture = new DrawableTexture(terrainTexture, this);
            texture.size = size;
            texture.layer = 1f;
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
            int frameCount = 25;

            int spriteWidth = (int)size.X;
            int spriteHeight = (int)size.Y;
            Texture2D texture = new Texture2D(Graphics.device, spriteWidth, spriteHeight*frameCount);
            Random rdm = new Random(seed);

            float[,] waterChanceData = land.waterChanceNoise.getValues(new Point(texture.Width, texture.Height), position - size / 2f, size);
            Vector3 noiseWaveOrigin = new Vector3(position.X - size.X / 2f, position.Y - size.Y / 2f, 0f);
            Vector3 noiseWaveArea = new Vector3(size.X, size.Y, (land.waterWavesNoise.periodLoop.Z + 1) * land.waterWavesNoise.period);
            float[, ,] waterWaveData = land.waterWavesNoise.getValues(new Point3(texture.Width, texture.Height, frameCount), noiseWaveOrigin, noiseWaveArea);

            Color[] colorArray = new Color[texture.Width * texture.Height];

            float waterChance = land.waterChanceTreshold;
            int x, y,k;
            Color floorColor;
            Color waveColor;
            float normalizedWaterChance;
            for (int i = 0; i < texture.Width * texture.Height; i++)
            {
                x = i % texture.Width;
                y = (i / texture.Width)%spriteHeight;
                k = (i / texture.Width) / spriteHeight;

                if (waterChanceData[x, y] > waterChance)
                {
                    normalizedWaterChance = (waterChanceData[x, y] - waterChance) / (1f - waterChance);
                    floorColor = Color.Lerp(Color.LightYellow, Color.Black, normalizedWaterChance);

                    waveColor = Color.Lerp(Color.LightSkyBlue, Color.DarkBlue, waterWaveData[x, y, k]);

                    colorArray[i] = Color.Lerp(floorColor,waveColor,normalizedWaterChance);
                }
                else
                {
                    colorArray[i] = Color.Transparent;
                }
            }

            texture.SetData<Color>(colorArray);

            return texture;
        }
        public override void Draw(DrawState state)
        {
            if (textureLoaded)
            {
                texture.Draw(state);
            }
        }
    }
}
