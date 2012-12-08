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
            KeyboardState keybState = Keyboard.GetState();

            for (int i = 0; i < Enum.GetValues(typeof(ActionKeyType)).Length; i++)
                keyboard.keyboard[(ActionKeyType)i].Update(state.elapsedTime, keybState.IsKeyDown(keyMapping[(ActionKeyType)i]));

            base.Update(state);
        }
    }
}
