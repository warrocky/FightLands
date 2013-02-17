using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    class LandCreature : LandObject
    {
        public MobProperties mobProperties;

        public LandCreature(Land land)
            :base(land)
        {

        }
    }

    class MobProperties
    {
        public float encounterRadius;
        public bool encountering;

        public MobProperties(float radius, bool encountering)
        {
            this.encountering = encountering;
            this.encounterRadius = radius;
        }
    }

    class Encounter : LandObject
    {
        public float radius;
        public LandCreature creature;

        public Encounter(Land land, LandCreature creature, float radius)
            :base(land)
        {
            this.creature = creature;
            this.radius = radius;
        }
    }
}
