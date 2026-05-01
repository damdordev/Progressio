using System;
using UnityEngine;

namespace Damdor.Progressio
{
    /// <summary>
    /// Holds animation settings for a ProgressBar.
    /// </summary>
    [Serializable]
    public class ProgressBarAnimation
    {
        /// <summary>
        /// Gets or sets a value indicating whether the progress bar should animate towards its target value.
        /// </summary>
        public bool Animated
        {
            get => animated;
            set => animated = value;
        }
        
        /// <summary>
        /// Gets or sets the speed of the animation. Represents how fast the progress transitions.
        /// </summary>
        public float Speed
        {
            get => speed;
            set => speed = value;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether the animation should ignore Time.timeScale.
        /// </summary>
        public bool IgnoreTimescale
        {
            get => ignoreTimescale;
            set => ignoreTimescale = value;
        }
        
        [Tooltip("If true, the progress bar will animate towards the target value smoothly.")]
        [SerializeField] private bool animated;

        [Tooltip("The speed of the animation. Represents how fast the progress transitions.")]
        [SerializeField] private float speed = 10f;

        [Tooltip("If true, the animation will ignore Time.timeScale, making it suitable for UI that needs to animate even when the game is paused.")]
        [SerializeField] private bool ignoreTimescale;
    }
}
