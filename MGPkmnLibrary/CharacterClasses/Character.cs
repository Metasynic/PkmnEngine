using MGPkmnLibrary.SpriteClasses;
using MGPkmnLibrary.PokemonClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MGPkmnLibrary.TileEngine;

namespace MGPkmnLibrary.CharacterClasses
{
    /* This enum represents what direction a character is moving in. */
    public enum Movement { Up, Down, Left, Right, None }
    
    /* This class is a character in the game, whether it be the player or an NPC.
     * Any character with a sprite being drawn on the map uses this class. */ 
    public class Character
    {
        /* StepCheck is a bit which tells the World class whether the character has just taken a step or not.
         * It is primarily used so that the World class can check for Pokemon spawns each time a step is taken.
         * If StepCheck is true, then the World needs to check for a possible spawn. */
        bool stepCheck;
        public bool StepCheck
        {
            get { return stepCheck; }
            set { stepCheck = value; }
        }

        /* CameraFollow simply shows whether the camera should follow this character or not.
         * As of now, this bit is always set the true for the player, and false for all other characters.
         * If cutscenes are implemented in the future, it might be useful to be able to focus the camera on another character. */
        bool cameraFollow;
        public bool CameraFollow
        {
            get { return cameraFollow; }
            set { cameraFollow = value; }
        }

        /* The Movement enum for the character, which determines how it moves (or if it moves at all). */
        Movement dir;
        public Movement Dir
        {
            get { return dir; }
            set { dir = value; }
        }

        /* A property to make it a bit easier to check if the character is currently moving. */
        public bool Moving
        {
            get { return (!(dir == Movement.None)); }
        }

        /* The Name property is an identifier for the character.
         * If the character is a trainer, this name displays on the screen during a battle with the character. */
        protected string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /* Pretty self-explanatory, this bit represents whether the character can be battled (trainer),
         * or is just an NPC who says stuff when you talk to them (non-trainer). */
        protected bool isTrainer;
        public bool IsTrainer
        {
            get { return isTrainer; }
        }

        /* The sprite for the character. It is drawn on the WorldScreen. */
        protected AnimatedSprite sprite;
        public AnimatedSprite Sprite
        {
            get { return sprite; }
        }

        /* The Trainer object for the character, which contains their battle dialogue, difficulty, rank, and Pokemon team. */
        protected Trainer trainer;
        public Trainer Trainer
        {
            get { return trainer; }
        }

        /* There are two overloads to the character constructor. Both constructors take a sprite and a name.
         * In both cases, stepCheck is set to false, and dir to none, as they should not move initially.
         * This overload is used for a trainer character and takes a Trainer object. */
        public Character(AnimatedSprite sprite, string name, Trainer trainer)
        {
            this.sprite = sprite;
            this.trainer = trainer;
            isTrainer = true;
            stepCheck = false;
            dir = Movement.None;
        }
        
        /* This overload is the same as the last one, except it is for non-trainer characters, such as the player.
         * Obviously, it does not take a Trainer object. */
        public Character(AnimatedSprite sprite, string name)
        {
            this.sprite = sprite;
            isTrainer = false;
            stepCheck = false;
            dir = Movement.None;
        }

        /* This function checks whether the character is able to move or not.
         * It does this by looking at whether the character would walk off the map if they moved, depending on their direction.
         * It also looks at the tile they're trying to move on to. If it's solid, they cannot move onto the tile. */
        public bool CanMove(char direction, TileMap map)
        {
            /* canMove is initially true, so if no reason is found to make it false, the character can move. */
            bool canMove = true;
            switch(direction)
            {
                /* Uses of foreach loops are necessary because the same tile on every layer must be checked for solidity.
                 * Depending on the direction, different tiles must be examined.
                 * If the player is fulfilling the conditions, they should not be able to move. */
                case 'u':
                    foreach(MapLayer layer in map.MapLayers)
                    {
                        /* Checks if the sprite is not on the top edge of the map, and whether the tile above is solid. */
                        if (sprite.TileLocation.Y > 0 && (layer.GetTile(sprite.TileLocation.X, sprite.TileLocation.Y - 1).Solid))
                            canMove = false;
                    }
                    break;
                case 'd':
                    foreach (MapLayer layer in map.MapLayers)
                    {
                        /* Checks if the sprite is not on the bottom edge of the map, and whether the tile below is solid. */
                        if (sprite.TileLocation.Y < layer.Height - 1 && (layer.GetTile(sprite.TileLocation.X, sprite.TileLocation.Y + 1).Solid))
                            canMove = false;
                    }
                    break;
                case 'l':
                    foreach (MapLayer layer in map.MapLayers)
                    {
                        /* Checks if the sprite is not on the left edge of the map, and whether the tile to the left is solid. */
                        if (sprite.TileLocation.X > 0 && (layer.GetTile(sprite.TileLocation.X - 1, sprite.TileLocation.Y).Solid))
                            canMove = false;
                    }
                    break;
                case 'r':
                    foreach (MapLayer layer in map.MapLayers)
                    {
                        /* Checks if the sprite is not on the right edge of the map, and whether the tile to the right is solid. */
                        if (sprite.TileLocation.X < layer.Width - 1 && (layer.GetTile(sprite.TileLocation.X + 1, sprite.TileLocation.Y).Solid))
                            canMove = false;
                    }
                    break;
                default:
                    break;
            }

            /* Finally, the canMove bit is returned. */
            return canMove;
        }

        /* This function kickstarts the movement of a character if they are told to move. */
        public virtual void Move(char direction, TileMap map)
        {
            /* Obviously if the character is already moving, then they can't start moving again. */
            if (dir == Movement.None)
            {
                switch (direction)
                {
                    /* In the case of each direction, the sprite animation is changed to match the direction.
                     * If the player can move in that direction, then dir is changed to that direction.
                     * When dir is changed, the Update() function will know which way to move the character. */
                    case 'u':
                        Sprite.CurrentAnimation = AnimationKey.Up;
                        if (CanMove(direction, map))
                            dir = Movement.Up;
                        break;
                    case 'd':
                        Sprite.CurrentAnimation = AnimationKey.Down;
                        if (CanMove(direction, map))
                            dir = Movement.Down;
                        break;
                    case 'l':
                        Sprite.CurrentAnimation = AnimationKey.Left;
                        if (CanMove(direction, map))
                            dir = Movement.Left;
                        break;
                    case 'r':
                        Sprite.CurrentAnimation = AnimationKey.Right;
                        if (CanMove(direction, map))
                            dir = Movement.Right;
                        break;
                }
            }
        }

        /* Updates the character every frame. */
        public virtual void Update(GameTime gameTime, Camera camera)
        {
            /* First, update the current sprite animation. */
            sprite.Update(gameTime);

            /* Create a new Vector2 to represent the character's motion. */
            Vector2 motion = new Vector2();

            switch (dir)
            {
                /* Depending on the direction, the motion Vector will be changed to represent it.
                 * We will now have a Vector2 showing which way the character should move.
                 * Diagonal movement is not allowed. */
                case Movement.Up:
                    motion.Y = -1;
                    break;
                case Movement.Down:
                    motion.Y = 1;
                    break;
                case Movement.Left:
                    motion.X = -1;
                    break;
                case Movement.Right:
                    motion.X = 1;
                    break;
            }

            /* If the character is supposed to move, then this block of code makes the character move. */
            if (motion != Vector2.Zero)
            {
                /* Tells the sprite that it should be animating. */
                Sprite.IsAnimating = true;

                /* Just in case the motion is not a unit vector, it is converted into one. */
                motion.Normalize();

                /* This line is where the actual movement happens.
                 * The motion is multiplied by a scale factor of the speed divided by 64.
                 * After being multiplied, the motion vector is added to the sprite's position vector, resulting in visible movement. */
                Sprite.Position += motion * Sprite.Speed / 64;

                /* The sprite is then locked to the bounds of the map so it can't vanish off the map.
                 * If the camera exists and the character is supposed to be followed by the camera, 
                 * The camera will lock itself to the character. */
                Sprite.LockToMap(camera.WorldRef.CurrentMap.WidthInPixels, camera.WorldRef.CurrentMap.HeightInPixels);
                if (cameraFollow && camera != null)
                {
                    if (camera.CameraMode == CameraMode.Follow)
                    {
                        camera.LockToSprite(Sprite);
                    }
                }
            }
            else
            {
                /* This bit only happens if the character isn't supposed to be moving.
                 * The sprite will remain static. */
                Sprite.IsAnimating = false;
            }

            /* This fancy block of code locks the sprite to the tile.
             * What this means is that if the sprite is aligned with a tile, it will stop moving.
             * This is extremely helpful because the sprite will not stop while moving between tiles.
             * First, the game needs to check the following:
             * 1) Whether the sprite's position in X is aligned to the tile width.
             * 2) Whether the sprite's position in Y is aligned to the tile height.
             * 3) Whether the sprite is currently moving.
             * The sprite position has (sprite width/height - tile width/height) added to it.
             * This is done so that any difference between sprite size and tile size is accounted for. */
            if (((Sprite.Position.X + (Sprite.Width - Engine.TileWidth)) % Engine.TileWidth == 0 && (Sprite.Position.Y + (Sprite.Height - Engine.TileHeight)) % Engine.TileHeight == 0) && Moving)
            {
                switch(dir)
                {
                    /* In the case of each direction, the game checks if the sprite is not on an edge tile.
                     * If the sprite isn't on an edge tile, the TileLocation property is changed to match the tile it's now on.
                     * This means the TileLocation only changes when the sprite finishes moving, which is useful. */
                    case Movement.Up:
                        if (Sprite.TileLocation.Y > 0)
                            Sprite.TileLocation = new Point(Sprite.TileLocation.X, Sprite.TileLocation.Y - 1);
                        break;
                    case Movement.Down:
                        if (Sprite.TileLocation.Y < (camera.WorldRef.CurrentMap.HeightInPixels / Engine.TileHeight) - 1)
                            Sprite.TileLocation = new Point(Sprite.TileLocation.X, Sprite.TileLocation.Y + 1);
                        break;
                    case Movement.Left:
                        if (Sprite.TileLocation.X > 0)
                            Sprite.TileLocation = new Point(Sprite.TileLocation.X - 1, Sprite.TileLocation.Y);
                        break;
                    case Movement.Right:
                        if (Sprite.TileLocation.X < (camera.WorldRef.CurrentMap.WidthInPixels / Engine.TileWidth) - 1)
                            Sprite.TileLocation = new Point(Sprite.TileLocation.X + 1, Sprite.TileLocation.Y);
                        break;
                }

                /* Finally, the sprite stops moving since it's aligned to a tile.
                 * The game now needs to check any events that occur when a step is taken, so stepCheck is set to true. */
                dir = Movement.None;
                stepCheck = true;
            }
        }

        /* The only thing in the character that needs drawing is its sprite. */
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            sprite.Draw(gameTime, spriteBatch);
        }

    }
}
