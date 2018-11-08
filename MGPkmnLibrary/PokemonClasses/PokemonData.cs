using System;
using System.Collections.Generic;

namespace MGPkmnLibrary.PokemonClasses
{
    /* This struct stores the likelihood for a newly generated Pokemon to be male or female. */
    public struct GenderRatio
    {
        /* Both of these values represent the percentage change of a new Pokemon having the specified gender. */
        public byte male;
        public byte female;

        /* The constructor takes the proportions and sets them if they add up to 100.
         * If they are both zero, this means the Pokemon is genderless.
         * If they're not zero but don't add up to 100, the ratios have not been entered correctly, and it defaults to 50/50. */
        public GenderRatio(byte male, byte female)
        {
            /* If the ratios add to 100, they can be set directly as they're correct. */
            if (male + female == 100)
            {
                this.male = male;
                this.female = female;
            }

            /* This block handles genderless Pokemon. User created genderless Pokemon will have ratios of 0,
             * but genderless Pokemon downloaded from the PokeAPI library return the strange ratio of (250, 156).
             * The strange ratio is converted into (0,0) when it's set. */
            else if ((male == 0 && female == 0) || (male == 250 && female == 156))
            {
                this.male = 0;
                this.female = 0;
            }

            /* This is another strange ratio returned by the PokeAPI when the Pokemon is exclusively female.
             * Curiously, exclusively male Pokemon do not return an odd ratio, but instead the normal (100, 0). */
            else if (male == 254 && female == 102)
            {
                this.male = 0;
                this.female = 100;
            }

            /* If the ratio hasn't been handled yet then it's invalid, so 50/50 is assigned to avoid crashes. */
            else
            {
                this.male = 50;
                this.female = 50;
                Console.WriteLine("ERROR: " + male + ", " + female + " ratio. 50/50 assigned.");
            }
        }
    }

    /* The EvYield struct represents what EVs are given to the player Pokemon when an opposing Pokemon is defeated in battle. */
    public struct EvYield
    {
        /* Each byte is how many EVs are given for that particular stat. */
        public byte HpYield;
        public byte AtkYield;
        public byte DefYield;
        public byte SpAtkYield;
        public byte SpDefYield;
        public byte SpdYield;
        
        /* The constructor just takes the six bytes and sets them. */
        public EvYield(byte hp, byte atk, byte def, byte spAtk, byte spDef, byte spd)
        {
            HpYield = hp;
            AtkYield = atk;
            DefYield = def;
            SpAtkYield = spAtk;
            SpDefYield = spDef;
            SpdYield = spd;
        }
    }

    /* The PokemonData class is a blueprint for a species of Pokemon.
     * Fields common to all Pokemon of the same species are stored in here. */
    public class PokemonData
    {
        /* The name of the Pokemon species. */
        public string PokemonName;

        /* The six base stats of the Pokemon species. */
        public byte BaseHP;
        public byte BaseAttack;
        public byte BaseDefence;
        public byte BaseSpecialAttack;
        public byte BaseSpecialDefence;
        public byte BaseSpeed;

        /* The ID of the Pokemon species. */
        public ushort ID;

        /* The (one or two) types of the Pokemon species. */
        public List<PkmnType> Type;

        /* This bit represents whether the Pokemon species is genderless or not. */
        public bool CanHaveGender;

        /* A descriptor for the species, e.g. Gyarados is "Atrocious". */
        public string Species;

        /* The weight and height of the Pokemon species.
         * The weight of a Pokemon determines the effectiveness of some moves such as Low Kick. */
        public double WeightKg;
        public double HeightM;

        /* This List stores the (one or two) Egg Groups that the Pokemon has. */
        public List<EggGroup> EggGroups;

        /* These five fields have various purposes.
         * BaseExp is a factor in determining how much XP is given for defeating one of these Pokemon in battle.
         * CaptureRate determines how easy it is to catch one of these Pokemon.
         * BaseHappiness is the happiness value that the Pokemon starts with.
         * GrowthRate is an enum dictating how much XP is required to level up.
         * EggCycles represents how many cycles of 256 steps it takes to hatch an egg containing one of these Pokemon. */
        public ushort BaseExp;
        public byte CaptureRate;
        public byte BaseHappiness;
        public GrowthRate GrowthRate;
        public byte EggCycles;

        /* The GenderRatio is how likely a Pokemon is to have a certain gender upon being created. */
        public GenderRatio GenderRatio;

        /* The EvYield has how many EV points are given to the player's Pokemon after defeating a Pokemon of this species. */
        public EvYield EvYield;

        /* There are three things that still need adding here:
         * #1. A list detailing the moves the Pokemon can learn, and what level they are learned at.
         * #2. A list of what abilities the Pokemon can have, and whether the ability is hidden.
         * #3. The new Pokemon species that this Pokemon species could evolve into and how. */
        
        /* The private constructor initializes everything as null.
         * It's used for deserializing PokemonData objects from an XML file. */
        private PokemonData()
        {
            Type = new List<PkmnType>();
            EggGroups = new List<EggGroup>();
        }

        /* The public constructor for a PokemonData takes a lot of parameters. */
        public PokemonData(string pokemonName, byte baseHP, byte baseAttack, byte baseDefence, byte baseSpecialAttack, byte baseSpecialDefence, byte baseSpeed, ushort id, PkmnType type, PkmnType? secondType, bool canHaveGender, string species, double weightKg, double heightM, EggGroup eggGroup, EggGroup? secondEggGroup, ushort baseExp, byte captureRate, byte baseHappiness, GrowthRate growthRate, byte eggCycles, byte maleRatio, byte femaleRatio, EvYield evYield)
        {
            /* The majority of the fields are set directly using the parameters passed in. */
            PokemonName = pokemonName;
            BaseHP = baseHP;
            BaseAttack = baseAttack;
            BaseDefence = baseDefence;
            BaseSpecialAttack = baseSpecialAttack;
            BaseSpecialDefence = baseSpecialDefence;
            BaseSpeed = baseSpeed;
            ID = id;
            CanHaveGender = canHaveGender;
            Species = species;
            WeightKg = weightKg;
            HeightM = heightM;
            BaseExp = baseExp;
            CaptureRate = captureRate;
            BaseHappiness = baseHappiness;
            GrowthRate = growthRate;
            EggCycles = eggCycles;
            GenderRatio = new GenderRatio(maleRatio, femaleRatio);
            EvYield = evYield;

            /* For the Type and EggGroups properties, which can contain one or two values each,
             * the second parameter can be passed as null.
             * If the second parameter is null, the lists will only contain one item.
             * If the second parameter has a value, the lists will contain two items. */
            Type = new List<PkmnType>();
            Type.Add(type);
            if (secondType.HasValue)
            {
                Type.Add(secondType.Value);
            }
            EggGroups = new List<EggGroup>();
            EggGroups.Add(eggGroup);
            if (secondEggGroup.HasValue)
            {
                EggGroups.Add(secondEggGroup.Value);
            }
        }

        /* This function represents the entire object as a string. It't not used and probably never will be. */
        public override string ToString()
        {
            string toString = "";
            toString += "Name: " + PokemonName.ToString() + ", ";
            toString += "Base HP: " + BaseHP.ToString() + ", ";
            toString += "Base Attack: " + BaseAttack.ToString() + ", ";
            toString += "Base Defence: " + BaseDefence.ToString() + ", ";
            toString += "Base Sp Attack: " + BaseSpecialAttack.ToString() + ", ";
            toString += "Base Sp Defence: " + BaseSpecialDefence.ToString() + ", ";
            toString += "Base Speed: " + BaseSpeed.ToString() + ", ";
            toString += "ID: #" + ID.ToString() + ", ";
            toString += "Gendered: " + CanHaveGender.ToString() + ", ";
            toString += "Type:" + Type.ToString() + ".";

            return toString;
        }

        /* This function returns an exact copy of the PokemonData object.
         * It's used so that if a PokemonData needs to be copied out of the master list in DataManager,
         * it will use a fresh copy of the object, not just a reference. */
        public object Clone()
        {
            PokemonData data = new PokemonData();
            data.PokemonName = this.PokemonName;
            data.BaseHP = this.BaseHP;
            data.BaseAttack = this.BaseAttack;
            data.BaseDefence = this.BaseDefence;
            data.BaseSpecialAttack = this.BaseSpecialAttack;
            data.BaseSpecialDefence = this.BaseSpecialDefence;
            data.BaseSpeed = this.BaseSpeed;
            data.Type = this.Type;
            data.ID = this.ID;
            data.CanHaveGender = this.CanHaveGender;
            data.Species = this.Species;
            data.WeightKg = this.WeightKg;
            data.HeightM = this.HeightM;
            data.EggGroups = this.EggGroups;
            data.BaseExp = this.BaseExp;
            data.CaptureRate = this.CaptureRate;
            data.BaseHappiness = this.BaseHappiness;
            data.GrowthRate = this.GrowthRate;
            data.EggCycles = this.EggCycles;
            data.GenderRatio = this.GenderRatio;
            data.EvYield = this.EvYield;
            return data;
        }
    }
}
