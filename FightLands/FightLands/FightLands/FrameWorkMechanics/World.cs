using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    class World : Updatable
    {
        protected List<GameObject> objectList;
        protected List<GameObject> objectAddList;
        protected List<GameObject> objectRemoveList;

        public World()
        {
            objectList = new List<GameObject>();
            objectAddList = new List<GameObject>();
            objectRemoveList = new List<GameObject>();
        }

        public virtual void Update(UpdateState state)
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
        public virtual void Draw(DrawState state)
        {
            for (int i = 0; i < objectList.Count; i++)
                objectList[i].Draw(state);
        }

        /// <summary>
        /// Adds the object to the world removal list and removes it without destroying it at the end of the first world Update cycle to occur.
        /// </summary>
        /// <param name="gameObject">The GameObject to be removed from the list.</param>
        public void RemoveObject(GameObject gameObject)
        {
            objectRemoveList.Add(gameObject);
        }
        /// <summary>
        /// Adds the object to the world addition list and adds it at the end of the first world Update cycle to occur.
        /// </summary>
        /// <param name="gameObject">The GameObject to be added.</param>
        public void AddObject(GameObject gameObject)
        {
            objectAddList.Add(gameObject);
        }
    }
}
