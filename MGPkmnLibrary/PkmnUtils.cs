using System;
using System.Collections.Generic;
using MGPkmnLibrary.BattleClasses;
using MGPkmnLibrary.PokemonClasses;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MGPkmnLibrary
{
    /* This class is a small library of useful things used throughout the engine. */
    public class PkmnUtils
    {
        /* This Random is used in some of the functions in the class. */
        static Random random = new Random();

        /* These two functions make life a bit easier to me. The regular random number generator is minimum-inclusive and maximum-exclusive.
         * Since I think in terms of inclusivity for both bounds, I created a function to generate random numbers inclusively.
         * One overload generates an integer between zero and the value passed in,
         * The other overload generates an integer between the two values passed in.
         * These functions are used in IV generation and move accuracy calculations. */
        public static int RandomInclusive(int maxValue)
        {
            return random.Next(0, maxValue + 1);
        }
        public static int RandomInclusive(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue + 1);
        }

        /* This Dictionary stores all the possible multipliers for type matchups in the engine.
         * A type matchup is a damage multiplier which is applied when a Pokemon of one type is attacked by a Move of one type.
         * The type matchups are constant and do not change, so they are set using the static functon below.
         * A TypePair (a struct with two PkmnTypes, defined in BattleClasses.Battle) is used as the key to the Dictionary.
         * The Dictionary is essentially a lookup table which is checked when a Move hits a Pokemon.
         * For example, if a water-type Move hits a fire-type Pokemon, a TypePair will be created with PkmnType.Water and PkmnType.Fire.
         * The Dictionary is then searched for a decimal with a key matching the created TypePair.
         * In this example, the TypePair with Water and Fire has an entry in the Dictionary of 2m.
         * This means that a damage multiplier of 2 will be applied, and thus Water is super-effective on Fire. */
        public static Dictionary<TypePair, decimal> TypeMatchups;

        /* This function is called in the PokemonEngine() constructor.
         * It sets all the type matchups correctly according to the ones used in the official games.
         * There are 121 non-one type matchups in total. 
         * Not every possible combination of types has a multiplier - most are a multiplier of one (neutral damage). */
        public static void InitTypeMatchups()
        {
            TypeMatchups = new Dictionary<TypePair, decimal>();
            TypeMatchups.Add(new TypePair(PkmnType.normal, PkmnType.rock), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.normal, PkmnType.ghost), 0m);
            TypeMatchups.Add(new TypePair(PkmnType.normal, PkmnType.steel), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.fire, PkmnType.fire), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.fire, PkmnType.water), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.fire, PkmnType.grass), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.fire, PkmnType.ice), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.fire, PkmnType.bug), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.fire, PkmnType.rock), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.fire, PkmnType.dragon), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.fire, PkmnType.steel), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.water, PkmnType.fire), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.water, PkmnType.water), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.water, PkmnType.grass), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.water, PkmnType.ground), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.water, PkmnType.rock), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.water, PkmnType.dragon), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.electric, PkmnType.water), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.electric, PkmnType.electric), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.electric, PkmnType.grass), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.electric, PkmnType.ground), 0m);
            TypeMatchups.Add(new TypePair(PkmnType.electric, PkmnType.flying), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.electric, PkmnType.dragon), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.grass, PkmnType.fire), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.grass, PkmnType.water), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.grass, PkmnType.grass), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.grass, PkmnType.poison), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.grass, PkmnType.ground), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.grass, PkmnType.flying), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.grass, PkmnType.bug), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.grass, PkmnType.rock), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.grass, PkmnType.dragon), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.grass, PkmnType.steel), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.ice, PkmnType.fire), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.ice, PkmnType.water), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.ice, PkmnType.grass), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.ice, PkmnType.ice), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.ice, PkmnType.ground), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.ice, PkmnType.flying), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.ice, PkmnType.dragon), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.ice, PkmnType.steel), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.fighting, PkmnType.normal), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.fighting, PkmnType.ice), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.fighting, PkmnType.poison), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.fighting, PkmnType.flying), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.fighting, PkmnType.psychic), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.fighting, PkmnType.bug), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.fighting, PkmnType.rock), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.fighting, PkmnType.ghost), 0m);
            TypeMatchups.Add(new TypePair(PkmnType.fighting, PkmnType.dark), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.fighting, PkmnType.steel), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.fighting, PkmnType.fairy), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.poison, PkmnType.grass), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.poison, PkmnType.poison), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.poison, PkmnType.ground), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.poison, PkmnType.rock), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.poison, PkmnType.ghost), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.poison, PkmnType.steel), 0m);
            TypeMatchups.Add(new TypePair(PkmnType.poison, PkmnType.fairy), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.ground, PkmnType.fire), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.ground, PkmnType.electric), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.ground, PkmnType.grass), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.ground, PkmnType.poison), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.ground, PkmnType.flying), 0m);
            TypeMatchups.Add(new TypePair(PkmnType.ground, PkmnType.bug), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.ground, PkmnType.rock), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.ground, PkmnType.steel), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.flying, PkmnType.electric), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.flying, PkmnType.grass), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.flying, PkmnType.fighting), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.flying, PkmnType.bug), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.flying, PkmnType.rock), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.flying, PkmnType.steel), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.psychic, PkmnType.fighting), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.psychic, PkmnType.poison), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.psychic, PkmnType.psychic), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.psychic, PkmnType.dark), 0m);
            TypeMatchups.Add(new TypePair(PkmnType.psychic, PkmnType.steel), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.bug, PkmnType.fire), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.bug, PkmnType.grass), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.bug, PkmnType.fighting), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.bug, PkmnType.poison), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.bug, PkmnType.flying), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.bug, PkmnType.psychic), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.bug, PkmnType.ghost), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.bug, PkmnType.dark), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.bug, PkmnType.steel), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.bug, PkmnType.fairy), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.rock, PkmnType.fire), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.rock, PkmnType.ice), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.rock, PkmnType.fighting), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.rock, PkmnType.ground), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.rock, PkmnType.flying), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.rock, PkmnType.bug), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.rock, PkmnType.steel), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.ghost, PkmnType.normal), 0m);
            TypeMatchups.Add(new TypePair(PkmnType.ghost, PkmnType.psychic), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.ghost, PkmnType.ghost), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.ghost, PkmnType.dark), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.dragon, PkmnType.dragon), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.dragon, PkmnType.steel), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.dragon, PkmnType.fairy), 0m);
            TypeMatchups.Add(new TypePair(PkmnType.dark, PkmnType.fighting), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.dark, PkmnType.psychic), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.dark, PkmnType.ghost), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.dark, PkmnType.dark), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.dark, PkmnType.fairy), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.steel, PkmnType.fire), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.steel, PkmnType.water), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.steel, PkmnType.electric), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.steel, PkmnType.ice), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.steel, PkmnType.rock), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.steel, PkmnType.steel), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.steel, PkmnType.fairy), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.fairy, PkmnType.fire), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.fairy, PkmnType.fighting), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.fairy, PkmnType.poison), 0.5m);
            TypeMatchups.Add(new TypePair(PkmnType.fairy, PkmnType.dragon), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.fairy, PkmnType.dark), 2m);
            TypeMatchups.Add(new TypePair(PkmnType.fairy, PkmnType.steel), 0.5m);
        }

        /* This function splits a string into chunks of a certain length and returns them as an IEnumerable<string>.
         * Any chunks that are not the correct length (the remainder) will be returned as part of the IEnumerable.
         * For example, passing this function a string of length 10 with a maxChunkSize of 3 would yield an IEnumerable with strings of length 3, 3, 3, and 1.
         * It is used by the BattleLog control to split the current string into lines which will fit inside the log. */
        public static IEnumerable<string> SplitString(string input, int chunkSize)
        {
            for (int i = 0; i < input.Length; i += chunkSize)
                yield return input.Substring(i, Math.Min(chunkSize, input.Length - i));
        }

        /* These two functions save/load an object of type T to the Saves folder with the extension ".pks".
         * For both functions, a new FileStream object is created with the file name passed in, 
         * and the FileMode/FileAccess parameters suitable for saving and loading.
         * A BinaryFormatter object is initialized and used to serialize/deserialize the object to/from the FileStream. */
        public static void BinarySave<T>(T data, string name)
        {
            using (FileStream stream = new FileStream(@"../../../../../Saves/" + name + ".pks", FileMode.OpenOrCreate, FileAccess.Write))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, data);
            }
        }
        public static T BinaryLoad<T>(string name)
        {
            using (FileStream stream = new FileStream(name, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

        /* NextLevel() returns the amount of XP needed to get from the level passed in, to the next level.
         * It takes a GrowthRate from the Pokemon it's finding out the amount of XP for.
         * It calls ToLevel() twice, passing in the level and the level plus one.
         * Subtracting them from each other gives the final value to be returned. */
        public static int NextLevel(GrowthRate growthRate, byte bLevel)
        {
            return (ToLevel(growthRate, (byte)(bLevel + 1))) - (ToLevel(growthRate, (bLevel)));
        }

        /* ToLevel() returns the amount of XP needed to reach a certain level from zero experience, based on a GrowthRate. */
        public static int ToLevel(GrowthRate growthRate, byte bLevel)
        {
            /* The byte passed in is converted to an integer for accuracy purposes. */
            int level = bLevel;

            /* The function applies different formulae to the desired level, depending on the GrowthRate and sometimes the level itself.
             * The formulae are mostly cubic in nature, and zero is returned if there is no valid GrowthRate. */
            switch(growthRate)
            {
                case GrowthRate.erratic:
                    if (level <= 50)
                        return (int)(((Math.Pow(level, 3)) * (100 - level)) / 50);
                    else if (level > 50 && level <= 68)
                        return (int)(((Math.Pow(level, 3)) * (150 - level)) / 100);
                    else if (level > 68 && level <= 98)
                        return (int)((Math.Pow(level, 3)) * ((1911 - (10 * level)) / 3));
                    else
                        return (int)(((Math.Pow(level, 3)) * (160 - level)) / 100);
                case GrowthRate.fast:
                    return (int)((4 * (Math.Pow(level, 3))) / 5);
                case GrowthRate.mediumfast:
                    return (int)(Math.Pow(level, 3));
                case GrowthRate.mediumslow:
                    return (int)(((6 * (Math.Pow(level, 3))) / 5) - (15 * (Math.Pow(level, 2))) + (100 * level) - 140);
                case GrowthRate.slow:
                    return (int)((5 * (Math.Pow(level, 3))) / 4);
                case GrowthRate.fluctuating:
                    if (level <= 15)
                        return (int)((Math.Pow(level, 3)) * ((((level + 1) / 3) + 24) / 50));
                    else if (level > 15 && level <= 36)
                        return (int)((Math.Pow(level, 3)) * ((level + 14) / 50));
                    else
                        return (int)((Math.Pow(level, 3)) * (((level / 2) + 32) / 50));
                default:
                    return 0;
            }
        }
    }

    /* A ProportionValue<T> object contains a Proportion out of one representing the chance of it being chosen.
     * It also has a Value of any type T which is returned when the ProportionValue<T> is chosen. */
    public class ProportionValue<T>
    {
        public double Proportion { get; set; }
        public T Value { get; set; }
    }

    /* This static class contains two important methods related to a ProportionValue<T>. */
    public static class ProportionValue
    {
        /* This function creates a new ProportionValue<T> using the proportion and value passed in. */
        public static ProportionValue<T> Create<T>(double proportion, T value)
        {
            return new ProportionValue<T> { Proportion = proportion, Value = value };
        }

        /* A Random object is needed for the next function to choose a ProportionValue in a random manner. */
        static Random random = new Random();

        /* This function chooses one of the proportion values in the a list passed in, using the proportions in the list items. */
        public static T ChooseByRandom<T>(this IEnumerable<ProportionValue<T>> list)
        {
            /* Firstly, a random double between zero and one is generated. */
            double r = random.NextDouble();

            /* Since the proportions in the list should add up to one, a value can be chosen proportionally.
             * The function iterates through the items in the list, and checks if the random double is less than the proportion of the current item.
             * If it is, the item is chosen and returned; if it isn't, the item's proportion is removed from the random double.
             * This means that a higher proportion is more likely to be chosen than a lower proportion. */
            foreach (var item in list)
            {
                if (r < item.Proportion)
                    return item.Value;
                r -= item.Proportion;
            }

            /* This code will only run if nothing has been returned, which would mean the proportions don't add up properly. */
            throw new InvalidOperationException("The proportions in the list do not add up to one.");
        }
    }
}
