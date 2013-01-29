using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FightLands
{
    abstract class Noise
    {
        public noiseFilterFuncton filter;

        public Noise()
        {
            filter = (float a, Vector2 b) => a;
        }
        public abstract float[,] getValues(float[,] values, noiseSumFunction sumFunc, Point samples, Vector2 origin, Vector2 area);

        public float[,] getValues(Point samples, Vector2 origin, Vector2 area)
        {
            float[,] values = new float[samples.X, samples.Y];
            return getValues(values, OverrideSum, samples, origin, area);
        }
        public abstract float getValueAt(Vector2 position);

        public delegate float noiseFilterFuncton(float noiseValue, Vector2 position);
        public delegate float noiseSumFunction(float arrayValue, float noiseValue);

        public static float IdentityFilter(float noiseValue, Vector2 position)
        {
            return noiseValue;
        }
        public static float AbsFilter(float noiseValue, Vector2 position)
        {
            return Math.Abs(noiseValue);
        }

        public static float OverrideSum(float arrayValue, float noiseValue)
        {
            return noiseValue;
        }

        /// <summary>
        /// Method that returns a Turbulence noise with the speficied parameters that has a customizable filter.
        /// </summary>
        /// <param name="period">Size of the noise.</param>
        /// <param name="steps">Detail of the noise</param>
        /// <param name="seed">The seed used</param>
        /// <returns>A SumNoise with several noises.</returns>
        public static Noise TurbulenceNoise(float period, int steps, int seed)
        {
            SumNoise sumNoise = new SumNoise();
            Random rdm = new Random(seed);

            BasicGradientNoise noise;

            float[] weights = new float[steps];
            weights[0] = 0.5f;
            float weightSum = 0.5f;

            for (int i = 1; i < steps; i++)
            {
                weights[i] = weights[i - 1] * 0.5f;
                weightSum += weights[i];
            }

            noiseSumFunction simpleSum = (float a, float b) => a + b;
            noiseSumFunction lastFunc = (float a, float b) => ((a + b) - 0.5f) * 2f;
            //float weight;
            for (int i = 0; i < steps; i++)
            {
                noise = new BasicGradientNoise(rdm.Next(), period);
                float weight = weights[i] + (weights[i] / weightSum) * (1f - weightSum);

                noise.filter = (float value, Vector2 position) => Math.Abs(value) * weight;
                //noise.filter = (float value, Vector2 position) => (Math.Abs(value) - 0.5f)*2f*weight;

                if (i != steps - 1)
                {
                    sumNoise.addNoise(noise, (float a, float b) => a + b);
                }
                else
                {
                    sumNoise.addNoise(noise, (float a, float b) => ((a + b) - 0.5f) * 2f);
                }



                period *= 0.5f;
            }

            //sumNoise.filter = (float a, Vector2 b) => { return (a + 1f) / 2f; };
            sumNoise.filter = (float a, Vector2 b) => a;
            return sumNoise;
        }
        public static Noise RegularNoise(float period, int steps, int seed)
        {
            if (steps == 0)
                throw new Exception("Can't create a noise regular noise with 0 steps");

            SumNoise sumNoise = new SumNoise();
            Random rdm = new Random(seed);

            BasicGradientNoise noise;

            float[] weights = new float[steps];
            weights[0] = 0.5f;
            float weightSum = 0.5f;

            for (int i = 1; i < steps; i++)
            {
                weights[i] = weights[i - 1] * 0.5f;
                weightSum += weights[i];
            }

            float weight;
            noiseSumFunction simpleSum = (float a, float b) => { return a + b; };
            for (int j = 0; j < steps; j++)
            {
                if (j < steps)
                {
                    noise = new BasicGradientNoise(rdm.Next(), period);
                    weight = weights[j] + (weights[j] / weightSum) * (1f - weightSum);
                    noise.filter = (float value, Vector2 position) => { return value * (weight); };

                    sumNoise.addNoise(noise, simpleSum);


                    period *= 0.5f;
                }
            }

            return sumNoise;
        }
        public static Noise MixedNoise(float period, int BasicRandomNoiseSteps, int BasicGradientNoiseSteps, int seed)
        {
            SumNoise sumNoise = new SumNoise();
            Random rdm = new Random(seed);

            Noise noise;

            int steps = BasicRandomNoiseSteps + BasicGradientNoiseSteps;
            float[] weights = new float[steps];
            weights[0] = 0.5f;
            float weightSum = 0.5f;

            for (int i = 1; i < steps; i++)
            {
                weights[i] = weights[i - 1] * 0.5f;
                weightSum += weights[i];
            }


            for (int i = 0; i < steps; i++)
            {
                float weight = weights[i] + (weights[i] / weightSum) * (1f - weightSum);

                if (i < BasicRandomNoiseSteps)
                {
                    noise = new BasicRandomNoise(rdm.Next(), period);
                    noise.filter = (float value, Vector2 position) => value * weight;
                }
                else
                {
                    noise = new BasicGradientNoise(rdm.Next(), period);
                    noise.filter = (float value, Vector2 position) => (Math.Abs(value) - 0.5f) * 2f * weight;
                }
                sumNoise.addNoise(noise, (float a, float b) => a + b);


                period *= 0.5f;
            }


            return sumNoise;
        }

        //public static float[,] BasicGradientNoise(int width, int height, int frequency, int seed)
        //{
        //    float[,] values = new float[width, height];
        //    Vector2[,] pivots = new Vector2[frequency + 1, frequency + 1];

        //    Random rdm = new Random(seed);
        //    for (int i = 0; i < pivots.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < pivots.GetLength(1); j++)
        //        {
        //            pivots[i, j] = randVect(rdm);
        //        }
        //    }

        //    for (int i = 0; i < width - 1; i++)
        //    {
        //        for (int j = 0; j < height - 1; j++)
        //        {
        //            float x = (float)(i * frequency) / (float)(width - 1);
        //            float y = (float)(j * frequency) / (float)(height - 1);

        //            float xdif = x - (int)x;
        //            float ydif = y - (int)y;
        //            float dleft = Vector2.Dot(pivots[(int)x, (int)y], new Vector2(xdif, ydif));
        //            float dright = Vector2.Dot(pivots[(int)x + 1, (int)y], new Vector2(xdif - 1f, ydif));
        //            float uleft = Vector2.Dot(pivots[(int)x, (int)y + 1], new Vector2(xdif, ydif - 1f));
        //            float uright = Vector2.Dot(pivots[(int)x + 1, (int)y + 1], new Vector2(xdif - 1f, ydif - 1f));
        //            float normal = NGInterp(dleft, dright, xdif);
        //            float upper =  NGInterp(uleft, uright, xdif);
        //            float v = NGInterp(normal,upper, ydif);
        //            values[i, j] =  dipole(v, 2);
        //        }
        //    }
        //    for (int i = 0; i < height - 1; i++)
        //    {
        //        float y = (float)(i * frequency) / (float)(height - 1);
        //        float dif = y - (int)y;
        //        values[width - 1, i] = NGInterp(pivots[frequency, (int)y].Y*dif,pivots[frequency, (int)y + 1].Y * (dif - 1), dif);
        //    }
        //    for (int i = 0; i < width - 1; i++)
        //    {
        //        float x = (float)(i * frequency) / (float)(width - 1);
        //        float dif = x - (int)x;
        //        values[i, height - 1] = NGInterp(pivots[(int)x, frequency].X*dif, pivots[(int)x + 1, frequency].X * (dif-1f),dif);
        //    }
        //    values[width - 1, height - 1] = 0f;

        //    return values;
        //}
        //public static float[,] BasicRandomNoise(int width, int height, int frequency, int seed)
        //{
        //    float[,] values = new float[width, height];
        //    float[,] pivots = new float[frequency + 1, frequency + 1];

        //    Random rdm = new Random(seed);
        //    for (int i = 0; i < pivots.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < pivots.GetLength(1); j++)
        //        {
        //            pivots[i, j] = (float)rdm.NextDouble();
        //        }
        //    }

        //    for (int i = 0; i < width - 1; i++)
        //    {
        //        for (int j = 0; j < height - 1; j++)
        //        {
        //            float x = (float)(i * frequency) / (float)(width - 1);
        //            float y = (float)(j * frequency) / (float)(height - 1);

        //            float xdif = x - (int)x;
        //            float ydif = y - (int)y;
        //            float dleft = pivots[(int)x, (int)y];
        //            float dright = pivots[(int)x + 1, (int)y];
        //            float uleft = pivots[(int)x, (int)y + 1];
        //            float uright = pivots[(int)x + 1, (int)y + 1];
        //            float normal = NGInterp(dleft, dright, xdif);
        //            float upper = NGInterp(uleft, uright, xdif);
        //            float v = NGInterp(normal, upper, ydif);
        //        }
        //    }
        //    for (int i = 0; i < height - 1; i++)
        //    {
        //        float y = (float)(i * frequency) / (float)(height - 1);
        //        float dif = y - (int)y;
        //        values[width - 1, i] = NGInterp(pivots[frequency, (int)y], pivots[frequency, (int)y + 1], dif);
        //    }
        //    for (int i = 0; i < width - 1; i++)
        //    {
        //        float x = (float)(i * frequency) / (float)(width - 1);
        //        float dif = x - (int)x;
        //        values[i, height - 1] = NGInterp(pivots[(int)x, frequency], pivots[(int)x + 1, frequency], dif);
        //    }
        //    values[width - 1, height - 1] = pivots[pivots.GetLength(0) - 1, pivots.GetLength(1) - 1];

        //    return values;
        //}
        //public static float[,] RegularNoise(int width, int height, int frequency, int seed, int steps)
        //{
        //    float[,] values = new float[width,height];
        //    List<float[,]> noises = new List<float[,]>();
        //    Random rdm = new Random(seed);
        //    int[] feqs = new int[steps];
        //    feqs[0] = frequency;
        //    for (int i = 1; i < steps; i++)
        //    {
        //        feqs[i] = feqs[i - 1] * 2;
        //    }

        //    float[] weights = new float[steps];
        //    float tt = 1f;
        //    for(int i=steps-1;i > 0;i--)
        //    {
        //        weights[i] = (float)Math.Pow(0.5f, i+1);
        //        tt -= weights[i];
        //    }
        //    weights[0] = tt;

        //    for (int i = 0; i < steps; i++)
        //    {
        //        noises.Add(BasicGradientNoise(width,height,feqs[i],rdm.Next()));
        //    }

        //    for (int i = 0; i < width; i++)
        //    {
        //        for (int j = 0; j < height; j++)
        //        {
        //            values[i, j] = 0;
        //            for (int k = 0; k < steps; k++)
        //            {
        //                values[i, j] += weights[k] * noises[k][i, j];
        //            }
        //        }
        //    }
        //    changeBias(values, new Vector2(-1f, 1f), new Vector2(0f, 1f));
        //    return values;
        //}
        //public static float[,] TurbulenceNoise(int width, int height, int frequency, int seed, int steps)
        //{
        //    float[,] values = new float[width,height];
        //    List<float[,]> noises = new List<float[,]>();
        //    Random rdm = new Random(seed);
        //    int[] feqs = new int[steps];
        //    feqs[0] = frequency;
        //    for (int i = 1; i < steps; i++)
        //    {
        //        feqs[i] = feqs[i - 1] * 2;
        //    }

        //    float[] weights = new float[steps];
        //    float tt = 1f;
        //    for(int i=steps-1;i > 0;i--)
        //    {
        //        weights[i] = (float)Math.Pow(0.5f, i+1);
        //        tt -= weights[i];
        //    }
        //    weights[0] = tt;

        //    for (int i = 0; i < steps; i++)
        //    {
        //        noises.Add(BasicGradientNoise(width,height,feqs[i],rdm.Next()));
        //    }

        //    for (int i = 0; i < width; i++)
        //    {
        //        for (int j = 0; j < height; j++)
        //        {
        //            values[i, j] = 0;
        //            for (int k = 0; k < steps; k++)
        //            {
        //                values[i, j] += weights[k] * (float)Math.Abs(noises[k][i, j]);
        //            }
        //        }
        //    }

        //    return values;
        //}
        //public static float[,] MixedNoise(int width, int height, int frequency, int seed, int steps)
        //{
        //    float[,] values = new float[width, height];
        //    List<float[,]> noises = new List<float[,]>();
        //    Random rdm = new Random(seed);
        //    int[] feqs = new int[steps];
        //    feqs[0] = frequency;
        //    for (int i = 1; i < steps; i++)
        //    {
        //        feqs[i] = feqs[i - 1] * 2;
        //    }

        //    float[] weights = new float[steps];
        //    float tt = 1f;
        //    for (int i = steps - 1; i > 0; i--)
        //    {
        //        weights[i] = (float)Math.Pow(0.5f, i + 1);
        //        tt -= weights[i];
        //    }
        //    weights[0] = tt;

        //    noises.Add(BasicRandomNoise(width,height,frequency, rdm.Next()));
        //    for (int i = 1; i < steps; i++)
        //    {
        //        noises.Add(BasicGradientNoise(width, height, feqs[i], rdm.Next()));
        //    }

        //    for (int i = 0; i < width; i++)
        //    {
        //        for (int j = 0; j < height; j++)
        //        {
        //            values[i, j] = 0;
        //            for (int k = 0; k < steps; k++)
        //            {
        //                values[i, j] += weights[k] * (float)Math.Abs(noises[k][i, j]);
        //            }
        //        }
        //    }

        //    return values;
        //}


        public static void changeBias(float[,] array, Vector2 originalBias, Vector2 newBias)
        {
            float scale = (newBias.Y - newBias.X) / (originalBias.Y - originalBias.X);

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] -= originalBias.X;
                    array[i, j] *= scale;
                    array[i, j] += newBias.X;
                }
            }
        }



        protected static Vector2 randVect(Random rdm)
        {
            double alpha = rdm.NextDouble() * 2 * Math.PI * 4;
            return new Vector2((float)Math.Cos(alpha), (float)Math.Sin(alpha));
        }
        protected static float NGInterp(float a, float b, float x)
        {
            float weight = (x * x) * (3 - 2 * x); //s3
            //float weight =  x; // linear
            //float weight =  (float)(1f - Math.Cos(x*Math.PI)) * 0.5f; // cosine
            return a * (1 - weight) + b * weight;
        }
        protected static float dipole(float value, int steps)
        {
            double v = value;
            for (int i = 0; i < steps; i++)
                v = Math.Cos((v + 1) * Math.PI / 2f);

            return (float)v;
        }
    }


    class BasicGradientNoise : Noise
    {
        int seed;
        float period;
        public BasicGradientNoise(int seed, float period)
        {
            this.seed = seed;
            this.period = period;
        }
        public override float[,] getValues(float[,] values, noiseSumFunction sumFunc, Point samples, Vector2 origin, Vector2 area)
        {
            origin *= 1f / period;
            area *= 1f / period;

            if (values.GetLength(0) != samples.X || values.GetLength(1) != samples.Y)
                throw new Exception("Invalid Array to sum To.");

            Rectangle gradientSamples = new Rectangle();
            gradientSamples.X = (int)Math.Floor(origin.X);
            gradientSamples.Y = (int)Math.Floor(origin.Y);
            gradientSamples.Width = (int)Math.Floor(origin.X + area.X) - gradientSamples.X + 1;
            gradientSamples.Height = (int)Math.Floor(origin.Y + area.Y) - gradientSamples.Y + 1;

            Vector2[,] gradientVectors = new Vector2[gradientSamples.Width + 1, gradientSamples.Height + 1];
            for (int i = 0; i < gradientSamples.Width + 1; i++)
                for (int j = 0; j < gradientSamples.Height + 1; j++)
                {
                    gradientVectors[i, j] = getNodeValue(gradientSamples.X + i, gradientSamples.Y + j);
                }


            float xDif, yDif;
            int p1, p2;
            float xStep = area.X / (samples.X - 1);
            float yStep = area.Y / (samples.Y - 1);
            float ll, lr, ur, ul, upper, lower;
            Vector2 pos = new Vector2();
            double val;
            for (int i = 0; i < samples.X; i++)
                for (int j = 0; j < samples.Y; j++)
                {
                    pos.X = origin.X + i * xStep;
                    pos.Y = origin.Y + j * yStep;
                    p1 = (int)Math.Floor(pos.X);
                    p2 = (int)Math.Floor(pos.Y);
                    xDif = pos.X - (float)p1;
                    yDif = pos.Y - (float)p2;
                    p1 = p1 - gradientSamples.X;
                    p2 = p2 - gradientSamples.Y;
                    pos *= period;

                    ll = Vector2.Dot(gradientVectors[p1, p2], new Vector2(xDif, yDif));
                    lr = Vector2.Dot(gradientVectors[p1 + 1, p2], new Vector2(xDif - 1f, yDif));
                    ur = Vector2.Dot(gradientVectors[p1 + 1, p2 + 1], new Vector2(xDif - 1f, yDif - 1f));
                    ul = Vector2.Dot(gradientVectors[p1, p2 + 1], new Vector2(xDif, yDif - 1f));

                    upper = NGInterp(ul, ur, xDif);
                    lower = NGInterp(ll, lr, xDif);
                    val = NGInterp(lower, upper, yDif);
                    val = Math.Cos((val + 1) * Math.PI / 2);
                    //val = Math.Cos((val + 1) * Math.PI / 2);
                    values[i, j] = sumFunc(values[i, j], filter((float)val, pos));
                }

            return values;
        }
        public override float getValueAt(Vector2 position)
        {
            position *= 1f / period;
            float llf, lrf, urf, ulf;
            int x, y;
            x = (int)Math.Floor(position.X);
            y = (int)Math.Floor(position.Y);

            float xdif = position.X - (float)x;
            float ydif = position.Y - (float)y;

            llf = Vector2.Dot(getNodeValue(x, y), new Vector2(xdif, ydif));
            lrf = Vector2.Dot(getNodeValue(x + 1, y), new Vector2(xdif - 1f, ydif));
            urf = Vector2.Dot(getNodeValue(x + 1, y + 1), new Vector2(xdif - 1f, ydif - 1f));
            ulf = Vector2.Dot(getNodeValue(x, y + 1), new Vector2(xdif, ydif - 1f));

            float lower = NGInterp(llf, lrf, xdif);
            float upper = NGInterp(ulf, urf, xdif);
            double val = NGInterp(lower, upper, ydif);
            //val = Math.Cos((val + 1) * Math.PI / 2);
            //val = Math.Cos((val + 1) * Math.PI / 2);
            return filter((float)val, position * period);
        }
        public Vector2 getNodeValue(int x, int y)
        {
            long a = MathHelper.cartesianToSpiral(x, y);
            long b = seed;
            long longy = a * b;
            longy = Math.Abs(longy);

            do
            {
                longy >>= 1;
            } while (longy > int.MaxValue
                     || longy < int.MinValue);

            Random rdm = new Random((int)longy);
            return randVect(rdm);

            //int a = (int)MathHelper.cartesianToSpiral(x, y);
            //return randVect(new Random((a*237)%seed));

            //Random rdm1 = new Random(x);
            //Random rdm2 = new Random(rdm1.Next() + y*x);
            //return randVect(rdm2);
        }
    }

    class BasicRandomNoise : Noise
    {
        int seed;
        float period;
        public BasicRandomNoise(int seed, float period)
        {
            this.seed = seed;
            this.period = period;
            filter = IdentityFilter;
        }
        public override float[,] getValues(float[,] values, noiseSumFunction sumFunc, Point samples, Vector2 origin, Vector2 area)
        {
            origin *= 1f / period;
            area *= 1f / period;

            if (values.GetLength(0) != samples.X || values.GetLength(1) != samples.Y)
                throw new Exception("Invalid Array to sum To.");

            Rectangle gradientSamples = new Rectangle();
            gradientSamples.X = (int)Math.Floor(origin.X);
            gradientSamples.Y = (int)Math.Floor(origin.Y);
            gradientSamples.Width = (int)Math.Floor(origin.X + area.X) - gradientSamples.X + 1;
            gradientSamples.Height = (int)Math.Floor(origin.Y + area.Y) - gradientSamples.Y + 1;


            float[,] gradientVectors = new float[gradientSamples.Width + 1, gradientSamples.Height + 1];
            for (int i = 0; i < gradientSamples.Width + 1; i++)
                for (int j = 0; j < gradientSamples.Height + 1; j++)
                {
                    gradientVectors[i, j] = getNodeValue(gradientSamples.X + i, gradientSamples.Y + j);
                }

            float xDif, yDif;
            int p1, p2;
            float xStep = area.X / (samples.X - 2);
            float yStep = area.Y / (samples.Y - 2);
            float ll, lr, ur, ul, upper, lower;
            Vector2 pos = new Vector2();
            double val;
            for (int i = 0; i < samples.X; i++)
                for (int j = 0; j < samples.Y; j++)
                {
                    pos.X = origin.X + i * xStep;
                    pos.Y = origin.Y + j * yStep;
                    p1 = (int)Math.Floor(pos.X);
                    p2 = (int)Math.Floor(pos.Y);
                    xDif = pos.X - (float)p1;
                    yDif = pos.Y - (float)p2;
                    p1 = p1 - gradientSamples.X;
                    p2 = p2 - gradientSamples.Y;
                    pos *= period;

                    ll = gradientVectors[p1, p2];
                    lr = gradientVectors[p1 + 1, p2];
                    ur = gradientVectors[p1 + 1, p2 + 1];
                    ul = gradientVectors[p1, p2 + 1];

                    upper = NGInterp(ul, ur, xDif);
                    lower = NGInterp(ll, lr, xDif);
                    val = NGInterp(lower, upper, yDif);
                    //val = Math.Cos((val + 1) * Math.PI / 2);
                    //val = Math.Cos((val + 1) * Math.PI / 2);
                    values[i, j] = sumFunc(values[i, j], filter((float)val, pos));
                }

            return values;
        }
        public override float getValueAt(Vector2 position)
        {
            position *= 1f / period;
            float llf, lrf, urf, ulf;
            int x, y;
            x = (int)position.X;
            y = (int)position.Y;

            float xdif = x - position.X;
            float ydif = y - position.Y;

            llf = getNodeValue(x, y);
            lrf = getNodeValue(x + 1, y);
            urf = getNodeValue(x + 1, y + 1);
            ulf = getNodeValue(x, y + 1);

            float lower = NGInterp(llf, lrf, xdif);
            float upper = NGInterp(ulf, urf, xdif);
            double val = NGInterp(lower, upper, ydif);
            val = Math.Cos((val + 1) * Math.PI / 2);
            val = Math.Cos((val + 1) * Math.PI / 2);
            return filter((float)val, position * period);
        }
        public float getNodeValue(int x, int y)
        {
            Random rdm = new Random((int)(MathHelper.cartesianToSpiral(x, y) * seed));
            return ((float)rdm.NextDouble() - 0.5f) * 2f;
        }
    }

    class IntegerRandomMap
    {
        int seed;

        public IntegerRandomMap(int seed)
        {
            this.seed = seed;
        }
        public int[,] getValues(Point samples, Point origin)
        {

            Rectangle SamplingRectangle = new Rectangle();
            SamplingRectangle.X = origin.X;
            SamplingRectangle.Y = origin.Y;
            SamplingRectangle.Width = samples.X;
            SamplingRectangle.Height = samples.Y;


            int[,] values = new int[SamplingRectangle.Width, SamplingRectangle.Height];
            for (int i = 0; i < SamplingRectangle.Width; i++)
                for (int j = 0; j < SamplingRectangle.Height; j++)
                {
                    values[i, j] = getNodeValue(SamplingRectangle.X + i, SamplingRectangle.Y + j);
                }


            return values;
        }
        public int getValueAt(Point position)
        {
            return getNodeValue(position.X, position.Y);
        }
        public int getNodeValue(int x, int y)
        {
            Random rdm = new Random((int)(MathHelper.cartesianToSpiral(x, y) * seed));
            return rdm.Next();
        }
    }

    class NoiseFilter : Noise
    {
        public NoiseFilter()
        {

        }

        public override float getValueAt(Vector2 position)
        {
            return filter(0f, position);
        }
        public override float[,] getValues(float[,] values, noiseSumFunction sumFunc, Point samples, Vector2 origin, Vector2 area)
        {
            float xStep = area.X / (samples.X - 1);
            float yStep = area.Y / (samples.Y - 1);
            Vector2 pos = new Vector2();
            for (int i = 0; i < samples.X; i++)
                for (int j = 0; j < samples.Y; j++)
                {
                    pos.X = origin.X + i * xStep;
                    pos.Y = origin.Y + j * yStep;

                    values[i, j] += sumFunc(values[i, j], filter(0f, pos));
                }

            return values;
        }
    }
    class SumNoise : Noise
    {
        List<Noise> noises;
        List<Noise.noiseSumFunction> sumFunctions;

        public SumNoise()
        {
            noises = new List<Noise>();
            sumFunctions = new List<noiseSumFunction>();// = (float baseStep, float subStep) => { return baseStep + subStep; };
        }

        public void addNoise(Noise noise, noiseSumFunction sumFunction)
        {
            noises.Add(noise);
            sumFunctions.Add(sumFunction);
        }
        public Noise getNoise(int index)
        {
            return noises[index];
        }
        public void removeNoise(int index)
        {
            noises.RemoveAt(index);
            sumFunctions.RemoveAt(index);
        }
        public void setNoiseSumFunction(int index, noiseSumFunction function)
        {
            sumFunctions[index] = function;
        }
        public override float[,] getValues(float[,] values, Noise.noiseSumFunction sumFunc, Point samples, Vector2 origin, Vector2 area)
        {
            values = new float[samples.X, samples.Y];

            for (int i = 0; i < noises.Count; i++)
            {
                noises[i].getValues(values, sumFunctions[i], samples, origin, area);
            }

            Vector2 position = new Vector2();
            float xStep = area.X / (samples.X - 1);
            float yStep = area.Y / (samples.Y - 1);
            for (int i = 0; i < values.GetLength(0); i++)
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    position.X = i * xStep + origin.X;
                    position.Y = j * yStep + origin.Y;
                    values[i, j] = filter(values[i, j], position);
                }
            return values;
        }
        public override float getValueAt(Vector2 position)
        {
            float sum = 0;
            for (int i = 0; i < noises.Count; i++)
            {
                sum = sumFunctions[i](sum, noises[i].getValueAt(position));
            }
            return filter(sum, position);
        }
    }

    class DiscreteMap<T>
    {
        float width;
        float height;
        float mapWidth;
        float mapHeight;
        public T[,] map;

        public DiscreteMap(T[,] map, int width, int height)
        {
            this.width = width;
            this.height = height;
            this.mapWidth = map.GetLength(0);
            this.mapHeight = map.GetLength(1);
            this.map = map;
        }
        public T get(int x, int y)
        {
            if (x < 0 || x > width || y < 0 || y > height)
                throw new Exception("Out of bounds");

            int i = (int)(((float)x) / (width / mapWidth));
            int j = (int)(((float)y) / (height / mapHeight));

            return map[i, j];
        }
    }

    class NodeMap
    {
        float width;
        float height;
        float mapWidth;
        float mapHeight;
        public float[,] map;

        public NodeMap(float[,] map, int width, int height)
        {
            this.width = width;
            this.height = height;
            this.mapWidth = map.GetLength(0);
            this.mapHeight = map.GetLength(1);
            this.map = map;
        }
        public float get(int x, int y)
        {
            if (x < 0 || x > width || y < 0 || y > height)
                throw new Exception("Out of bounds");

            float dx, dy;
            //int i = (int)(((float)x)/(width / (mapWidth+1)));
            //int i = (int)(x * ((mapWidth - 1) / width));
            dx = (x * (1f / width) * (mapWidth - 1));
            int i = (int)dx;
            dx = dx - i;
            //int j = (int)(((float)y) / (height / (mapHeight+1)));
            //int j = (int)(y * ((mapHeight - 1) / height));
            dy = (y * (1f / height) * (mapHeight - 1));
            int j = (int)dy;
            dy = dy - j;

            float a = (map[i + 1, j] - map[i, j]) * dx + map[i, j];
            float b = (map[i + 1, j + 1] - map[i, j + 1]) * dx + map[i, j + 1];
            return (b - a) * dy + a;
        }
    }
}

