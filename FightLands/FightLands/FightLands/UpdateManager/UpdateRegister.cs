using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class UpdateRegister
    {
        public readonly Updatable updatedObject;
        public readonly String tag;
        public UpdateFrequency frequency;

        private int updatesPassed;
        private int timeElapsedSinceLastUpdate;
        /// <summary>
        /// This register UpdateID. As in the number of times the Object under this register has been updated.
        /// </summary>
        private int currentUpdateID;

        public UpdateRegister(Updatable updatedObject, String tag)
        {
            this.tag = tag;
            this.updatedObject = updatedObject;
            updatesPassed = 0;
            frequency = UpdateFrequency.One;
            timeElapsedSinceLastUpdate = 0;
        }

        public virtual void Update(GameTime time, int GlobalUpdateID)
        {
            bool update = false;

            updatesPassed++;
            timeElapsedSinceLastUpdate = time.ElapsedGameTime.Milliseconds;

            switch (frequency)
            {
                case UpdateFrequency.One:
                    update = true;
                    break;
                case UpdateFrequency.OneHalf:
                    if (updatesPassed % 2 == 0)
                        update = true;
                    break;
                case UpdateFrequency.OneQuarter:
                    if (updatesPassed % 4 == 0)
                        update = true;
                    break;
                case UpdateFrequency.OneEigth:
                    if (updatesPassed % 8 == 0)
                        update = true;
                    break;
                case UpdateFrequency.OneSixteenth:
                    if (updatesPassed % 16 == 0)
                        update = true;
                    break;
            }

            if (update)
            {
                currentUpdateID++;

                UpdateState state = new UpdateState(((float)timeElapsedSinceLastUpdate)/1000f, GlobalUpdateID, currentUpdateID, this);

                updatedObject.Update(state);
                updatesPassed = 0;
                timeElapsedSinceLastUpdate = 0;
            }
        }

        public enum UpdateFrequency { One, OneHalf, OneQuarter, OneEigth, OneSixteenth }
    }
}
