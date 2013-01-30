using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    static class MathHelper
    {
        /// <summary>
        /// A method that rotates a vector a specified amount in radians.
        /// </summary>
        /// <param name="rotation">The angle in radians in which to rotate the vector.</param>
        /// <param name="vector">The vector to be rotated</param>
        /// <returns>The rotated vector.</returns>
        public static Vector2 rotateVector2(float rotation, Vector2 vector)
        {
            float cos = (float)Math.Cos(rotation);
            float sin = (float)Math.Sin(rotation);
            return new Vector2(cos*vector.X - sin*vector.Y, sin*vector.X + cos*vector.Y);
        }
        /// <summary>
        /// A method that returns a unitary vector with the specified angle in relation to the Ox axis.
        /// </summary>
        /// <param name="angle">The angle in radians of the desired vector in relation to the Ox axis.</param>
        /// <returns>A unitary vector (length = 1) with the desired angle in relation to the Ox axis.</returns>
        public static Vector2 getDirectionFromAngle(float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            return new Vector2(cos, sin);
        }
        /// <summary>
        /// A method that calculates the angle of a unitary vector (length = 1).
        /// </summary>
        /// <param name="direction">The unitary vector (length = 1).</param>
        /// <returns>"The angle from 0 to 2*pi."</returns>
        public static float getAngleFromDirection(Vector2 direction)
        {
            if (direction.Y > 0)
                return (float)Math.Acos(direction.X);
            else
                return (float)(Math.PI * 2 - Math.Acos(direction.X));
        }
        /// <summary>
        /// A method that calculates the angle of a vector in relation to the Ox axis.
        /// </summary>
        /// <param name="vector">The vector to get the angle from</param>
        /// <returns>The angle the vector has in relation to the 0x axis.</returns>
        public static float getAngleFromVector(Vector2 vector)
        {
            vector.Normalize();
            return getAngleFromDirection(vector);
        }

        /// <summary>
        /// A method that calculates the Spiral coordinates of a point from cartesian coordinates.
        /// </summary>
        /// <param name="x">The value in the x axis.</param>
        /// <param name="y">The value in the y axis</param>
        /// <returns></returns>
        public static long cartesianToSpiral(int x, int y)
        {
            long nivel, origem, stride;

            if (Math.Abs(x) > Math.Abs(y))
            {
                nivel = Math.Abs(x);

                origem = 1 + 4 * (nivel * (nivel - 1));

                stride = nivel * 2;
                if (x > 0)
                    if (y >= 0)
                    {
                        return origem + y;
                    }
                    else
                    {
                        return origem + 4 * stride + y;
                    }
                else
                {
                    //x não pode ser 0 pois é maior que y e y é maior que 0.
                    return origem + 2 * stride - y;
                }
            }
            else
            {
                nivel = Math.Abs(y);

                origem = 1 + 4 * (nivel * (nivel - 1));

                stride = nivel * 2;

                if (y > 0)
                {
                    return origem + stride - x;
                }
                else if (y == 0)
                {
                    return 0;
                }
                else
                {
                    return origem + 3 * stride + x;
                }
            }
        }

        /// <summary>
        /// Returns a seemingly normally distributed random number from a Random object
        /// </summary>
        /// <param name="rdm">The Random object the number will be retrieved from.</param>
        /// <returns>A normally distributed random number centered on 0 with a maximum deviation of 1.</returns>
        public static float getNextNormalDistributedFloat(Random rdm)
        {
            return getNextNormalDistributedFloat(0f, 1f, rdm);
        }
        /// <summary>
        /// Returns a seemingly normally distributed random number from a Random object.
        /// </summary>
        /// <param name="center">The center of the distribution (average).</param>
        /// <param name="deviation">The maximum deviation of the distribution.</param>
        /// <param name="rdm">The random from which the value will be retrieved.</param>
        /// <returns>A seemingly normally distributed random number.</returns>
        public static float getNextNormalDistributedFloat(float center, float deviation, Random rand)
        {
            //Random rand = new Random(); //reuse this if you are generating many
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         center + deviation/2f * randStdNormal; //random normal(mean,stdDev^2)

            return (float)randNormal;
        }

        /// <summary>
        /// Restricts a value to be within the specified boundaries
        /// </summary>
        /// <param name="value">The value to be clamped</param>
        /// <param name="lowerLimit">Lower boundary</param>
        /// <param name="upperLimit">Upper bountary</param>
        /// <returns>A value between the specified boundaries.</returns>
        public static float Clamp(float value, float lowerLimit, float upperLimit)
        {
            if (value < lowerLimit)
                return lowerLimit;
            else
                if (value > upperLimit)
                    return upperLimit;
                else
                    return value;
        }
        public static int Clamp(int value, int lowerLimit, int upperLimit)
        {
            if (value < lowerLimit)
                return lowerLimit;
            else
                if (value > upperLimit)
                    return upperLimit;
                else
                    return value;
        }

        public static float SCurveInterpolation(float a, float b, float x)
        {
            float weight = (x * x) * (3 - 2 * x); //s3
            //float weight =  x; // linear
            //float weight =  (float)(1f - Math.Cos(x*Math.PI)) * 0.5f; // cosine
            return a * (1 - weight) + b * weight;
        }
    }
}
