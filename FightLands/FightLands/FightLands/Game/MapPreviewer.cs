using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FightLands
{
    class MapPreviewer : GameObject
    {
        DrawableTexture minimap;

        public MapPreviewer(World world, int seed)
            :base(world)
        {
            Texture2D textureBase = getMinimapTexture(seed);

            AssetTexture text = new AssetTexture(textureBase, "minimapTexture");

            minimap = new DrawableTexture(text, this);
        }

        public Texture2D getMinimapTexture(int seed)
        {
            Random rdm = new Random(seed);

            int width, height;
            width = 400;
            height = 400;

            Texture2D text = new Texture2D(Graphics.device, width, height);

            Color[] colors = new Color[width * height];

            float[,] values = new float[width,height];
            float[,] forestChance = new float[width, height];
            float[,] waterChance = new float[width, height];
            int[,] terrain = new int[width, height];

            Noise a = Noise.TurbulenceNoise(1f, 4, rdm.Next());
            Noise forest = Noise.TurbulenceNoise(1f, 4, rdm.Next());
            Noise water = Noise.RegularNoise(0.5f, 5, rdm.Next());

            values = a.getValues(new Point(width, height), Vector2.Zero, new Vector2(2, 2));
            forestChance = forest.getValues(new Point(width, height), Vector2.Zero, new Vector2(9, 9));
            waterChance = water.getValues(new Point(width, height), Vector2.Zero, new Vector2(1, 1));

            Vector2 center = new Vector2(width/2f, height / 2f);
            Vector2 pos = new Vector2();
            float distx, disty;
            float dist;
            for(int i=0; i<values.GetLength(0);i++)
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    pos.X = i;
                    pos.Y = j;

                    //distx = Math.Abs((pos - center).X)/100f;
                    //disty = Math.Abs((pos - center).Y)/100f;
                    //if (distx >= disty)
                    //    dist = distx;
                    //else
                    //    dist = disty;

                    dist = (pos - center).Length() / (width/2f);

                    dist = dist*dist;
                    values[i, j] = ((values[i,j]/2f+0.5f)*(dist) + (1f - dist));
                    forestChance[i, j] = (1f - forestChance[i,j])*((float)Math.Sqrt(dist));

                    if (values[i, j] < 0.4f)
                        terrain[i, j] = 0;
                    else
                        if (values[i, j] < 0.8f && forestChance[i, j] > 0.8f)
                                terrain[i, j] = 1;
                        else
                            if (values[i, j] > 0.9f && waterChance[i,j] > 0.01f)
                                if(values[i,j] < 0.905f || waterChance[i,j] < 0.016f)
                                    terrain[i,j] = 4;
                                else
                                    terrain[i, j] = 3;
                            else
                                terrain[i, j] = 2;
                }

            for (int i = 0; i < colors.Length; i++)
            {
                switch (terrain[i % width, i / height])
                {
                    case 0:
                        colors[i] = Color.DarkGray;
                        break;
                    case 1:
                        colors[i] = Color.ForestGreen;
                        break;
                    case 2:
                        colors[i] = Color.LawnGreen;
                        break;
                    case 3:
                        colors[i] = Color.DeepSkyBlue;
                        break;
                    case 4:
                        colors[i] = Color.LightYellow;
                        break;
                    default:
                        break;
                }
            }

            text.SetData<Color>(colors);
            return text;
        }
        protected static float NGInterp(float a, float b, float x)
        {
            float weight = (x * x) * (3 - 2 * x); //s3
            //float weight =  x; // linear
            //float weight =  (float)(1f - Math.Cos(x*Math.PI)) * 0.5f; // cosine
            return a * (1 - weight) + b * weight;
        }
        protected static Vector2 randVect(Random rdm)
        {
            double alpha = rdm.NextDouble() * 2 * Math.PI;
            return new Vector2((float)Math.Cos(alpha), (float)Math.Sin(alpha));
        }

        public override void Draw(DrawState state)
        {
            minimap.Draw(state);
        }
    }
}
