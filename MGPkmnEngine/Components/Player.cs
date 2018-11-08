using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MGPkmnLibrary;
using MGPkmnLibrary.TileEngine;
using MGPkmnLibrary.SpriteClasses;
using MGPkmnLibrary.CharacterClasses;
using MGPkmnLibrary.PokemonClasses;
using MGPkmnLibrary.ItemClasses;
using MGPkmnLibrary.WorldClasses;

namespace PkmnEngine.Components
{
    /* This class is a central component of the engine, representing the player in the world. */
    public class Player
    {
        /* The Player class has some important fields. The bag field stores the Player's bag, which holds all their items.
         * The camera field is the Camera that displays to the screen and is focussed on the player sprite.
         * The class also contains a Character object that stores the player's movement methods and sprites.
         * There is a string to store the character choice of the player (male or female).
         * Lastly, there is an array of Pokemon with a length of six holding the player's Pokemon team. */
        Bag bag;
        Camera camera;
        PokemonEngine gameRef;
        Character character;
        Pokemon[] team;
        string gender;

        public Bag Bag
        {
            get { return bag; }
        }
        public Camera Camera
        {
            get { return camera; }
            set { camera = value; }
        }
        public Pokemon[] Team
        {
            get { return team; }
        }
        public string Gender
        {
            get { return gender; }
        }

        /* There are a couple of properties here to expose fields in the player's Character object.
         * Moving shows whether the player's movement is currently not Movement.None.
         * StepCheck represents whether checks made after a step is taken, such as checking for wild Pokemon spawns, need to take place.
         * Sprite exposes the Character's AnimatedSprite object which gets drawn to the screen.
         * TilePosition exposes the player's location on the map, and is used when saving the game. */
        public bool Moving
        {
            get { return character.Moving; }
        }
        public bool StepCheck
        {
            get { return character.StepCheck; }
            set { character.StepCheck = value; }
        }
        public AnimatedSprite Sprite
        {
            get { return character.Sprite; }
        }
        public Point TilePosition
        {
            get { return Sprite.TileLocation; }
        }

        /* The Player constructor takes a reference to the game, a string for the gender choice, and a Character object.
         * A World object is passed through to the Camera. */
        public Player (Game game, Character character, string gender, World worldRef)
        {
            /* The gameRef, character and gender fields are set from the parameters passed in. */
            gameRef = (PokemonEngine)game;
            this.character = character;
            this.gender = gender;

            /* The camera is initialized using the window rectangle, and a zoom of two.
             * CameraFollow is set to true, since we want the camera to follow the player.
             * The team is initialized with a length of six, and the bag is also created. */
            camera = new Camera(gameRef.ScreenRectangle, 2f, worldRef);
            character.CameraFollow = true;
            team = new Pokemon[6];
            bag = new Bag();
        }

        /* The Update() function updates the player by updating its camera and character, and by handling movement. */
        public void Update(GameTime gameTime)
        {
            camera.Update(gameTime);
            character.Update(gameTime, camera);

            /* This commented out block of code allows the user to zoom in and out using the Camera.ZoomIn() and Camera.ZoomOut() functions.
             * It is not currently usable in the engine, since you can't zoom in or out in regular Pokemon games. */
            /* if (InputHandler.KeyReleased(Keys.O) || InputHandler.ButtonReleased(Buttons.LeftShoulder, PlayerIndex.One))
            {
                camera.ZoomIn();
                if (camera.CameraMode == CameraMode.Follow)
                    camera.LockToSprite(Sprite);
            }
            else if (InputHandler.KeyReleased(Keys.P) || InputHandler.ButtonReleased(Buttons.RightShoulder, PlayerIndex.One))
            {
                camera.ZoomOut();
                if (camera.CameraMode == CameraMode.Follow)
                    camera.LockToSprite(Sprite);
            } */

            /* This block handles input relating to the movement of the player. 
             * If the relevant W/A/S/D key (or gamepad stick) is down, then the character.Move() function is called with the direction and a reference to the map. */
            if (character.Dir == Movement.None)
            {
                if (InputHandler.KeyDown(Keys.W) || InputHandler.ButtonDown(Buttons.LeftThumbstickUp, PlayerIndex.One))
                {
                    character.Move('u', GameScreens.WorldScreen.World.CurrentMap);
                }
                else if (InputHandler.KeyDown(Keys.S) || InputHandler.ButtonDown(Buttons.LeftThumbstickDown, PlayerIndex.One))
                {
                    character.Move('d', GameScreens.WorldScreen.World.CurrentMap);
                }
                else if (InputHandler.KeyDown(Keys.A) || InputHandler.ButtonDown(Buttons.LeftThumbstickLeft, PlayerIndex.One))
                {
                    character.Move('l', GameScreens.WorldScreen.World.CurrentMap);
                }
                else if (InputHandler.KeyDown(Keys.D) || InputHandler.ButtonDown(Buttons.LeftThumbstickRight, PlayerIndex.One))
                {
                    character.Move('r', GameScreens.WorldScreen.World.CurrentMap);
                }
            }

            /* The window title is updated with the tile coordinates of the player for debugging purposes. */
            gameRef.Window.Title = "PlayerX: " + Sprite.TileLocation.X + " PlayerY: " + Sprite.TileLocation.Y;

            /* This commented out block allows the user to switch between "free" and "follow" camera modes.
             * At the moment, only "follow" is allowed, since the camera should follow the player. */
            /* if (InputHandler.KeyReleased(Keys.F) || InputHandler.ButtonReleased(Buttons.RightStick, PlayerIndex.One))
            {
                camera.ToggleCameraMode();
                if (camera.CameraMode == CameraMode.Follow)
                    camera.LockToSprite(Sprite);
            }
            if (camera.CameraMode != CameraMode.Follow)
            {
                if (InputHandler.KeyReleased(Keys.C) || InputHandler.ButtonReleased(Buttons.LeftStick, PlayerIndex.One))
                {
                    camera.LockToSprite(Sprite);
                }
            }
            */
        }

        /* The Draw() function for the player just draws the character. */
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            character.Draw(gameTime, spriteBatch);
        }

        /* InitTeam() resets the team array, and sets its first item to the Pokemon passed in. */
        public void InitTeam(Pokemon starter)
        {
            Array.Clear(team, 0, team.Length);
            team[0] = starter;
        }

        /* AddToTeam() takes one Pokemon as a parameter and adds it to the end of the team array if it isn't full. */
        public void AddToTeam(Pokemon pokemon)
        {
            int teamCount = (team.Count(s => s != null));
            if (teamCount < 6) {
                team[teamCount] = pokemon;
            }
        }
    }
}
