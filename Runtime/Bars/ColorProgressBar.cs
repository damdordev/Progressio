using UnityEngine;
using UnityEngine.UI;

namespace Damdor.Progressio
{
    /// <summary>
    /// Progress bar that modifies the color of a Graphic (e.g., Image, Text).
    /// </summary>
    public class ColorProgressBar : ProgressBar
    {
        /// <summary>
        /// Gets or sets the target Graphic.
        /// </summary>
        public Graphic Target 
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
        /// Gets or sets the color mapped to progress 0.
        /// </summary>
        public Color StartColor 
        { 
            get => startColor; 
            set 
            { 
                if (startColor == value) return;
                startColor = value; 
                Refresh(); 
            } 
        }

        /// <summary>
        /// Gets or sets the color mapped to progress 1.
        /// </summary>
        public Color EndColor 
        { 
            get => endColor; 
            set 
            { 
                if (endColor == value) return;
                endColor = value; 
                Refresh(); 
            }
        }
        
        [SerializeField] private Graphic target;
        [SerializeField] private Color startColor = Color.white;
        [SerializeField] private Color endColor = Color.white;

        /// <summary>
        /// Modifies the color of the target Graphic based on the new progress value.
        /// </summary>
        /// <param name="newValue">The progress value between 0 and 1.</param>
        protected override void Apply(float newValue)
        {
            if (target != null)
            {
                target.color = Color.Lerp(startColor, endColor, newValue);
            }
        }
    }
}
