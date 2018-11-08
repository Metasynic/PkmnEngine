namespace MGPkmnLibrary.ItemClasses
{
    /* This extremely long enum stores the different possible types of berry. */
    public enum BerryType { Cheri, Chesto, Pecha, Rawst, Aspear, Leppa, Oran, Persim, Lum, Sitrus, Figy, Wiki, Mago, Aguav, Iapapa, Razz, Bluk, Nanab, Wepear, Pinap, Pomeg, Kelpsy, Qualot, Hondew, Grepa, Tamato, Cornn, Magost, Rabuta, Nomel, Spelon, Pamtre, Watmel, Durin, Belue, Occa, Passho, Wacan, Rindo, Yache, Chople, Kebia, Shuca, Coba, Payapa, Tanga, Charti, Kasib, Haban, Colbur, Babiri, Chilan, Liechi, Ganlon, Salac, Petaya, Apicot, Lansat, Starf, Enigma, Micle, Custap, Jaboca, Rowap, Roseli, Kee, Maranga }

    /* The Berry class is essentially a BaseItem with a BerryType attached to it. */
    public class Berry : BaseItem
    {
        /* The berryType field stores the BerryType of the Berry. */
        private BerryType berryType;
        public BerryType BerryType
        {
            get { return berryType; }
        }

        /* The Berry constructor only takes a BerryType.
         * The parent BaseItem is initialized using Holdable as true.
         * The item name is created by using the Berry Type and the word "Berry".
         * For example, the name of a Berry with the Razz Type will be "Razz Berry".
         * The price is set to 0, as berries cannot be bought or sold. */
        public Berry(BerryType berryType) : base(true, berryType.ToString() + " Berry", 0)
        {
            this.berryType = berryType;
        }

        /* The Clone() method will be used when a new berry is picked up by the player.
         * It returns an exact copy of the Berry, which will go into the player inventory. */
        public override object Clone()
        {
            Berry berry = new Berry(this.berryType);
            return berry;
        }

        /* The ToString() function describes the Berry as a string.
         * It currently has no use. */
        public override string ToString()
        {
            string berryString = "Berry: ";
            berryString += berryType.ToString();
            berryString += Name;
            berryString += Price;
            return berryString;
        }
    }
}
