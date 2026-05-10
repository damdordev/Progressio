#if DAMDOR_PROGRESSIO_UIELEMENTS

using System.Threading;
using Cysharp.Threading.Tasks;
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
            ProgressBarUpdateProvider.Register(Update);
        }

        /// <summary>
        /// Resets the wrapper, releasing its resources and internal controller to the pool.
        /// </summary>
        public void Reset()
        {
            ProgressioManager.ReleaseController(controller);
            ProgressBarUpdateProvider.Unregister(Update);
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

        /// <inheritdoc />
        public void StartChanges() => controller.StartChanges();

        /// <inheritdoc />
        public void CommitChanges() => controller.CommitChanges();

        /// <inheritdoc />
        public void SetValueWithoutAnimation(float newValue) => controller.SetValueWithoutAnimation(newValue);

        /// <inheritdoc />
        public UniTask<bool> AnimateTo(float value, CancellationToken cancellationToken = default)
            => controller.AnimateTo(value, cancellationToken);

        private void Apply(float value)
        {
            innerProgressBar.value = 100f * value;
        }
        
        private void Update(float deltaTime, float unscaledDeltaTime)
        {
            controller.Update(controller.Animation != null
                ? controller.Animation.IgnoreTimescale ? unscaledDeltaTime : deltaTime
                : deltaTime);
        }
        
    }
}

#endif