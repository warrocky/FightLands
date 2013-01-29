using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class SumNoise3D : Noise3D
    {
        List<Noise3D> noises;
        List<Noise3D.noiseSumFunction> sumFunctions;

        public SumNoise3D()
        {
            noises = new List<Noise3D>();
            sumFunctions = new List<noiseSumFunction>();// = (float baseStep, float subStep) => { return baseStep + subStep; };
        }

        public void addNoise(Noise3D noise, noiseSumFunction sumFunction)
        {
            noises.Add(noise);
            sumFunctions.Add(sumFunction);
        }
        public Noise3D getNoise(int index)
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
        public override float[,,] getValues(float[,,] values, Noise3D.noiseSumFunction sumFunc, Point3 samples, Vector3 origin, Vector3 area)
        {
            values = new float[samples.X, samples.Y,samples.Z];

            for (int i = 0; i < noises.Count; i++)
            {
                noises[i].getValues(values, sumFunctions[i], samples, origin, area);
            }

            Vector3 position = new Vector3();
            Vector3 step = new Vector3(area.X / (samples.X - 1), area.Y / (samples.Y - 1), area.Z / (samples.Z - 1));

            for(int i=0;i<values.GetLength(0);i++)
                for(int j=0;j<values.GetLength(1);j++)
                    for (int k = 0; k < values.GetLength(2); k++)
                    {
                        position.X = i * step.X + origin.X;
                        position.Y = j * step.Y + origin.Y;
                        position.Z = k * step.Z + origin.Z;

                        values[i, j, k] = filter(values[i, j, k], position);
                    }

            return values;
        }
        public override float getValueAt(Vector3 position)
        {
            float sum = 0;
            for (int i = 0; i < noises.Count; i++)
            {
                sum = sumFunctions[i](sum, noises[i].getValueAt(position));
            }
            return filter(sum, position);
        }
    }

}
