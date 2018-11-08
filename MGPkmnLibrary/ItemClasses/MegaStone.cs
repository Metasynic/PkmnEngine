using MGPkmnLibrary.PokemonClasses;

namespace MGPkmnLibrary.ItemClasses
{
    /* This class inherits from BaseItem, and is a blueprint for a category of item called a Mega Stone.
     * A Mega Stone is an item held by a Pokemon in battle which allows it to temporarily change form.
     * The temporary form is called a "Mega Evolution" and is not a part of the user requirements. */
    public class MegaStone : BaseItem
    {
        /* The basepkmn field holds the species of Pokemon that can be acted on by the Mega Stone.
         * A Mega Stone can only ever act on one specific species of Pokemon. */
        private PokemonData basepkmn;
        public PokemonData BasePkmn
        {
            get { return basepkmn; }
        }

        /* The constructor for a Mega Stone takes the species of Pokemon it acts on, its name, and its price.
         * The species is set in the constructor, and the name and price are passed to the parent constructor.
         * Holdable (the first argument in the parent constructor) is automatically set as true since all Mega Stones are holdable. */
        public MegaStone(PokemonData basepkmn, string name, int price) : base(true, name, price)
        {
            this.basepkmn = basepkmn;
        }

        /* This function clones the Mega Stone to return a new copy of the object.
         * It is used to copy a Mega Stone from the master list in the ItemManager. */
        public override object Clone()
        {
            MegaStone stone = new MegaStone(basepkmn, Name, Price);
            return stone;
        }

        /* This function is not yet used, but it displays the properties of the Mega Stone as a string. */
        public override string ToString()
        {
            string stoneString = "Mega Stone: ";
            stoneString += Name + ", ";
            stoneString += Price + ", ";
            stoneString += Holdable;
            stoneString += basepkmn.PokemonName;
            return stoneString;
        }
    }
}
