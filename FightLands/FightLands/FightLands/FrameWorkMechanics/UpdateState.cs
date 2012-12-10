using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FightLands
{
    /// <summary>
    /// A class that carrys the Update state.
    /// </summary>
    class UpdateState
    {
        static int currentID;

        public UpdateState(GameTime time, KeyboardState keybState)
        {
            ID = currentID;
            currentID++;
            this.time = time;
            elapsedTime = ((float)time.ElapsedGameTime.Milliseconds)/1000f;
            keyboardState = keybState;
        }
        public readonly int ID;
        public readonly GameTime time;
        /// <summary>
        /// The elapsed float time in seconds.
        /// </summary>
        public readonly float elapsedTime;

        public readonly KeyboardState keyboardState;
    }
}
