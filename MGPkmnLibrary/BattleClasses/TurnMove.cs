using System;
using MGPkmnLibrary.PokemonClasses;

namespace MGPkmnLibrary.BattleClasses
{
    /* A TurnMove is where a Pokemon uses a move on another Pokemon, inflicting damage or changing stat stages.
     * It inherits from BaseTurn as it is a type of turn that gets used during a battle round. */
    public class TurnMove : BaseTurn
    {
        /* This field holds the actual move being used. */
        protected Move move;
        public Move Move
        {
            get { return move; }
        }

        /* The constructor takes all the parameters for a BaseTurn plus the move being used.
         * The other arguments are passed to the base constructor, while the move is set into the field in the object. */
        public TurnMove(PokemonInBattle user, PokemonInBattle target, Battle battleRef, Move move) : base(user, target, battleRef)
        {
            this.move = move;
        }

        /* Execute() is called when the move is actually used.
         * It returns a boolean dictating whether or not the battle needs to end after this turn. */
        public override bool Execute()
        {
            /* The damage field holds the amount of health that will be removed from the target Pokemon when the move is used. */
            decimal damage;

            /* The multiplier is a factor applied to the damage to reflect type advantage and STAB (Same Type Attack Bonus).
             * These two things will be explained further down the class. */
            decimal multiplier;

            /* First, the engine checks whether the move is going to hit the target or not.
             * It calls the static AccuracyCheck() function, passing in the relevant stats. */
            if (Battle.AccuracyCheck(move.Accuracy, user.AccStage, target.EvaStage))
            {
                /* This whole block executes if the user Pokemon is able to hit the target.
                 * As only damaging moves are required for this engine's user requirements, the stat stage affecting moves "Growl" and "Leer" do nothing.
                 * Hence, the engine checks if the move being used is one of these, and if it isn't, it moves on to damage calculations. */
                if (move.Name == "Growl" || move.Name == "Leer")
                {

                }

                else
                {
                    /* The same formula is used for damage calculation, regardless of whether the attack is of physical or special category.
                     * What differs is that physical moves take into account the user attack and target defence,
                     * whereas special moves use the user special attack and target special defence.
                     * In both cases, the user's level and the move's power value are also part of the final formula. */
                    if (move.Category == MoveCategory.physical)
                        damage = ((((2 * user.Level / 5 + 2) * user.Attack * move.Power / target.Defence) / 50) + 2);
                    else if (move.Category == MoveCategory.special)
                        damage = ((((2 * user.Level / 5 + 2) * user.SpecialAttack * move.Power / target.SpecialDefence) / 50) + 2);

                    /* If the move is of "Status" category (moves which burn/freeze/paralyze the target), an exception is thrown, as this category of moves is not implemented. */
                    else
                        throw new NotImplementedException("Status moves not yet implemented.");

                    /* Now that the damage is calculated, the engine must determine the multiplier.
                     * It starts with a default value of one (the empty product), which will leave the damage unchanged. */
                    multiplier = 1.0m;

                    /* The multiplier can be affected by type advantage/disadvantage.
                     * For instance, if a water-type move is used against a fire-type, the damage doubles, but if it's the other way round, it halves.
                     * This is achieved by creating a TypePair object containing the attack type and the defence type, then checking the DataManager.TypeMatchups entry for that TypePair.
                     * As the target may have two types, the check needs to happen once for each of the target's types, and the effects on the multiplier are stacked.
                     * This means the maximum possible multiplier at this point is 4.0, and the minimum is 0.0 (which only happens if one of the combinations is an immunity). */
                    if (PkmnUtils.TypeMatchups.ContainsKey(new TypePair(move.Type, target.Type[0])))
                        multiplier *= PkmnUtils.TypeMatchups[new TypePair(move.Type, target.Type[0])];
                    if (target.IsDualType && PkmnUtils.TypeMatchups.ContainsKey(new TypePair(move.Type, target.Type[1])))
                        multiplier *= PkmnUtils.TypeMatchups[new TypePair(move.Type, target.Type[1])];

                    /* The other factor affecting the multiplier is STAB (Same Type Attack Bonus).
                     * This is applied when a Pokemon uses a move of the same type as itself.
                     * So, if a psychic type Pokemon uses a psychic type move, the multiplier increases by 1.5 in addition to any type advantage modifiers. */
                    foreach(PkmnType type in user.Type)
                    {
                        if (type == move.Type)
                            multiplier *= 1.5m;
                    }

                    /* Now that we have the initial damage and multiplier, the multiplier is applied to the initial damage to get the final damage.
                     * The name of the user, move, and amount of damage is written to the battle's log. */
                    damage *= multiplier;
                    battleRef.WriteToLog(user.Nickname + " used " + move.Name + "! (" + (ushort)damage + " damage)");

                    /* If a type advantage multiplier was used, then another message will be written to the log.
                     * The engine has to check for two values per message - one with STAB applied, and one without. */
                    if (multiplier == 4.0m || multiplier == 6.0m)
                        battleRef.WriteToLog("It's super duper effective!");
                    else if (multiplier == 2.0m || multiplier == 3.0m)
                        battleRef.WriteToLog("It's super effective!");
                    else if (multiplier == 0.5m || multiplier == 0.75m)
                        battleRef.WriteToLog("It's not very effective...");
                    else if (multiplier == 0.25m || multiplier == 0.375m)
                        battleRef.WriteToLog("It's really not very effective at all...");
                    else if (multiplier == 0m)
                        battleRef.WriteToLog("It had no effect!");

                    /* Finally, the damage is subtracted from the target Pokemon's health, doing the damage. */
                    target.Health.Subtract((ushort)damage);
                }

                /* Now that the move has been executed, the engine checks if the target Pokemon has no health left.
                 * If this is the case, its fainted flag is set as true, and the information is written to the battle log. */
                if (target.Health.CurrentValue <= 0)
                {
                    target.Fainted = true;
                    battleRef.WriteToLog(target.Nickname + " Fainted!");

                    /* If the Pokemon that used the move belongs to the player, then they need to earn the correct amount of XP points. */
                    if (user == battleRef.CurrentPlayer)
                    {
                        /* The XP value to be added is calculated using the defeated Pokemon's base experience multiplied by its level, all divided by seven.
                         * This number is added to the player Pokemon's XP. Note that the Pokemon's XP field holds the amount of XP they've gained since the last level up,
                         * not the total amount of XP gained overall.
                         * The amount of earned XP is written to the log. */
                        int xp = ((Battle.PokemonSpeciesRef[target.PokemonID].BaseExp * target.Level) / 7);
                        user.XP += xp;
                        battleRef.WriteToLog(user.Nickname + " earned " + xp + " XP!");

                        /* Now the engine checks for level ups. In a loop, the engine checks whether the user's XP is greater than or equal to the amount of XP they need to level up.
                         * In addition, a level 100 Pokemon cannot level up, so it checks for that as well.
                         * The formula to calculate the next level is in the PkmnUtils class and is explained there.
                         * If the user can level up, the XP required to do so is removed from their XP pool, and their level increases by one.
                         * They might have become level 100, in which case any excess XP is stripped as it is of no use.
                         * The level up message is written to the log.
                         * Since this happens in a while loop, if there's enough XP to level up multiple times, it will happen automatically. */
                        while (user.XP >= PkmnUtils.NextLevel(Battle.PokemonSpeciesRef[user.PokemonID].GrowthRate, user.Level) && user.Level < 100)
                        {
                            user.XP -= PkmnUtils.NextLevel(Battle.PokemonSpeciesRef[user.PokemonID].GrowthRate, user.Level);
                            user.Level++;
                            if (user.Level == 100)
                                user.XP = 0;
                            battleRef.WriteToLog(user.Nickname + " levelled up to Lv " + user.Level);
                        }
                    }
                }                
            }

            /* This block executes if the user misses with their move. It writes a message to the log. */
            else
            {
                battleRef.WriteToLog(user.Nickname + "'s attack missed!");
            }

            /* Lastly, since the move has been used, its PP decreases by one point.
             * The EmptyLog flag in the battle is set to true so that the log is displayed.
             * The function returns false as the battle should not end at the moment. */
            move.PP.CurrentValue--;
            battleRef.EmptyLog = true;
            return false;
        }
    }
}
