using System;
using UnityEngine;

namespace Damdor.Progressio
{
    /// <summary>
    /// Base class for creating extensions that react to ProgressBar value changes.
    /// Handles automatic attachment and detachment of listeners.
    /// </summary>
    [ExecuteAlways]
    public class ProgressBarExtension : MonoBehaviour
    {
        /// <summary>
        /// The progress bar this extension is attached to.
        /// Changing this value at runtime will automatically reattach listeners if the component is active.
        /// </summary>
        public ProgressBar ProgressBar
        {
            get => progressBar;
            set
            {
                progressBar = value;
                if (isActiveAndEnabled) Reattach();
            }
        }
        
        /// <summary>
        /// The currently attached progress bar target.
        /// </summary>
        public ProgressBar Target { get; private set; }
        
        [SerializeField] private ProgressBar progressBar;
        
        protected virtual void OnValidate()
        {
            if(isActiveAndEnabled) Reattach();
        }

        private void OnEnable()
        {
            Reattach();
        }

        private void OnDisable()
        {
            Detach();
        }
                
        private void Reattach()
        {
            if (Target == progressBar) return;
            Detach();
            if (progressBar == null) return;
            Target = progressBar;
            Target.Events.OnValueChanged.AddListener(OnValueChanged);
            Target.Events.OnDisplayedValueChanged.AddListener(OnDisplayedValueChanged);
            OnAttach();
            OnValueChanged(Target.Value);
            OnDisplayedValueChanged(Target.DisplayedValue);
        }

        private void Detach()
        {
            if (Target == null) return;
            try
            {
                if (Target.Events != null)
                {
                    Target.Events.OnValueChanged.RemoveListener(OnValueChanged);
                    Target.Events.OnDisplayedValueChanged.RemoveListener(OnDisplayedValueChanged);
                }

                OnDetach();
            }
            finally { Target = null; }
        }

        /// <summary>
        /// Invoked when the extension successfully attaches to a ProgressBar.
        /// </summary>
        protected virtual void OnAttach(){}

        /// <summary>
        /// Invoked before the extension detaches from its target ProgressBar.
        /// </summary>
        protected virtual void OnDetach(){}

        /// <summary>
        /// Invoked when the target ProgressBar's target value changes.
        /// </summary>
        /// <param name="value">The new target value (0 to 1).</param>
        protected virtual void OnValueChanged(float value){}

        /// <summary>
        /// Invoked when the target ProgressBar's displayed value changes (useful for animations).
        /// </summary>
        /// <param name="displayedValue">The currently displayed value (0 to 1).</param>
        protected virtual void OnDisplayedValueChanged(float displayedValue){}
    }
}