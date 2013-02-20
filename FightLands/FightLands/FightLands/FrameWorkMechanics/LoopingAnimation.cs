using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class LoopingAnimation : GameObject
    {
        GameObject target;

        bool started;
        bool running;
        bool ended;
        float looptime;
        float initialPhase;
        float currentTime;
        float phase;

        public enum InterpolationType { linear , Trignometric};
        InterpolationType interpolationType;

        bool interpolPosition;
        Vector2 originalPosition;
        Vector2 destinationPosition;

        bool interpolRotation;
        float originalRotation;
        float destinationRotation;

        bool interpolSize;
        Vector2 originalSize;
        Vector2 destinationSize;

        
        /// <summary>
        /// Animate position, rotation and size. (GameObject must implement Resizable)
        /// </summary>
        /// <param name="world"></param>
        /// <param name="targetObject"></param>
        /// <param name="originalPosition"></param>
        /// <param name="destinationPosition"></param>
        /// <param name="originalRotation"></param>
        /// <param name="destinationRotation"></param>
        /// <param name="originalSize"></param>
        /// <param name="destinationSize"></param>
        /// <param name="duration"></param>
        /// <param name="interpolationType"></param>
        /// <param name="start"></param>
        public LoopingAnimation(
            World world, 
            GameObject targetObject, 
            Vector2 originalPosition, 
            Vector2 destinationPosition,
            float originalRotation,
            float destinationRotation,
            Vector2 originalSize,
            Vector2 destinationSize,
            float loopTime, 
            InterpolationType 
            interpolationType, 
            bool start,
            float initialPhase
            )
            : base(world)
        {
            target = targetObject;

            if (!(target is Resizable))
                throw new Exception("Attempt at resizing a unresizable object. (Implement Resizable interface)");

            started = start;
            running = start;
            ended = false;

            this.looptime = loopTime;
            this.initialPhase = initialPhase;

            interpolPosition = true;
            interpolRotation = true;
            interpolSize = true;

            this.interpolationType = interpolationType;

            this.originalPosition = originalPosition;
            this.destinationPosition = destinationPosition;
            this.originalRotation = originalRotation;
            this.destinationRotation = destinationRotation;
            this.originalSize = originalSize;
            this.destinationSize = destinationSize;
        }

        public void Start()
        {
            started = true;
            running = true;
        }
        public void Pause()
        {
            running = false;
        }
        public void Resume()
        {
            running = true;
        }
        public bool isFinished()
        {
            return ended;
        }
        public override void Update(UpdateState state)
        {
            if (running)
            {
                currentTime += state.elapsedTime;

                phase = initialPhase + (currentTime / looptime);

                switch (interpolationType)
                {
                    case InterpolationType.linear:
                        phase *= 2f;

                        // clamp phase between 0 and 2
                        phase = phase - (((int)phase) / 2) * 2f;

                        // invert
                        if (phase > 1f)
                            phase = 2f - phase;
                        break;
                    case InterpolationType.Trignometric:
                        phase = 1f - ((float)Math.Cos(phase*Math.PI*2f) + 1f)/2f;
                        break;
                }


                if (interpolPosition)
                    target.position = originalPosition * (1f - phase) + destinationPosition * phase;

                if (interpolRotation)
                    target.rotation = originalRotation * (1f - phase) + destinationRotation * phase;

                if (interpolSize)
                    ((Resizable)target).setSize(originalSize * (1f - phase) + destinationSize * phase);
            }
            else
            {
                if (ended)
                    destroy();
            }
        }

    }
}
