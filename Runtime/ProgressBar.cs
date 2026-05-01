using UnityEngine;

namespace Damdor.Progressio
{
    /// <summary>
    /// Base class for all progress bars. Provides an abstraction to visually represent a progress value.
    /// It updates automatically in the Editor when the value changes.
    /// </summary>
    [ExecuteAlways]
    public abstract class ProgressBar : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the current progress value.
        /// Changing this value will automatically update the visual representation.
        /// </summary>
        public float Value
        {
            get => value;
            set
            {
                var clampedValue = Mathf.Clamp01(value);
                if (Mathf.Approximately(this.value, clampedValue)) return;
                this.value = clampedValue;
                Refresh();
            }
        }
        
        /// <summary>
        /// The value currently being displayed. Might differ from 'Value' during an animation.
        /// </summary>
        public float DisplayedValue { get; private set; }

        [Tooltip("The target progress value between 0 and 1.")]
        [SerializeField, Range(0f, 1f)] private float value;
        
        [Tooltip("Animation settings for the progress bar.")]
        [SerializeField] private ProgressBarAnimation animation = new();

        private int changesLevel;
        private bool needRefresh;

        /// <summary>
        /// Begins a batch update operation, deferring visual refresh until <see cref="CommitChanges"/> is called.
        /// </summary>
        /// <remarks>
        /// Use this when making multiple property changes to avoid redundant refreshes.
        /// Must be paired with <see cref="CommitChanges"/> to apply the updates.
        /// </remarks>
        public void StartChanges()
        {
            ++changesLevel;
        }

        /// <summary>
        /// Ends a batch update operation and triggers an immediate refresh if any changes were made.
        /// </summary>
        public void CommitChanges()
        {
            --changesLevel;
            if (changesLevel <= 0 && needRefresh) Refresh();
        }

        /// <summary>
        /// Sets the progress value and instantly updates the visual representation, bypassing any animation.
        /// </summary>
        /// <param name="newValue">The progress value between 0 and 1.</param>
        public void SetValueWithoutAnimation(float newValue)
        {
            var clampedValue = Mathf.Clamp01(newValue);
            value = clampedValue;
            DisplayedValue = value;
            Refresh();
        }
        
        /// <summary>
        /// Invoked internally when the progress value changes. 
        /// It should not modify the variable value, only modify the UI/Transform according to newValue.
        /// </summary>
        /// <param name="newValue">The new progress value to apply to the representation.</param>
        protected abstract void Apply(float newValue);

        /// <summary>
        /// Forces an update of the visual representation using the current progress value.
        /// This should be called when any properties that affect the representation change.
        /// This method respects the StartChanges/CommitChanges block.
        /// </summary>
        protected void Refresh()
        {
            if (changesLevel > 0)
            {
                needRefresh = true;
                return;
            }
            needRefresh = false;

            if (!animation.Animated || !Application.isPlaying)
            {
                DisplayedValue = value;
            }

            Apply(DisplayedValue);
        }

        protected virtual void OnEnable()
        {
            DisplayedValue = value;
            Apply(DisplayedValue);
        }

        protected virtual void Update()
        {
            if (!animation.Animated || !Application.isPlaying || changesLevel > 0) return;
            if (Mathf.Approximately(DisplayedValue, value)) return;

            var dt = animation.IgnoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
            DisplayedValue = Mathf.Lerp(DisplayedValue, value, dt * animation.Speed);
            if (Mathf.Abs(DisplayedValue - value) < 0.001f)
            {
                DisplayedValue = value;
            }
            Apply(DisplayedValue);
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            Refresh();
        }
#endif
    }
}
