using System;
using System.Collections.Generic;
using System.Linq;
using Damdor.Statio;
using UnityEngine;

#if DAMDOR_PROGRESSIO_STATIO

namespace Damdor.Progressio
{
    /// <summary>
    /// A progress bar that changes its visual state based on a defined set of value thresholds.
    /// </summary>
    public class VisualStateProgressBar : ProgressBar
    {
        /// <summary>
        /// Gets or sets the list of thresholds that define which state to activate at a given minimum value.
        /// </summary>
        public List<VisualStateProgressBarThreshold> Thresholds
        {
            get => thresholds;
            set
            {
                thresholds = value;
                SortThresholds();
                Refresh();
            }
        }
        
        [SerializeField] private VisualState visualState;
        [SerializeField] private List<VisualStateProgressBarThreshold> thresholds;

        private readonly List<VisualStateProgressBarThreshold> sortedThresholds = new();
        private bool isInitialized;
        
        protected override void Apply(float newValue)
        {
            if (visualState == null) return;
            
            var state = GetState(newValue);
            if (string.IsNullOrEmpty(state)) return;
            if (!visualState.States.Contains(state)) return;
            
            visualState.ChangeState(state);
        }

        protected override void OnValidate()
        {
            isInitialized = false;
            base.OnValidate();
        }

        private void SortThresholds()
        {
            sortedThresholds.Clear();
            if (thresholds != null)
            {
                sortedThresholds.AddRange(thresholds);
                sortedThresholds.Sort((t1, t2) => t2.MinValue.CompareTo(t1.MinValue));
            }
            isInitialized = true;
        }

        private string GetState(float value)
        {
            if (!isInitialized) SortThresholds();

            foreach (var threshold in sortedThresholds)
            {
                if(value < threshold.MinValue) continue;
                return threshold.State;
            }
            
            return null;
        }
    }

    /// <summary>
    /// Defines a state change threshold for the <see cref="VisualStateProgressBar"/>.
    /// </summary>
    [Serializable]
    public struct VisualStateProgressBarThreshold
    {
        /// <summary>
        /// The minimum progress value required to activate the visual state.
        /// </summary>
        public float MinValue
        {
            get => minValue;
            set => minValue = value;
        }

        /// <summary>
        /// The name of the visual state to activate.
        /// </summary>
        public string State
        {
            get => state;
            set => state = value;
        }

        [SerializeField]
        private float minValue;
        [SerializeField]
        private string state;
    }
}

#endif