using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    static class PlayerManager
    {
        static List<Player> players;
        static List<String> tags;

        public static void Initialize()
        {
            players = new List<Player>();
            tags = new List<string>();
        }
        public static void addPlayer(Player player, String tag)
        {
            if(checkIfTagExists(tag))
                throw new Exception("A player with that tag already exists in the PlayerManager.");

            players.Add(player);
            tags.Add(tag);
        }

        public static bool checkIfTagExists(String tag)
        {
            for (int i = 0; i < tags.Count; i++)
                if (tag == tags[i])
                    return true;

            return false;
        }
        public static Player getPlayer(String tag)
        {
            for (int i = 0; i < tags.Count; i++)
                if (tag == tags[i])
                    return players[i];

            throw new Exception("No player with such tag: \"" + tag + "\".");
        }
        public static bool checkIfPlayerExists(Player player)
        {
            for (int i = 0; i<players.Count; i++)
                if (players[i].ID == player.ID)
                    return true;

            return false;
        }
        public static String getTag(Player player)
        {
            for (int i = 0; i < players.Count; i++)
                if (players[i].ID == player.ID)
                    return tags[i];

            throw new Exception("Player does not exist.");
        }

        public static void Update(UpdateState state)
        {
            for (int i = 0; i < players.Count; i++)
                players[i].Update(state);
        }
    }

}
