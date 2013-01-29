using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class LoopableNoise3D : Noise3D
    {
        readonly int seed;
        public readonly float period;
        public readonly Point3 periodLoop;

        Vector3[, ,] gradientVectors;

        public LoopableNoise3D(int seed, float period, Point3 periodLoop)
        {
            this.seed = seed;
            this.period = period;
            this.periodLoop = periodLoop;

            Random rdm = new Random(seed);
            gradientVectors = new Vector3[periodLoop.X, periodLoop.Y, periodLoop.Z];

            for(int i=0;i<gradientVectors.GetLength(0);i++)
                for(int j=0;j<gradientVectors.GetLength(1);j++)
                    for (int k = 0; k < gradientVectors.GetLength(2); k++)
                    {
                        gradientVectors[i, j, k] = randVect(rdm);
                    }
        }
        public override float[,,] getValues(float[,,] values, Noise3D.noiseSumFunction sumFunc, Point3 samples, Vector3 origin, Vector3 area)
        {
            origin *= 1f / period;
            area *= 1f / period;


            float difX, difY,difZ;
            Vector3 difVector = new Vector3();
            Vector3 step = new Vector3(area.X / (samples.X - 1), area.Y / (samples.Y - 1), area.Z / (samples.Z - 1));
            Vector3 pos = new Vector3();
            int cubeX,cubeY,cubeZ;
            Vector3 gradientVector;
            float lll, llu, lul, luu, ull, ulu, uul, uuu;
            float ll, lu, ul, uu;
            float l,u;
            float value;
            for(int i=0;i<samples.X;i++)
                for(int j=0;j<samples.Y;j++)
                    for (int k = 0; k < samples.Z; k++)
                    {
                        pos.X = origin.X + i * step.X;
                        pos.Y = origin.Y + j * step.Y;
                        pos.Z = origin.Z + k * step.Z;

                        cubeX = (int)Math.Floor(pos.X);
                        cubeY = (int)Math.Floor(pos.Y);
                        cubeZ = (int)Math.Floor(pos.Z);

                        difX = pos.X - (float)cubeX;
                        difY = pos.Y - (float)cubeY;
                        difZ = pos.Z - (float)cubeZ;

                        pos *= period;


                        //lll llu luu  lul uul uuu ulu ull
                        //lll
                        difVector.X = difX;
                        difVector.Y = difY;
                        difVector.Z = difZ;
                        gradientVector = getGradientVector(cubeX, cubeY, cubeZ);
                        Vector3.Dot(ref gradientVector, ref difVector, out lll);

                        //llu
                        difVector.Z = difZ - 1f;
                        gradientVector = getGradientVector(cubeX, cubeY, cubeZ + 1);
                        Vector3.Dot(ref gradientVector, ref difVector, out llu);

                        //luu
                        difVector.Y = difY - 1f;
                        gradientVector = getGradientVector(cubeX, cubeY + 1, cubeZ + 1);
                        Vector3.Dot(ref gradientVector, ref difVector, out luu);

                        //lul
                        difVector.Z = difZ;
                        gradientVector = getGradientVector(cubeX, cubeY + 1, cubeZ);
                        Vector3.Dot(ref gradientVector, ref difVector, out lul);

                        //uul
                        difVector.X = difX - 1f;
                        gradientVector = getGradientVector(cubeX + 1, cubeY + 1, cubeZ);
                        Vector3.Dot(ref gradientVector, ref difVector, out uul);

                        //uuu
                        difVector.Z = difZ - 1f;
                        gradientVector = getGradientVector(cubeX + 1, cubeY + 1, cubeZ + 1);
                        Vector3.Dot(ref gradientVector, ref difVector, out uuu);

                        //ulu
                        difVector.Y = difY;
                        gradientVector = getGradientVector(cubeX + 1, cubeY, cubeZ + 1);
                        Vector3.Dot(ref gradientVector, ref difVector, out ulu);

                        //ull
                        difVector.Z = difZ;
                        gradientVector = getGradientVector(cubeX + 1, cubeY, cubeZ);
                        Vector3.Dot(ref gradientVector, ref difVector, out ull);

                        ll = NGInterp(lll, ull, difX);
                        lu = NGInterp(llu, ulu, difX);
                        ul = NGInterp(lul, uul, difX);
                        uu = NGInterp(luu, uuu, difX);

                        l = NGInterp(ll, ul, difY);
                        u = NGInterp(lu, uu, difY);

                        value = NGInterp(l, u, difZ);
                        value = (float)Math.Cos((value + 1) * Math.PI / 2);
                        values[i, j, k] = sumFunc(values[i, j, k], filter(value, pos));
                    }

            return values;
        }
        public override float getValueAt(Vector3 position)
        {
            //position *= 1f / period;
            //float llf, lrf, urf, ulf;
            //int x, y;
            //x = (int)Math.Floor(position.X);
            //y = (int)Math.Floor(position.Y);

            //float xdif = position.X - (float)x;
            //float ydif = position.Y - (float)y;

            //llf = Vector2.Dot(getNodeValue(x, y), new Vector2(xdif, ydif));
            //lrf = Vector2.Dot(getNodeValue(x + 1, y), new Vector2(xdif - 1f, ydif));
            //urf = Vector2.Dot(getNodeValue(x + 1, y + 1), new Vector2(xdif - 1f, ydif - 1f));
            //ulf = Vector2.Dot(getNodeValue(x, y + 1), new Vector2(xdif, ydif - 1f));

            //float lower = NGInterp(llf, lrf, xdif);
            //float upper = NGInterp(ulf, urf, xdif);
            //double val = NGInterp(lower, upper, ydif);
            ////val = Math.Cos((val + 1) * Math.PI / 2);
            ////val = Math.Cos((val + 1) * Math.PI / 2);
            //return filter((float)val, position * period);

            return 0f;
        }
        public Vector3 getGradientVector(int x, int y, int z)
        {
            return gradientVectors[x%periodLoop.X, y%periodLoop.Y, z%periodLoop.Z];
        }
    }
}
