using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace FightLands
{
    class LocalPlayer : Player
    {

        Dictionary<ActionKeyType, Keys> keyMapping;
        public LocalPlayer(Dictionary<ActionKeyType, Keys> keyMapping)
        {
            this.keyMapping = keyMapping;
        }
        public override void Update(UpdateState state)
        {
            KeyboardState keybState = state.keyboardState;

            ActionKeyType keyType;
            int keybLength = Enum.GetValues(typeof(ActionKeyType)).Length;
            for (int i = 0; i < keybLength; i++)
            {
                keyType = (ActionKeyType)i;
                keyboard.keyboard[keyType].Update(state.elapsedTime, keybState.IsKeyDown(keyMapping[keyType]),state.ID);
            }

            base.Update(state);
        }
    }
}
