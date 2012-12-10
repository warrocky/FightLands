using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    /// <summary>
    /// The interface that a generic entry in a Menu needs to implement.
    /// </summary>
    interface MenuEntry
    {
        /// <summary>
        /// A method that fires when the entry is selected.
        /// </summary>
        void Select();
        /// <summary>
        /// A method that fires when the entry is focused.
        /// </summary>
        void Focus();
        /// <summary>
        /// A method that fires when the entry is unfocused.
        /// </summary>
        void Unfocus();
    }
}
