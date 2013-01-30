using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    abstract class Noise3D
    {
        public delegate float noiseFilterFuncton(float noiseValue, Vector3 position);
        public delegate float noiseSumFunction(float arrayValue, float noiseValue);

        public noiseFilterFuncton filter;

        public Noise3D()
        {
            filter = (float a, Vector3 b) => a;
        }

        public abstract float[,,] getValues(float[,,] values, noiseSumFunction sumFunc, Point3 samples, Vector3 origin, Vector3 area);

        public float[,,] getValues(Point3 samples, Vector3 origin, Vector3 area)
        {
            float[,,] values = new float[samples.X, samples.Y,samples.Z];
            return getValues(values, OverrideSum, samples, origin, area);
        }
        public abstract float getValueAt(Vector3 position);

        public static float IdentityFilter(float noiseValue, Vector3 position)
        {
            return noiseValue;
        }
        public static float AbsFilter(float noiseValue, Vector3 position)
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
        public static Noise3D TurbulenceNoise(float period, int steps, Point3 periodLoop, int seed)
        {
            SumNoise3D sumNoise = new SumNoise3D();
            Random rdm = new Random(seed);

            LoopableNoise3D noise;

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
                noise = new LoopableNoise3D(rdm.Next(), period, periodLoop);
                //float weight = weights[i] + (weights[i] / weightSum) * (1f - weightSum);
                float weight = weights[i] / weightSum;

                noise.filter = (float value, Vector3 position) => Math.Abs(value) * weight;
                //noise.filter = (float value, Vector2 position) => (Math.Abs(value) - 0.5f)*2f*weight;

                //o filtro do ultimo noise coloca os valores no intervalo [-1,1]
                if (i != steps - 1)
                {
                    sumNoise.addNoise(noise, (float a, float b) => a + b);
                }
                else
                {
                    sumNoise.addNoise(noise, (float a, float b) => ((a + b) - 0.5f) * 2f);
                }



                period *= 0.5f;
                periodLoop.X *= 2;
                periodLoop.Y *= 2;
                periodLoop.Z *= 2;
            }

            //sumNoise.filter = (float a, Vector2 b) => { return (a + 1f) / 2f; };
            sumNoise.filter = (float a, Vector3 b) => a;
            return sumNoise;
        }
        public static Noise3D RegularNoise(float period, int steps, Point3 periodLoop, int seed)
        {
            if (steps == 0)
                throw new Exception("Can't create a noise regular noise with 0 steps");

            SumNoise3D sumNoise = new SumNoise3D();
            Random rdm = new Random(seed);

            LoopableNoise3D noise;

            float[] weights = new float[steps];
            weights[0] = 0.5f;
            float weightSum = 0.5f;

            for (int i = 1; i < steps; i++)
            {
                weights[i] = weights[i - 1] * 0.5f;
                weightSum += weights[i];
            }

            //float weight;
            Noise3D.noiseSumFunction simpleSum = (float a, float b) => { return a + b; };
            for (int j = 0; j < steps; j++)
            {
                noise = new LoopableNoise3D(rdm.Next(), period, periodLoop);
                float weight = weights[j] / weightSum;
                noise.filter = (float value, Vector3 position) => { return value * (weight); };

                sumNoise.addNoise(noise, simpleSum);


                period *= 0.5f;
                periodLoop.X *= 2;
                periodLoop.Y *= 2;
                periodLoop.Z *= 2;
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

        public static void changeBias(float[,,] array, Vector2 originalBias, Vector2 newBias)
        {
            float scale = (newBias.Y - newBias.X) / (originalBias.Y - originalBias.X);

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    for (int k = 0; k < array.GetLength(2); k++)
                    {
                        array[i, j, k] -= originalBias.X;
                        array[i, j, k] *= scale;
                        array[i, j, k] += newBias.X;
                    }
                }
            }
        }



        protected static Vector3 randVect(Random rdm)
        {
            Vector3 randomVector = new Vector3(MathHelper.getNextNormalDistributedFloat(rdm), MathHelper.getNextNormalDistributedFloat(rdm), MathHelper.getNextNormalDistributedFloat(rdm));
            randomVector.Normalize();
            return randomVector;
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
}
