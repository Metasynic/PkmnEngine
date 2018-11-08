using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MGPkmnLibrary
{
    /* This enum represents the three different types of state change that can take place.
     * The state manager is stack based, so there can be a pop, push, or change (which empties the stack then pushes). */
    public enum ChangeType { Change, Pop, Push }

    /* The GameStateManager a core class of the engine, which governs the order in which game screens are displayed. */
    public class GameStateManager : GameComponent
    {
        /* The OnStateChange event handler is an event that is triggered when the game state is changed in any way. */
        public event EventHandler OnStateChange;

        /* The gameStates stack is the main component of the manager. It holds all the GameStates that are currently in memory. */
        Stack<GameState> gameStates = new Stack<GameState>();

        /* These three integers are parts of the draw number system.
         * Every GameState is a DrawableGameComponent, so it has a DrawOrder property.
         * The initialDraw field will be the DrawOrder of the first item on the stack.
         * The incrementDraw field is the amount the DrawOrder increases for each new GameState on the stack.
         * The currentDraw field is the DrawOrder of the current GameState at the top of the stack. */
        const int initialDraw = 5000;
        const int incrementDraw = 100;
        int currentDraw;

        /* This property exposes the top GameState on the stack using the Stack<T>.Peek() method. */
        public GameState CurrentState
        {
            get { return gameStates.Peek(); }
        }

        /* The constructor passes the game reference to the parent constructor, and sets currentDraw to initialDraw. */
        public GameStateManager(Game game) : base(game)
        {
            currentDraw = initialDraw;
        }

        /* The Initialize() function calls its parent function. */
        public override void Initialize()
        {
            base.Initialize();
        }

        /* The Update() function also calls its parent. */
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        
        /* This function handles pushing a new state to the stack. 
         * The first thing it does is push the newState argument on to the stack.
         * Next, it adds the newState to the game's list of components. This means it will get updated and drawn.
         * Finally, the OnStateChange event handler has the newState's StateChange added to it. */
        private void AddState(GameState newState)
        {
            gameStates.Push(newState);
            Game.Components.Add(newState);
            OnStateChange += newState.StateChange;
        }

        /* RemoveState() is a glorified wrapper for popping a GameState off the stack.
         * It saves a reference to the top state into the state field. 
         * It then removes the state's StateChange event from the OnStateChange() event handler.
         * Finally, it removes the state from the game's master list of components (stopping it being updated and drawn), and pops it off the stack. */
        private void RemoveState()
        {
            GameState state = gameStates.Peek();
            OnStateChange -= state.StateChange;
            Game.Components.Remove(state);
            gameStates.Pop();
        }

        /* This function adds a state to the stack and handles draw order incrementing. */
        public void PushState(GameState newState)
        {
            /* First, the current draw order is incremented by the correct interval.
             * Then, the new state's DrawOrder property is set to the currentDraw field (meaning it will get drawn at the top).
             * The state is added to the stack using AddState, and then the OnStateChange event handler is invoked so its event is executed if it isn't null. */
            currentDraw += incrementDraw;
            newState.DrawOrder = currentDraw;
            AddState(newState);
            OnStateChange?.Invoke(this, null);
        }

        /* The PopState() function checks if there's something on the stack.
         * If there is, the top GameState is removed using RemoveState(), and the currentDraw field is decremented by the interval.
         * Finally, the OnStateChange event handler is invoked so that its event runs if it's not null. */
        public void PopState()
        {
            if (gameStates.Count > 0)
            {
                RemoveState();
                currentDraw -= incrementDraw;
                OnStateChange?.Invoke(this, null);
            }
        }

        /* The ChangeState function resets the stack and adds the newState argument as the only GameState on the stack. */
        public void ChangeState(GameState newState)
        {
            /* First, every state is removed from the stack. */
            while (gameStates.Count > 0)
            {
                RemoveState();
            }

            /* After this, the newState's DrawOrder property and the currentDraw are both set to the initial draw order.
             * The newState is added to the stack, and the new OnStateChange event is invoked. */
            newState.DrawOrder = initialDraw;
            currentDraw = initialDraw;
            AddState(newState);
            OnStateChange?.Invoke(this, null);
        }
    }
}
