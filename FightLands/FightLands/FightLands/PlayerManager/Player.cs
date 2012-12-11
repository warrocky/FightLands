using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    abstract class Player : Updatable
    {
        static int currentID;
        /// <summary>
        /// A unique ID for each instance of player that starts on 0.
        /// </summary>
        public readonly int ID;

        protected PlayerKeyboard keyboard;
        List<Controlable> controlables;

        public Player()
        {
            ID = currentID;
            currentID++;

            keyboard = new PlayerKeyboard();
            controlables = new List<Controlable>();
        }
        /// <summary>
        /// A method to add a Controlable to the player, that Controlable will receive input state from the player as long as the player is being updated.
        /// </summary>
        /// <param name="controlable">A object that implements the Controlable interface.</param>
        public void addControlable(Controlable controlable)
        {
            controlables.Add(controlable);
        }
        /// <summary>
        /// A method to remove a controlable from the list of controlables the player is pushing input to.
        /// </summary>
        /// <param name="controlable">The controlable to remove.</param>
        public void removeControlable(Controlable controlable)
        {
            controlables.Remove(controlable);
        }

        public virtual void Update(UpdateState state)
        {
            for (int i = 0; i < controlables.Count; i++)
                controlables[i].Interact(keyboard);
        }
    }

    public enum ActionKeyType { Left, Right, Up, Down }
    public class PlayerKeyboard
    {
        public Dictionary<ActionKeyType, ActionKey> keyboard;

        public PlayerKeyboard()
        {
            keyboard = new Dictionary<ActionKeyType, ActionKey>();

            for (int i = 0; i < Enum.GetValues(typeof(ActionKeyType)).Length; i++)
            {
                keyboard.Add((ActionKeyType)i, new ActionKey());
            }
        }
    }
    public class ActionKey
    {
        public enum KeyState { Pressed, Released, JustPressed, JustReleased }

        public KeyState keyState;
        public float timeInState;

        //HACKS
        public bool JustPressed { get { return keyState == KeyState.JustPressed; } }
        public bool Pressed { get { return keyState == KeyState.Pressed; } }
        public bool JustReleased { get { return keyState == KeyState.JustReleased; } }
        public bool Released { get { return keyState == KeyState.Released; } }

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
                        keyState = KeyState.Released;
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
