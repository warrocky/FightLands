using System;using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    class Tree : LandObject
    {
        int seed;
        DrawableTexture drawTexture;
        DrawableTexture shadowTexture;
        public float radius;
        float oscilation;

        public Tree(int seed,  float radius, Land world)
            :base(world)
        {
            Random rdm = new Random(seed);
            drawTexture = new DrawableTexture(world.getTextureForTree(radius),this);
            //texture.filter = Color.ForestGreen;
            drawTexture.size = Vector2.One * radius*2f;
            drawTexture.layer = 0.05f;
            drawTexture.rotation = (float)rdm.NextDouble() * 2f * (float)Math.PI;
            this.radius = radius;
            oscilation += (float)rdm.NextDouble()*(float)Math.PI*2f;

            shadowTexture = new DrawableTexture(world.getTextureForTree(radius), this);
            shadowTexture.size = Vector2.One * radius * 2f * 1.1f;
            shadowTexture.layer = 0.1f;
            shadowTexture.rotation = drawTexture.rotation;
            shadowTexture.filter = Color.Lerp(Color.Transparent,Color.Black, 0.9f);

            physicalProperties = new LandPhysicalProperties();
            physicalProperties.activelyColliding = false;
            physicalProperties.collisionType = LandCollisionTypes.Static;
            physicalProperties.radius = 2f;
        }

        public override void Draw(DrawState state)
        {
            shadowTexture.Draw(state);
            drawTexture.Draw(state);
        }

        public override void Update(UpdateState state)
        {
            oscilation += state.elapsedTime/4f;
            drawTexture.position.X = (float)Math.Cos(oscilation) * 2f;
            shadowTexture.position = drawTexture.position - new Vector2(1f, -1f)*(radius/2f + 1f)*0.3f;
        }


        public static Texture2D createTexture(int seed, float radius)
        {
            Random rdm = new Random(seed);
            Texture2D texture = new Texture2D(Graphics.device, (int)(radius * 2), (int)(radius * 2));
            Noise roughnessNoise = Noise.RegularNoise(20f, 1, rdm.Next());
            Noise fissureNoise = Noise.TurbulenceNoise(10f, 4, rdm.Next());
            roughnessNoise.filter = (float a, Vector2 b) => (a + 1) / 2f;
            fissureNoise.filter = (float a, Vector2 b) => (a + 1) / 2f;

            Color[] colorArray = new Color[texture.Width * texture.Height];

            float[,] diffuseValues = roughnessNoise.getValues(new Point(texture.Width, texture.Height), Vector2.Zero, new Vector2(radius, radius));
            float[,] angleDistortionValues = roughnessNoise.getValues(new Point(100, 100), Vector2.Zero - new Vector2(25,25), new Vector2(50, 50));
            float[,] fissureValues = fissureNoise.getValues(new Point(texture.Width, texture.Height), Vector2.Zero, new Vector2(radius, radius));

            float[] angleDistortions = new float[1000];
            float angle;
            Vector2 direction, center = new Vector2(texture.Width, texture.Height) / 2f;
            Vector2 distortionCenter = new Vector2(50f, 50f);
            for (int i = 0; i < 1000; i++)
            {
                angle = (float)(Math.PI * 2f / 1000f) * (float)i;
                direction = MathHelper.getDirectionFromAngle(angle);
                direction = distortionCenter + direction * 40f;
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
                //fissureColor = Color.Lerp(Color.DimGray, Color.Transparent, (float)(Math.Sqrt(fissureValues[x, y])) - (float)rdm.NextDouble() * 0.05f);
                displacement = new Vector2(x, y) - center;
                positionCenterRatio = (displacement.Length() / center.X);
                displacementS2 = displacement;
                if (displacementS2 != Vector2.Zero)
                {
                    displacementS2.Normalize();
                    positionCenterRatio *= 1f + 0.5f * angleDistortions[(int)(MathHelper.getAngleFromDirection(displacementS2) * 999f / (2f * Math.PI))];
                    //displacementS3 = displacementS2;
                    displacementS2 = (displacementS2) * (center.X * 0.4f) * positionCenterRatio * positionCenterRatio;
                    //displacementS3 = (displacementS3) * center.X * (float)Math.Sqrt(positionCenterRatio);
                }
                else
                {
                    displacementS2 = Vector2.Zero;
                    //displacementS3 = Vector2.Zero;
                }

                x = (int)MathHelper.Clamp(displacementS2.X * 3f + center.X, 0, texture.Width - 1);
                y = (int)MathHelper.Clamp(displacementS2.Y * 3f + center.Y, 0, texture.Height - 1);

                diffuseValue = diffuseValues[x, y];
                fissureValue = fissureValues[x, y];

                //background big feel
                mountainColor = Color.Lerp(Color.LawnGreen, Color.DarkGreen, 1f - (fissureValue + (1f - positionCenterRatio) * 0.5f + 0.2f));
                //mountainColor = Color.Lerp(mountainColor, Color.Black, (positionCenterRatio) * (((float)rdm.NextDouble()) * 0.3f + 0.3f) * (fissureValue + (1f - positionCenterRatio) * 0.5f + 0.2f) + (float)rdm.NextDouble() * 0.1f);
                //mountainColor = Color.Lerp(Color.Black, mountainColor, (1.1f - positionCenterRatio) * (fissureValue + (1f - positionCenterRatio) * 0.5f + 0.2f));

                //background feel
                //mountainColor = Color.Lerp(Color.LawnGreen, mountainColor, diffuseValue - (float)rdm.NextDouble() * 0.2f + positionCenterRatio * 0.3f + (1f - fissureValue) * 0.2f);

                //fissures
                //TODO: Lerp Black to Darkgreen using positionCenterRatio
                mountainColor = Color.Lerp(Color.Lerp(Color.DarkGreen,Color.Black,positionCenterRatio), Color.Lerp(Color.DarkGreen, mountainColor, fissureValue*(0.5f + (float)rdm.NextDouble()*0.5f)), 8*fissureValue*( - (float)rdm.NextDouble() * 0.1f + (1f - positionCenterRatio)*0.6f + 0.2f) + (float)rdm.NextDouble()*0.2f);

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
                    if (positionCenterRatio > 0.8f)
                        colorArray[i] = Color.Lerp(mountainColor, Color.Transparent, 1f - (1f - (positionCenterRatio - 0.8f) * 5f) * (1f - fissureValue + (float)rdm.NextDouble()));
                    else
                        colorArray[i] = mountainColor;
                else
                    colorArray[i] = Color.Transparent;


                //if (i % texture.Width == 0 || i % texture.Width == texture.Width - 1 || i / texture.Width == 0 || i / texture.Width == texture.Height - 1)
                //    colorArray[i] = Color.DarkGreen;
            }

            texture.SetData<Color>(colorArray);
            return texture;
        }

        public override bool AuthorizeCollision(LandObject collider)
        {
            return true;
        }
    }
}
