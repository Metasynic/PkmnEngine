using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MGPkmnLibrary
{
    /* This class manages all input for the engine. The engine has one instance of this class.
     * it contains methods to handle input from both keyboard and Xbox 360 controller. 
     * Xbox360 controller handling is not finished and not a part of the user requirements.
     * InputHandler inherits from GameComponent because the InputHandler is a Component that will be added to Game.Components. */
    public class InputHandler : GameComponent
    {
        /* A KeyboardState is an XNA object storing a snapshot of the current state of the keyboard.
         * A GamePadState is an XNA object storing a snapshot of an Xbox 360 controller.
         * An array of GamePadStates is needed because there may be more than one Xbox 360 controller connected.
         * The KeyboardStates are single objects because the engine should only accept input from one keyboard.
         * There are two fields each for the keyboard and Xbox 360 controllers.
         * One stores the current state of the input device, and the other stores the state from the last iteration of the game loop. */
        static KeyboardState keyboardState;
        static KeyboardState lastKeyboardState;
        static GamePadState[] gamePadStates;
        static GamePadState[] lastGamePadStates;

        public static KeyboardState KeyboardState
        {
            get { return keyboardState; }
        }
        public static KeyboardState LastKeyboardState
        {
            get { return lastKeyboardState; }
        }
        public static GamePadState[] GamePadStates
        {
            get { return gamePadStates; }
        }
        public static GamePadState[] LastGamePadStates
        {
            get { return lastGamePadStates; }
        }

        /* The constructor for an InputHandler takes a reference to the current game, and passes it into the parent GameComponent constructor. */
        public InputHandler(Game game) : base(game)
        {
            /* The keyboardState field is set to the current state of the keyboard.
             * The gamePadStates array is initialized with the length of the PlayerIndex enum (which should be four).
             * Then, each value in the array is set to the current state of the corresponding game pad. */
            keyboardState = Keyboard.GetState();
            gamePadStates = new GamePadState[Enum.GetValues(typeof(PlayerIndex)).Length];
            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
            {
                gamePadStates[(int)index] = GamePad.GetState(index);
            }
        }

        /* The Initialize() function just calls the parent's Initialize() function. */
        public override void Initialize()
        {
            base.Initialize();
        }

        /* The Update() function updates the component every frame. */
        public override void Update(GameTime gameTime)
        {
            /* The Flush() function is used to move the previous input states into the last state fields.
             * The new keyboardState is obtained using Keyboard.GetState().
             * The same happens with the gamePadStates, except GamePad.GetState() is used to get the current state of each game pad. */
            Flush();
            keyboardState = Keyboard.GetState();
            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
            {
                gamePadStates[(int)index] = GamePad.GetState(index);
            }

            /* Finally, the parent's Update() function is also called. */
            base.Update(gameTime);
        }

        /* This function is used to push the current input states into the fields for the last input states. */
        public static void Flush()
        {
            lastKeyboardState = keyboardState;
            lastGamePadStates = (GamePadState[])gamePadStates.Clone();
        }

        /* The way the class works out if a key/button has been pressed/released is by comparing its current state to its last current state.
         * For instance, KeyReleased() works by checking if the key was down last time, and is up this time. */
        public static bool KeyReleased(Keys key)
        {
            return keyboardState.IsKeyUp(key) && lastKeyboardState.IsKeyDown(key);
        }

        /* Similarly, KeyPressed() works by checking if the key was up last time, and is down this time. */
        public static bool KeyPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key);
        }

        /* KeyDown() just checks whether a certain key is currently being held down. */
        public static bool KeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        /* All of the Button functions work the same way as the Key functions, except they take a PlayerIndex,
         * which represents which game pad is being checked. The index enum must be cast as an int to be used to reference the pad's location in the arrays. */
        public static bool ButtonReleased(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonUp(button) && lastGamePadStates[(int)index].IsButtonDown(button);
        }
        public static bool ButtonPressed(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonDown(button) && lastGamePadStates[(int)index].IsButtonUp(button);
        }
        public static bool ButtonDown(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonDown(button);
        }
    }
}
