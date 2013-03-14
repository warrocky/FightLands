using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    class Faction
    {
        String name;
        Land land;

        List<Faction> allies;
        List<Faction> enemys;

        bool agressiveTowardsNeutrals;
        bool allyTowardsNeutrals;

        public Faction(Land land)
        {
            this.land = land;
            allies = new List<Faction>();
            enemys = new List<Faction>();
            agressiveTowardsNeutrals = false;
            allyTowardsNeutrals = false;
        }

        public bool checkIfAgressive(Faction faction)
        {
            if (faction == null)
                if (agressiveTowardsNeutrals)
                    return true;
                else
                    return false;

            for (int i = 0; i < enemys.Count; i++)
                if (faction == enemys[i])
                    return true;

            return false;
        }
        public bool checkIfAlly(Faction faction)
        {
            if (faction == null)
                if (allyTowardsNeutrals)
                    return true;
                else
                    return false;

            for (int i = 0; i < allies.Count; i++)
                if (faction == allies[i])
                    return true;

            return false;
        }
    }
}
