using System.Collections.Generic;

namespace MGPkmnLibrary.PokemonClasses
{
    /* This simple class is used in the PkmnEditor and contains a dictionary of PokemonData, with the ID as the key. */
    public class PokemonDataManager
    {
        readonly Dictionary<ushort, PokemonData> pokemonData = new Dictionary<ushort, PokemonData>();
        public Dictionary<ushort, PokemonData> PokemonData
        {
            get { return pokemonData; }
        }
    }
}
