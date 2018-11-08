using MGPkmnLibrary.PokemonClasses;

namespace MGPkmnLibrary.BattleClasses
{
    /* This class represents a Pokemon specifically while it is in a battle.
     * A separate class to the regular Pokemon class was needed because while in battle,
     * a Pokemon can have different stages applied to its stats, which translate to a multiplier to the stat.
     * In addition, certain status problems affect a Pokemon differently inside and outside of battle. */
    public class PokemonInBattle : Pokemon
    {
        int runCount;
        public int RunCount
        {
            get { return runCount; }
            set { runCount = value; }
        }

        /* These sbytes should be between -6 and 6 inclusive, and show the stat stage for the PokemonInBattle. */
        private sbyte atkStage, defStage, spAtkStage, spDefStage, spdStage, accStage, evaStage;
        public sbyte AtkStage
        {
            get { return atkStage; }
        }
        public sbyte DefStage
        {
            get { return defStage; }
        }
        public sbyte SpAtkStage
        {
            get { return spAtkStage; }
        }
        public sbyte SpDefStage
        {
            get { return spDefStage; }
        }
        public sbyte SpdStage
        {
            get { return spdStage; }
        }
        public sbyte AccStage
        {
            get { return accStage; }
        }
        public sbyte EvaStage
        {
            get { return evaStage; }
        }
        
        /* While in battle, Pokemon's stats are modified according to the relevant stat stage.
         * Whenever the stat is needed, the correct stat stage is applied to it using ApplyStatStage(). */
        public ushort FinalAttack
        {
            get { return ApplyStatStage(Attack, atkStage); }
        }
        public ushort FinalDefence
        {
            get { return ApplyStatStage(Defence, defStage); }
        }
        public ushort FinalSpecialAttack
        {
            get { return ApplyStatStage(SpecialAttack, spAtkStage); }
        }
        public ushort FinalSpecialDefence
        {
            get { return ApplyStatStage(SpecialDefence, spDefStage); }
        }
        public ushort FinalSpeed
        {
            get { return ApplyStatStage(Speed, spdStage); }
        }

        /* The constructor simply initializes the parent Pokemon class, and sets all stat stages to zero. */
        public PokemonInBattle(PokemonData pokemonData, Gender gender, Move[] moves, byte level, TrainerRank? rank, int xp) : base(pokemonData, gender, moves, level, rank)
        {
            runCount = atkStage = defStage = spAtkStage = spDefStage = spdStage = accStage = evaStage = 0;
            this.XP = xp;
        }

        /* This function applies the right modifier to a stat.
         * The modifier varies depending on the stage (between -6 and 6). */
        public static ushort ApplyStatStage(ushort stat, sbyte stage)
        {
            switch (stage)
            {
                case -6:
                    return (ushort)(stat * 1 / 4);
                case -5:
                    return (ushort)(stat * 2 / 7);
                case -4:
                    return (ushort)(stat * 1 / 3);
                case -3:
                    return (ushort)(stat * 2 / 5);
                case -2:
                    return (ushort)(stat * 1 / 2);
                case -1:
                    return (ushort)(stat * 2 / 3);
                case 0:
                    return stat;
                case 1:
                    return (ushort)(stat * 3 / 2);
                case 2:
                    return (ushort)(stat * 2);
                case 3:
                    return (ushort)(stat * 5 / 2);
                case 4:
                    return (ushort)(stat * 3);
                case 5:
                    return (ushort)(stat * 7 / 2);
                case 6:
                    return (ushort)(stat * 4);
                default:
                    return stat;
            }
        }

        /* This function applies modifiers to the accuracy stage calculated in Battle.AccuracyCheck().
         * The modifiers are different for accuracy than for other stats, hence the use of a separate function.
         * Accuracy stat stages are also between -6 and 6. */
        public static ushort ApplyAccStage(ushort stat, sbyte stage)
        {
            switch (stage)
            {
                case -6:
                    return (ushort)(stat * 1 / 3);
                case -5:
                    return (ushort)(stat * 3 / 8);
                case -4:
                    return (ushort)(stat * 3 / 7);
                case -3:
                    return (ushort)(stat * 1 / 2);
                case -2:
                    return (ushort)(stat * 3 / 5);
                case -1:
                    return (ushort)(stat * 3 / 4);
                case 0:
                    return stat;
                case 1:
                    return (ushort)(stat * 4 / 3);
                case 2:
                    return (ushort)(stat * 5 / 3);
                case 3:
                    return (ushort)(stat * 2);
                case 4:
                    return (ushort)(stat * 7 / 3);
                case 5:
                    return (ushort)(stat * 8 / 3);
                case 6:
                    return (ushort)(stat * 3);
                default:
                    return stat;
            }
        }

        /* This function resets the runCount and all the stat stages back to zero. It is called when switching out in battle. */
        public void Reset()
        {
            runCount = atkStage = defStage = spAtkStage = spDefStage = spdStage = accStage = evaStage = 0;
        }
    }
}
