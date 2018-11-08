using System;

namespace MGPkmnLibrary.PokemonClasses
{
    /* An AttributePair class represents a combination of a current and maximum value.
     * This is used, for example, for the HP of a Pokemon - it has a current and maximum value.
     * It is also used for the PP of Moves. */
    [Serializable]
    public class AttributePair
    {
        /* These two numerical fields store the current value of the AttributePair, and the maximum value it can store. */
        int currentValue;
        int maximumValue;
        public int CurrentValue
        {
            get { return currentValue; }
            set { currentValue = value; }
        }
        public int MaximumValue
        {
            get { return maximumValue; }
            set { maximumValue = value; }
        }

        /* A static Attribute Pair with current and maximum values of zero. Not currently used. */
        public static AttributePair Zero
        {
            get { return new AttributePair(); }
        }

        /* This is the private constructor for an AttributePair, which is used when an AttributePair is deserialized from XML. */
        private AttributePair()
        {
            currentValue = 0;
            maximumValue = 0;
        }

        /* The public constructor, which takes a maximum value.
         * When an AttributePair is created, it is "full" (as in the current value is the same as the maximum). */
        public AttributePair(int maxValue)
        {
            currentValue = maxValue;
            maximumValue = maxValue;
        }

        /* These two functions are like plus and minus operators for AttributePairs.
         * The Add() function adds a value to the current value, but will not allow it to exceed the maximum value.
         * The Subtract() function removes a value from the current value, but will not allow it to be less than zero.
         * This means that whenever a Pokemon is damaged or healed, its HP will stay within the correct bounds. */
        public void Add(ushort value)
        {
            currentValue += value;
            if (currentValue > maximumValue)
                currentValue = maximumValue;
        }
        public void Subtract(ushort value)
        {
            currentValue -= value;
            if (currentValue < 0)
                currentValue = 0;
        }

        /* This function sets the current value of the AttributePair directly, 
         * but still won't let it exceed the maximum value or go below zero. */
        public void SetCurrent(int value)
        {
            currentValue = value;
            if (currentValue > maximumValue)
                currentValue = maximumValue;
            if (currentValue < 0)
                currentValue = 0;
        }

        /* This function sets the maximum value for the AttributePair, 
         * but will not allow the maximum to be below the current value. */
        public void SetMaximum(int value)
        {
            maximumValue = value;
            if (currentValue > maximumValue)
                currentValue = maximumValue;
        }

        /* This makes the representation of the AttributePair a bit neater when viewing it in the debugger. */
        public override string ToString()
        {
            return (currentValue + "/" + maximumValue);
        }
    }
}
