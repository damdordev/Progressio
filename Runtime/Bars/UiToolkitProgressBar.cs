using UnityEngine.UIElements;

namespace Damdor.Progressio
{
    /// <summary>
    /// A wrapper for UI Toolkit's AbstractProgressBar that integrates it with the Progressio library.
    /// Provides animation and event support for UI Toolkit progress bars.
    /// </summary>
    public class UiToolkitProgressBar : IProgressBar
    {
        private ProgressBarController controller;
        private AbstractProgressBar innerProgressBar;
        
        /// <summary>
        /// Sets up the wrapper with a UI Toolkit progress bar, animation settings, and events.
        /// </summary>
        /// <param name="innerProgressBar">The underlying UI Toolkit progress bar element.</param>
        /// <param name="animation">Animation settings. Can be null</param>
        /// <param name="events">Events associated with the progress bar. Can be null</param>
        public void Setup(AbstractProgressBar innerProgressBar, ProgressBarAnimation animation = null, ProgressBarEvents events = null)
        {
            this.innerProgressBar = innerProgressBar;
            controller = ProgressioManager.GetController(Apply, animation, events);
        }

        /// <summary>
        /// Updates the controller, driving animations if enabled. Usually called every frame.
        /// </summary>
        public void Update()
        {
            controller.Update();
        }

        /// <summary>
        /// Resets the wrapper, releasing its resources and internal controller to the pool.
        /// </summary>
        public void Reset()
        {
            ProgressioManager.ReleaseController(controller);
            controller = null;
            innerProgressBar = null;
        }

        /// <inheritdoc />
        public float Value
        {
            get => controller.Value;
            set => controller.Value = value;
        }

        /// <inheritdoc />
        public float DisplayedValue => controller.DisplayedValue;
        
        /// <inheritdoc />
        public ProgressBarAnimation Animation => controller.Animation;
        
        /// <inheritdoc />
        public ProgressBarEvents Events => controller.Events;

        private void Apply(float value)
        {
            innerProgressBar.value = 100f * value;
        }
        
    }
}