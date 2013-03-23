using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    class LandCreature : LandObject
    {
        public Creature creature;
        public Encounter encounterTrigger;

        public LandCreature(Creature creature, Land land)
            :base(land)
        {
            this.creature = creature;
            creature.landCreature = this;
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
