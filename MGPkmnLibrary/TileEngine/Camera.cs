using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MGPkmnLibrary.SpriteClasses;
using MGPkmnLibrary.WorldClasses;

namespace MGPkmnLibrary.TileEngine
{
    /* The CameraMode enum descibes whether a Camera is free or follow.
     * "Free" means the camera is not locked to anything and can be moved manually by the player.
     * "Follow" means the camera is locked onto a certain sprite and will follow the sprite.
     * In ordinary gameplay, the camera is on "Follow" mode and follows the player sprite. */
    public enum CameraMode
    {
        Free, Follow
    }

    /* The Camera class is the player's viewpoint of the in-game world. */
    public class Camera
    {
        /* A Camera object has five essential fields.
         * Position is a Vector2 containing the pixel coordinates of the top-left corner of the camera's viewpoint.
         * Speed represents how fast the camera is moving.
         * Zoom defines how big the Rectangle viewing the world is. The bigger the zoom, the smaller the Rectangle.
         * Mode is which CameraMode the camera is in.
         * There is also a reference to the current World, which is used to obtain the map size. */
        Vector2 position;
        float speed;
        float zoom;
        Rectangle viewportRectangle;
        CameraMode mode;
        World worldRef;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public float Speed
        {
            get { return speed; }
            set { speed = MathHelper.Clamp(speed, 1f, 16f); }
        }
        public float Zoom
        {
            get { return zoom; }
        }
        public CameraMode CameraMode
        {
            get { return mode; }
        }
        public World WorldRef
        {
            get { return worldRef; }
        }

        /* The Transformation Matrix applies the zoom and subtracts the camera's position from the destination Rectangle on the screen.
         * This means the camera's viewport Rectangle is always drawn on (0,0) on the screen, and has the same width and height. */
        public Matrix Transformation
        {
            get { return Matrix.CreateScale(zoom) * Matrix.CreateTranslation(new Vector3(-Position, 0f)); }
        }

        /* The ViewportRectangle property clones the Rectangle when returning it, instead of returning a reference. */
        public Rectangle ViewportRectangle
        {
            get { return new Rectangle(viewportRectangle.X, viewportRectangle.Y, viewportRectangle.Width, viewportRectangle.Height); }
        }

        /* The Camera class has two constructors. One takes a position, and the other does not.
         * In both cases, speed is set as one by default, and the zoom, viewportRectangle and worldRef are passed in.
         * The CameraMode is on "follow" initially, but can be changed later.
         * The overload without a position leaves the position as (0,0).
         * The overload with a position sets the position to the parameter passed in. */
        public Camera(Rectangle viewportRect, float initialZoom, World worldRef)
        {
            speed = 1f;
            zoom = initialZoom;
            viewportRectangle = viewportRect;
            mode = CameraMode.Follow;
            this.worldRef = worldRef;
        }
        public Camera(Rectangle viewportRect, Vector2 position, float initialZoom, World worldRef)
        {
            speed = 1f;
            zoom = initialZoom;
            viewportRectangle = viewportRect;
            Position = position;
            mode = CameraMode.Follow;
            this.worldRef = worldRef;
        }

        /* The Update() function for the camera handles its movement in "free" camera mode. */
        public void Update(GameTime gameTime)
        {
            /* If the CameraMode is set to "follow", that means the camera position is handled elsewhere, so nothing else needs updating. */
            if (mode == CameraMode.Follow)
                return;

            /* The motion vector is reset to zero, as movement is separate for each frame.
             * After this, the position vector has the speed added or subtracted depending on which arrow key is pressed.
             * This has the effect of the motion vector representing upwards movement when the up arrow is pressed, for example. */
            Vector2 motion = Vector2.Zero;
            if (InputHandler.KeyDown(Keys.Left) || InputHandler.ButtonDown(Buttons.RightThumbstickLeft, PlayerIndex.One))
                position.X -= speed;
            else if (InputHandler.KeyDown(Keys.Right)|| InputHandler.ButtonDown(Buttons.RightThumbstickRight, PlayerIndex.One))
                position.X += speed;
            if (InputHandler.KeyDown(Keys.Up) || InputHandler.ButtonDown(Buttons.RightThumbstickUp, PlayerIndex.One))
                position.Y -= speed;
            else if (InputHandler.KeyDown(Keys.Down) || InputHandler.ButtonDown(Buttons.RightThumbstickDown, PlayerIndex.One))
                position.Y += speed;

            /* If the motion has movement in it, the motion is normalized (set to a length of one).
             * After this, (motion * speed) is added to the position.
             * This means the position will move in motion's direction, by speed's distance.
             * Finally, the camera is locked so it can't move outside the bounds of the map. */
            if (motion != Vector2.Zero)
            {
                motion.Normalize();
                position += motion * speed;
                LockCamera();
            }
        }

        /* The ZoomIn() function increases the zoom by a quarter, and makes sure it doesn't exceed the maximum value of 2.5.
         * ZoomOut() is the same except it decreases the zoom by a quarter, and stops it going below the minimum value of 0.5.
         * Next, the camera is snapped to the position times zoom, by using SnapToPosition() in both functions.
         * This basically means that the camera zooms in/out centred around the centre of the viewport, not the top left. */
        public void ZoomIn()
        {
            zoom += 0.25f;
            if (zoom >= 2.5f)
                zoom = 2.5f;
            Vector2 newPosition = position * zoom;
            SnapToPosition(newPosition);
        }
        public void ZoomOut()
        {
            zoom -= 0.25f;
            if (zoom <= 0.5f)
                zoom = 0.5f;
            Vector2 newPosition = position * zoom;
            SnapToPosition(newPosition);
        }

        /* This function snaps the Camera to a given Vector2 position.
         * The position passed in is the position of the centre of the camera.
         * This means half the dimensions of the viewport rectange must be subtracted from the position,
         * in order to give the position of the top left of the camera. */
        private void SnapToPosition(Vector2 newPosition)
        {
            position.X = newPosition.X - viewportRectangle.Width / 2;
            position.Y = newPosition.Y - viewportRectangle.Height / 2;
            LockCamera();
        }

        /* LockCamera() is the function that makes sure the camera stays within the bounds of the map.
         * It does this by clamping the camera's position between zero and the maximum distance the camera can go without going off the map.
         * This maximum distance is the world's current map's width/height times the zoom, minus the width/height of the viewport rectangle.
         * The worldRef field is needed because of this function needing to check the width and height of the current map. */
        public void LockCamera()
        {
            position.X = MathHelper.Clamp(position.X, 0, worldRef.CurrentMap.WidthInPixels * zoom - viewportRectangle.Width);
            position.Y = MathHelper.Clamp(position.Y, 0, worldRef.CurrentMap.HeightInPixels * zoom - viewportRectangle.Height);
        }

        /* This function locks the camera to a certain sprite, such as the player sprite.
         * It changes the position to the centre of the sprite, then subtracts half the dimensions of the viewport rectangle.
         * This means the sprite will end up in the centre of the viewport rectangle. */
        public void LockToSprite(AnimatedSprite sprite)
        {
            position.X = (sprite.Position.X + sprite.Width / 2) * zoom - (viewportRectangle.Width / 2);
            position.Y = (sprite.Position.Y + sprite.Height / 2) * zoom - (viewportRectangle.Height / 2);
            LockCamera();
        }

        /* This function simply changes the CameraMode between free and follow.
         * If it's currently free, it's set to follow, and vice versa. */
        public void ToggleCameraMode()
        {
            if (mode == CameraMode.Follow)
                mode = CameraMode.Free;
            else if (mode == CameraMode.Free)
                mode = CameraMode.Follow;
        }
    }
}
