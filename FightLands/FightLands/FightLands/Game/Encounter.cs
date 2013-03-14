using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class Encounter : LandObject
    {
        static float encounterRadius = 50f;
        LandCreature anchor;

        DrawableTexture Dummy;

        public Encounter(LandCreature anchor, Land land)
            : base(land)
        {
            this.anchor = anchor;

            physicalProperties = new LandPhysicalProperties();
            physicalProperties.activelyColliding = true;
            physicalProperties.collisionType = LandCollisionTypes.Static;
            physicalProperties.radius = encounterRadius;

            Dummy = new DrawableTexture("whiteCircle100x100", this);
            Dummy.size = new Vector2(100f, 100f);
            Dummy.filter = Color.Transparent;
            Dummy.layer = 0.98f;
        }
        public override void Update(UpdateState state)
        {
            this.position = anchor.position;
            Dummy.filter = Color.Lerp(Dummy.filter, Color.Transparent, state.elapsedTime*3f);
        }

        public override bool AuthorizeCollision(LandObject collider)
        {
            if (collider is Encounter)
                return true;
            else
                return false;
        }

        public override void CollideEffect(LandObject collider)
        {
            Dummy.filter = Color.Lerp(Color.Red,Color.Transparent, 0.5f);
        }

        public override void Draw(DrawState state)
        {
            Dummy.Draw(state);
        }
    }
}
