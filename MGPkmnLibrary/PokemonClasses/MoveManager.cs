using System.Collections.Generic;

namespace MGPkmnLibrary.PokemonClasses
{
    /* The MoveManager is a class used in the command-line PkmnEditor program.
     * It will be expanded at some point to allow greater functionality. */
    public class MoveManager
    {
        /* The moves Dictionary contains all the moves in the manager.
         * In the PkmnEditor, it is a "master move list" containing every move.
         * In the main PkmnEngine, the "master move list" is instead stored in the DataManager. */
        readonly Dictionary<string, Move> moves;
        public Dictionary<string, Move> Moves
        {
            get { return moves; }
        }

        /* The MoveManager constuctor simply initializes the moves list. */
        public MoveManager()
        {
            moves = new Dictionary<string, Move>();
        }
    }
}
