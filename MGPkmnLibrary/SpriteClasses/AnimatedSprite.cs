using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MGPkmnLibrary.TileEngine;
namespace MGPkmnLibrary.SpriteClasses
{
    /* This class represents a Sprite which follows an animation cycle.
     * Every Character has an AnimatedSprite which is drawn on the screen. */
    public class AnimatedSprite
    {
        /* The class is mostly finished, but sequence data needs to be added.
         * This would allow the animation to play in a different order to the order in the sprite sheet. */

        /* The TileLocation has the tile coordinates of the AnimatedSprite on the map (as opposed to its pixel coordinates). */
        Point tileLocation;
        public Point TileLocation
        {
            set { tileLocation = value; }
            get { return tileLocation; }
        }

        /* The AnimatedSprite has seven other fields.
         * The animations field stores four animations, organized by their AnimationKey.
         * The currentAnimation field stores the AnimationKey matching the current sprite's animation.
         * IsAnimating is a self-explanatory bit which is used to check if the Sprite is already animating.
         * The texture field stores the sprite sheet for the AnimatedSprite.
         * Position is a Vector2 containing the pixel coordinates of the AnimatedSprite on the map.
         * Velocity is another Vector2 representing the movement of the AnimatedSprite in two dimensions,
         * it is always normalized upon being set.
         * Speed represents how fast the AnimatedSprite is moving, and it must always be a multiple of 16. */
        Dictionary<AnimationKey, Animation> animations;
        AnimationKey currentAnimation;
        bool isAnimating;
        Texture2D texture;
        Vector2 position;
        Vector2 velocity;
        float speed = 32f;
        public AnimationKey CurrentAnimation
        {
            get { return currentAnimation; }
            set { currentAnimation = value; }
        }
        public bool IsAnimating
        {
            get { return isAnimating; }
            set { isAnimating = value; }
        }
        public int Width {
            get { return animations[currentAnimation].FrameWidth; }
        }
        public int Height
        {
            get { return animations[currentAnimation].FrameHeight; }
        }
        public float Speed
        {
            get { return speed; }
            set { speed = MathHelper.Clamp(speed, 1.6f, 400f); }
        }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector2 Velocity
        {
            get { return velocity; }
            set {
                velocity = value;
                if (velocity != Vector2.Zero)
                    velocity.Normalize();
            }
        }

        /* The constructor for an AnimatedSprite takes the sprite sheet, animation Dictionary, and the starting position. */
        public AnimatedSprite(Texture2D sprite, Dictionary<AnimationKey, Animation> animation, Point startTile)
        {
            /* The texture, animations, and tileLocation fields are set from the parameters passed in.
             * The animation Dictionary is copied using the Animation.Clone() function to create a new copy of the Animations. */
            texture = sprite;
            tileLocation.X = startTile.X;
            tileLocation.Y = startTile.Y;
            animations = new Dictionary<AnimationKey, Animation>();
            foreach (AnimationKey key in animation.Keys)
            {
                animations.Add(key, (Animation)animation[key].Clone());
            }

            /* The position Vector is calculated from the startTile parameter, by multiplying it by the tile width and height.
             * The (Height - Engine.TileHeight) value is subtracted from the position.Y,
             * so that the bottom-left corner of the AnimatedSprite lines up with the Tile it's on. */
            position.X = startTile.X * Engine.TileWidth;
            position.Y = (startTile.Y * Engine.TileHeight) - (Height - Engine.TileHeight);
        }

        /* The Update() function updates the current animation in the animation list. */
        public void Update(GameTime gameTime)
        {
            if (isAnimating)
                animations[currentAnimation].Update(gameTime);
        }

        /* The Draw() function draws the texture sprite sheet using the position and a White colour for no tint.
         * The Current Frame Rectangle of the animation is passed in as the source rectangle. */
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, animations[currentAnimation].CurrentFrameRect, Color.White);
        }

        /* This function keeps the sprite within the bounds of the map.
         * It does this by clamping the position vector between zero and the bottom of the map minus the height of the sprite.
         * This means the bottom of the sprite cannot go below the bottom of the map. */
        public void LockToMap(int mapPixelWidth, int mapPixelHeight)
        {
            position.X = MathHelper.Clamp(position.X, 0 - (Width - Engine.TileWidth), mapPixelWidth - Engine.TileWidth);
            position.Y = MathHelper.Clamp(position.Y, 0 - (Height - Engine.TileHeight), mapPixelHeight - Height);
        }
    }
}