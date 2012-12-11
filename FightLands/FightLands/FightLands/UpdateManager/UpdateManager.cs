using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FightLands
{
    static class UpdateManager
    {
        static List<UpdateRegister> updatedList;
        static int UpdateID = 0;

        public static void Initialize()
        {
            updatedList = new List<UpdateRegister>();
        }

        public static void Update(GameTime time)
        {
            UpdateID++;

            //Manager Updates
            Statistics.Update(time);

            for (int i = 0; i < updatedList.Count; i++)
            {
                updatedList[i].Update(time,UpdateID);
            }
        }

        public static void addUpdateRegister(UpdateRegister register)
        {
            if (checkIfRegisterExists(register.tag))
                throw new Exception("There already exists a register with the tag \"" + register.tag + "\" in the UpdateManager.");

            updatedList.Add(register);
        }
        public static bool checkIfRegisterExists(String tag)
        {
            for (int i = 0; i < updatedList.Count; i++)
                if (updatedList[i].tag == tag)
                    return true;

            return false;
        }
        public static void removeUpdateRegister(String tag)
        {
            for (int i = 0; i < updatedList.Count; i++)
                if (updatedList[i].tag == tag)
                {
                    updatedList.RemoveAt(i);
                    return;
                }

            throw new Exception("No such register to remove: \"" + tag + "\".");
        }
    }
}
