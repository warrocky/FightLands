﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    class WaterTile : GameObject
    {
        static int reduceFactor = 1;
        Land land;
        int seed;
        DrawableTextureStrip texture;
        AssetTextureStrip waterTexture;
        Vector2 size;

        bool textureLoaded;

        public WaterTile(Vector2 position, Vector2 size, Land land, int seed)
            : base(land)
        {
            this.position = position;
            this.land = land;
            this.seed = seed;

            waterTexture = AssetTextureStrip.createFromAssetTexture(AssetManager.getAssetTexture("whiteSquare"), "waterTile");
            texture = new DrawableTextureStrip(waterTexture, this);
            texture.size = size;
            texture.layer = 0.99f;
            this.size = size;

            textureLoaded = false;
        }
        public void loadTexture()
        {
            if (!textureLoaded)
            {
                Texture2D newContent = createTexture();
                waterTexture.setContent(newContent, new Point((int)size.X/reduceFactor, (int)size.Y/reduceFactor), AssetTextureStrip.StripOrientation.TopToBottom);

                //correct gaps
                //texture.size.X += 1f;
                texture.size.Y += 1f;

                textureLoaded = true;
            }
        }
        private Texture2D createTexture()
        {
            int frameCount = land.waterWavesFrameCount;

            int spriteWidth = (int)size.X/reduceFactor;
            int spriteHeight = (int)size.Y/reduceFactor;
            Texture2D texture = new Texture2D(Graphics.device, spriteWidth, spriteHeight*frameCount);
            Random rdm = new Random(seed);

            float[,] waterChanceData = land.waterChanceNoise.getValues(new Point(spriteWidth, spriteHeight), position - size / 2f, size);
            Vector3 noiseWaveOrigin = new Vector3(position.X - size.X / 2f, position.Y - size.Y / 2f, 0f);
            Vector3 noiseWaveArea = new Vector3(size.X, size.Y, (land.waterWavesLoop.Z + 1) * land.waterWavesDistancePeriod);
            float[, ,] waterWaveData = land.waterWaveValues;//land.waterWavesNoise.getValues(new Point3(texture.Width, texture.Height, frameCount), noiseWaveOrigin, noiseWaveArea);
            Point waterWaveDataLengths = new Point(waterWaveData.GetLength(0) ,waterWaveData.GetLength(1) );

            Point waterWaveDataOffset = new Point();
            waterWaveDataOffset.X = (((int)(position.X - size.X / 2)) % waterWaveDataLengths.X + waterWaveDataLengths.X) % waterWaveDataLengths.X;
            waterWaveDataOffset.Y = (((int)(position.Y - size.Y / 2)) % waterWaveDataLengths.Y + waterWaveDataLengths.Y) % waterWaveDataLengths.Y;

            Color[] colorArray = new Color[texture.Width * texture.Height];

            float waterChance = land.waterChanceTreshold;
            int x, y,k;
            Color floorColor;
            Color waveColor;
            float normalizedWaterChance;
            float rootedWaterChance;
            float rootedWaveValue;
            float curvedWaterChance;
            Point waterWaveCoordinates = new Point();
            float shore_ocean = 0.7f;
            float temp;
            Color[,] sandColor = new Color[spriteWidth, spriteHeight];
            for(int i=0;i<spriteWidth;i++)
                for (int j = 0; j < spriteHeight; j++)
                {
                    normalizedWaterChance = (waterChanceData[i, j] - waterChance) / (1f - waterChance);
                    rootedWaterChance = (float)Math.Sqrt(normalizedWaterChance);

                    temp = (float)rdm.NextDouble();
                    sandColor[i, j] = Color.Lerp(Color.Lerp(Color.LightYellow, Color.SaddleBrown, temp * temp), Color.Transparent, 1f - (rootedWaterChance) * (float)rdm.NextDouble());
                }
            for (int i = 0; i < texture.Width * texture.Height; i++)
            {
                x = i % texture.Width;
                y = (i / texture.Width)%spriteHeight;
                k = (i / texture.Width) / spriteHeight;

                if (waterChanceData[x, y] > waterChance)
                {

                    waterWaveCoordinates.X = ((waterWaveDataOffset.X + x*reduceFactor + k*2) % waterWaveDataLengths.X + waterWaveDataLengths.X) % waterWaveDataLengths.X;
                    waterWaveCoordinates.Y = ((waterWaveDataOffset.Y + y*reduceFactor) % waterWaveDataLengths.Y + waterWaveDataLengths.Y) % waterWaveDataLengths.Y;

                    normalizedWaterChance = (waterChanceData[x, y] - waterChance) / (1f - waterChance);
                    rootedWaterChance = (float)Math.Sqrt(normalizedWaterChance);
                    rootedWaveValue = (float)Math.Sqrt(MathHelper.SCurveInterpolation(0, 1, waterWaveData[waterWaveCoordinates.X, waterWaveCoordinates.Y, k]));
                    //curvedWaterChance = MathHelper.SCurveInterpolation(0, 1, rootedWaterChance);

                    //depth meter
                    if(rootedWaterChance < shore_ocean)
                        floorColor = Color.Lerp(sandColor[x,y], Color.Blue, (rootedWaterChance / shore_ocean)*(1f +  0.5f * waterWaveData[waterWaveCoordinates.X, waterWaveCoordinates.Y, k] * ((float)rdm.NextDouble()*0.5f + 0.5f)));
                    else
                        floorColor = Color.Lerp(Color.Blue, Color.Black, (rootedWaterChance - shore_ocean) / (1f - shore_ocean) * (1f + 0.2f * waterWaveData[waterWaveCoordinates.X, waterWaveCoordinates.Y, k] * ((float)rdm.NextDouble() * 0.5f + 0.5f)));

                    waveColor = Color.Lerp(floorColor, Color.Black, waterWaveData[waterWaveCoordinates.X, waterWaveCoordinates.Y, k]*rootedWaterChance - (float)rdm.NextDouble()*(1f - rootedWaterChance)*0.1f + 0.1f);

                    colorArray[i] = waveColor;//Color.Lerp(floorColor, waveColor, normalizedWaterChance);
                }
                else
                {
                    colorArray[i] = Color.Transparent;
                }
            }

            texture.SetData<Color>(colorArray);

            return texture;
        }
        public override void Update(UpdateState state)
        {
            texture.period = land.waterWavesTimePeriod;
            texture.Phase = land.waterWavesPhase;
            base.Update(state);
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