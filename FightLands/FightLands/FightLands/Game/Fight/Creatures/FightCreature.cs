using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    class FightCreature : FightObject , Controlable
    {
        DrawableTexture texture;
        bool inFloor;

        public FightCreature(Creature creature, FightWorld fight)
            : base(fight)
        {
            texture = new DrawableTexture(creature.getFightTexture(), this);
            texture.size.X = 30f;
        }
        public override void Draw(DrawState state)
        {
            texture.Draw(state);
        }
        public override void Update(UpdateState state)
        {
            velocity *= 0.90f;
            velocity.Y += 1.2f;

            base.Update(state);

            if (position.Y > fight.floorLevel)
            {
                inFloor = true;
                velocity.Y = 0f;
                position.Y = fight.floorLevel;
            }
            else
                inFloor = false;
        }

        public void Interact(PlayerKeyboard actionKeyboard)
        {
            if (actionKeyboard.keyboard[ActionKeyType.Left].keyState == ActionKey.KeyState.Pressed)
                position.X -= 5f;

            if (actionKeyboard.keyboard[ActionKeyType.Right].keyState == ActionKey.KeyState.Pressed)
                position.X += 5f;

            if ((actionKeyboard.keyboard[ActionKeyType.Up].Pressed) && inFloor)
                velocity.Y -= 40f;
        }
    }
}
