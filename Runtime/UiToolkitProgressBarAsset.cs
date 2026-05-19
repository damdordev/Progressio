using UnityEngine;

namespace Damdor.Progressio
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> that holds animation and event settings for a <see cref="UiToolkitProgressBar"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "sequence", menuName = "Damdor/Progressio/UiToolkit progress bar asset")]
    public class UiToolkitProgressBarAsset : ScriptableObject
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