using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Damdor.Progressio
{
    /// <summary>
    /// Global settings and object pooling for the Progressio library.
    /// </summary>
    public static class ProgressioSettings
    {
        /// <summary>
        /// Maximum number of ProgressBarController instances to keep in the pool.
        /// </summary>
        public static int MaxControllerPoolSize
        {
            get => maxControllerPoolSize;
            set
            {
                maxControllerPoolSize = value;
                while (controllers.Count >= maxControllerPoolSize) controllers.Pop();
            }
        }
        
        /// <summary>
        /// Maximum number of UiToolkitProgressBar instances to keep in the pool.
        /// </summary>
        public static int MaxUiToolkitProgressBarPoolSize
        {
            get => maxUiToolkitProgressBarPoolSize;
            set
            {
                maxUiToolkitProgressBarPoolSize = value;
                while (uiToolkitProgressBars.Count >= maxUiToolkitProgressBarPoolSize) uiToolkitProgressBars.Pop();
            }
        }
        
        private static readonly Stack<ProgressBarController> controllers = new();
        private static readonly Stack<UiToolkitProgressBar> uiToolkitProgressBars = new();
        private static int maxControllerPoolSize = 20;
        private static int maxUiToolkitProgressBarPoolSize = 20;

        /// <summary>
        /// Retrieves a ProgressBarController from the pool or creates a new one.
        /// </summary>
        /// <param name="apply">The action to apply progress value updates.</param>
        /// <param name="animation">Animation settings.</param>
        /// <param name="events">Events associated with the progress bar.</param>
        /// <returns>A ProgressBarController configured with the provided settings.</returns>
        public static ProgressBarController GetController(
            Action<float> apply,
            ProgressBarAnimation animation = null, 
            ProgressBarEvents events = null)
        {
            var controller = controllers.Count > 0 ? controllers.Pop() : new ProgressBarController();
            controller.Setup(apply, animation, events);
            return controller;
        }

        /// <summary>
        /// Returns a ProgressBarController to the pool for reuse.
        /// </summary>
        /// <param name="controller">The controller to release.</param>
        public static void ReleaseController(ProgressBarController controller)
        {
            if (controller == null) return;
            controller.Reset();
            if (controllers.Count < maxControllerPoolSize) controllers.Push(controller);
        }

        /// <summary>
        /// Retrieves a UiToolkitProgressBar from the pool or creates a new one.
        /// </summary>
        /// <param name="innerProgressBar">The underlying UI Toolkit AbstractProgressBar.</param>
        /// <param name="animation">Animation settings.</param>
        /// <param name="events">Events associated with the progress bar.</param>
        /// <returns>A UiToolkitProgressBar wrapped around the provided UI element.</returns>
        public static UiToolkitProgressBar GetUiToolkitProgressBar(
            AbstractProgressBar innerProgressBar,
            ProgressBarAnimation animation = null, 
            ProgressBarEvents events = null)
        {
            var progressBar = uiToolkitProgressBars.Count > 0 ? uiToolkitProgressBars.Pop() : new UiToolkitProgressBar();
            progressBar.Setup(innerProgressBar, animation, events);
            return progressBar;
        }
        
        /// <summary>
        /// Returns a UiToolkitProgressBar to the pool for reuse.
        /// </summary>
        /// <param name="progressBar">The progress bar to release.</param>
        public static void ReleaseUiToolkitProgressBar(UiToolkitProgressBar progressBar)
        {
            if (progressBar == null) return;
            progressBar.Reset();
            if (uiToolkitProgressBars.Count < maxUiToolkitProgressBarPoolSize) uiToolkitProgressBars.Push(progressBar);
        }
        
    }
}