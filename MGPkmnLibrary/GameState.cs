using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MGPkmnLibrary
{
    /* The base state/screen class for the game. It inherits from Microsoft.Framework.DrawableGameComponent because it will be drawn to the screen. */
    public abstract partial class GameState : DrawableGameComponent
    {
        /* This is simply a list of components that are associated with the current screen.
         * These are the components that will be updated and drawn when the GameState is updated and drawn. */
        List<GameComponent> childComponents;
        public List<GameComponent> Components
        {
            get { return childComponents; }
        }

        /* This field stores a reference to the current GameState object.
         * It is used when comparing one GameState to another. */
        GameState tag;
        public GameState Tag
        {
            get { return tag; }
        }
        
        /* This field stores a reference to the game's GameStateManager.
         * It allows the GameState to call GameStateManager functions such as changing the screen. */
        protected GameStateManager StateManager;

        /* The GameState() constructor takes a reference to the Game and a reference to the GameStateManeger.
         * It passes the reference to the game into the parent DrawableGameComponent() constructor. */
        public GameState(Game game, GameStateManager manager) : base(game)
        {
            /* The StateManager reference is set from the parameter passed in.
             * The childComponents list is initialized, and the tag is set as a reference to the current object. */
            StateManager = manager;
            childComponents = new List<GameComponent>();
            tag = this;
        }

        /* The GameState does not require any special initialization code, so it just calls the parent function. */
        public override void Initialize()
        {
            base.Initialize();
        }

        /* The Update() function updates every child component in the childComponent list, then calls the parent function. */
        public override void Update(GameTime gameTime)
        {
            foreach(GameComponent component in childComponents)
            {
                if (component.Enabled)
                {
                    component.Update(gameTime);
                }
            }
            base.Update(gameTime);
        }

        /* The Draw() function draws every component in the list of childComponents.
         * However, things are a bit more complicated here because some components in the list will be
         * GameComponent objects, and some are DrawableGameComponent objects.
         * As the name suggests, only DrawableGameComponents can be drawn. */ 
        public override void Draw(GameTime gameTime)
        {
            /* To achieve this, a local DrawableGameComponent field is declared. */
            DrawableGameComponent drawComponent;
            foreach(GameComponent component in childComponents)
            {
                /* The function loops through each component and checks if it's a DrawableGameComponent. */
                if (component is DrawableGameComponent)
                {
                    /* If it is, a reference to the component is cast as a DrawableGameComponent and added to the local field.
                     * If the local field's drawable component is visible, it is drawn to the screen. */
                    drawComponent = component as DrawableGameComponent;
                    if (drawComponent.Visible)
                    {
                        drawComponent.Draw(gameTime);
                    }
                }
            }

            /* Finally, the parent's Draw() function is called. */
            base.Draw(gameTime);
        }

        /* This function is called when there is a change to which game screen is currently in use.
         * In this case, the sender argument is a reference to the new GameState.
         * If the new state is the current state being checked, then the state needs to be shown and Show() is called.
         * Otherwise, the new state isn't the same and needs to be hidden, so Hide() is called. */
        internal protected virtual void StateChange(object sender, EventArgs e)
        {
            if (StateManager.CurrentState == Tag)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        /* This function makes everything in the GameState enabled and visible.
         * It is called when the game screen is changed to the current GameState. */
        protected virtual void Show()
        {
            /* First, the Visible and Enabled properties of the current GameState are set to true. */
            Visible = true;
            Enabled = true;
            foreach(GameComponent component in childComponents)
            {
                /* Next, for each of the child components, the child component's Enabled property is set to true.
                 * If the child component is drawable, then its Visible property is also set to true. */
                component.Enabled = true;
                if (component is DrawableGameComponent)
                {
                    ((DrawableGameComponent)component).Visible = true;
                }
            }
        }

        /* This function does the opposite of Show(), and is called when the game screen is changed from this GameState to another one. */
        protected virtual void Hide()
        {
            /* Visible and Enabled are set to false, and all child components have Enabled (and Visible if applicable) set to false. */
            Visible = false;
            Enabled = false;
            foreach (GameComponent component in childComponents)
            {
                component.Enabled = false;
                if (component is DrawableGameComponent)
                {
                    ((DrawableGameComponent)component).Visible = false;
                }
            }
        }
    }
}
