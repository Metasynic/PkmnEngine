using System;
using System.Collections.Generic;
using MGPkmnLibrary.ItemClasses;

namespace MGPkmnLibrary.PokemonClasses
{
    /* Gender has three possible values, as a Pokemon can be male, female, or genderless. */
    public enum Gender { female, male, none }

    /* PkmnType is used in a lot of places throughout the engine, and contains the eighteen different Pokemon types.
     * Pokemon Types are used to determine whether a move is super effective, as well as granting various status immunities. */
    public enum PkmnType { bug, dark, dragon, electric, fairy, fighting, fire, flying, ghost, grass, ground, ice, normal, poison, psychic, rock, steel, water }

    /* The EggGroup is an enum describing the different breeding categories for Pokemon.
     * Breeding is not in the user requirements, but I'm adding it just in case.
     * Two Pokemon can breed if they are of opposite genders and have at least one Egg Group in common.
     * Don't ask me why they all lay eggs, because I don't know. */
    public enum EggGroup { bug, ditto, dragon, fairy, flying, field, humanlike, amorphous, mineral, monster, undiscovered, grass, water1, water2, water3}

    /* GrowthRate represents how fast a Pokemon levels up at different points.
     * Pokemon with an Erratic growth rate level up comparatively slowly at lower levels, but level up faster at higher levels.
     * As growth rate slows down, more total experience points are required to level up.
     * A Pokemon with a Fluctuating growth rate levels up faster at lower levels, but levels up comparatively slowly at higher levels. */
    public enum GrowthRate { erratic, fast, mediumfast, mediumslow, slow, fluctuating }

    /* This is the class for an indiviual Pokemon. It can be in a player/trainer inventory or in the wild. */
    [Serializable]
    public class Pokemon
    {
        /* This static function uses the Gender Ratio of the Pokemon's species to randomly generate a Gender value for the Pokemon,
         * according to the proportions in the Gender Ratio. */
        public static Gender GetGender(GenderRatio ratio)
        {
            /* If the ratio's male and female values are both zero, the Pokemon is genderless. */
            if (ratio.male == 0 && ratio.female == 0)
                return Gender.none;
            else
            {
                /* If the Pokemon can be male or female, the function uses the Mechanics.ProportionValue<T> class.
                 * An array of two ProportionValue<Gender>s is created for the male and female proportion values.
                 * The male and female ratioes are added to the array.
                 * The ratios are bytes between 0 and 100, so they are converted to doubles between 0 and 1 to be used in the ProportionValue.
                 * Finally, the ChooseByRandom() function is used to return a gender using the ratios from the GenderRatio passed in.
                 * ChooseByRandom() works such that, for example, if the GenderRatio was 75 for male and 25 for female,
                 * there is a 75% chance of the function returning male, and a 25% chance of it returning female. */
                ProportionValue<Gender>[] genders = new ProportionValue<Gender>[2];
                genders[0] = ProportionValue.Create(Convert.ToDouble(ratio.male) / 100, Gender.male);
                genders[1] = ProportionValue.Create(Convert.ToDouble(ratio.female) / 100, Gender.female);
                return genders.ChooseByRandom();
            }
        }

        /* The one or two types of the Pokemon are stored in this list.
         * A single PkmnType cannot be used since some Pokemon are dual types, so a List<PkmnType> is needed. */
        private List<PkmnType> type;
        public List<PkmnType> Type
        {
            get { return type; }
            set { type = value; }
        }

        /* This handy property checks if the type List has two items in it.
         * It's useful because gameplay can be affected by whether a Pokemon is a dual type or not. */
        public bool IsDualType
        {
            get { return (type.Count == 2); }
        }

        /* The pokemonID field stores the Pokemon's species ID, for example, any Mewtwo has an ID of #150.
         * The gender field is the Pokemon's gender. */
        private ushort pokemonID;
        private Gender gender;
        public ushort PokemonID
        {
            get { return pokemonID; }
            set { pokemonID = value; }
        }
        public Gender PokemonGender
        {
            get { return gender; }
            set { gender = value; }
        }

        /* The Nickname of a Pokemon is the name that displays when it is viewed or used in battle.
         * The default Nickname of a Pokemon is its species name.
         * Changing the Nickname of a Pokemon is not part of the user requirements, but I may add it anyway. */
        private string nickname;
        public string Nickname
        {
            get { return nickname; }
            set { nickname = value; }
        }

        /* The level of a Pokemon is a measure of how powerful it is. Level can be between 1 and 100.
         * XP is the amount of experience points the Pokemon has.
         * XP is earned after successfully winning a battle. Once a Pokemon has enough XP, it can level up.
         * Levelling up has not yet been implemented but it will be soon.
         * A Pokemon can also evolve when it reaches a certain level or has a certain item used on it. This hasn't been added yet. */
        private byte level;
        public byte Level
        {
            get { return level; }
            set { level = value; }
        }
        private int xp;
        public int XP
        {
            get { return xp; }
            set { xp = value; }
        }

        /* The fainted bit represents whether the Pokemon has fainted or is conscious.
         * If a Pokemon has fainted, it cannot be used in battle. A Pokemon faints when its HP reaches zero. */
        private bool fainted;
        public bool Fainted
        {
            get { return fainted; }
            set { fainted = value; }
        }

        /* These six bytes are the base stats of the Pokemon's species.
         * All Pokemon of the same species have exactly the same base stats.
         * The base stats of a Pokemon are required to calculate the final stats. */
        private byte baseHP;
        private byte baseAttack;
        private byte baseDefence;
        private byte baseSpecialAttack;
        private byte baseSpecialDefence;
        private byte baseSpeed;
        public byte BaseHP
        {
            get { return baseHP; }
            set { baseHP = value; }
        }
        public byte BaseAttack
        {
            get { return baseAttack; }
            set { baseAttack = value; }
        }
        public byte BaseDefence
        {
            get { return baseDefence; }
            set { baseDefence = value; }
        }
        public byte BaseSpecialAttack
        {
            get { return baseSpecialAttack; }
            set { baseSpecialAttack = value; }
        }
        public byte BaseSpecialDefence
        {
            get { return baseSpecialDefence; }
            set { baseSpecialDefence = value; }
        }
        public byte BaseSpeed
        {
            get { return baseSpeed; }
            set { baseSpeed = value; }
        }

        /* The six IV bytes store the Pokemon's individual values (IVs).
         * IVs are a random number between 0 and 31 representing the Pokemon's talent in that stat.
         * IVs are generated when the Pokemon is created, and cannot be altered once the Pokemon has been created.
         * The IV bytes are required to calculate the Pokemon's final stats. */
        private byte iv_HP;
        private byte iv_Attack;
        private byte iv_Defence;
        private byte iv_SpecialAttack;
        private byte iv_SpecialDefence;
        private byte iv_Speed;
        public byte IV_HP
        {
            get { return iv_HP; }
            set { iv_HP = value; }
        }
        public byte IV_Attack
        {
            get { return iv_Attack; }
            set { iv_Attack = value; }
        }
        public byte IV_Defence
        {
            get { return iv_Defence; }
            set { iv_Defence = value; }
        }
        public byte IV_SpecialAttack
        {
            get { return iv_SpecialAttack; }
            set { iv_SpecialAttack = value; }
        }
        public byte IV_SpecialDefence
        {
            get { return iv_SpecialDefence; }
            set { iv_SpecialDefence = value; }
        }
        public byte IV_Speed
        {
            get { return iv_Speed; }
            set { iv_Speed = value; }
        }

        /* The six EV bytes store the Pokemon's Effort Values (EVs), which can be between 0 and 255.
         * If IVs are a Pokemon's "talent", then EVs are a measurement of its "training" in various stats.
         * Defeating different Pokemon species gives a boost to different EV values.
         * For example, defeating a Pikachu will reward the Pokemon with 2 speed EVs.
         * Once a Pokemon has 510 EVs, it cannot gain any more. This cap has not yet been implemented.
         * The EV bytes are required to calculate the final stats. */
        private byte ev_HP;
        private byte ev_Attack;
        private byte ev_Defence;
        private byte ev_SpecialAttack;
        private byte ev_SpecialDefence;
        private byte ev_Speed;
        public byte EV_HP
        {
            get { return ev_HP; }
            set { ev_HP = value; }
        }
        public byte EV_Attack
        {
            get { return ev_Attack; }
            set { ev_Attack = value; }
        }
        public byte EV_Defence
        {
            get { return ev_Defence; }
            set { ev_Defence = value; }
        }
        public byte EV_SpecialAttack
        {
            get { return ev_SpecialAttack; }
            set { ev_SpecialAttack = value; }
        }
        public byte EV_SpecialDefence
        {
            get { return ev_SpecialDefence; }
            set { ev_SpecialDefence = value; }
        }
        public byte EV_Speed
        {
            get { return ev_Speed; }
            set { ev_Speed = value; }
        }

        /* At this point it's worth mentioning that Pokemon also have natures,
         * which act as multipliers to the final stats. Natures are not in the user requirements. */

        /* These six fields represent the final stats of the Pokemon.
         * These stats are the base for the stats of a PokemonInBattle which are multiplied by stat stages.
         * The final stats are calculated using an interesting formula which incorporates base, IV, EV, and level. */
        public ushort HP
        {
            get { return (ushort)(((iv_HP + 2 * baseHP + (ev_HP / 4)) * level / 100) + 10 + level); }
        }
        public ushort Attack
        {
            get { return (ushort)(((iv_Attack + 2 * baseAttack + (ev_Attack / 4)) * level / 100) + 5); }
        }
        public ushort Defence
        {
            get { return (ushort)(((iv_Defence + 2 * baseDefence + (ev_Defence / 4)) * level / 100) + 5); }
        }
        public ushort SpecialAttack
        {
            get { return (ushort)(((iv_SpecialAttack + 2 * baseSpecialAttack + (ev_SpecialAttack / 4)) * level / 100) + 5); }
        }
        public ushort SpecialDefence
        {
            get { return (ushort)(((iv_SpecialDefence + 2 * baseSpecialDefence + (ev_SpecialDefence / 4)) * level / 100) + 5); }
        }
        public ushort Speed
        {
            get { return (ushort)(((iv_Speed + 2 * baseSpeed + (ev_Speed / 4)) * level / 100) + 5); }
        }

        /* The moves array stores the four moves that a Pokemon has.
         * New moves are learned on levelling up - this is not yet implemented. */
        private Move[] moves = new Move[4];
        public Move[] Moves
        {
            get { return moves; }
            set { moves = value; }
        }

        /* The AttributePair for health actually represents how much HP the Pokemon has,
         * as opposed to the HP stat which is only its maximum HP value.
         * The maximum value of the Health needs to increase on levelling up, to match the HP stat.
         * This field is the one that gets modified when a Pokemon is damaged or healed. */
        private AttributePair health;
        public AttributePair Health
        {
            get { return health; }
            set { health = value; }
        }

        /* This field represents the item being held by the Pokemon.
         * Held items can have varying effects on a battle, depending on what the item is. */
        private BaseItem heldItem;
        public BaseItem HeldItem
        {
            get { return heldItem; }
            set { heldItem = value; }
        }

        /* The private constructor for a Pokemon initializes the object with null values.
         * It is used when a Pokemon is deserialized from an XML file. */
        private Pokemon()
        {
            baseHP = baseAttack = baseDefence = baseSpecialAttack = baseSpecialDefence = baseSpeed = 0;
            health = new AttributePair(0);
            iv_HP = iv_Attack = iv_Defence = iv_SpecialAttack = iv_SpecialDefence = iv_Speed = 0;
            ev_HP = ev_Attack = ev_Defence = ev_SpecialAttack = ev_SpecialDefence = ev_Speed = 0;
            type = new List<PkmnType>();
        }

        /* The main constructor for a Pokemon takes its species (PokemonData), Gender, moveset, level, and a TrainerRank.
         * The TrainerRank is nullable so that null can be passed in for a wild Pokemon. */
        public Pokemon(PokemonData pokemonData, Gender gender, Move[] moves, byte level, TrainerRank? rank)
        {
            /* The nickname, type and base stats for the Pokemon are copied from the PokemonData for its species. */
            Nickname = pokemonData.PokemonName;
            pokemonID = pokemonData.ID;
            baseHP = pokemonData.BaseHP;
            baseAttack = pokemonData.BaseAttack;
            baseDefence = pokemonData.BaseDefence;
            baseSpecialAttack = pokemonData.BaseSpecialAttack;
            baseSpecialDefence = pokemonData.BaseSpecialDefence;
            baseSpeed = pokemonData.BaseSpeed;
            type = new List<PkmnType>();
            type = pokemonData.Type;

            /* Gender and Level are set according to what was passed in. */
            this.gender = gender;
            this.level = level;

            /* Fainted is false by default, as a Pokemon is initially conscious. */
            fainted = false;

            /* The moveset is copied using Array.Copy(). */
            Array.Copy(moves, this.moves, 4);

            /* The IVs for the Pokemon are randomly generated between 0 and 31. */
            iv_HP = (byte)PkmnUtils.RandomInclusive(31);
            iv_Attack = (byte)PkmnUtils.RandomInclusive(31);
            iv_Defence = (byte)PkmnUtils.RandomInclusive(31);
            iv_SpecialAttack = (byte)PkmnUtils.RandomInclusive(31);
            iv_SpecialDefence = (byte)PkmnUtils.RandomInclusive(31);
            iv_Speed = (byte)PkmnUtils.RandomInclusive(31);

            /* The EVs of the Pokemon are set according to the TrainerRank passed in.
             * This will only happen if the Pokemon is owned by an NPC trainer.
             * The TrainerRank will be null for player Pokemon (since they earn EVs through battle and start at 0).
             * It will also be null for wild Pokemon since they have EVs of 0. */
            if (rank.HasValue)
            {
                if (rank.Value == TrainerRank.Normal)
                {
                    ev_HP = ev_Attack = ev_Defence = ev_SpecialAttack = ev_SpecialDefence = ev_Speed = 0;
                }
                else if (rank.Value == TrainerRank.Grunt)
                {
                    ev_HP = ev_Attack = ev_Defence = ev_SpecialAttack = ev_SpecialDefence = ev_Speed = 10;
                }
                else if (rank.Value == TrainerRank.Elite)
                {
                    ev_HP = ev_Attack = ev_Defence = ev_SpecialAttack = ev_SpecialDefence = ev_Speed = 20;
                }
                else if (rank.Value == TrainerRank.GymLeader)
                {
                    ev_HP = ev_Attack = ev_Defence = ev_SpecialAttack = ev_SpecialDefence = ev_Speed = 40;
                }
                else if (rank.Value == TrainerRank.EliteFour)
                {
                    ev_HP = ev_Attack = ev_Defence = ev_SpecialAttack = ev_SpecialDefence = ev_Speed = 60;
                }
                else if (rank.Value == TrainerRank.Champion)
                {
                    ev_HP = ev_Attack = ev_Defence = ev_SpecialAttack = ev_SpecialDefence = ev_Speed = 80;
                }
            }
            else
            {
                /* This block executes if the TrainerRank passed in is null.
                 * This means newly created wild Pokemon or player Pokemon have EVs of 0. */
                ev_HP = ev_Attack = ev_Defence = ev_SpecialAttack = ev_SpecialDefence = ev_Speed = 0;
            }

            /* Finally, set the health of the Pokemon using the new HP stats. */
            health = new AttributePair(HP);
        }
    }
}
