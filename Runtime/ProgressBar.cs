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

        [SerializeField, Range(0f, 1f)] private float value;
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
            Apply(value);
        }

        protected virtual void OnEnable()
        {
            Apply(value);
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            Refresh();
        }
#endif
    }
}
