using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    class FactionManager 
    {
        Land land;

        List<Faction> factionList;

        public FactionManager(Land land)
        {
            this.land = land;
            factionList = new List<Faction>();
        }
    }
}
