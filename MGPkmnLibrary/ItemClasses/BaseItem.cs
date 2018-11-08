using System;

namespace MGPkmnLibrary.ItemClasses
{
    /* This is the parent item class, from which all other items inherit.
     * Items are anything that can be stored in the player's bag. */
    [Serializable]
    public class BaseItem
    {
        /* A BaseItem has four essential fields.
         * Holdable is a bit indicating whether the item can be held in battle by a Pokemon.
         * Name is a descriptor for the item.
         * Price is the amount of money required to buy the item from a shop (not used yet).
         * IsHeld is a bit describing whether the item is currently being held by a Pokemon. */
        bool holdable;
        string name;
        int price;
        bool isHeld;
        public bool Holdable
        {
            get { return holdable; }
            set { holdable = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Price
        {
            get { return price; }
            set { price = value; }
        }
        public bool IsHeld
        {
            get { return isHeld; }
            set { isHeld = value; }
        }

        /* This is a private constructor for use when deserializing an item object from XML, and when using Clone(). */
        private BaseItem()
        {

        }

        /* The constructor takes the holdable, name, and price as parameters.
         * IsHeld is false by default, as the majority of items start off not being held by a Pokemon. */
        public BaseItem(bool holdable, string name, int price)
        {
            Holdable = holdable;
            Name = name;
            Price = price;
            IsHeld = false;
        }

        /* Unfinished functions for when the item is used, in or out of battle.
         * Will probably be replaced in the relevant Inventory or Battle screen. */
        public virtual void UseInBattle()
        {

        }
        public virtual void UseOutBattle()
        {

        }

        /* The clone function returns a new, exact copy of the BaseItem.
         * The function is there so that when an item is added to the inventory,
         * One can be cloned into the Inventory, instead of referencing the item in the master item list. */
        public virtual object Clone()
        {
            BaseItem item = new BaseItem();
            item.Holdable = this.holdable;
            item.IsHeld = this.isHeld;
            item.Name = this.name;
            item.Price = this.price;
            return item;
        }

        /* This function describes the BaseItem as a string. It currently serves no purpose and may be removed. */
        public override string ToString()
        {
            string itemString = "";
            itemString += Name + ", ";
            itemString += Price.ToString() + ", ";
            itemString += Holdable.ToString();
            return itemString;
        }
    }
}
