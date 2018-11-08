using System;
using static PkmnEditor.EditorUtils;
using static PkmnEditor.MoveUtils;
using static PkmnEditor.PokemonDataUtils;
using static PkmnEditor.TrainerUtils;

namespace PkmnEditor
{
    /* This is the main class for the PkmnEditor program, a command line editor which allows manual addition and editing of Pokemon, Moves, and Trainers.
     * The functions in the program are organized into static classes, which are imported above, for better readability. */
    class Program
    {
        /* The Main() function is run when the program is run. It acts as a wrapper for the rest of the program. */
        static void Main(string[] args)
        {
            /* The quit bool keeps the program running while the user hasn't selected the quit option. */
            bool quit = false;

            /* These three functions are called to obtain the XML lists of Pokemon, Moves, and Trainers that were saved last time. */
            LoadMoves();
            LoadPokemon();
            LoadTrainers();

            /* The while loop here keeps the program running until the user decides to exit. */
            while (quit == false)
            {
                /* With each iteration of the main menu loop, the Border() function creates an aesthetic first few lines for the tool. */
                Border("PkmnEngine Editor - Main Menu");

                /* Next, the list of possible options is written to the console. */
                Console.WriteLine("Choose an option:\nT: Add/Edit Trainer\nVT: View Trainer\nM: Add/Edit Move\nVM: View Move\nP: Add/Edit Pokemon\nVP: View Pokemon\nQ: Quit");

                /* The user's option choice is read in from the console. */
                string choice = Console.ReadLine();
                
                /* Depending on the choice, the editor will do different things. */
                switch(choice)
                {
                    /* If the user chooses T or t, the program will move to adding a new trainer. */
                    case "T":
                    case "t":
                        AddTrainer(PokemonDataManager, MoveManager);
                        break;

                    /* If the user chooses VT or vt, the program will move to viewing an existing trainer. */
                    case "VT":
                    case "vt":
                        ViewTrainer(PokemonDataManager);
                        break;

                    /* If the user chooses M or m, the program goes to the screen to make a new move. */
                    case "M":
                    case "m":
                        NewMove();
                        break;

                    /* If the user chooses VM or vm, the program goes to the screen to view an existing move. */
                    case "VM":
                    case "vm":
                        ViewMove();
                        break;
                    
                    /* If the user chooses P or p, the program will move to adding a new Pokemon. */
                    case "P":
                    case "p":
                        AddPokemon();
                        break;

                    /* If the user chooses VP or vp, the program goes to the screen for viewing an existing Pokemon. */
                    case "VP":
                    case "vp":
                        ViewPokemon();
                        break;

                    /* Finally, if the user chooses Q or q, the program will save everything and then quit. */
                    case "Q":
                    case "q":
                        quit = true;
                        SaveMoves();
                        SavePokemon();
                        SaveTrainers();
                        break;

                    /* If the option is not recognized, the console notifies the user, gives them half a second to read the message, then loops around again. */
                    default:
                        Console.WriteLine("Option not recognised.");
                        System.Threading.Thread.Sleep(500);
                        break;
                }
            }
        }
    }
}
