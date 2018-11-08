using System.Collections.Generic;

namespace MGPkmnLibrary.ItemClasses
{
    /* The bag is an inventory to hold the player's current items.
     * It contains different lists of GameItems to represent the pockets in the bag. */
    public class Bag
    {
        /* These three lists represent the three pockets containing miscellaneous items, mega stones, and berries. */
        readonly List<GameItem> miscItems;
        readonly List<GameItem> megaStones;
        readonly List<GameItem> berries;
        public List<GameItem> MiscItems
        {
            get { return miscItems; }
        }
        public List<GameItem> MegaStones
        {
            get { return megaStones; }
        }
        public List<GameItem> Berries
        {
            get { return berries; }
        }

        /* This property returns the total count of everything contained in the three pockets. */
        public int TotalCapacity
        {
            get { return miscItems.Count + megaStones.Count + berries.Count; }
        }

        /* The constructor for a bag just initializes the three pocket lists. */
        public Bag()
        {
            miscItems = new List<GameItem>();
            megaStones = new List<GameItem>();
            berries = new List<GameItem>();
        }

        /* This function adds an item to the bag. It will eventually work out which pocket to put the item in.
         * Currently, the Add() function adds the newly passed in GameItem to the miscellaneous items pocket. */
        public void Add(GameItem gameItem)
        {
            miscItems.Add(gameItem);
        }

        /* The Remove() function is not finished. It will take the pocket index and item index of the item to be removed. */
        public void Remove(GameItem gameItem)
        {
            miscItems.Remove(gameItem);
        }
    }
}
