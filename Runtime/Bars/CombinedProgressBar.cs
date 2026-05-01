using System.Collections.Generic;
using UnityEngine;

namespace Damdor.Progressio
{
    /// <summary>
    /// Progress bar that combines multiple progress bars and passes the progress value to everyone.
    /// </summary>
    public class CombinedProgressBar : ProgressBar
    {
        /// <summary>
        /// Gets the list of combined progress bars.
        /// </summary>
        public List<ProgressBar> ProgressBars
        {
            get => progressBars;
            set
            {
                progressBars.Clear();
                progressBars.AddRange(value);
                Refresh();
            }
        }
        
        [SerializeField] private List<ProgressBar> progressBars = new();

        /// <summary>
        /// Passes the progress value to all combined progress bars.
        /// </summary>
        /// <param name="newValue">The progress value between 0 and 1.</param>
        protected override void Apply(float newValue)
        {
            if (progressBars == null) return;
            foreach (var progressBar in progressBars)
            {
                if (progressBar == null || progressBar == this) continue;
                progressBar.Value = newValue;
            }
        }
    }
}
