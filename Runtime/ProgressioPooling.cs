using System;
using System.Collections.Generic;
#if DAMDOR_PROGRESSIO_UIELEMENTS
using UnityEngine.UIElements;
#endif

namespace Damdor.Progressio
{
    internal static class ProgressioPooling
    {
        public static int MaxControllerPoolSize
        {
            get => maxControllerPoolSize;
            set
            {
                maxControllerPoolSize = value;
                while (controllers.Count >= maxControllerPoolSize) controllers.Pop();
            }
        }
        
        private static readonly Stack<ProgressBarController> controllers = new();

        private static int maxControllerPoolSize = 20;

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
            if (controllers.Contains(controller)) return;
            controller.Reset();
            if (controllers.Count < maxControllerPoolSize) controllers.Push(controller);
        }
        
#if DAMDOR_PROGRESSIO_UIELEMENTS
        private static readonly Stack<UiToolkitProgressBar> uiToolkitProgressBarsPool = new();
        private static readonly Stack<ProgressBarAnimation> animationsPool = new();
        private static readonly Stack<ProgressBarEvents> eventsPool = new();
        private static int maxUiToolkitProgressBarPoolSize = 20;
        private static int maxAnimationPoolSize = 25;
        private static int maxEventsPoolSize = 25;
        
        /// <summary>
        /// Maximum number of UiToolkitProgressBar instances to keep in the pool.
        /// </summary>
        public static int MaxUiToolkitProgressBarPoolSize
        {
            get => maxUiToolkitProgressBarPoolSize;
            set
            {
                maxUiToolkitProgressBarPoolSize = value;
                while (uiToolkitProgressBarsPool.Count >= maxUiToolkitProgressBarPoolSize) uiToolkitProgressBarsPool.Pop();
            }
        }

        public static int MaxAnimationPoolSize
        {
            get => maxAnimationPoolSize;
            set
            {
                maxAnimationPoolSize = value;
                while (animationsPool.Count >= maxAnimationPoolSize) animationsPool.Pop();
            }
        }
        
        public static int MaxEventsPoolSize
        {
            get => maxEventsPoolSize;
            set
            {
                maxEventsPoolSize = value;
                while (eventsPool.Count >= maxEventsPoolSize) eventsPool.Pop();
            }
        }
        
        public static UiToolkitProgressBar GetUiToolkitProgressBar()
        {
            return uiToolkitProgressBarsPool.Count > 0 ? uiToolkitProgressBarsPool.Pop() : new UiToolkitProgressBar();
        }
        
        public static void ReleaseUiToolkitProgressBar(UiToolkitProgressBar progressBar)
        {
            if (progressBar == null) return;
            if (uiToolkitProgressBarsPool.Contains(progressBar)) return;
            progressBar.Reset();
            if (uiToolkitProgressBarsPool.Count < maxUiToolkitProgressBarPoolSize) uiToolkitProgressBarsPool.Push(progressBar);
        }       
        
        public static ProgressBarAnimation GetAnimation()
        {
            return animationsPool.Count > 0 ? animationsPool.Pop() : new ProgressBarAnimation();
        }
        
        public static void ReleaseAnimation(ProgressBarAnimation animation)
        {
            if (animation == null) return;
            animation.Reset();
            if (animationsPool.Count < maxAnimationPoolSize) animationsPool.Push(animation);
        } 
        
        public static ProgressBarEvents GetEvents()
        {
            return eventsPool.Count > 0 ? eventsPool.Pop() : new ProgressBarEvents();
        }
        
        public static void ReleaseEvents(ProgressBarEvents events)
        {
            if (events == null) return;
            events.Reset();
            if (eventsPool.Count < maxEventsPoolSize) eventsPool.Push(events);
        } 
        
#endif
        
    }
}