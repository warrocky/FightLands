using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    abstract class Creature
    {
        Land land;
        public bool alive;
        public int strength;
        public int dexterity;
        public int inteligence;
        public Faction faction;

        public LandCreature landCreature;

        public Creature(Land land)
        {
            this.land = land;
            alive = true;
        }

        public FightCreature createFightCreature(FightWorld fight)
        {
            return new FightCreature(this, fight);
        }
        public virtual AssetTexture getFightTexture()
        {
            return AssetManager.getAssetTexture("whiteSquare");
        }
        public virtual AssetTexture getLandTexture()
        {
            return AssetManager.getAssetTexture("whiteSquare");
        }
        public bool checkIfAgressiveTowards(Creature creature)
        {
            if (faction == null)
                return true;
            else
                return faction.checkIfAgressive(creature.faction);

        }
        public void destroy()
        {
            alive = false;
            landCreature.destroy();
        }
    }
}
