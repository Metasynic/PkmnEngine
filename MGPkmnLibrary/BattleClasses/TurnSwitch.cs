/* This is only one of two files in the engine that don't need any using statements. */

namespace MGPkmnLibrary.BattleClasses
{
    /* A TurnSwitch is a type of turn that changes one Pokemon for another in battle.
     * It inherits from BaseTurn so that it can be added to the turn list for battle rounds. */
    public class TurnSwitch : BaseTurn
    {
        /* This byte will hold the index value of the Pokemon that has been chosen to be switched in. */
        byte newIndex;

        /* The TurnSwitch constructor takes the BaseTurn parameters plus the new index, which is set into the correct field. */
        public TurnSwitch(PokemonInBattle user, PokemonInBattle target, Battle battleRef, byte newIndex) : base(user, target, battleRef)
        {
            this.newIndex = newIndex;
        }

        /* Execute() is called after a choice has been made about which Pokemon to switch in, via the SwitchScreen. */
        public override bool Execute()
        {
            /* Firstly, the move's user is reset, meaning all stat stages are set to zero so they don't carry over next time. */
            user.Reset();

            /* In both cases of the user being the player or the opponent, appropriate messages are written to the log.
             * Note that for TurnSwitch turns, the target is the allied Pokemon being switched in, NOT the opposing Pokemon on the other side of the battle.
             * The PlayerIndex or OpponentIndex is set to the newIndex field, thus actually making the switch. */
            if (user == battleRef.CurrentPlayer)
            {
                battleRef.WriteToLog("Come back! " + user.Nickname + "!");
                battleRef.PlayerIndex = newIndex;
                battleRef.WriteToLog("Go! " + target.Nickname + "!");
            }
            else
            {
                battleRef.WriteToLog(user.Nickname + " was withdrawn!");
                battleRef.OpponentIndex = newIndex;
                battleRef.WriteToLog(target.Nickname + " was sent out!");
            }

            /* After completion of the turn, the EmptyLog flag is set to true so that the battle knows to start displaying the log.
             * False is returned because the battle should not end after a switch occurs. */
            battleRef.EmptyLog = true;
            return false;
        }
    }
}
