using System;
using UnityEngine;
using UnityEngine.Events;

namespace Damdor.Progressio
{
    [Serializable]
    public class ProgressBarEvents
    {
        [Tooltip("Invoked when the target Value changes.")]
        public UnityEvent<float> OnValueChanged;

        [Tooltip("Invoked when the visually displayed value changes (during animation or direct assignment).")]
        public UnityEvent<float> OnDisplayedValueChanged;

        [Tooltip("Invoked when a visual animation towards the target Value starts.")]
        public UnityEvent OnAnimationStarted;

        [Tooltip("Invoked when a visual animation towards the target Value finishes.")]
        public UnityEvent OnAnimationFinished;
    }
}