using UnityEngine;

namespace Damdor.Progressio
{
    /// <summary>
    /// Progress bar that modifies the rotation (Euler angles) of a Transform.
    /// </summary>
    public class RotationProgressBar : ProgressBar
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
        /// Gets or sets the rotation (Euler angles) mapped to progress 0.
        /// </summary>
        public Vector3 StartRotation 
        { 
            get => startRotation; 
            set 
            { 
                if (startRotation == value) return;
                startRotation = value; 
                Refresh(); 
            } 
        }

        /// <summary>
        /// Gets or sets the rotation (Euler angles) mapped to progress 1.
        /// </summary>
        public Vector3 EndRotation 
        { 
            get => endRotation; 
            set 
            { 
                if (endRotation == value) return;
                endRotation = value; 
                Refresh(); 
            } 
        }

        /// <summary>
        /// Gets or sets whether to use localRotation instead of rotation.
        /// </summary>
        public bool UseLocalRotation 
        { 
            get => useLocalRotation; 
            set 
            { 
                if (useLocalRotation == value) return;
                useLocalRotation = value; 
                Refresh(); 
            }
        }
        
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 startRotation;
        [SerializeField] private Vector3 endRotation;
        [SerializeField] private bool useLocalRotation = true;

        /// <summary>
        /// Modifies the rotation of the target Transform based on the new progress value.
        /// </summary>
        /// <param name="newValue">The progress value between 0 and 1.</param>
        protected override void Apply(float newValue)
        {
            if (target != null)
            {
                Vector3 currentRotation = Vector3.Lerp(startRotation, endRotation, newValue);
                if (useLocalRotation)
                {
                    target.localEulerAngles = currentRotation;
                }
                else
                {
                    target.eulerAngles = currentRotation;
                }
            }
        }
    }
}
