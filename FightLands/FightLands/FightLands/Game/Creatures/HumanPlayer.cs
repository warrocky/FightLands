using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class HumanPlayer : Human , Controlable, Land.LandContentRequirer, Land.LandUpdateNode
    {
        DrawableTexture guy;

        float movingSpeed;
        bool movingForward;
        bool movingBackward;
        bool rotateLeft;
        bool rotateRight;

        public HumanPlayer(World world)
            : base(world)
        {
            guy = new DrawableTexture("whiteSquare", this);
            guy.size.X = 10f;
            guy.size.Y = 10f;
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
        }
        public override void Draw(DrawState state)
        {
            guy.Draw(state);
        }

        public float LandContentRequirerRadius(Land land)
        {
            return 20f;
        }
        public Vector2 LandContentRequirerPosition(Land land)
        {
            return position;
        }

        public float LandUpdateNodeRadius(Land land)
        {
            return 50f;
        }
        public Vector2 LandUpdateNodePosition(Land land)
        {
            return position;
        }
    }
}
