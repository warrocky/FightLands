using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FightLands
{
    /// <summary>
    /// A GameObject class that handles a linear Menu of generic entrys, handles Focus, Unfocus and Select.
    /// </summary>
    /// <typeparam name="T">A generic type implementing the MenuEntry Interface</typeparam>
    class Menu<T> : GameObject where T : MenuEntry
    {
        protected List<T> entries;

        /// <summary>
        /// Check true if this menu loops from the last entry to the first one. If it is false the menu will resselect the last/first entry.
        /// </summary>
        public bool loopable;

        /// <summary>
        /// Check true if this menu refocuses entries when reselecting a entry.
        /// </summary>
        public bool refocus;

        private int _selectedEntry;
        /// <summary>
        /// Property that returns the current selected entry. Set to select a new entry firing associated events.
        /// </summary>
        public int selectedEntry
        {
            get { return _selectedEntry; }
            set 
            {
                if (entries.Count == 0)
                    return;

                //No entry selected at the moment
                if (_selectedEntry == -1)
                {
                    if (value < 0)
                        if (loopable)
                            _selectedEntry = entries.Count - 1; // loop to last
                        else
                            _selectedEntry = 0; //go to floor
                    else
                        if (value >= entries.Count)
                            if (loopable)
                                _selectedEntry = value % entries.Count; // loop
                            else
                                _selectedEntry = entries.Count - 1; // go to last
                        else
                            _selectedEntry = value; //go to entry

                    FocusEntry(_selectedEntry);
                }

                if (loopable)
                {
                    //Is loopable
                    if ((_selectedEntry != value % entries.Count) || refocus)
                    {
                        //Refocus is innactive or current entry is different than new.
                        UnfocusEntry(_selectedEntry);
                        _selectedEntry = value % entries.Count;
                        FocusEntry(_selectedEntry);
                    }
                }
                else
                    if (value >= entries.Count)
                    {
                        if (refocus || _selectedEntry != entries.Count - 1)
                        {
                            //Refocus is innactive or current entry is different than last.
                            UnfocusEntry(_selectedEntry);
                            _selectedEntry = entries.Count - 1;
                            FocusEntry(_selectedEntry);
                        }
                    }
                    else
                        if (value < 0)
                        {
                            if (refocus || _selectedEntry != 0)
                            {
                                //Refocus is innactive or current entry is different than first.
                                UnfocusEntry(_selectedEntry);
                                _selectedEntry = 0;
                                FocusEntry(_selectedEntry);
                            }
                        }
                        else
                        {
                            //Regular entry change.
                            UnfocusEntry(_selectedEntry);
                            _selectedEntry = value;
                            FocusEntry(_selectedEntry);
                        }
            }
        }

        public Menu(World world)
            :base(world)
        {
            entries = new List<T>();
            _selectedEntry = -1;
        }


        /// <summary>
        /// An overridable method that fires when a entry is unselected.
        /// </summary>
        /// <param name="index"></param>
        protected virtual void UnfocusEntry(int index)
        {
            entries[index].Unfocus();
        }
        /// <summary>
        /// An overridable method that fires when a entry is selected.
        /// </summary>
        /// <param name="index"></param>
        protected virtual void FocusEntry(int index)
        {
            entries[index].Focus();
        }
        /// <summary>
        /// A method that selects the currentEntry.
        /// </summary>
        protected virtual void SelectCurrentEntry()
        {
            //if (_selectedEntry == -1)
            //    throw new Exception("No entry selected.");

            if(selectedEntry != -1)
                entries[selectedEntry].Select();
        }

        /// <summary>
        /// A method that Deselects the current selected entry and Unfocuses it.
        /// </summary>
        public void DeselectCurrentEntry()
        {
            entries[_selectedEntry].Unfocus();
            _selectedEntry = -1;
        }
    }
}
