using UnityEngine;

namespace Damdor.Progressio
{
    /// <summary>
    /// Progress bar that modifies the local scale of a Transform.
    /// </summary>
    public class ScaleProgressBar : ProgressBar
    {
        /// <summary>
        /// Gets or sets the target Transform.
        /// </summary>
        public Transform Target 
        { 
            get => target; 
            set 
            { 
                if (target == value) return;
                target = value; 
                Refresh(); 
            } 
        }

        /// <summary>
        /// Gets or sets the scale mapped to progress 0.
        /// </summary>
        public Vector3 StartScale 
        { 
            get => startScale; 
            set 
            { 
                if (startScale == value) return;
                startScale = value; 
                Refresh(); 
            } 
        }

        /// <summary>
        /// Gets or sets the scale mapped to progress 1.
        /// </summary>
        public Vector3 EndScale 
        { 
            get => endScale; 
            set 
            { 
                if (endScale == value) return;
                endScale = value; 
                Refresh(); 
            }
        }
        
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 startScale = Vector3.zero;
        [SerializeField] private Vector3 endScale = Vector3.one;

        /// <summary>
        /// Modifies the local scale of the target Transform based on the new progress value.
        /// </summary>
        /// <param name="newValue">The progress value between 0 and 1.</param>
        protected override void Apply(float newValue)
        {
            if (target != null)
            {
                target.localScale = Vector3.Lerp(startScale, endScale, newValue);
            }
        }
    }
}
