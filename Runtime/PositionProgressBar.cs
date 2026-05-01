using UnityEngine;

namespace Damdor.Progressio
{
    /// <summary>
    /// Progress bar that modifies the position of a Transform.
    /// </summary>
    public class PositionProgressBar : ProgressBar
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
        /// Gets or sets the position mapped to progress 0.
        /// </summary>
        public Vector3 StartPosition 
        { 
            get => startPosition; 
            set 
            { 
                if (startPosition == value) return;
                startPosition = value; 
                Refresh(); 
            } 
        }

        /// <summary>
        /// Gets or sets the position mapped to progress 1.
        /// </summary>
        public Vector3 EndPosition 
        { 
            get => endPosition; 
            set 
            { 
                if (endPosition == value) return;
                endPosition = value; 
                Refresh(); 
            } 
        }

        /// <summary>
        /// Gets or sets whether to use localPosition instead of position.
        /// </summary>
        public bool UseLocalPosition 
        { 
            get => useLocalPosition; 
            set 
            { 
                if (useLocalPosition == value) return;
                useLocalPosition = value; 
                Refresh(); 
            }
        }
        
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 endPosition;
        [SerializeField] private bool useLocalPosition = true;

        /// <summary>
        /// Modifies the position of the target Transform based on the new progress value.
        /// </summary>
        /// <param name="newValue">The progress value between 0 and 1.</param>
        protected override void Apply(float newValue)
        {
            if (target != null)
            {
                Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, newValue);
                if (useLocalPosition)
                {
                    target.localPosition = currentPosition;
                }
                else
                {
                    target.position = currentPosition;
                }
            }
        }
    }
}
