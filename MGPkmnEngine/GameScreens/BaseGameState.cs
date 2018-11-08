using System;
using MGPkmnLibrary;
using MGPkmnLibrary.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PkmnEngine.GameScreens
{
    /* The BaseGameState class is the parent class to all the screens in the game. */
    public abstract partial class BaseGameState : GameState
    {
        /* All screens need to contain some common fields.
         * GameRef is a reference to the current engine instance.
         * ControlManager is the ControlManager object for the screen. Each screen has a separate ControlManager.
         * The playerIndexInControl field represents which gamepad is currently being used (if any). */
        protected PokemonEngine GameRef;
        protected ControlManager ControlManager;
        protected PlayerIndex playerIndexInControl;

        /* These fields are a part of transitioning from one state to another.
         * DestinationState holds a reference to the next state that's going to be used.
         * The other fields handle timings if there needs to be a time gap between the call to change state, and actually changing state.
         * Changing is a bit which represents whether the state is currently changing.
         * ChngType is the ChangeType enum with the type of change that is happening.
         * ChangeTimer holds the amount of time that has elapsed since the actual call to change the state.
         * Interval is the target amount of time that should pass before the engine changes state. */
        protected BaseGameState DestinationState;
        protected bool Changing;
        protected ChangeType ChngType;
        protected TimeSpan ChangeTimer;
        protected TimeSpan Interval = TimeSpan.FromSeconds(0.5);

        /* The BaseGameState constructor takes a reference to the game and the state manager, which are both passed to the parent GameState() constructor.
         * The GameRef field is filled with a reference to the game passed in.
         * The default playerIndexInControl is set to the gamepad plugged into slot one. */
        public BaseGameState(Game game, GameStateManager manager) : base(game, manager)
        {
            GameRef = (PokemonEngine)game;
            playerIndexInControl = PlayerIndex.One;
        }

        /* The Initialize() function calls the parent function. */
        public override void Initialize()
        {
            base.Initialize();
        }

        /* There are several things that the LoadContent() methods of all the screens need to have in common.
         * First, the menu SpriteFont is loaded in using the game's ContentManager.
         * The ControlManager is then initialized using the menu font, and the parent function is called. */
        protected override void LoadContent()
        {
            ContentManager Content = Game.Content;
            SpriteFont menuFont = Content.Load<SpriteFont>(@"Fonts\ControlFont");
            ControlManager = new ControlManager(menuFont);
            base.LoadContent();
        }

        /* The Update() function handles changing of state. */
        public override void Update(GameTime gameTime)
        {
            /* If Changing is true, then state changing needs to be updated. */
            if (Changing)
            {
                /* If we wanted to have a set transition time between screens, then it would be necessary to increment the ChangeTimer,
                 * using the amount of time that has passed since the last call to Update().
                 * However, we want instant transition, so instead of checking that the ChangeTimer has exceeded the required Interval,
                 * we bypass the condition by replacing it with if(true). */
                // ChangeTimer += gameTime.ElapsedGameTime;
                if (true) // if (ChangeTimer >= Interval)
                    {
                    /* If the state is going to be changed, then the Changing bit is set to false so that the class won't try to change state again next time. */
                    Changing = false;
                    switch(ChngType)
                    {
                        /* In each case for the different types of ChangeType, the correct StateManger function is called for that ChangeType.
                         * For example, if it's a Push, the StateManager.PushState() function is called, taking DestinationState as a parameter. */
                        case ChangeType.Change:
                            StateManager.ChangeState(DestinationState);
                            break;
                        case ChangeType.Pop:
                            StateManager.PopState();
                            break;
                        case ChangeType.Push:
                            StateManager.PushState(DestinationState);
                            break;
                    }
                }
            }

            /* Finally, the parent function is called. */
            base.Update(gameTime);
        }

        /* As drawing is specific to each screen, the BaseGameState.Draw() function just calls its parent function. */
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        /* This function is called when the engine wants to change state. It takes the ChangeType of the change, and a reference to the destination state. */
        public virtual void Change(ChangeType type, BaseGameState state)
        {
            /* Changing is set to true, the ChangeType is saved to the ChngType field, 
             * DestinationState is set to the state passed in, and the timer resets to zero. */
            Changing = true;
            ChngType = type;
            DestinationState = state;
            ChangeTimer = TimeSpan.Zero;
        }
    }
}
