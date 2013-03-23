using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class HumanPlayer : Human
    {
        public HumanPlayer(Land land)
            : base(land)
        {

        }
    }
    class LandHumanPlayer : LandHuman , Controlable, Land.LandContentRequirer, Land.LandUpdateNode
    {
        DrawableTexture guy;

        float movingSpeed;
        bool movingForward;
        bool movingBackward;
        bool rotateLeft;
        bool rotateRight;

        public List<Town> townsInRadius;

        public GameManager gameManager;

        public LandHumanPlayer(Land world, HumanPlayer human, GameManager gameManager)
            : base(world,human)
        {
            this.gameManager = gameManager;
            guy = new DrawableTexture("whiteSquare", this);
            guy.size.X = 10f;
            guy.size.Y = 10f;

            encounterTrigger.encounterHandlers += CreatureEncountered;

            physicalProperties = new LandPhysicalProperties();
            physicalProperties.activelyColliding = true;
            physicalProperties.radius = 10f;
            physicalProperties.collisionType = LandCollisionTypes.Solid;
        }
        public void CreatureEncountered(LandCreature otherCreature)
        {
            movingSpeed = 0f;
            gameManager.StartFight(creature, otherCreature.creature, position);
        }
        public void Interact(PlayerKeyboard actionKeyboard)
        {
            if (actionKeyboard.keyboard[ActionKeyType.Up].Pressed)
                movingForward = true;
            else
                movingForward = false;

            if (actionKeyboard.keyboard[ActionKeyType.Down].Pressed)
                movingBackward = true;
            else
                movingBackward = false;

            if (actionKeyboard.keyboard[ActionKeyType.Right].Pressed)
                rotateRight = true;
            else
                rotateRight = false;

            if (actionKeyboard.keyboard[ActionKeyType.Left].Pressed)
                rotateLeft = true;
            else
                rotateLeft = false;

        }

        public override void Update(UpdateState state)
        {
            if (movingForward)
                movingSpeed += 0.05f;
            if (movingBackward)
                movingSpeed -= 0.05f;

            if (movingSpeed > 0f)
            {
                if (movingSpeed > 1f)
                    movingSpeed = 1f;

                position += forward * movingSpeed * state.elapsedTime * 200f;
            }
            else
            {
                if (movingSpeed < -1f)
                    movingSpeed = -1f;

                position += forward * movingSpeed * state.elapsedTime * 100f;
            }

            if (!(movingForward || movingBackward))
                if (Math.Abs(movingSpeed) < 0.15f)
                    movingSpeed = 0f;
                else
                    movingSpeed *= 0.9f;

            if (rotateLeft)
                rotation -= state.elapsedTime * 4f;

            if (rotateRight)
                rotation += state.elapsedTime * 4f;


            //Town Detection and Reaction
            townsInRadius  = land.findObjectsInArea<Town>(position, 200f);


            if (townsInRadius.Count != 0)
                guy.filter = Color.Cyan;
            else
                guy.filter = Color.White;

            //MOB encountering
            //creaturesInRange = land.findObjectsInArea<LandCreature>(position, 200f);

            //for (int i = 0; i < creaturesInRange.Count; i++)
            //{
            //    if (creaturesInRange[i].mobProperties != null && creaturesInRange[i].mobProperties.encountering)
            //    {
            //        if ((creaturesInRange[i].position - position).Length() < creaturesInRange[i].mobProperties.encounterRadius + 30f)
            //        {
            //            guy.filter = Color.Red;
            //        }
            //    }
            //}
        }
        public override void Draw(DrawState state)
        {
            guy.Draw(state);
        }

        public float LandContentRequirerRadius(Land land)
        {
            return 300f;
        }
        public Vector2 LandContentRequirerPosition(Land land)
        {
            return position;
        }

        public float LandUpdateNodeRadius(Land land)
        {
            return 400f;
        }
        public Vector2 LandUpdateNodePosition(Land land)
        {
            return position;
        }

        public override bool AuthorizeCollision(LandObject collider)
        {
            return true;
        }

        public override void CollideEffect(LandObject collider)
        {

        }
    }
}
