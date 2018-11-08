/* For some reason this file requires absolutely no using statements.
 * This was not intentional and I'm surprised that it works. */

namespace MGPkmnLibrary.BattleClasses
{
    /* A TurnRun is used when the player chooses to "Run" from the battle.
     * It inherits from BaseTurn as it's a type of turn used in a round. */
    public class TurnRun : BaseTurn
    {
        /* The constructor requires no special parameters unique to itself, and passes them all into the parent contructor. */
        public TurnRun(PokemonInBattle user, PokemonInBattle target, Battle battleRef) : base(user, target, battleRef)
        {

        }

        /* When a TurnRun is executed, the engine will work out whether the user can run away. */
        public override bool Execute()
        {
            /* A byte representing the probability of a successful escape is generated using this formula.
             * It is then compared to a random integer between 0 and 255 to determine whether the escape can happen. */
            byte escapeChance = (byte)((((user.Speed * 128) / target.Speed) + (30 * user.RunCount)) % 256);
            bool canRun = (PkmnUtils.RandomInclusive(255) < escapeChance);

            /* If the player Pokemon has tried to run away, and they have managed it, then it is written to the log.
             * The battle's EmptyLog flag is set to true so that the log is displayed to the screen, and the function returns true.
             * This means that the battle will end after this turn, since the user has run away.
             * However, if the player cannot run, this is also written to the log, EmptyLog is set to true and the function returns false since the battle will continue. */
            if (user == battleRef.CurrentPlayer)
            {
                if (canRun)
                {
                    battleRef.WriteToLog("You fled from the battle!");
                    battleRef.EmptyLog = true;
                    return true;
                }
                else
                {
                    battleRef.WriteToLog("Couldn't escape!");
                    battleRef.EmptyLog = true;
                    return false;
                }
            }

            /* Otherwise, the opponent Pokemon is the one trying to run away.
             * If the run has succeeded, then it's written to the log, EmptyLog is set to true, and the function returns true.
             * In the other case, the run failed, and this is written to the log. EmptyLog is again set to true and false is returned. */
            else
            {
                if (canRun) 
                { 
                    battleRef.WriteToLog(target.Nickname + " fled from the battle!");
                    battleRef.EmptyLog = true;
                    return true;
                }
                else
                {
                    battleRef.WriteToLog(target.Nickname + " tried to run but couldn't escape!");
                    battleRef.EmptyLog = true;
                    return false;
                }
            }
        }
    }
}
