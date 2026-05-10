#if DAMDOR_PROGRESSIO_UGUI

using UnityEngine;
using UnityEngine.UI;

namespace Damdor.Progressio
{
    /// <summary>
    /// Progress bar that modifies the fillAmount property of an Image.
    /// </summary>
    public class ImageFillProgressBar : ProgressBar
    {
        /// <summary>
        /// Gets or sets the target Image.
        /// </summary>
        public Image Target 
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
        /// Gets or sets the fillAmount value mapped to progress 0.
        /// </summary>
        public float StartValue 
        { 
            get => startValue; 
            set 
            { 
                if (Mathf.Approximately(startValue, value)) return;
                startValue = value; 
                Refresh(); 
            } 
        }

        /// <summary>
        /// Gets or sets the fillAmount value mapped to progress 1.
        /// </summary>
        public float EndValue 
        { 
            get => endValue; 
            set 
            { 
                if (Mathf.Approximately(endValue, value)) return;
                endValue = value; 
                Refresh(); 
            }
        }
        
        [SerializeField] private Image target;
        [SerializeField] private float startValue;
        [SerializeField] private float endValue = 1f;

        /// <summary>
        /// Modifies the fillAmount of the target Image based on the new progress value.
        /// </summary>
        /// <param name="newValue">The progress value between 0 and 1.</param>
        protected override void Apply(float newValue)
        {
            if (target == null) return;
            target.fillAmount = Mathf.Lerp(startValue, endValue, newValue);
        }
    }
}

#endif