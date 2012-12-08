using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    static class PlayerManager
    {
        static List<Player> players;

        public static void Initialize()
        {
            players = new List<Player>();
        }
        public static void addPlayer(Player player)
        {
            players.Add(player);
        }

        public static void Update(UpdateState state)
        {
            for (int i = 0; i < players.Count; i++)
                players[i].Update(state);
        }
    }

}
