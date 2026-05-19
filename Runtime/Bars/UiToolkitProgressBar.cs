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
        /// Creates a new instance of <see cref="UiToolkitProgressBar"/> or retrieves one from the pool.
        /// </summary>
        /// <param name="innerProgressBar">The UI Toolkit progress bar to wrap.</param>
        /// <param name="data">The data object containing animation and event settings.</param>
        /// <returns>A configured instance of <see cref="UiToolkitProgressBar"/>.</returns>
        public static UiToolkitProgressBar Create(AbstractProgressBar innerProgressBar, UiToolkitProgressBarData data)
            => Create(innerProgressBar, data.Animation, data.Events);
        
        /// <summary>
        /// Creates a new instance of <see cref="UiToolkitProgressBar"/> or retrieves one from the pool.
        /// </summary>
        /// <param name="innerProgressBar">The UI Toolkit progress bar to wrap.</param>
        /// <param name="asset">The asset containing animation and event settings.</param>
        /// <returns>A configured instance of <see cref="UiToolkitProgressBar"/>.</returns>
        public static UiToolkitProgressBar Create(AbstractProgressBar innerProgressBar, UiToolkitProgressBarAsset asset)
            => Create(innerProgressBar, asset.Animation, asset.Events);
        
        /// <summary>
        /// Creates a new instance of <see cref="UiToolkitProgressBar"/> or retrieves one from the pool.
        /// </summary>
        /// <param name="innerProgressBar">The UI Toolkit progress bar to wrap.</param>
        /// <param name="animation">The animation settings.</param>
        /// <param name="events">The event settings.</param>
        /// <returns>A configured instance of <see cref="UiToolkitProgressBar"/>.</returns>
        public static UiToolkitProgressBar Create(AbstractProgressBar innerProgressBar, ProgressBarAnimation animation, ProgressBarEvents events)
        {
            var progressBar = ProgressioPooling.GetUiToolkitProgressBar();
            progressBar.Setup(innerProgressBar, animation, events);
            return progressBar;
        }
        
        private void Setup(AbstractProgressBar innerProgressBar, ProgressBarAnimation animation, ProgressBarEvents events)
        {
            this.innerProgressBar = innerProgressBar;
            controller = ProgressioPooling.GetController(
                Apply,
                animation?.Clone() ?? ProgressioPooling.GetAnimation(),
                events?.Clone() ?? ProgressioPooling.GetEvents()
            );
            ProgressBarUpdateProvider.Register(Update);
        }

        /// <summary>
        /// Releases the <see cref="UiToolkitProgressBar"/> instance back to the pool.
        /// This should be called when the progress bar is no longer needed.
        /// </summary>
        public void Release()
        {
            ProgressioPooling.ReleaseUiToolkitProgressBar(this);
        }

        /// <summary>
        /// Resets the wrapper, releasing its resources and internal controller to the pool.
        /// </summary>
        internal void Reset()
        {
            ProgressioPooling.ReleaseController(controller);
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