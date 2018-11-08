namespace PkmnEngine
{
    /* The engine is currently only confirmed to work on Windows, but support for other platforms could be added using MonoGame. */
#if WINDOWS
    /* This class is the main wrapper for the PkmnEngine. It is the outermost layer of programming in the engine. */
    static class Program
    {
        /* This is the actual entry point for the engine. It can take arguments but none need to be given. */
        static void Main(string[] args)
        {
            /* A new PokemonEngine is created, and then it is run. */
            using (PokemonEngine game = new PokemonEngine())
            {
                game.Run();
            }
        }
    }
#endif
}

