using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class Animation : GameObject
    {
        GameObject target;

        bool started;
        bool running;
        bool ended;
        float lifetime;
        float currentTime;

        public enum InterpolationType { linear };
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
        public Animation(
            World world, 
            GameObject targetObject, 
            Vector2 originalPosition, 
            Vector2 destinationPosition,
            float originalRotation,
            float destinationRotation,
            Vector2 originalSize,
            Vector2 destinationSize,
            float duration, 
            InterpolationType 
            interpolationType, 
            bool start
            )
            : base(world)
        {
            target = targetObject;

            if (!(target is Resizable))
                throw new Exception("Attempt at resizing a unresizable object. (Implement Resizable interface)");

            started = start;
            running = start;
            ended = false;

            lifetime = duration;

            interpolPosition = true;
            interpolRotation = true;
            interpolSize = true;

            this.originalPosition = originalPosition;
            this.destinationPosition = destinationPosition;
            this.originalRotation = originalRotation;
            this.destinationRotation = destinationRotation;
            this.originalSize = originalSize;
            this.destinationSize = destinationSize;
        }

        /// <summary>
        /// Animate position.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="targetObject"></param>
        /// <param name="originalPosition"></param>
        /// <param name="destinationPosition"></param>
        /// <param name="duration"></param>
        /// <param name="interpolationType"></param>
        /// <param name="start"></param>
        public Animation(
            World world,
            GameObject targetObject,
            Vector2 originalPosition,
            Vector2 destinationPosition,
            float duration,
            InterpolationType
            interpolationType,
            bool start
            )
            : base(world)
        {
            target = targetObject;

            started = start;
            running = start;
            ended = false;

            lifetime = duration;

            interpolPosition = true;
            interpolRotation = false;
            interpolSize = false;

            this.originalPosition = originalPosition;
            this.destinationPosition = destinationPosition;
        }

        /// <summary>
        /// Animate rotation.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="targetObject"></param>
        /// <param name="originalRotation"></param>
        /// <param name="destinationRotation"></param>
        /// <param name="duration"></param>
        /// <param name="interpolationType"></param>
        /// <param name="start"></param>
        public Animation(
            World world,
            GameObject targetObject,
            float originalRotation,
            float destinationRotation,
            float duration,
            InterpolationType
            interpolationType,
            bool start
            )
            : base(world)
        {
            target = targetObject;

            started = start;
            running = start;
            ended = false;

            lifetime = duration;

            interpolPosition = false;
            interpolRotation = true;
            interpolSize = false;

            this.originalRotation = originalRotation;
            this.destinationRotation = destinationRotation;
        }

        /// <summary>
        /// Animate to destination position.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="targetObject"></param>
        /// <param name="destinationPosition"></param>
        /// <param name="time"></param>
        /// <param name="interpolationType"></param>
        /// <param name="start"></param>
        public Animation(World world, GameObject targetObject, Vector2 destinationPosition, float time, InterpolationType interpolationType, bool start)
            : base(world)
        {
            target = targetObject;

            started = start;
            running = start;
            ended = false;

            lifetime = time;

            interpolPosition = true;
            interpolRotation = false;
            interpolSize = false;

            originalPosition = target.position;
            this.destinationPosition = destinationPosition;
        }

        /// <summary>
        /// Animate to destination rotation.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="targetObject"></param>
        /// <param name="destinationRotation"></param>
        /// <param name="duration"></param>
        /// <param name="interpolationType"></param>
        /// <param name="start"></param>
        public Animation(
            World world,
            GameObject targetObject,
            float destinationRotation,
            float duration,
            InterpolationType
            interpolationType,
            bool start
            )
            : base(world)
        {
            target = targetObject;

            started = start;
            running = start;
            ended = false;

            lifetime = duration;

            interpolPosition = false;
            interpolRotation = true;
            interpolSize = false;

            this.originalRotation = target.rotation;
            this.destinationRotation = destinationRotation;
        }

        /// <summary>
        /// Animate to destination rotation, position and size.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="targetObject"></param>
        /// <param name="destinationPosition"></param>
        /// <param name="destinationRotation"></param>
        /// <param name="destinationSize"></param>
        /// <param name="duration"></param>
        /// <param name="interpolationType"></param>
        /// <param name="start"></param>
        public Animation(
            World world,
            GameObject targetObject,
            Vector2 destinationPosition,
            float destinationRotation,
            Vector2 destinationSize,
            float duration,
            InterpolationType
            interpolationType,
            bool start
            )
            : base(world)
        {
            target = targetObject;

            if (!(target is Resizable))
                throw new Exception("Attempt at resizing a unresizable object. (Implement Resizable interface)");

            started = start;
            running = start;
            ended = false;

            lifetime = duration;

            interpolPosition = true;
            interpolRotation = true;
            interpolSize = true;

            this.originalPosition = target.position;
            this.destinationPosition = destinationPosition;
            this.originalRotation = target.rotation;
            this.destinationRotation = destinationRotation;
            this.originalSize = ((Resizable)target).getSize();
            this.destinationSize = destinationSize;
        }


        public void Start()
        {
            if (ended)
                throw new Exception("Attempt at starting an ended animation.");

            started = true;
            running = true;
        }
        public void Pause()
        {
            running = false;
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

                float animationFactor = currentTime / lifetime;

                if (animationFactor > 1f)
                {
                    animationFactor = 1f;
                    ended = true;
                    running = false;
                }

                if (interpolPosition)
                    target.position = originalPosition * (1f - animationFactor) + destinationPosition * animationFactor;

                if (interpolRotation)
                    target.rotation = originalRotation * (1f - animationFactor) + destinationRotation * animationFactor;

                if (interpolSize)
                    ((Resizable)target).setSize(originalSize * (1f - animationFactor) + destinationSize * animationFactor);
            }
            else
            {
                if (ended)
                    destroy();
            }
        }

    }
}
