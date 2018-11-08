using System;
using Microsoft.Xna.Framework;

namespace MGPkmnLibrary.TileEngine
{
    /* This class represents a tile whose source rectangle changes every few frames, giving an animation effect.
     * This class is incomplete and not a part of the user requirements. */
    public class AnimatedTile
    {
        /* The fps field stores the number of frame changes per second. The setter for fps clamps it between 1 and 60.
         * The TileIndex field stores the index of the tile on the tileset. */
        private int fps;
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
                Length = TimeSpan.FromSeconds(1 / (double)fps);
            }
        }
        public int TileIndex;

        /* CurrentFrame is a property that represents the current frame in the list of frames.
         * FrameCount is the number of frames in the list. */
        public int CurrentFrame;
        public int FrameCount;

        /* Elapsed is the amount of time that has passed since the last frame change.
         * Length is the amount of time that should pass before the next frame change occurs. */
        public TimeSpan Elapsed;
        public TimeSpan Length;

        /* AnimatedTile constructor takes the tile index and frame count and sets them appropriately.
         * CurrentFrame starts at zero (the beginning of the animation) and the FPS is 8 by default.
         * Elapsed starts at zero, and Length is calculated by dividing one second by the frame count. */
        public AnimatedTile(int tileIndex, int frameCount)
        {
            TileIndex = tileIndex;
            FrameCount = frameCount;
            CurrentFrame = 0;
            fps = 8;
            Elapsed = TimeSpan.Zero;
            Length = TimeSpan.FromSeconds(1 / (double)fps);
        }

        /* The Update() function adds the amount of time that has passed since the last call to Update() to Elapsed.
         * If the Elapsed time exceeds the Length, the frame needs to change, so Elapsed goes back to zero.
         * When the frame changes, the current frame has one added to it, and is modded by the frame count.
         * This ensures that the current frame is always within the bounds of the array. */
        public void Update(GameTime gameTime)
        {
            Elapsed += gameTime.ElapsedGameTime;
            if (Elapsed >= Length)
            {
                Elapsed = TimeSpan.Zero;
                CurrentFrame = (CurrentFrame + 1) % FrameCount;
            }
        }
    }
}
