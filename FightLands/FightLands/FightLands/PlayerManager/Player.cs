using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    abstract class Player
    {

        public enum ActionKeyType { Left, Right, Up, Down }

        protected PlayerKeyboard keyboard;
        List<Controlable> controlables;

        public Player()
        {
            keyboard = new PlayerKeyboard();
            controlables = new List<Controlable>();
        }
        public void addControlable(Controlable controlable)
        {
            controlables.Add(controlable);
        }

        public virtual void Update(UpdateState state)
        {
            for (int i = 0; i < controlables.Count; i++)
                controlables[i].Interact(keyboard);
        }

        public class PlayerKeyboard
        {
            public Dictionary<ActionKeyType, ActionKey> keyboard;
            public PlayerKeyboard()
            {
                for (int i = 0; i < Enum.GetValues(typeof(ActionKeyType)).Length; i++)
                {
                    keyboard.Add((ActionKeyType)i, new ActionKey());
                }
            }
        }
        public class ActionKey
        {
            public enum KeyState{Pressed, Released, JustPressed,JustReleased}

            public KeyState keyState;
            public float timeInState;
            public void Update(float timeSinceLastUpdate, bool isDown)
            {
                switch (keyState)
                {
                    case KeyState.JustPressed:
                        if (isDown)
                        {
                            keyState = KeyState.Pressed;
                            timeInState = timeSinceLastUpdate;
                        }
                        else
                        {
                            keyState = KeyState.JustReleased;
                            timeInState = 0;
                        }
                        break;
                    case KeyState.Pressed:
                        if (isDown)
                            timeInState += timeSinceLastUpdate;
                        else
                            keyState = KeyState.JustReleased;
                        break;
                    case KeyState.JustReleased:
                        if (isDown)
                        {
                            keyState = KeyState.JustPressed;
                            timeInState = 0;
                        }
                        else
                        {
                            keyState = KeyState.Pressed;
                            timeInState += timeSinceLastUpdate;
                        }
                        break;
                    case KeyState.Released:
                        if (isDown)
                            keyState = KeyState.JustPressed;
                        else
                            timeInState += timeSinceLastUpdate;
                        break;
                }
            }
        }
    }
}
