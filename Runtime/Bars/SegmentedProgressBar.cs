using System.Collections.Generic;
using UnityEngine;

namespace Damdor.Progressio
{
    /// <summary>
    /// A progress bar that divides its progress across multiple child progress bar segments.
    /// </summary>
    public class SegmentedProgressBar : ProgressBar
    {
        /// <summary>
        /// Gets or sets the list of progress bar segments.
        /// </summary>
        public List<ProgressBar> Segments
        {
            get => segments;
            set
            {
                segments = value;
                Refresh();
            }
        }
        
        [SerializeField] private List<ProgressBar> segments;
        
        protected override void Apply(float newValue)
        {
            if (segments == null || segments.Count == 0) return;

            var count = segments.Count;
            var segmentLength = 1f / count;
            
            for (var i = 0; i < count; i++)
            {
                var segment = segments[i];
                if (segment == null) continue;

                var min = i * segmentLength;
                var max = (i + 1) * segmentLength;
                var segmentProgress = 0f;
                if (newValue >= max) segmentProgress = 1f; 
                else if (newValue >= min) segmentProgress = Mathf.InverseLerp(min, max, newValue);
                segment.Value = segmentProgress;
            }
        }

        /// <summary>
        /// Creates a specified amount of new segments from a prefab and removes existing segments
        /// </summary>
        /// <param name="prefab">The progress bar prefab to instantiate for each segment.</param>
        /// <param name="amount">The number of segments to create.</param>
        public void CreateSegments(ProgressBar prefab, int amount)
        {
            foreach (var segment in segments)
            {
                if (segment == null) continue;
                if (Application.isPlaying)
                    Destroy(segment.gameObject);
                else
                    DestroyImmediate(segment.gameObject);
            }
            segments.Clear();

            for (var i = 0; i < amount; i++)
            {
                var newSegment = Instantiate(prefab, transform);
                segments.Add(newSegment);
            }

            Refresh();
        }
    }
}