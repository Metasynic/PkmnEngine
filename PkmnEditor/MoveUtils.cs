using System;
using api = PokeAPI;
using MGPkmnLibrary.PokemonClasses;
using static PkmnEditor.EditorUtils;
using System.IO;

namespace PkmnEditor
{
    /* This class is a collection of functions relating to Pokemon moves that are used by the editor. */
    public static class MoveUtils
    {
        /* The MoveManager holds all of the currently loaded moves. */
        public static MoveManager MoveManager = new MoveManager();

        /* The DownloadMoves() function downloads the first 621 (which is currently all) the moves from PokeAPI.
         * It is asynchronous as the API wrapper uses an await keyword to download from the API. */
        public static async void DownloadMoves()
        {
            for (int i = 1; i <= 621; i++)
            {
                /* Each move is downloaded from PokeAPI using DataFetcher.GetApiObject<T>(). */
                api.Move m = await api.DataFetcher.GetApiObject<api.Move>(i);

                /* The array of move flags is created and set with every bit as false for now.
                 * The flags are not in the API so I have to make them here. */
                bool[] flags = new bool[20];
                for (int j = 0; j < 20; j++)
                {
                    flags[j] = false;
                }

                /* A new Move is generated and added using the data from the PokeAPI Move object. In several cases, the NullToZero() function is used for conversion. */
                MoveManager.Moves.Add(m.Names[0].Name, new Move(m.Names[0].Name, (byte)NullToZero(m.Power), (byte)NullToZero(m.Accuracy), (sbyte)m.Priority, ParseEnum<MoveCategory>(m.DamageClass.Name), ParseEnum<PkmnType>(m.Type.Name), (byte)NullToZero(m.PP), flags));
                Console.WriteLine("Move #" + i + " (" + m.Name + ") has been added.");
            }
        }

        /* This function is called when the user wants to view a specific move. */
        public static void ViewMove()
        {
            /* The border is updated with the correct title and the program takes an input for the name of the move to be viewed. */
            Border("PkmnEngine Editor - View Move");
            Console.WriteLine("Enter the name of the move you want to view.");
            string input = Console.ReadLine();

            /* If the entered name matches a move in the MoveManager, information about the move is displayed. */
            if (MoveManager.Moves.ContainsKey(input))
            {
                Move move = MoveManager.Moves[input];
                Console.WriteLine("Move \"{0}\"", move.Name);
                Console.WriteLine("Category: {0}, Type: {1}", move.Category, move.Type);
                Console.WriteLine("Power: {0}, Accuracy: {1}, Max PP: {2}", move.Power, move.Accuracy, move.PP);
                Console.WriteLine("Press return to continue.");
                Console.ReadLine();
            }

            /* Otherwise, the user is told that the move doesn't exist, and the program goes back to the menu after half a second. */
            else
            {
                Console.WriteLine("Move {0} does not exist.", input);
                System.Threading.Thread.Sleep(500);
            }
        }

        /* The SaveMoves() function serializes every move in the MoveManager into an XML file in /GameData/MoveData/. */
        public static void SaveMoves()
        {
            Border("PkmnEngine Editor - Save Moves");
            Console.WriteLine("Saving Moves...");
            foreach (string i in MoveManager.Moves.Keys)
            {
                XmlTool.Save(@"../../GameData/MoveData/" + i + ".xml", MoveManager.Moves[i]);
            }
            Console.WriteLine("Serialised successfully!");
            System.Threading.Thread.Sleep(500);
        }

        /* LoadMoves() is run at the start of the program, and loads every valid XML file in /GameData/MoveData/ as a move.
         * The loaded moves are added to the MoveManager. Every 25 moves, a message is displayed on the screen to show progress. */
        public static void LoadMoves()
        {
            Border("PkmnEngine Editor - Load Moves");
            Console.WriteLine("Loading Moves...");

            /* The fileNames array is filled with the names of every XML file in the target folder. */
            string[] fileNames = Directory.GetFiles(@"../../GameData/MoveData/", "*.xml");

            /* Each file is then deserialized into a Move object. */
            foreach (string s in fileNames)
            {
                Move move = XmlTool.Load<Move>(s);
                MoveManager.Moves.Add(move.Name, move);
                if (MoveManager.Moves.Count % 25 == 0)
                    Console.WriteLine(MoveManager.Moves.Count + " moves loaded...");
            }
            Console.WriteLine("Deserialised successfully!");
            System.Threading.Thread.Sleep(500);
        }

        /* This function allows the user to manually add a new move to the MoveManager. */
        public static void NewMove()
        {
            Border("PkmnEngine Editor - New Move");

            /* The program prompts the user for the name of the move, and the user inputs it. */
            Console.WriteLine("Enter Name of move.");
            string name = Console.ReadLine();

            /* If a move with the same name already exists, then the user is given a warning before it is overwritten. */
            if (MoveManager.Moves.ContainsKey(name))
            {
                Console.WriteLine("Move {0} already exists. Overwrite (y/n)?", name);
                string choice = Console.ReadLine();
                if (choice[0] != 'Y' && choice[0] != 'y')
                {
                    /* If the user does not want to override the move, the function returns. */
                    return;
                }

                /* Otherwise, the user is happy to overwrite the move and the function proceeds. */
                Console.WriteLine("Move will be overwitten.");
            }

            /* One by one, every property of the move is entered by the user and parsed into its correct type. */
            Console.WriteLine("Enter Power of move.");
            byte power = byte.Parse(Console.ReadLine());
            Console.WriteLine("Enter Accuracy of move.");
            byte accuracy = byte.Parse(Console.ReadLine());
            Console.WriteLine("Enter Priority of move.");
            sbyte priority = sbyte.Parse(Console.ReadLine());
            Console.WriteLine("Enter Category of move.");
            MoveCategory type = ParseEnum<MoveCategory>(Console.ReadLine());
            Console.WriteLine("Enter Type of move.");
            PkmnType element = ParseEnum<PkmnType>(Console.ReadLine());
            Console.WriteLine("Enter Max PP of move.");
            byte pp = byte.Parse(Console.ReadLine());
            Console.WriteLine("Enter 20 flags of move, as T and F, without spaces.");

            /* When it comes to the array of bit flags, each character in the input is parsed into T -> true, F -> false. */
            char[] flagchars = Console.ReadLine().ToLower().ToCharArray();
            bool[] flags = new bool[20];
            for (int i = 0; i < 20; i++)
            {
                if (flagchars[i] == 't')
                {
                    flags[i] = true;
                }
                else if (flagchars[i] == 'f')
                {
                    flags[i] = false;
                }
                else
                    throw new Exception("Invalid Move Flag was entered.");
            }

            /* Finally, the Move is constructed using the properties entered by the user, and it is added to the MoveManager. */
            Move move = new Move(name, power, accuracy, priority, type, element, pp, flags);
            MoveManager.Moves.Add(move.Name, move);
        }
    }
}
