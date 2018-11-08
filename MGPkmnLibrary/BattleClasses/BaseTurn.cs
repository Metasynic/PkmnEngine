using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGPkmnLibrary.BattleClasses
{
    /* This class is a comparer for BaseTurns. It inherits from IComparer, and sorts Turns according to their priority in battle. */
    public class TurnComparer : IComparer<BaseTurn>
    {
        /* This is the function that gets called when once BaseTurn is compared to another.
         * If it returns -1, x < y.
         * If it returns 0, x = y.
         * If it returns 1, x > y.
         * Turns will occur in ascending order in battle.
         * The order should be: TurnRun first, then TurnSwitch in order of user speed, then TurnMove in order of move priority then user speed. */
        public int Compare(BaseTurn x, BaseTurn y)
        {
            /* If the first turn is a TurnRun, then it takes the highest priority, so x is smaller. */
            if (x is TurnRun && (y is TurnMove || y is TurnSwitch)) {
                return -1;
            }

            /* Similarly, if the second turn is a TurnRun, then it has a higher priority, so x is bigger. */
            else if ((x is TurnMove || x is TurnSwitch) && y is TurnRun)
            {
                return 1;
            }

            /* Next, if one turn is a TurnSwitch and the other is a TurnMove, the TurnSwitch takes priority. */
            else if (x is TurnSwitch && y is TurnMove)
            {
                return -1;
            }
            else if (x is TurnMove && y is TurnSwitch)
            {
                return 1;
            }

            /* If the two moves are both TutnRun or TurnSwitch, priority is determined by the final calculated speed of the turn's user. 
             * When both speeds are equal, the turns are considered to be equal in order. */
            else if ((x is TurnSwitch && y is TurnSwitch) || (x is TurnRun && y is TurnRun))
            {
                if (x.User.FinalSpeed > y.User.FinalSpeed)
                {
                    return -1;
                }
                else if (x.User.FinalSpeed < y.User.FinalSpeed)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            /* In the majority of cases, both turns will be a TurnMove, in which case the moves are ordered by their special Priority property. */
            else if (x is TurnMove && y is TurnMove)
            {
                if ((x as TurnMove).Move.Priority > (y as TurnMove).Move.Priority)
                {
                    return -1;
                }
                else if ((x as TurnMove).Move.Priority < (y as TurnMove).Move.Priority)
                {
                    return 1;
                }

                /* However, many moves have an equal Priority value, and in this case, the decision is made based on the final calculated speed of the user. */
                else
                {
                    if (x.User.FinalSpeed > y.User.FinalSpeed)
                    {
                        return -1;
                    }
                    else if (x.User.FinalSpeed < y.User.FinalSpeed)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            /* If for some reason the engine ever reaches this code, something has gone wrong, so zero is returned by default. */
            return 0;
        }
    }

    /* This class holds the template for all turns that can be used in a battle.
     * It is the parent class for TurnSwitch, TurnRun, and TurnMove. */
    public abstract class BaseTurn
    {
        /* These two fields hold references to the user of the move, and the target of the move. */
        protected PokemonInBattle user;
        public PokemonInBattle User
        {
            get { return user; }
        }
        protected PokemonInBattle target;
        public PokemonInBattle Target
        {
            get { return target; }
        }

        /* There is also a reference to the current battle, which will be needed for various things. */
        protected Battle battleRef;

        /* When a base turn is created, the user, target and battle reference are passed in.
         * The fields are set using the arguments. */
        public BaseTurn(PokemonInBattle user, PokemonInBattle target, Battle battleRef)
        {
            this.user = user;
            this.target = target;
            this.battleRef = battleRef;
        }

        /* This function will hold the code that gets executed when the turn is actually used.
         * It returns a flag representing whether the battle should end or not. */
        public abstract bool Execute();
    }
}
