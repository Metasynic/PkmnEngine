using System;
using MGPkmnLibrary.PokemonClasses;
using api = PokeAPI;
using static PkmnEditor.EditorUtils;
using System.IO;

namespace PkmnEditor
{
    /* This class handles all functions related to saving, loading, and editing PokemonData in the program. */
    public static class PokemonDataUtils
    {
        /* The PokemonDataManager holds all the PokemonData objects currently loaded in. */
        public static PokemonDataManager PokemonDataManager = new PokemonDataManager();

        /* This method asyncronously downloads the first 721 Pokemon species from PokeAPI. */
        public static async void DownloadPokemon()
        {
            for (int k = 0; k <= 721; k++)
            {
                /* Both the relevant PokeAPI.Pokemon and the PokeAPI.PokemonSpecies objects are downloaded from PokeAPI. */
                api.Pokemon p = await api.DataFetcher.GetApiObject<api.Pokemon>(k);
                api.PokemonSpecies ps = await api.DataFetcher.GetApiObject<api.PokemonSpecies>(k);

                /* The PokemonData's EvYield is created using the PokeAPI.Pokemon's stat effort values. */
                EvYield evYield = new EvYield((byte)p.Stats[5].Effort, (byte)p.Stats[4].Effort, (byte)p.Stats[3].Effort, (byte)p.Stats[2].Effort, (byte)p.Stats[1].Effort, (byte)p.Stats[0].Effort);

                /* The constructor for the new PokemonData is called with different parameters depending on whether it's a dual type and has two egg groups. */
                if (p.Types.Length == 1)
                {
                    if (ps.EggGroups.Length == 1)
                    {
                        /* This constructor is used if the Types and EggGroups both have a length of one. Null is passed as the second type and second egg group. */
                        PokemonDataManager.PokemonData.Add((ushort)p.ID, new PokemonData(p.Name, (byte)p.Stats[5].BaseValue, (byte)p.Stats[4].BaseValue, (byte)p.Stats[3].BaseValue, (byte)p.Stats[2].BaseValue, (byte)p.Stats[1].BaseValue, (byte)p.Stats[0].BaseValue, (ushort)p.ID, ParseEnum<PkmnType>(p.Types[0].Type.Name), null, ps.FemaleToMaleRate != -1, ps.Genera[0].Name, (double)p.Height / 10, (double)p.Height / 10, ParseEnum<EggGroup>(FixEnum(ps.EggGroups[0].Name)), null, (ushort)p.BaseExperience, (byte)ps.CaptureRate, (byte)ps.BaseHappiness, ParseEnum<GrowthRate>(FixEnum(ps.GrowthRate.Name)), (byte)ps.HatchCounter, (byte)(100 - (byte)(ps.FemaleToMaleRate * 100)), (byte)(ps.FemaleToMaleRate * 100), evYield));
                    }
                    else // ps.EggGroups.Length == 2
                    {
                        /* This constructor is used if the Pokemon is single type and has two egg groups. */
                        PokemonDataManager.PokemonData.Add((ushort)p.ID, new PokemonData(p.Name, (byte)p.Stats[5].BaseValue, (byte)p.Stats[4].BaseValue, (byte)p.Stats[3].BaseValue, (byte)p.Stats[2].BaseValue, (byte)p.Stats[1].BaseValue, (byte)p.Stats[0].BaseValue, (ushort)p.ID, ParseEnum<PkmnType>(p.Types[0].Type.Name), null, ps.FemaleToMaleRate != -1, ps.Genera[0].Name, (double)p.Height / 10, (double)p.Height / 10, ParseEnum<EggGroup>(FixEnum(ps.EggGroups[0].Name)), ParseEnum<EggGroup>(FixEnum(ps.EggGroups[1].Name)), (ushort)p.BaseExperience, (byte)ps.CaptureRate, (byte)ps.BaseHappiness, ParseEnum<GrowthRate>(FixEnum(ps.GrowthRate.Name)), (byte)ps.HatchCounter, (byte)(100 - (byte)(ps.FemaleToMaleRate * 100)), (byte)(ps.FemaleToMaleRate * 100), evYield));
                    }
                }
                else // p.Types.Length == 2
                {
                    if (ps.EggGroups.Length == 1)
                    {
                        /* Similarly, this constructor call creates a Pokemon with dual type and one egg group. */
                        PokemonDataManager.PokemonData.Add((ushort)p.ID, new PokemonData(p.Name, (byte)p.Stats[5].BaseValue, (byte)p.Stats[4].BaseValue, (byte)p.Stats[3].BaseValue, (byte)p.Stats[2].BaseValue, (byte)p.Stats[1].BaseValue, (byte)p.Stats[0].BaseValue, (ushort)p.ID, ParseEnum<PkmnType>(p.Types[0].Type.Name), ParseEnum<PkmnType>(p.Types[1].Type.Name), ps.FemaleToMaleRate != -1, ps.Genera[0].Name, (double)p.Height / 10, (double)p.Height / 10, ParseEnum<EggGroup>(FixEnum(ps.EggGroups[0].Name)), null, (ushort)p.BaseExperience, (byte)ps.CaptureRate, (byte)ps.BaseHappiness, ParseEnum<GrowthRate>(FixEnum(ps.GrowthRate.Name)), (byte)ps.HatchCounter, (byte)(100 - (byte)(ps.FemaleToMaleRate * 100)), (byte)(ps.FemaleToMaleRate * 100), evYield));
                    }
                    else // ps.EggGroups.Length == 2
                    {
                        /* Finally, this constructor call creates a Pokemon with dual type and two egg groups. */
                        PokemonDataManager.PokemonData.Add((ushort)p.ID, new PokemonData(p.Name, (byte)p.Stats[5].BaseValue, (byte)p.Stats[4].BaseValue, (byte)p.Stats[3].BaseValue, (byte)p.Stats[2].BaseValue, (byte)p.Stats[1].BaseValue, (byte)p.Stats[0].BaseValue, (ushort)p.ID, ParseEnum<PkmnType>(p.Types[0].Type.Name), ParseEnum<PkmnType>(p.Types[1].Type.Name), ps.FemaleToMaleRate != -1, ps.Genera[0].Name, (double)p.Height / 10, (double)p.Height / 10, ParseEnum<EggGroup>(FixEnum(ps.EggGroups[0].Name)), ParseEnum<EggGroup>(FixEnum(ps.EggGroups[1].Name)), (ushort)p.BaseExperience, (byte)ps.CaptureRate, (byte)ps.BaseHappiness, ParseEnum<GrowthRate>(FixEnum(ps.GrowthRate.Name)), (byte)ps.HatchCounter, (byte)(100 - (byte)(ps.FemaleToMaleRate * 100)), (byte)(ps.FemaleToMaleRate * 100), evYield));
                    }
                }
                Console.WriteLine("Pokemon # " + k + " (" + p.Name + ") has been added.");
            }
        }

        /* ViewPokemon() is called when the user chooses to look at an already existing Pokemon. */
        public static void ViewPokemon()
        {
            Border("PkmnEngine Editor - View Pokemon");

            /* First, the user enters the ID of their chosen Pokemon. */
            Console.WriteLine("Enter the ID of the Pokemon you wish to view.");
            ushort input = ushort.Parse(Console.ReadLine());

            /* If a Pokemon with that ID exists in the PokemonDataManager, then an overview of that Pokemon is printed to the screen. */
            if (PokemonDataManager.PokemonData.ContainsKey(input))
            {
                PokemonData pokemonData = PokemonDataManager.PokemonData[input];
                if (pokemonData.Type.Count == 2)
                {
                    Console.WriteLine("Pokemon #{0}, Name: \"{1}\", Types: {2}, {3}", pokemonData.ID, pokemonData.PokemonName, pokemonData.Type[0], pokemonData.Type[1]);
                }
                else
                {
                    Console.WriteLine("Pokemon #{0}, Name: \"{1}\", Type: {2}", pokemonData.ID, pokemonData.PokemonName, pokemonData.Type[0]);
                }
                Console.WriteLine("Base HP: {0}", pokemonData.BaseHP);
                Console.WriteLine("Base Attack: {0}", pokemonData.BaseAttack);
                Console.WriteLine("Base Defence: {0}", pokemonData.BaseDefence);
                Console.WriteLine("Base Special Attack: {0}", pokemonData.BaseSpecialAttack);
                Console.WriteLine("Base Special Defence: {0}", pokemonData.BaseSpecialDefence);
                Console.WriteLine("Base Speed: {0}", pokemonData.BaseSpeed);
                int baseStatTotal = pokemonData.BaseHP + pokemonData.BaseAttack + pokemonData.BaseDefence + pokemonData.BaseSpecialAttack + pokemonData.BaseSpecialDefence + pokemonData.BaseSpeed;
                Console.WriteLine("Base Stat Total: {0}, Gendered: {1}", baseStatTotal, pokemonData.CanHaveGender);
                Console.WriteLine("Press return to continue.");
                Console.ReadLine();
            }

            /* Otherwise, the user has put in the ID wrong, and the program returns to the main menu. */
            else
            {
                Console.WriteLine("Pokemon with ID {0} not found.", input);
                System.Threading.Thread.Sleep(500);
            }
        }

        /* SavePokemon() serializes all the items in the PokemonDataManager into XML files in /GameData/PokemonData/. */
        public static void SavePokemon()
        {
            Border("PkmnEngine Editor - Save Pokemon");
            Console.WriteLine("Saving Pokemon...");
            foreach (ushort i in PokemonDataManager.PokemonData.Keys)
            {
                XmlTool.Save(@"../../GameData/PokemonData/" + i + ".xml", PokemonDataManager.PokemonData[i]);
            }
            Console.WriteLine("Serialised successfully!");
            System.Threading.Thread.Sleep(500);
        }

        /* LoadPokemon() reads in all valid XML files in /GameData/PokemonData/ and adds them to the PokemonDataManager. */
        public static void LoadPokemon()
        {
            Border("PkmnEngine Editor - Load Pokemon");
            Console.WriteLine("Loading Pokemon...");

            /* The fileNames array is filled with the names of each XML file in the target folder using Directory.GetFiles(). */
            string[] fileNames = Directory.GetFiles(@"../../GameData/PokemonData/", "*.xml");

            /* Each file is then loaded in and deserialized into a PokemonData object. */
            foreach (string s in fileNames)
            {
                PokemonData pokemonData = XmlTool.Load<PokemonData>(s);
                PokemonDataManager.PokemonData.Add(pokemonData.ID, pokemonData);
                if (PokemonDataManager.PokemonData.Count % 25 == 0)
                    Console.WriteLine(PokemonDataManager.PokemonData.Count + " Pokemon loaded...");
            }
            Console.WriteLine("Deserialised successfully!");
            System.Threading.Thread.Sleep(500);
        }

        /* This function allows a user to add a new Pokemon species manually. */
        public static void AddPokemon()
        {
            Border("PkmnEngine Editor - Add New Pokemon");

            /* First, the user enters the ID for the Pokemon they're adding. */
            Console.WriteLine("Enter the ID of the Pokemon you wish to add.");
            ushort id = ushort.Parse(Console.ReadLine());

            /* If a PokemonData with that ID already exists, the user is prompted asking if they want to overwrite it. */
            if (PokemonDataManager.PokemonData.ContainsKey(id))
            {
                Console.WriteLine("Pokemon with ID {0} already exists [{1}]. Overwrite? (y/n)", id, PokemonDataManager.PokemonData[id].PokemonName);
                string choice = Console.ReadLine();

                /* If they don't want to overwrite, the function returns. */
                if (choice[0] != 'y' && choice[0] != 'Y')
                {
                    return;
                }
                Console.WriteLine("Pokemon will be overwritten.");
            }

            /* If the user chooses to overwrite the PokemonData, they will then enter all the new information about the PokemonData. */
            Console.WriteLine("Enter name of Pokemon.");
            string name = Console.ReadLine();
            Console.WriteLine("Enter Base HP of Pokemon.");
            byte baseHP = byte.Parse(Console.ReadLine());
            Console.WriteLine("Enter Base Attack of Pokemon.");
            byte baseAttack = byte.Parse(Console.ReadLine());
            Console.WriteLine("Enter Base Defence of Pokemon.");
            byte baseDefence = byte.Parse(Console.ReadLine());
            Console.WriteLine("Enter Base Special Attack of Pokemon.");
            byte baseSpecialAttack = byte.Parse(Console.ReadLine());
            Console.WriteLine("Enter Base Special Defence of Pokemon.");
            byte baseSpecialDefence = byte.Parse(Console.ReadLine());
            Console.WriteLine("Enter Base Speed of Pokemon.");
            byte baseSpeed = byte.Parse(Console.ReadLine());

            Console.WriteLine("Enter whether Pokemon can have genders (True/False).");
            bool canHaveGender = bool.Parse(Console.ReadLine());
            Console.WriteLine("Enter type of Pokemon.");
            PkmnType type = ParseEnum<PkmnType>(Console.ReadLine());
            Console.WriteLine("Enter second type of Pokemon (null for single types).");
            string secondType = Console.ReadLine();
            Console.WriteLine("Enter species description, e.g. Gyarados is \"Atrocious\".");
            string species = Console.ReadLine();
            species = species + " Pokemon"; // Converts "Atrocious" into "Atrocious Pokemon", for example
            Console.WriteLine("Enter the weight of the Pokemon in KG.");
            double weightKg = double.Parse(Console.ReadLine());
            Console.WriteLine("Enter the height of the Pokemon in M.");
            double heightM = double.Parse(Console.ReadLine());

            Console.WriteLine("Enter the Egg Group of the Pokemon.");
            EggGroup eggGroup = ParseEnum<EggGroup>(Console.ReadLine());
            Console.WriteLine("Enter the second Egg Group of the Pokemon (can be null).");
            string secondEggGroup = Console.ReadLine();
            Console.WriteLine("Enter the Base Experience of the Pokemon.");
            ushort baseExp = ushort.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Capture Rate of the Pokemon.");
            byte captureRate = byte.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Base Happiness of the Pokemon.");
            byte baseHappiness = byte.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Growth Rate of the Pokemon.");
            GrowthRate growthRate = ParseEnum<GrowthRate>(Console.ReadLine());
            Console.WriteLine("Enter the Male Gender Ratio of the Pokemon out of 100 (0 for genderless).");
            byte maleRatio = byte.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Female Gender Ratio of the Pokemon out of 100 (0 for genderless).");
            byte femaleRatio = byte.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Egg Cycle count of the Pokemon.");
            byte eggCycles = byte.Parse(Console.ReadLine());

            Console.WriteLine("Enter the HP EV Yield of the Pokemon.");
            byte hpEvYield = byte.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Attack EV Yield of the Pokemon.");
            byte atkEvYield = byte.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Defence EV Yield of the Pokemon.");
            byte defEvYield = byte.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Special Attack EV Yield of the Pokemon.");
            byte spAtkEvYield = byte.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Special Defence EV Yield of the Pokemon.");
            byte spDefEvYield = byte.Parse(Console.ReadLine());
            Console.WriteLine("Enter the Speed EV Yield of the Pokemon.");
            byte spdEvYield = byte.Parse(Console.ReadLine());
            EvYield evYield = new EvYield(hpEvYield, atkEvYield, defEvYield, spAtkEvYield, spDefEvYield, spdEvYield);

            /* Once everything has been calculated, the base stat total is calculated and displayed. */
            int stattotal = (baseHP + baseAttack + baseDefence + baseSpecialAttack + baseSpecialDefence + baseSpeed);
            Console.WriteLine("Stat total is {0}. Adding Pokemon...", stattotal);

            /* One of four possible constructor calls is made, depending on whether the secondType and secondEggGroup are null or not. */
            if (secondType == "null" || secondType == "Null")
            {
                if (secondEggGroup == "null" || secondEggGroup == "Null")
                {
                    PokemonDataManager.PokemonData.Add(id, new PokemonData(name, baseHP, baseAttack, baseDefence, baseSpecialAttack, baseSpecialDefence, baseSpeed, id, type, null, canHaveGender, species, weightKg, heightM, eggGroup, null, baseExp, captureRate, baseHappiness, growthRate, eggCycles, maleRatio, femaleRatio, evYield));
                }
                else
                {
                    EggGroup otherEggGroup = ParseEnum<EggGroup>(secondEggGroup);
                    PokemonDataManager.PokemonData.Add(id, new PokemonData(name, baseHP, baseAttack, baseDefence, baseSpecialAttack, baseSpecialDefence, baseSpeed, id, type, null, canHaveGender, species, weightKg, heightM, eggGroup, otherEggGroup, baseExp, captureRate, baseHappiness, growthRate, eggCycles, maleRatio, femaleRatio, evYield));
                }
            }
            else // secondType != "null" || secondType != "Null"
            {
                PkmnType otherType = ParseEnum<PkmnType>(secondType);
                if (secondEggGroup == "null" || secondEggGroup == "Null")
                {
                    PokemonDataManager.PokemonData.Add(id, new PokemonData(name, baseHP, baseAttack, baseDefence, baseSpecialAttack, baseSpecialDefence, baseSpeed, id, type, otherType, canHaveGender, species, weightKg, heightM, eggGroup, null, baseExp, captureRate, baseHappiness, growthRate, eggCycles, maleRatio, femaleRatio, evYield));
                }
                else
                {
                    EggGroup otherEggGroup = ParseEnum<EggGroup>(secondEggGroup);
                    PokemonDataManager.PokemonData.Add(id, new PokemonData(name, baseHP, baseAttack, baseDefence, baseSpecialAttack, baseSpecialDefence, baseSpeed, id, type, otherType, canHaveGender, species, weightKg, heightM, eggGroup, otherEggGroup, baseExp, captureRate, baseHappiness, growthRate, eggCycles, maleRatio, femaleRatio, evYield));
                }
            }
            Console.WriteLine("{0} has been added successfully!", name);
            System.Threading.Thread.Sleep(2000);
        }
    }
}
