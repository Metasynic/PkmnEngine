using System.Collections.Generic;

namespace MGPkmnLibrary.ItemClasses
{
    /* This class acts as a master list for all BaseItems in the game.
     * It contains different lists for different types of BaseItem. */
    public class ItemManager
    {
        /* These three private dictionaries store the different items in the manager.
         * MiscItems have strings as keys because they are stored by name.
         * MegaStones are stored according to the ID of the Pokemon they act on.
         * Berries are stored using their BerryType as the key. */
        Dictionary<string, BaseItem> miscItems = new Dictionary<string, BaseItem>();
        Dictionary<ushort, MegaStone> megaStones = new Dictionary<ushort, MegaStone>();
        Dictionary<BerryType, Berry> berries = new Dictionary<BerryType, Berry>();

        /* These three KeyCollection fields expose the list of keys in each dictionary.
         * They are currently not used and may be redundant. */
        public Dictionary<string, BaseItem>.KeyCollection MiscItemKeys
        {
            get { return miscItems.Keys; }
        }
        public Dictionary<ushort, MegaStone>.KeyCollection MegaStoneKeys
        {
            get { return megaStones.Keys; }
        }
        public Dictionary<BerryType, Berry>.KeyCollection BerryKeys
        {
            get { return berries.Keys; }
        }

        /* The ItemManager() constructor is empty as the dictionaries were initialized after being declared. */
        public ItemManager()
        {

        }

        /* This function adds an item to the list of items.
         * It checks if there's already an item with the same key in the list.
         * If there isn't, the item is added to the list.
         * Adding items to the misc list uses the item's name as the key.
         * Adding mega stones uses the stone's BasePkmn field's ID.
         * Adding berries uses the berry's BerryType. */
        public void AddMiscItem(BaseItem item)
        {
            if (!miscItems.ContainsKey(item.Name))
            {
                miscItems.Add(item.Name, item);
            }
        }
        public void AddMegaStone(MegaStone stone)
        {
            if (!megaStones.ContainsKey(stone.BasePkmn.ID))
            {
                megaStones.Add(stone.BasePkmn.ID, stone);
            }
        }
        public void AddBerry(Berry berry)
        {
            if (!berries.ContainsKey(berry.BerryType))
            {
                miscItems.Add(berry.Name, berry);
            }
        }

        /* This function clones an item from the list of items.
         * It has to clone the item otherwise the function would return a reference to the item in the list.
         * These functions will be used whenever a new item is added to the world or inventory. */
        public BaseItem GetMiscItem(string name)
        {
            if (miscItems.ContainsKey(name))
            {
                return (BaseItem)miscItems[name].Clone();
            }
            return null;
        }
        public MegaStone GetMegaStone(ushort id)
        {
            if (megaStones.ContainsKey(id))
            {
                return (MegaStone)megaStones[id].Clone();
            }
            return null;
        }
        public Berry GetBerry(BerryType type)
        {
            if (berries.ContainsKey(type))
            {
                return (Berry)berries[type].Clone();
            }
            return null;
        }

        /* This function returns a bit representing whether the specified key is already in the item list. */
        public bool ContainsMiscItem(string name)
        {
            return miscItems.ContainsKey(name);
        }
        public bool ContainsMegaStone(ushort id)
        {
            return megaStones.ContainsKey(id);
        }
        public bool ContainsBerry(BerryType type)
        {
            return berries.ContainsKey(type);
        }
    }
}
