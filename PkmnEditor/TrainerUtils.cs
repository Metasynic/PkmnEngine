using System;
using MGPkmnLibrary.PokemonClasses;
using static PkmnEditor.EditorUtils;
using System.IO;

namespace PkmnEditor
{
    /* This class manages all of the editor functions related to NPC trainers. */
    public class TrainerUtils
    {
        /* The TrainerManager holds all the Trainers loaded in. */
        public static TrainerManager TrainerManager = new TrainerManager();

        /* ViewTrainer() allows the user to view an NPC Trainer. */
        public static void ViewTrainer(PokemonDataManager pokemonDataManager)
        {
            Border("PkmnEngine Editor - View Trainer");

            /* The user inputs the trainer's name. */
            Console.WriteLine("Enter name of trainer:");
            string input = Console.ReadLine();

            /* The program checks to see if a Trainer with that name exists.
             * If it does, information about the Trainer with that name is displayed to the screen. */
            if (TrainerManager.Trainers.ContainsKey(input))
            {
                Trainer trainer = TrainerManager.Trainers[input];
                Console.WriteLine("Trainer Name: {0}, Rank: {1}, Difficulty: {2}", trainer.TrainerName, trainer.Rank, trainer.Difficulty);
                Console.WriteLine("Trainer Dialogue: \"{0}\".", trainer.Dialogue);

                Console.WriteLine("Press return to continue.");
                Console.ReadLine();
                Border("PkmnEngine Editor - View Trainer");

                /* In addition to the Trainer information, the species, stats and moves of each of the Trainer's Pokemon team are displayed. */
                for (int i = 0; i < 6; i++)
                {
                    if (trainer.TrainerPokemon[i] != null)
                    {
                        Pokemon pokemon = trainer.TrainerPokemon[i];
                        Console.WriteLine("Pokemon #{0}: No. {1} ({2}), Level {3}, Gender: {4}", i + 1, pokemon.PokemonID, pokemon.Nickname, pokemon.Level, pokemon.PokemonGender);
                        Console.WriteLine("STATS | HP: {0} | ATK: {1} | DEF: {2} | SP. ATK: {3} | SP. DEF: {4} | SPD: {5}", pokemon.HP, pokemon.Attack, pokemon.Defence, pokemon.SpecialAttack, pokemon.SpecialDefence, pokemon.Speed);
                        Console.WriteLine("IVs | HP: {0} | ATK: {1} | DEF: {2} | SP. ATK: {3} | SP. DEF: {4} | SPD: {5}", pokemon.IV_HP, pokemon.IV_Attack, pokemon.IV_Defence, pokemon.IV_SpecialAttack, pokemon.IV_SpecialDefence, pokemon.IV_Speed);
                        Console.WriteLine("Moves: ");
                        for (int j = 0; j < 4; j++)
                        {
                            if (pokemon.Moves[j] != null)
                            {
                                Console.Write("{0} ", pokemon.Moves[j]);
                            }
                        }
                        Console.WriteLine();
                    }
                }
                Console.WriteLine("\nPress return to continue.");
                Console.ReadLine();
            }

            /* Alternatively, if no trainer is found with the requested name, the user goes back to the main menu screen. */
            else
            {
                Console.WriteLine("Trainer with name {0} does not exist.", input);
                System.Threading.Thread.Sleep(500);
            }
        }

        /* LoadTrainers() deserializes all the saved XML trainer files from /GameData/TrainerData/. */
        public static void LoadTrainers()
        {
            Border("PkmnEngine Editor - Load Trainers");
            Console.WriteLine("Loading Trainers...");

            /* A list of the names of XML files in the target folder is obtained using Directory.GetFiles(). */
            string[] fileNames = Directory.GetFiles(@"../../GameData/TrainerData/", "*.xml");

            /* Each XML file is then loaded into the program and added to the TrainerManager. */
            foreach (string s in fileNames)
            {
                Trainer trainer = XmlTool.Load<Trainer>(s);
                TrainerManager.Trainers.Add(trainer.TrainerName, trainer);
            }
            Console.WriteLine("Deserialised successfully!");
            System.Threading.Thread.Sleep(500);
        }

        /* SaveTrainers() serializes every Trainer in the TrainerManager to an XML file in /GameData/TrainerData/. */
        public static void SaveTrainers()
        {
            Border("PkmnEngine Editor - Save Trainers");
            Console.WriteLine("Saving Trainers...");
            foreach (string i in TrainerManager.Trainers.Keys)
            {
                XmlTool.Save(@"../../GameData/TrainerData/" + i + ".xml", TrainerManager.Trainers[i]);
            }
            Console.WriteLine("Serialised successfully!");
            System.Threading.Thread.Sleep(500);
        }

        /* The AddTrainer() function allows the user to add an NPC trainer manually.
         * It needs the PokemonDataManager and MoveManager as arguments since it uses them to create the trainer's team. */
        public static void AddTrainer(PokemonDataManager pokemonDataManager, MoveManager moveManager)
        {
            Border("PkmnEngine Editor - New Trainer");

            /* The user is prompted to enter the Trainer's name. */
            Console.WriteLine("Enter Trainer Name (must be unique for now)");
            string name = Console.ReadLine();

            /* If the TrainerManager already contains a Trainer with the entered name, the user is prompted to ask if they want to overwrite it. */
            if (TrainerManager.Trainers.ContainsKey(name))
            {
                Console.WriteLine("Trainer with name {0} already exists. Overwrite? (y/n)", TrainerManager.Trainers[name].TrainerName);
                string choice = Console.ReadLine();

                /* If the user doesn't want to overwrite, the function returns. */
                if (choice[0] != 'y' && choice[0] != 'Y')
                {
                    return;
                }
                Console.WriteLine("Trainer will be overwritten.");
            }

            /* Otherwise, the user will then enter all the Trainer's details into the program. */
            Console.WriteLine("Enter Trainer Rank (Normal, Grunt, Elite, GymLeader, EliteFour, Champion)");
            TrainerRank rank = ParseEnum<TrainerRank>(Console.ReadLine());
            Console.WriteLine("Enter Trainer Difficulty (VeryEasy, Easy, Normal, Hard, VeryHard)");
            TrainerDifficulty difficulty = ParseEnum<TrainerDifficulty>(Console.ReadLine());
            Console.WriteLine("How many Pokemon does the trainer have? (Max 6)");
            byte pokemonCount = byte.Parse(Console.ReadLine());
            Pokemon[] pokemonTeam = new Pokemon[6];
            System.Threading.Thread.Sleep(500);

            /* After taking the Trainer information, the program then gets data about the Trainer's Pokemon team from the user.
             * The floor loop iterates through each Pokemon in the team according to the count specified by the user. */
            for (int i = 0; i < pokemonCount; i++)
            {
                Border("PkmnEngine Editor - New Trainer - Pokemon #" + (i + 1));
                Console.WriteLine("Enter ID of Pokemon species.");
                byte id = byte.Parse(Console.ReadLine());
                PokemonData species = pokemonDataManager.PokemonData[id];
                Console.WriteLine("Enter level of Pokemon. (1-100)");
                byte level = byte.Parse(Console.ReadLine());
                Gender gender;
                if (species.CanHaveGender)
                {
                    Console.WriteLine("Enter gender of Pokemon (Male/Female).");
                    gender = ParseEnum<Gender>(Console.ReadLine());
                }
                else
                {
                    gender = Gender.none;
                }
                Console.WriteLine("How many moves does the Pokemon have?");
                byte moveCount = byte.Parse(Console.ReadLine());
                Move[] pokemonMoves = new Move[4];

                /* Similarly to the iteraton through the Pokemon team, the program also iterates through each Pokemon's moveset. */
                for (int j = 0; j < moveCount; j++)
                {
                    Console.WriteLine("Enter name of move #" + (j + 1));
                    pokemonMoves[j] = moveManager.Moves[Console.ReadLine()];
                }

                /* Each new Pokemon is constructed using the parameters passed in, and added to the pokemonTeam array. */
                Pokemon pokemon = new Pokemon(species, gender, pokemonMoves, level, rank);
                pokemonTeam[i] = pokemon;
                System.Threading.Thread.Sleep(500);
            }
            Border("PkmnEngine Editor - Add Pokemon");

            /* Finally, the user enters the Trainer dialogue (what the NPC says when you fight them). */
            Console.WriteLine("Enter the test dialogue for the trainer:");
            string dialogue = Console.ReadLine();

            /* The new Trainer is constructed and added to the TrainerManager. */
            Trainer trainer = new Trainer(name, rank, difficulty, pokemonTeam, dialogue, pokemonCount);
            TrainerManager.AddTrainer(trainer);
        }
    }
}
