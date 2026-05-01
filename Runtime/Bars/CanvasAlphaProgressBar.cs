using UnityEngine;

namespace Damdor.Progressio
{
    /// <summary>
    /// Progress bar that modifies the alpha of a CanvasGroup.
    /// </summary>
    public class CanvasAlphaProgressBar : ProgressBar
    {
        /// <summary>
        /// Gets or sets the target CanvasGroup.
        /// </summary>
        public CanvasGroup Target 
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
        /// Gets or sets the alpha value mapped to progress 0.
        /// </summary>
        public float StartAlpha 
        { 
            get => startAlpha; 
            set 
            { 
                if (Mathf.Approximately(startAlpha, value)) return;
                startAlpha = value; 
                Refresh(); 
            } 
        }

        /// <summary>
        /// Gets or sets the alpha value mapped to progress 1.
        /// </summary>
        public float EndAlpha 
        { 
            get => endAlpha; 
            set 
            { 
                if (Mathf.Approximately(endAlpha, value)) return;
                endAlpha = value; 
                Refresh(); 
            }
        }
        
        [SerializeField] private CanvasGroup target;
        [SerializeField] private float startAlpha;
        [SerializeField] private float endAlpha = 1f;

        /// <summary>
        /// Modifies the alpha of the target CanvasGroup based on the new progress value.
        /// </summary>
        /// <param name="newValue">The progress value between 0 and 1.</param>
        protected override void Apply(float newValue)
        {
            if (target == null) return;
            target.alpha = Mathf.Lerp(startAlpha, endAlpha, newValue);
        }
    }
}
