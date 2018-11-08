using System;
using Microsoft.Xna.Framework;
namespace MGPkmnLibrary.SpriteClasses
{
    /* This enum contains the four possible directions that a sprite can move in a 2D world.
     * A sprite will have a different animation for each AnimationKey. */
    public enum AnimationKey
    {
        Down, Left, Right, Up
    }

    /* The Animation class represents an Animation with multiple frames taken from the same sprite sheet.
     * All Characters have four Animations, one for each direction. */
    public class Animation
    {
        /* This array stores the different source rectangles for each frame of the animation.
         * For instance, an animation with four frames will have four source rectangles in this array. */
        Rectangle[] frames;

        /* The frameLength field represents how long each frame should be displayed for.
         * The frameTimer is used as a running total describing how long the current frame has been displaying. */
        TimeSpan frameLength;
        TimeSpan frameTimer;

        /* The fps (frames per second) is self explanatory. It is the number of times the frame changes per second.
         * The setter clamps the fps between 1 and 60, and sets the frameLength to one second divided by the fps.
         * This that fps times frameLength will equal one second. */
        int fps;
        public int FPS
        {
            get { return fps; }
            set
            {
                if (value < 1)
                    fps = 1;
                else if (value > 60)
                    fps = 60;
                else
                    fps = value;
                frameLength = TimeSpan.FromSeconds(1 / (double)fps);
            }
        }

        /* The currentFrame field stores the current array index of the frame being displayed.
         * The setter clamps it between zero and the length of the array, so that the index can't go outside the array. */
        int currentFrame;
        public int CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = (int)MathHelper.Clamp(value, 0, frames.Length - 1); }
        }

        /* CurrentFrameRect is a property that returns the current source rectangle of the frame being displayed. */
        public Rectangle CurrentFrameRect
        {
           get
            {
                return frames[currentFrame];
            }
        }

        /* These fields represent the width and height of the animation. 
         * All frames in an animation should have the same width and height. */
        int frameWidth;
        int frameHeight;
        public int FrameWidth
        {
            get { return frameWidth; }
        }
        public int FrameHeight
        {
            get { return frameHeight; }
        }

        /* The private constructor takes an animation as the parameter, and sets a default FPS of 5. */
        private Animation(Animation animation)
        {
            this.frames = animation.frames;
            FPS = 5;
        }

        /* The public constructor for an Animation takes the frame count, width, height, and offsets for the first frame on the sprite sheet.
         * The constructor assumes that all the frames in the Animation are on the same row of the sheet, and will not wrap around to the next row. */
        public Animation (int frameCount, int frameWidth, int frameHeight, int xOffset, int yOffset)
        {
            /* The frames array is initialized with the number of frames in the animation. */
            frames = new Rectangle[frameCount];

            /* The width and height are set from the parameters. */
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;

            /* Each source rectangle is added using the offsets. The x offset is increased by the frame width times the frame number.
             * The y offset stays the same because the animation is all assumed to be on the same row on the sprite sheet. */
            for (int i = 0; i < frameCount; i++)
            {
                frames[i] = new Rectangle(xOffset + (frameWidth * i), yOffset, frameWidth, frameHeight);
            }

            /* The FPS is set to five as default, and the Animation is reset. */
            FPS = 5;
            Reset();
        }

        /* The Update() function increases the frameTimer by the time that has elapsed since the last call to Update().
         * Once the frameTimer becomes greater than or equal to the frameLength, it's time to change the frame.
         * When the frame is changed, the frameTimer goes back to zero and the frame index has one added to it.
         * The index is modded by frames.Length, which ensures it loops back around once it goes outside the length of the array. */
        public void Update(GameTime gameTime)
        {
            frameTimer += gameTime.ElapsedGameTime;
            if (frameTimer >= frameLength)
            {
                frameTimer = TimeSpan.Zero;
                currentFrame = (currentFrame + 1) % frames.Length;
            }
        }

        /* Reset() just sets the frame index to the beginning of the animation, and the timer to zero. */
        public void Reset()
        {
            currentFrame = 0;
            frameTimer = TimeSpan.Zero;
        }

        /* An animation needs to be clonable, so this function creates an exact copy of the object by using the private constructor.
         * The animation clone needs to be an object, not a reference, so this function returns a new object. */
        public object Clone()
        {
            Animation animationClone = new Animation(this);
            animationClone.frameWidth = this.frameWidth;
            animationClone.frameHeight = this.frameHeight;
            animationClone.Reset();
            return animationClone;
        }
    }
}
