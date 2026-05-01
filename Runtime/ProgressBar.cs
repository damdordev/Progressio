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
                if (Mathf.Approximately(this.value, value)) return;
                this.value = value;
                Apply(this.value);
            }
        }

        [SerializeField] private float value;

        /// <summary>
        /// Invoked internally when the progress value changes. 
        /// It should not modify the variable value, only modify the UI/Transform according to newValue.
        /// </summary>
        /// <param name="newValue">The new progress value to apply to the representation.</param>
        protected abstract void Apply(float newValue);

        /// <summary>
        /// Forces an update of the visual representation using the current progress value.
        /// This should be called when any properties that affect the representation change.
        /// </summary>
        protected void Refresh()
        {
            Apply(value);
        }

        protected virtual void OnEnable()
        {
            Apply(value);
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            Apply(value);
        }
#endif
    }
}
