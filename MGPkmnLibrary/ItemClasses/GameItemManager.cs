using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MGPkmnLibrary.ItemClasses
{
    /* This class is the manager for the game items. It will act as a master list of GameItems.
     * GameItems will be cloned from this list.
     * The SpriteFont is not currently used and may not be needed.
     * It may be better to add another list to the Data Manager.
     * THIS CLASS IS NOT FINISHED AND MAY BE REDUNDANT. PLEASE IGNORE. */
    public class GameItemManager
    {
        readonly Dictionary<string, GameItem> gameItems = new Dictionary<string, GameItem>();
        static SpriteFont spriteFont;
        public Dictionary<string, GameItem> GameItems
        {
            get { return gameItems; }
        }
        public static SpriteFont SpriteFont
        {
            get { return SpriteFont; }
            private set { spriteFont = value; }
        }

    }
}
