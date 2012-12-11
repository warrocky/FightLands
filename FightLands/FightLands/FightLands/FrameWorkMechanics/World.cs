using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    class World : Updatable
    {
        List<GameObject> objectList;
        List<GameObject> objectAddList;
        List<GameObject> objectRemoveList;

        public World()
        {
            objectList = new List<GameObject>();
            objectAddList = new List<GameObject>();
            objectRemoveList = new List<GameObject>();
        }

        public void Update(UpdateState state)
        {
            for (int i = 0; i < objectList.Count; i++)
            {
                objectList[i].Update(state);
            }

            //Remove objects awaiting removal
            while (objectRemoveList.Count != 0)
            {
                objectList.Remove(objectRemoveList[0]);
                objectRemoveList.RemoveAt(0);
            }

            //Add adicional objects
            while (objectAddList.Count != 0)
            {
                objectList.Add(objectAddList[0]);
                objectAddList.RemoveAt(0);
            }
        }
        public void Draw(DrawState state)
        {
            for (int i = 0; i < objectList.Count; i++)
                objectList[i].Draw(state);
        }

        public void RemoveObject(GameObject gameObject)
        {
            objectRemoveList.Add(gameObject);
        }
        public void AddObject(GameObject gameObject)
        {
            objectAddList.Add(gameObject);
        }
    }
}
