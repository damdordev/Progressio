using System;
using UnityEngine;

namespace Damdor.Progressio
{
    /// <summary>
    /// A struct that holds animation and event settings for a <see cref="UiToolkitProgressBar"/>.
    /// </summary>
    [Serializable]
    public struct UiToolkitProgressBarData
    {
        /// <summary>
        /// Gets the animation settings.
        /// </summary>
        public ProgressBarAnimation Animation
        {
            get => animation;
        }

        /// <summary>
        /// Gets the event settings.
        /// </summary>
        public ProgressBarEvents Events
        {
            get => events;
        }

        [SerializeField] private ProgressBarAnimation animation;
        [SerializeField] private ProgressBarEvents events;
    }
}