namespace Damdor.Progressio
{
    /// <summary>
    /// Global settings for the Progressio library.
    /// </summary>
    public static class ProgressioSettings
    {
        /// <summary>
        /// Gets or sets the maximum number of <see cref="ProgressBarController"/> instances to keep in the pool.
        /// </summary>
        public static int MaxControllerPoolSize
        {
            get => ProgressioPooling.MaxControllerPoolSize;
            set => ProgressioPooling.MaxControllerPoolSize = value;
        }
        
#if DAMDOR_PROGRESSIO_UIELEMENTS
        /// <summary>
        /// Gets or sets the maximum number of <see cref="UiToolkitProgressBar"/> instances to keep in the pool.
        /// </summary>
        public static int MaxUiToolkitProgressBarPoolSize
        {
            get => ProgressioPooling.MaxUiToolkitProgressBarPoolSize;
            set => ProgressioPooling.MaxUiToolkitProgressBarPoolSize = value;
        }
        
        /// <summary>
        /// Gets or sets the maximum number of <see cref="ProgressBarAnimation"/> instances to keep in the pool.
        /// </summary>
        public static int MaxAnimationPoolSize
        {
            get => ProgressioPooling.MaxAnimationPoolSize;
            set => ProgressioPooling.MaxAnimationPoolSize = value;
        }
        
        /// <summary>
        /// Gets or sets the maximum number of <see cref="ProgressBarEvents"/> instances to keep in the pool.
        /// </summary>
        public static int MaxEventsPoolSize
        {
            get => ProgressioPooling.MaxEventsPoolSize;
            set => ProgressioPooling.MaxEventsPoolSize = value;
        }
        
#endif
    }
}