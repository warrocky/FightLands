using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    abstract class Creature
    {
        Land land;
        public int strength;
        public int dexterity;
        public int inteligence;
        public Faction faction;

        public Creature(Land land)
        {
            this.land = land;
        }

        public bool checkIfAgressiveTowards(Creature creature)
        {
            if (faction == null)
                return true;
            else
                return faction.checkIfAgressive(creature.faction);

        }
    }
}
