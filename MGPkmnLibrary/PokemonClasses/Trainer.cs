using System;

namespace MGPkmnLibrary.PokemonClasses
{
    /* This enum represents the Rank of the Trainer. It's used to determine the EVs of the Trainer's team of Pokemon.
     * The higher-ranking an opponent, the more EVs their Pokemon will have to represent more training. */
    public enum TrainerRank { Normal, Grunt, Elite, GymLeader, EliteFour, Champion}
    
    /* This enum stores the AI level of the Trainer. The harder the difficulty, the more likely the Trainer is to use super effective moves in battle. */
    public enum TrainerDifficulty { VeryEasy, Easy, Normal, Hard, VeryHard }

    /* This is the main class for an NPC Pokemon Trainer in the game. */
    public class Trainer
    {
        /* The Trainer's name will appear when the player initiates a battle with them.
         * It's currently used as the identifier for the Trainer, meaning it has to be unique. */
        protected string trainerName;
        public string TrainerName
        {
            get { return trainerName; }
            set { trainerName = value; }
        }

        /* The dialogue is the text that is shown when the Trainer is defeated.
         * For example, "Oh no! How could I possibly lose?". */
        protected string dialogue;
        public string Dialogue
        {
            get { return dialogue; }
            set { dialogue = value; }
        }

        /* The Rank of the Trainer, used to determine the EVs of its Pokemon. */
        protected TrainerRank rank;
        public TrainerRank Rank
        {
            get { return rank; }
            set { rank = value; }
        }

        /* The Difficulty of the Trainer, used to determine its AI algorithm. */
        protected TrainerDifficulty difficulty;
        public TrainerDifficulty Difficulty
        {
            get { return difficulty; }
            set { difficulty = value; }
        }

        /* This is the array containing the Trainer's team of Pokemon.
         * A Trainer (or player) can only have up to six Pokemon. */
        protected Pokemon[] trainerPokemon = new Pokemon[6];
        public Pokemon[] TrainerPokemon
        {
            get { return trainerPokemon; }
            set { trainerPokemon = value; }
        }

        /* This private constructor initializes a Trainer with null values.
         * It is used in the Clone() function, and when deserializing a Trainer from XML. */
        private Trainer()
        {

        }

        /* The public Trainer constructor takes the name, rank, difficulty, team, and dialogue of the Trainer.
         * The fields are all set appropriately from the parameters. */
        public Trainer(string name, TrainerRank rank, TrainerDifficulty difficulty, Pokemon[] trainerPokemon, string dialogue, byte pokemonCount)
        {
            trainerName = name;
            this.rank = rank;
            this.difficulty = difficulty;
            Array.Copy(trainerPokemon, this.trainerPokemon, 6);
            this.dialogue = dialogue;
        }

        /* The Clone() function returns an exact copy of the Trainer.
         * It is used when copying a Trainer from the master list in DataManager.
         * This means that when a Trainer is cloned, it creates a new copy instead of passing a reference. */
        public object Clone()
        {
            Trainer trainer = new Trainer();
            trainer.trainerName = trainerName;
            trainer.rank = rank;
            trainer.difficulty = difficulty;
            for (int i = 0; i < 6; i++)
            {
                trainer.trainerPokemon[i] = trainerPokemon[i];
            }
            trainer.dialogue = dialogue;
            return trainer;
        }
    }
}
