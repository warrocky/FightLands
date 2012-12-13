using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    abstract class GameObject : Updatable
    {
        public readonly World world;

        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public float rotationSpeed;

        public Vector2 forward
        {
            get { return MathHelper.getDirectionFromAngle(rotation); }
        }

        public GameObject(World world)
        {
            this.world = world;
            world.AddObject(this);
        }

        public virtual void destroy()
        {
            world.RemoveObject(this);
        }

        public virtual void Update(UpdateState state)
        {
            this.position += velocity;
            this.rotation += rotationSpeed;
        }
        public virtual void Draw(DrawState state)
        {

        }
    }
}
