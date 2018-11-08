using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MGPkmnLibrary.Controls
{
    /* The BattleLog is a type of Control that draws text to the screen character by character.
     * It is used on the BattleScreen to display messages like "Squirtle used Scratch! (5 damage)" or similar. */
    public class BattleLog : Control
    {
        /* There are a lot of fields which work together to make the BattleLog work correctly.
         * The charCounter is the index of the current character in the message that is being displayed.
         * Two TimeSpan objects, timeCounter and timeInterval, hold the amount of time since the last character was added to the display string,
         * and the amount of time between adding characters to the display string respectively.
         * The currentLogItem is the whole string of the message that is being written.
         * The currentString is a specific portion of the currentLogItem that is visible on the screen at the current point in time.
         * A flag, emptying, stores whether the log is currently in the process of displaying its contents to the screen.
         * There is a reference to a Queue<string> named logRef, which stores the location of the battle log list.
         * A Texture2D stores the background image for the BattleLog, and the finishedEmptying flag is set to true once there's nothing left to display. */
        int charCounter;
        TimeSpan timeCounter;
        TimeSpan timeInterval;
        string currentLogItem;
        string currentString;
        bool emptying;
        Queue<string> logRef;
        Texture2D background;
        bool finishedEmptying;

        public bool Emptying
        {
            get { return emptying; }
        }
        public Queue<string> LogRef
        {
            get { return logRef; }
        }
        public bool FinishedEmptying
        {
            get { return finishedEmptying; }
            set { finishedEmptying = value; }
        }

        /* The BattleLog constructor takes the background image, queue reference, and the time interval in seconds.
         * Conversion from double to TimeSpan uses TImeSpan.FromSeconds(). */
        public BattleLog(Texture2D image, Queue<string> list, double interval)
        {
            background = image;
            logRef = list;
            timeInterval = TimeSpan.FromSeconds(interval);
        }

        /* The Update() function is where most of the work gets done for the BattleLog.
         * It uses the gameTime parameter to get how much time has passed between this call and the last call of the function. */
        public override void Update(GameTime gameTime)
        {
            /* The entirety of the function is enclosed inside this if statement, since it only does something if it's emptying. */
            if (emptying)
            {
                /* If there isn't a currentLogItem, then either the log has just started emptying, or has just finished displaying one item from the queue. */
                if (currentLogItem == null)
                {
                    /* If there are still items left in the log, then set the currentLogItem to the first thing in the queue.
                     * The currentString is now blank, as with a new item, nothing should be displayed yet.
                     * The charCounter and timeCounter are reset to zero. */
                    if (logRef.Count > 0)
                    {
                        currentLogItem = logRef.Dequeue();
                        currentString = "";
                        charCounter = 0;
                        timeCounter = TimeSpan.Zero;
                    }

                    /* Otherwise, there's nothing left in the log queue to display, so the log is disabled, and becomes invisible.
                     * The emptying flag is now set to false, and finishedEmptying to true. */
                    else
                    {
                        emptying = false;
                        visible = false;
                        enabled = false;
                        finishedEmptying = true;
                    }
                }

                /* If the amount of time elapsed since the last change of character is greater than or equal to the interval, a new character needs to be displayed. */
                if (timeCounter >= timeInterval)
                {
                    /* If the charCounter has reached the end of the currentLogItem string, then the next character from the log item is added to the currentString.
                     * The charCounter then increases by one. */
                    if (charCounter < currentLogItem.Length)
                    {
                        currentString += currentLogItem[charCounter];
                        charCounter++;
                    }

                    /* Otherwise, we've reached the end of the currentLogItem, so it's set to null so it gets set next time round.
                     * The charCounter is also reset to zero. */
                    else
                    {
                        currentLogItem = null;
                        charCounter = 0;
                    }

                    /* Since the new character has been added, the interval is now removed from the counter. */
                    timeCounter -= timeInterval;
                }

                /* In all cases, the ElapsedGameTime since the last call to Update() is added to the counter. */
                timeCounter += gameTime.ElapsedGameTime;
            }
        }

        /* The Draw() function is in charge of rendering the BattleLog to the screen. It takes a reference to the game's SpriteBatch. */
        public override void Draw(SpriteBatch spriteBatch)
        {
            /* A Vector2 variable called drawTo is used to hold the current location of the drawing "pen".
             * First, the background is drawn to the BattleLog's position, with no tint colour.
             * Then, the "pen" is moved down and right by 10 pixels. */
            Vector2 drawTo = position;
            spriteBatch.Draw(background, drawTo, Color.White);
            drawTo.X += 10;
            drawTo.Y += 10;

            /* If there's a valid string in currentString, then it will be split into an IEnumerable of lines using PkmnUtils.SplitString(), passing in the string and background width, as well as the width of one character.
             * Then, each of the lines is drawn using the BattleLog's spriteFont in black.
             * Each time a new line is drawn, the "pen" is moved down by the font's line spacing plus three pixels. */
            if (currentString != null)
            {
                IEnumerable<string> lines = PkmnUtils.SplitString(currentString, background.Width / (int)(spriteFont.MeasureString(" ").X));
                foreach(string s in lines)
                {
                    spriteBatch.DrawString(spriteFont, s, drawTo, Color.Black);
                    drawTo.Y += spriteFont.LineSpacing + 3f;
                }
            }
        }

        /* The log takes no input whatsoever, so this function is empty. */
        public override void HandleInput(PlayerIndex playerIndex)
        {

        }

        /* This function is called when the BattleLog needs to start displaying text to the screen.
         * First, it becomes visible and enabled. The emptying flag is set to true so that the code in the Update() function is executed.
         * The currentLogItem is initialised as null, and finishedEmptying is set to false.
         * Finally, a fresh reference to the log list is set using the argument passed in. */
        public void Empty(Queue<string> list)
        {
            emptying = true;
            visible = true;
            enabled = true;
            currentLogItem = null;
            finishedEmptying = false;
            logRef = list;
        }
    }
}
