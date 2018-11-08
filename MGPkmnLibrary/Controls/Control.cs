using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MGPkmnLibrary.Controls
{
    /* This class is quite a loose blueprint for anything that gets drawn to the screen.
     * It is a parent class which is inherited by all the other classes in MGPkmnLibrary.Controls, except ControlManager. */
    public abstract class Control
    {
        /* Controls have a lot of overarching properties. Not all of them are used by child classes.
         * Name is the name of the control.
         * Text is only used in text-based controls and shows the text that will be displayed.
         * Size and position are both self-explanatory Vector2 objects. Position's Y value is rounded to an int when it's set.
         * Value is not used yet but holds an object selected by a menu control.
         * HasFocus represents whether a menu control currently accepts input from the player.
         * Enabled represents whether it's possible for the control to have focus and accept input.
         * Visible dictates whether the control is drawn or not.
         * TabStop is used to show whether the control can be selected using the arrow keys.
         * SpriteFont is the font used to render any text in the control.
         * Color is the colour of the control, where applicable.
         * Type is not yet implemented. */
        protected string name;
        protected string text;
        protected Vector2 size;
        protected Vector2 position;
        protected object value;
        protected bool hasFocus;
        protected bool enabled;
        protected bool visible;
        protected bool tabStop;
        protected SpriteFont spriteFont;
        protected Color color;
        protected string type;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        public Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; position.Y = (int)position.Y; }
        }
        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public virtual bool HasFocus
        {
            get { return hasFocus; }
            set { hasFocus = value; }
        }
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }
        public bool TabStop
        {
            get { return tabStop; }
            set { tabStop = value; }
        }
        public SpriteFont SpriteFont
        {
            get { return spriteFont; }
            set { spriteFont = value; }
        }
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        /* This event handler represents the event that will happen when the control is selected in a menu. */
        public event EventHandler Selected;
        
        /* The constructor sets some default values for the new control. The default colour is white.
         * The default is for a control to be Enabled and Visible, and use the ControlManager's SpriteFont.
         * Any of these default values can be changed by setting the relevant property. */
        public Control()
        {
            Color = Color.White;
            Enabled = true;
            Visible = true;
            SpriteFont = ControlManager.SpriteFont;
        }

        /* Abstract functions to Update, Draw, and Handle Input for the control.
         * Since different controls behave differently, they have different Update/Draw/HandleInput functions.
         * This means these three functions need to be abstract. */
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void HandleInput(PlayerIndex playerIndex);

        /* This function is called when the control is selected. It fires the Selected() EventHandler. */
        protected virtual void OnSelected(EventArgs e)
        {
            Selected?.Invoke(this, e);
        }
    }
}
