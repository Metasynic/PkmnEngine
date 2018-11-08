using System;

namespace XLvlEditor
{
    /* A MapDisplay is a basic class that draws the map on the level editor screen.
     * It inherits from GraphicsDeviceControl using the WinFormsGraphicsDevice API class.
     * NB TO EXAMINER: The "GraphicsDeviceControl.cs", "GraphicsDeviceService.cs" and "ServiceContainer.cs" are not annotated with my comments because they're not my code.
     * Instead, they are API classes used in the level editor and this class to draw an XNA object to a windows form. */
    public class MapDisplay : WinFormsGraphicsDevice.GraphicsDeviceControl
    {
        /* Two event handlers are defined here, OnInitialize and OnDraw.
         * OnInitialize is invoked when the MapDisplay is initialized, by calling it inside the override Initialize() function.
         * OnDraw is invoked when the MapDisplay is drawn, by calling it inside the override Draw() function.
         * These two event handlers are necessary because I want to define what happens on initialization and drawing elsewhere. */
        public event EventHandler OnInitialize;
        public event EventHandler OnDraw;
        protected override void Initialize()
        {
            OnInitialize?.Invoke(this, null);
        }
        protected override void Draw()
        {
            OnDraw?.Invoke(this, null);
        }
    }
}
