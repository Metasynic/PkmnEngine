using System.Collections.Generic;

namespace MGPkmnLibrary.PokemonClasses
{
    /* The TrainerManager is a class used in the PkmnEditor.
     * It manages a Dictionary<string, Trainer> used to store all the Trainers in the editor. */
    public class TrainerManager
    {
        /* This Dictionary stores the Trainers in the manager.
         * The key of the Dictionary is the trainer name. */
        readonly Dictionary<string, Trainer> trainers = new Dictionary<string, Trainer>();
        public Dictionary<string, Trainer> Trainers
        {
            get { return trainers; }
        }
        
        /* The TrainerKeys property returns a KeyCollection of all the trainer names. */
        public Dictionary<string, Trainer>.KeyCollection TrainerKeys
        {
            get { return trainers.Keys; }
        }

        /* The constructor is empty since the Dictionary was initialized after declaration. */
        public TrainerManager()
        {

        } 

        /* The AddTrainer() function takes a trainer as a parameter.
         * After that, it checks if the Dictionary already contains a Trainer with the same name.
         * If it doesn't, then the Trainer is added to the list using its name as the key. */
        public void AddTrainer(Trainer trainer)
        {
            if (!trainers.ContainsKey(trainer.TrainerName))
            {
                trainers.Add(trainer.TrainerName, trainer);
            }
        }

        /* GetTrainer() returns a clone of the Trainer with the requested name, as long as it exists in the list.
         * The function returns null if the Trainer does not exist. Exception handling could be better here. */
        public Trainer GetTrainer(string name)
        {
            if (trainers.ContainsKey(name))
            {
                return (Trainer)trainers[name].Clone();
            }
            return null;
        }

        /* This function returns a bit describing whether the Dictionary contains a Trainer with the specified name. */
        public bool ContainsTrainer(string name)
        {
            return trainers.ContainsKey(name);
        }
    }
}
