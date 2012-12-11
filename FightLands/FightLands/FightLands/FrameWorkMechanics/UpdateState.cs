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
        /// <summary>
        /// The elapsed float time in seconds.
        /// </summary>
        public readonly float elapsedTime;
        /// <summary>
        /// The number of Updates since the beginning of the application.
        /// </summary>
        public readonly int GlobalUpdateID;
        /// <summary>
        /// This register UpdateID. As in the number of times the Update root under this register has been updated.
        /// </summary>
        public readonly int RegisterUpdateID;
        /// <summary>
        /// The register managing this update root update cycle.
        /// </summary>
        public readonly UpdateRegister register;

        public UpdateState(float elapsedTime, int GlobalID, int RegisterID, UpdateRegister register)
        {
            this.GlobalUpdateID = GlobalID;
            this.RegisterUpdateID = RegisterID;
            this.elapsedTime = elapsedTime;
            this.register = register;
        }
    }
}
