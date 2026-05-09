namespace Damdor.Progressio
{
    /// <summary>
    /// Base interface for all progress bars in the Progressio library.
    /// </summary>
    public interface IProgressBar
    {
        /// <summary>
        /// The target progress value between 0 and 1. Setting this value may trigger animations.
        /// </summary>
        float Value { get; set; }

        /// <summary>
        /// The currently displayed progress value between 0 and 1. This value is updated during animations.
        /// </summary>
        float DisplayedValue { get; }

        /// <summary>
        /// Animation settings for the progress bar.
        /// </summary>
        ProgressBarAnimation Animation { get; }

        /// <summary>
        /// Events associated with the progress bar's lifecycle and value changes.
        /// </summary>
        ProgressBarEvents Events { get; }
    }
}