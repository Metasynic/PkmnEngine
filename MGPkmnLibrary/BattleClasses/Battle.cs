using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MGPkmnLibrary.PokemonClasses;
using MGPkmnLibrary;

namespace MGPkmnLibrary.BattleClasses
{
    /* The TypePair represents a unique pair of PkmnTypes used specifically in the context of type advantage.
     * It is used as a key in the Mechanics.TypeMatchups dictionary.
     * The AttackType is the type of the move being used, and the DefenceType is the type of the Pokemon being attacked. */
    public struct TypePair
    {
        public PkmnType AttackType;
        public PkmnType DefenceType;
        public TypePair(PkmnType atk, PkmnType def)
        {
            AttackType = atk;
            DefenceType = def;
        }
    }

    /* A class for a battle between the player and an opponent. */
    public class Battle
    {
        /* A TurnComparer will be needed in order to sort the turns each round. It's static because it doesn't need to be specific to a certain battle. */
        static TurnComparer turnComparer = new TurnComparer();

        /* There is also a static reference to the DataManager's PokemonSpecies dictionary.
         * It's needed for XP calculations. */
        static Dictionary<ushort, PokemonData> pokemonSpeciesRef;
        public static Dictionary<ushort, PokemonData> PokemonSpeciesRef
        {
            get { return pokemonSpeciesRef; }
            set { pokemonSpeciesRef = value; }
        }

        /* The emptyLog flag is true when the battle wants the BattleScreen to empty the log before executing any more turns. */
        bool emptyLog;
        public bool EmptyLog
        {
            get { return emptyLog; }
            set { emptyLog = value;}
        }

        /* This flag is true when the battle wants the BattleScreen to open the SwitchScreen. */
        bool openSwitchMenu;
        public bool OpenSwitchMenu
        {
            get { return openSwitchMenu; }
            set { openSwitchMenu = value; }
        }

        /* The battle log list is in the form of a Queue, so that what's added first is removed first.
         * This log stores messages that are written to the screen during the battle, such as "Squirtle used Bubblebeam! (7 damage)". */
        Queue<string> logList = new Queue<string>();
        public Queue<string> LogList
        {
            get { return logList; }
        }

        /* The turn list holds all the turns happening in the round. This is the list that gets sorted and iterated through in order to execute a round. */
        List<BaseTurn> turns;
        public List<BaseTurn> Turns
        {
            get { return turns; }
            set { turns = value; }
        }

        /* This flag is true when the battle wants to tell the BattleScreen that the battle is finished. */
        bool battleFinished;
        public bool BattleFinished
        {
            get { return battleFinished; }
        }

        /* These two lists hold the player's team of Pokemon, and the opponent's team of Pokemon. */
        List<PokemonInBattle> playerTeam;
        List<PokemonInBattle> opponents;
        public List<PokemonInBattle> PlayerTeam
        {
            get { return playerTeam; }
        }
        public List<PokemonInBattle> Opponents
        {
            get { return opponents; }
        }

        /* These two bytes store the current list index of the Pokemon being used in the battle by the player and opponent. */
        byte playerIndex;
        public byte PlayerIndex
        {
            get { return playerIndex; }
            set { playerIndex = value; }
        }
        byte opponentIndex;
        public byte OpponentIndex
        {
            get { return opponentIndex; }
            set { opponentIndex = value; }
        }

        /* These two properties expose the current Pokemon being used by the player and opponent. */
        public PokemonInBattle CurrentPlayer
        {
            get { return playerTeam[playerIndex]; }
        }
        public PokemonInBattle CurrentOpponent
        {
            get { return opponents[opponentIndex]; }
        }

        /* The constructor initializes the lists and adds the Pokemon to each list from the arrays passed in.
         * LINQ is used to determine how many Pokemon are not null in each array.
         * The initial indexes for the battle are also set to zero.
         * This constructor is for a wild battle. Another constructor will be required for trainer battles. */
        public Battle(Pokemon[] playerTeam, Pokemon[] opponents, Dictionary<ushort, PokemonData> pokemonSpecies, bool trainer)
        {
            this.playerTeam = new List<PokemonInBattle>();
            this.opponents = new List<PokemonInBattle>();
            for (int i = 0; i < playerTeam.Count(s => s != null); i++)
            {
                Pokemon p = playerTeam[i];
                PokemonInBattle pb = new PokemonInBattle(pokemonSpecies[p.PokemonID], p.PokemonGender, p.Moves, p.Level, null, p.XP);
                pb.Health = p.Health;
                for(int j = 0; j < p.Moves.Count(m => m != null); j++)
                {
                    pb.Moves[j].PP = p.Moves[j].PP;
                }
                this.playerTeam.Add(pb);
            }
            for (int i = 0; i < opponents.Count(s => s != null); i++)
            {
                Pokemon p = opponents[i];
                this.opponents.Add(new PokemonInBattle(pokemonSpecies[p.PokemonID], p.PokemonGender, p.Moves, p.Level, null, p.XP));
            }
            playerIndex = 0;
            opponentIndex = 0;

            logList.Clear();

            if (trainer)
                WriteToLog("Trainer # challenged you to a battle!");
            else
                WriteToLog("A wild " + CurrentOpponent.Nickname + " appeared!");

            WriteToLog("Go! " + CurrentPlayer.Nickname + "!");
            emptyLog = true;
            openSwitchMenu = false;
            battleFinished = false;
            turns = new List<BaseTurn>();
        }

        /* Does nothing yet */
        public void Update(GameTime gameTime)
        {

        }

        /* This function applies the correct formulae to detemine whether a move hits or not.
         * The attacking Pokemon's accuracy stage and the defending Pokemon's evasion stage are combined
         * The calculated stage is then sent to the PokemonInBattle.ApplyAccStage() function, which returns a percentage change of the attack hitting.
         * Finally, the percentage is compared to a random number between 0 and 99 to determine if the move actually hits. */
        public static bool AccuracyCheck(byte moveAccuracy, sbyte attackerAccStage, sbyte defenderEvaStage)
        {
            sbyte stage = (sbyte)MathHelper.Clamp((attackerAccStage - defenderEvaStage), -6, 6);
            byte accuracy = (byte)MathHelper.Clamp((PokemonInBattle.ApplyAccStage(moveAccuracy, stage)), 0, 100);
            return (PkmnUtils.RandomInclusive(99) < accuracy);
        }

        /* This function applies type effectiveness multipliers to damage.
         * The damage is first changed to a decimal to ensure accuracy.
         * For each of the defending Pokemon's (one or two) types, a TypePair key is created with the attacking move's type.
         * The Mechanics.TypeMatchups dictionary is checked to see if it contains a multiplier for the TypePair.
         * If a multiplier is found, it is applied to the damage. Before being returned, the damage is converted back to a whole number. */
        public static ushort TypeCheck(ushort damage, PkmnType atkType, List<PkmnType> defTypes)
        {
            decimal calculatedDamage = damage;
            foreach(PkmnType defType in defTypes)
            {
                TypePair tp = new TypePair(atkType, defType);
                if (PkmnUtils.TypeMatchups.ContainsKey(tp))
                {
                    calculatedDamage *= PkmnUtils.TypeMatchups[tp];
                }
            }
            return (ushort)calculatedDamage;
        }

        /* WriteToLog() adds an item to the logList queue. */
        public void WriteToLog(string item)
        {
            logList.Enqueue(item);
        }

        /* This function creates a default NPC turn to add to the round. */
        public void AddNpcTurn(PokemonInBattle user, PokemonInBattle target)
        {
            /* A new ProportionValue<Move> array is created with a length equal to the user's move array.
             * The moves are assigned with equal proportions so that each move has the same chance of being used. */
            ProportionValue<Move>[] moves = new ProportionValue<Move>[user.Moves.Count(m => m != null)];
            for (int i = 0; i < 4; i++)
            {
                if (user.Moves[i] != null)
                {
                    moves[i] = ProportionValue.Create(1.0 / user.Moves.Count(m => m != null), user.Moves[i]); // DOES NOT HANDLE UNCOMPACTED MOVE ARRAYS
                }
            }

            /* A new turn is created using the user, target and a move chosen randomly via ProportionValue choosing. */
            turns.Add(new TurnMove(user, target, this, moves.ChooseByRandom()));
        }

        /* OrderTurns() is called before round execution and sorts the turn list using the TurnComparer. */
        public void OrderTurns()
        {
            turns.Sort(turnComparer);
        }

        /* This function is called when the battle log has finished emptying and another turn needs to be executed. */
        public void ExecuteNextTurn()
        {
            /* The engine executes the first move in the list, and if the Execute() function returns true, the battleFinished flag is set to true.
             * This will let the BattleScreen know that the battle needs to end once the log has finished emptying. */
            if (turns[0].Execute())
            {
                battleFinished = true;
            }

            /* Otherwise, the engine also checks if all of the Pokemon in the player's team have fainted.
             * If they have, the player has lost, and the battle is over. */
            else if (!playerTeam.Any(p => !p.Fainted))
            {
                battleFinished = true;
            }

            /* Alternatively, if all the opponent's Pokemon have fainted, then the battle is over.
             * The player has won, and the battleFinished flag is set to true. */
            else if (!opponents.Any(p => !p.Fainted))
            {
                battleFinished = true;
            }

            /* If the move execution has caused the current player to faint, then the switch menu will be brought up 
             * so the player can choose a new Pokemon to switch into the battle. */
            if (CurrentPlayer.Fainted)
            {
                openSwitchMenu = true;
            }
        }
    }
}
