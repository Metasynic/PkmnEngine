using System;

namespace PkmnEditor
{
    /* This class contains various useful functions that are used throughout the editor.
     * Many of them convert from the format used when downloading from PokeAPI into my own object formats. */
    public static class EditorUtils
    {
        /* This function simply checks if the double passed in is null and makes it zero if it is. */
        public static double NullToZero(double? number)
        {
            if (number.HasValue)
                return number.Value;
            else
                return 0;
        }

        /* Some of the enums such as growth rates and egg groups are named differently in PokeAPI than in my implementation.
         * Thus is it necessary to have this function, which fixes the discrepancies in enum names.
         * For example, what the PokeAPI calls the "medium" growth rate, I call the "mediumfast" growth rate. */
        public static string FixEnum(string input)
        {
            switch (input)
            {
                case "plant":
                    return "grass";
                case "medium-slow":
                    return "mediumslow";
                case "medium":
                    return "mediumfast";
                case "ground":
                    return "field";
                case "no-eggs":
                    return "undiscovered";
                case "humanshape":
                    return "humanlike";
                case "indeterminate":
                    return "amorphous";
                case "fast-then-very-slow":
                    return "fluctuating";
                case "slow-then-very-fast":
                    return "erratic";
                default:
                    return input;
            }
        }

        /* ParseToEnum<T>() very simply turns the string passed in into an enum of type T. */
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /* Border() creates an semi-aesthetic design across the top of the console window.
         * It is used to make things look a bit nicer in the editor. */
        public static void Border(string title)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(new string('-', 80));
            Console.WriteLine(title);
            Console.WriteLine(new string('-', 80));
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
