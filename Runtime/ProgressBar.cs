using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Damdor.Progressio
{
    /// <summary>
    /// Abstract base class for creating mono-based progress bars.
    /// </summary>
    [ExecuteAlways]
    public abstract class ProgressBar : MonoBehaviour, IProgressBar
    {
        private ProgressBarController innerController;
        private ProgressBarController controller => innerController ??= ProgressioPooling.GetController(Apply, animation, events);

        /// <inheritdoc />
        public float Value
        {
            get => controller.Value;
            set => controller.Value = value;
        }

        /// <inheritdoc />
        public float DisplayedValue => controller.DisplayedValue;

        /// <inheritdoc />
        public ProgressBarAnimation Animation => animation;
        
        /// <inheritdoc />
        public ProgressBarEvents Events => events;

        [Tooltip("The target progress value between 0 and 1.")]
        [SerializeField, Range(0f, 1f)] private float value;
        
        [Tooltip("Animation settings for the progress bar.")]
        [SerializeField] private new ProgressBarAnimation animation = new();
        
        [Tooltip("Events associated with the progress bar's lifecycle and value changes.")]
        [SerializeField] private ProgressBarEvents events = new();

        /// <summary>
        /// Starts a batch of changes to the progress bar. This will prevent the progress bar from refreshing until CommitChanges is called.
        /// </summary>
        public void StartChanges() => controller.StartChanges();
        
        /// <summary>
        /// Commits a batch of changes to the progress bar. This will refresh the progress bar if there are any pending changes.
        /// </summary>
        public void CommitChanges() => controller.CommitChanges();
        
        /// <summary>
        /// Sets the value of the progress bar without triggering any animations.
        /// </summary>
        /// <param name="newValue">The new value to set.</param>
        public void SetValueWithoutAnimation(float newValue) => controller.SetValueWithoutAnimation(newValue);
        
        public UniTask<bool> AnimateTo(float value, CancellationToken cancellationToken = default)
            => controller.AnimateTo(value, cancellationToken);
        
        /// <summary>
        /// This method is called to apply the new progress value to the visual representation of the progress bar.
        /// </summary>
        /// <param name="newValue">The new value to apply.</param>
        protected abstract void Apply(float newValue);
        
        /// <summary>
        /// Refreshes the progress bar, applying any pending changes.
        /// </summary>
        protected void Refresh() => controller.Refresh();

        protected virtual void Start() => controller.Start();

        protected virtual void Update() =>
            controller.Update(animation.IgnoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime);

        protected void OnDestroy()
        {
            ProgressioPooling.ReleaseController(innerController);
            innerController = null;
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;
            controller.Value = value;
        }
#endif
    }
}
