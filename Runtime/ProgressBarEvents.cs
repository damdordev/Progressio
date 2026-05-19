using System;
using UnityEngine;
using UnityEngine.Events;

namespace Damdor.Progressio
{
    /// <summary>
    /// Holds events related to a ProgressBar.
    /// </summary>
    [Serializable]
    public class ProgressBarEvents
    {
        /// <summary>
        /// Invoked when the target Value changes.
        /// </summary>
        [Tooltip("Invoked when the target Value changes.")]
        public UnityEvent<float> OnValueChanged = new();

        /// <summary>
        /// Invoked when the visually displayed value changes (during animation or direct assignment).
        /// </summary>
        [Tooltip("Invoked when the visually displayed value changes (during animation or direct assignment).")]
        public UnityEvent<float> OnDisplayedValueChanged = new();

        /// <summary>
        /// Invoked when a visual animation towards the target Value starts.
        /// </summary>
        [Tooltip("Invoked when a visual animation towards the target Value starts.")]
        public UnityEvent OnAnimationStarted = new();

        /// <summary>
        /// Invoked when a visual animation towards the target Value finishes.
        /// </summary>
        [Tooltip("Invoked when a visual animation towards the target Value finishes.")]
        public UnityEvent OnAnimationFinished = new();
        
        internal ProgressBarEvents Clone()
        {
            var events = ProgressioPooling.GetEvents();
            return events;
        }
        
        public void Release()
        {
            ProgressioPooling.ReleaseEvents(this);
        }
        
        internal void Reset()
        {
            OnValueChanged.RemoveAllListeners();
            OnDisplayedValueChanged.RemoveAllListeners();
            OnAnimationStarted.RemoveAllListeners();
            OnAnimationFinished.RemoveAllListeners();
        }
        
    }
 }
