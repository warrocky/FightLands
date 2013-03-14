using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    class LandCreature : LandObject
    {
        Creature creature;
        Encounter encounterTrigger;

        public LandCreature(Creature creatur, Land land)
            :base(land)
        {

            encounterTrigger = new Encounter(this, land);
        }

        public override void destroy()
        {
            encounterTrigger.destroy();
            base.destroy();
        }

        public virtual bool authorizeEncounter(LandCreature boogie)
        {
            return creature.checkIfAgressiveTowards(boogie.creature);
        }
    }
}
