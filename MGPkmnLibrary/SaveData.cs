using System;
using Microsoft.Xna.Framework;
using MGPkmnLibrary.PokemonClasses;

namespace MGPkmnLibrary
{
    /* A SaveData is an object that holds all the information necessary to save the game.
     * It is marked as serializable for obvious reasons - it can't be saved externally otherwise. */
    [Serializable]
    public class SaveData
    {
        /* This field holds the ID of the level the player is in. */
        private string levelID;
        public string LevelID
        {
            get { return levelID; }
            set { levelID = value; }
        }

        /* These two ints hold the tile location of the player on the map. */
        private int tileX, tileY;
        public int TileX
        {
            get { return tileX; }
            set { tileX = value; }
        }
        public int TileY
        {
            get { return tileY; }
            set { tileY = value; }
        }

        /* The player's team is also saved as a part of the data. */
        Pokemon[] team;
        public Pokemon[] Team
        {
            get { return team; }
            set { team = value; }
        }

        /* The player's character gender needs to be remembered as well. */
        string gender;
        public string Gender
        {
            get { return gender; }
            set { gender = value; }
        }

        /* The private constructor is only used when deserializing the object, and initializes the Pokemon team array. */
        private SaveData()
        {
            team = new Pokemon[6];
        }

        /* The main public constructor takes all the fields needed to create a SaveData and sets them accordingly. */
        public SaveData(string levelID, Point tileCoords, Pokemon[] team, string gender)
        {
            this.levelID = levelID;
            tileX = tileCoords.X;
            tileY = tileCoords.Y;
            this.team = new Pokemon[6];
            Array.Copy(team, this.team, 6);
            this.gender = gender;
        }
    }
}
