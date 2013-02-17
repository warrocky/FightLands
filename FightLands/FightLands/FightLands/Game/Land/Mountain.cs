using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    class Mountain : LandObject
    {
        int seed;
        public float radius;

        DrawableTexture texture;
        AssetTexture assetTexture;

        bool textureLoaded;

        public Mountain(int seed, Land world, float radius)
            :base(world)
        {
            this.seed = seed;
            assetTexture = AssetManager.getAssetTexture("whiteSquare").createAssetCopy("mountainTexture");
            texture = new DrawableTexture(assetTexture, this);

            Random rdm = new Random(seed);
            texture.rotation = (float)rdm.NextDouble() * (float)Math.PI * 2f;
            this.radius = radius;
            //radius = MathHelper.Clamp(MathHelper.getNextNormalDistributedFloat(3f, 1f, rdm) * 50f, 30f, 500f);

            texture.size = new Vector2(radius*2f, radius*2f);
            texture.layer = 0.07f;
            textureLoaded = false;
        }
        private Texture2D createTexture()
        {
            Random rdm = new Random(seed);
            Texture2D texture = new Texture2D(Graphics.device, (int)radius * 2, (int)radius * 2);
            Noise roughnessNoise = Noise.RegularNoise(20f, 1, rdm.Next());
            Noise fissureNoise = Noise.TurbulenceNoise(40f, 4, rdm.Next());
            roughnessNoise.filter = (float a, Vector2 b) => (a + 1)/2f;
            fissureNoise.filter = (float a, Vector2 b) => (a + 1) / 2f; 

            Color[] colorArray = new Color[texture.Width * texture.Height];

            float[,] diffuseValues = roughnessNoise.getValues(new Point(texture.Width, texture.Height), Vector2.Zero, new Vector2(radius, radius));
            float[,] angleDistortionValues = roughnessNoise.getValues(new Point(100, 100), Vector2.Zero, new Vector2(100, 100));
            float[,] fissureValues = fissureNoise.getValues(new Point(texture.Width, texture.Height), Vector2.Zero, new Vector2(radius, radius));

            float[] angleDistortions = new float[1000];
            float angle;
            Vector2 direction, center = new Vector2(texture.Width,texture.Height)/2f;
            Vector2 distortionCenter = new Vector2(50f, 50f);
            for (int i = 0; i < 1000; i++)
            {
                angle = (float)(Math.PI * 2f / 1000f) * (float)i;
                direction = MathHelper.getDirectionFromAngle(angle);
                direction = distortionCenter + direction * MathHelper.Clamp(radius, 40f,80f)/2f;
                angleDistortions[i] = Math.Abs(angleDistortionValues[(int)direction.X, (int)direction.Y]);
            }


            int x, y;
            center = new Vector2(texture.Width, texture.Height) / 2f;
            Color mountainColor;
            Vector2 displacement;
            Vector2 displacementS2;
            //Vector2 displacementS3;
            float positionCenterRatio;
            float fissureValue, diffuseValue;
            Color fissureColor;
            for (int i = 0; i < texture.Width * texture.Height; i++)
            {
                x = i % texture.Width;
                y = i / texture.Width;
                fissureColor = Color.Lerp(Color.DimGray, Color.Transparent, (float)(Math.Sqrt(fissureValues[x,y])) - (float)rdm.NextDouble()*0.05f);
                displacement = new Vector2(x,y) - center;
                positionCenterRatio = (displacement.Length() / center.X);
                displacementS2 = displacement;
                if (displacementS2 != Vector2.Zero)
                {
                    displacementS2.Normalize();
                    positionCenterRatio *= 1f + 0.5f*angleDistortions[(int)(MathHelper.getAngleFromDirection(displacementS2)*999f/(2f*Math.PI))];
                    //displacementS3 = displacementS2;
                    displacementS2 = (displacementS2) * (center.X*0.4f) * positionCenterRatio * positionCenterRatio;
                    //displacementS3 = (displacementS3) * center.X * (float)Math.Sqrt(positionCenterRatio);
                }
                else
                {
                    displacementS2 = Vector2.Zero;
                    //displacementS3 = Vector2.Zero;
                }

                x = (int)MathHelper.Clamp(displacementS2.X*3f + center.X,0,texture.Width - 1);
                y = (int)MathHelper.Clamp(displacementS2.Y*3f + center.Y,0,texture.Height - 1);

                diffuseValue = diffuseValues[x, y];
                fissureValue = fissureValues[x, y];

                //background big feel
                mountainColor = Color.Lerp(Color.DimGray, Color.ForestGreen, 1f - (fissureValue + (1f - positionCenterRatio) * 0.5f + 0.2f));

                //background feel
                mountainColor = Color.Lerp(Color.White, mountainColor, diffuseValue - (float)rdm.NextDouble()*0.2f + positionCenterRatio*0.3f + (1f - fissureValue)*0.2f);

                //fissures
                mountainColor = Color.Lerp(Color.Black, mountainColor, 2f * fissureValue*(1f - positionCenterRatio*positionCenterRatio) - (float)rdm.NextDouble() * 0.2f + (1f - positionCenterRatio) * 0.5f + 0.2f);

                //outer fissure
                //fissureColor = Color.Lerp(fissureColor, Color.Transparent, (positionCenterRatio - 0.8f)*5f);
                //if (positionCenterRatio > 0.7f)
                //    if (positionCenterRatio < 0.9f)
                //        mountainColor = Color.Lerp(mountainColor, fissureColor, (positionCenterRatio + (float)rdm.NextDouble()*0.05f - 0.7f) * 4f);
                //    else
                //        mountainColor = fissureColor;

                //mountainColor = Color.Lerp(mountainColor, Color.White, (float)Math.Sqrt(positionCenterRatio));

                //snow
                //x = MathHelper.Clamp(displacementS3.X + center.X, 0, texture.Width - 1);
                //y = MathHelper.Clamp(displacementS3.Y + center.Y, 0, texture.Height - 1);

                //mountainColor = Color.Lerp(mountainColor, Color.White, (1f - 6f * values[(int)y, (int)x] * positionCenterRatio * positionCenterRatio));

                ////outer body
                ////mountainColor = Color.Lerp(Color.Black, mountainColor, (1f - positionCenterRatio+0.5f));

                //x = MathHelper.Clamp(displacementS2.X * 3f + center.X, 0, texture.Width - 1);
                //y = MathHelper.Clamp(displacementS2.Y * 3f + center.Y, 0, texture.Height - 1);

                if (positionCenterRatio < 1f)
                    if(positionCenterRatio > 0.99f)
                        colorArray[i] = Color.Lerp(mountainColor, Color.Transparent, 1f - (1f - (positionCenterRatio-0.99f)*100f)*(1f - fissureValue - (float)rdm.NextDouble()*0.3f));
                    else
                        colorArray[i] = mountainColor;
                else
                    colorArray[i] = Color.Transparent;

                //if (i % texture.Width == 0 || i % texture.Width == texture.Width - 1 || i / texture.Width == 0 || i / texture.Width == texture.Height - 1)
                //    colorArray[i] = Color.Blue;
            }

            texture.SetData<Color>(colorArray);
            return texture;
        }

        public void loadTexture()
        {
            if (!textureLoaded)
            {
                assetTexture.setContent(createTexture());
                textureLoaded = true;
            }
        }

        public override void Draw(DrawState state)
        {
            texture.Draw(state);
        }
    }
}
