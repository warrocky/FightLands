using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    interface Controlable
    {
        void Interact(Player.PlayerKeyboard actionKeyboard);
    }
}
