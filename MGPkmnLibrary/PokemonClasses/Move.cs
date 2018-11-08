using System;

namespace MGPkmnLibrary.PokemonClasses
{
    /* Pokemon Moves are divided into three different categories, represented by an enum.
     * Physical moves use the Attack/Defence stats.
     * Special moves use the Special Attack/Special Defence stats.
     * Status moves don't use stats, but instead change stat stages or inflict status conditions. */
    public enum MoveCategory { physical, special, status }

    /* This class represents a Move that can be used by a Pokemon. A Pokemon can have up to four moves.
     * Different moves have differing effects in battle. */
    [Serializable]
    public class Move
    {
        /* The flags array contains twenty bits of information about the move.
         * These bits are needed for certain moves to work on other Pokemon.
         * 
         * The flags represent whether the move:
         * 1. Makes contact
         * 2. Has a charge turn before the move is used
         * 3. Has a recharge turn after the move is used
         * 4. Can be blocked by the moves Protect and Detect
         * 5. Can be reflected using Magic Coat/Magic Bounce
         * 6. Can be stolen using the move Snatch
         * 7. Can be copied using Mirror Move
         * 8. Is punch based
         * 9. Is sound based
         * 10. Is not usable when "Gravity" is in effect.
         * 11. Defrosts the opposing Pokemon when used.
         * 12. Can be used in triple battles to target the furthest Pokemon away from the user.
         * 13. Is a healing move
         * 14. Ignores Substitute constructs
         * 15. Is powder based
         * 16. Is jaw based
         * 17. Is pulse based
         * 18. Is ballistics based
         * 19. Has mental effects
         * 20. Is not usable during sky battles. */
        protected bool[] flags;
        public bool[] Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        /* The type field represents the PkmnType of the move, such as fire or ice.
         * The type of the move affects whether it does super effective damage or not. */
        protected PkmnType type;
        public PkmnType Type
        {
            get { return type; }
            set { type = value; }
        }

        /* The category of the move (physical, special, or status). */
        protected MoveCategory category;
        public MoveCategory Category
        {
            get { return category; }
            set { category = value; }
        }

        /* The name of the move. */
        protected string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /* The power byte represents the base power of the move. 
         * This number is used when determining the damage done by the move.
         * A higher base power means more damage is done.
         * Base power ranges from roughly 20 to 150. */
        protected byte power;
        public byte Power
        {
            get { return power; }
            set { power = value; }
        }

        /* The accuracy value of a Move represents a percentage between 0 and 100.
         * This percentage is how likely a Move is to hit its target. */
        protected byte accuracy;
        public byte Accuracy
        {
            get { return accuracy; }
            set { accuracy = value; }
        }

        /* This AttributePair is the PP of a move.
         * The PP of a move is how many times it can be used before it runs out.
         * Each time a move it used, the PP decreases by one. */
        private AttributePair pp;
        public AttributePair PP
        {
            get { return pp; }
            set { pp = value; }
        }

        /* The priority of a Move shows when that Move should take place when Moves are sorted into what order they should be used in.
         * A Move with a higher priority takes place before a move with a lower priority. */
        private sbyte priority;
        public sbyte Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        /* The private constructor for a move initializes everything with null values.
         * It is used when Moves are deserialized from XML. */
        private Move()
        {
            name = "";
            power = 0;
            accuracy = 0;
            Category = MoveCategory.physical;
            Type = PkmnType.normal;
            pp = new AttributePair(0);
            priority = 0;
            flags = new bool[20];
        }

        /* The main Move constructor takes the name, power, accuracy, priority, type, maximum PP, and flags of the move.
         * All the parameters are set appropriately. Array.Copy() is used to copy the flags array. */
        public Move(string name, byte power, byte accuracy, sbyte priority, MoveCategory moveCategory, PkmnType moveType, byte maxPP, bool[] flags)
        {
            this.name = name;
            this.power = power;
            this.accuracy = accuracy;
            this.priority = priority;
            this.category = moveCategory;
            this.type = moveType;
            pp = new AttributePair(maxPP);
            this.flags = new bool[20];
            Array.Copy(flags, this.flags, 20);
        }
    }
}
