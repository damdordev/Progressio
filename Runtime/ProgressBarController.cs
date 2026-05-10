using System;
using UnityEngine;

namespace Damdor.Progressio
{
    /// <summary>
    /// Controls the logic and state of a progress bar, including value clamping, animations, and events.
    /// This class can be used to manage progress functionality independently of any specific UI framework.
    /// </summary>
    public sealed class ProgressBarController
    {
        /// <summary>
        /// The target progress value between 0 and 1. Setting this value may trigger animations and events.
        /// </summary>
        public float Value
        {
            get => value;
            set
            {
                if (!wasValueSet)
                {
                    SetValueWithoutAnimation(value);
                    return;
                }
                var clampedValue = Mathf.Clamp01(value);
                if (Mathf.Approximately(this.value, clampedValue)) return;
                this.value = clampedValue;
                Refresh();
                events.OnValueChanged?.Invoke(this.value);
            }
        }
        
        /// <summary>
        /// The currently displayed progress value between 0 and 1. This value is updated during animations.
        /// </summary>
        public float DisplayedValue 
        { 
            get => displayedValue; 
            private set
            {
                if (Mathf.Approximately(displayedValue, value)) return;
                displayedValue = value;
                events.OnDisplayedValueChanged?.Invoke(displayedValue);
            }
        }
        
        /// <summary>
        /// Animation settings used by the controller.
        /// </summary>
        public ProgressBarAnimation Animation => animation;

        /// <summary>
        /// Events associated with the controller's lifecycle and value changes.
        /// </summary>
        public ProgressBarEvents Events => events;

        private float value;
        
        private ProgressBarAnimation animation = new();
        private ProgressBarEvents events = new();
        private float displayedValue;
        private int changesLevel;
        private bool needRefresh;
        private bool isAnimating;
        private bool wasValueSet;
        
        private Action<float> apply;
        
        /// <summary>
        /// Sets up the controller with an apply action, animation settings, and events.
        /// </summary>
        /// <param name="apply">An action to apply the displayed value to the visual representation.</param>
        /// <param name="animation">Animation settings.</param>
        /// <param name="events">Progress bar events.</param>
        public void Setup(Action<float> apply, ProgressBarAnimation animation, ProgressBarEvents events)
        {
            this.apply = apply;
            this.animation = animation;
            this.events = events;
        }
        
        /// <summary>
        /// Initializes the controller state, triggers initial events, and applies the initial value.
        /// </summary>
        public void Start()
        {
            if (events != null)
            {
                events.OnValueChanged?.Invoke(value);
                events.OnDisplayedValueChanged?.Invoke(value);
            }
            DisplayedValue = value;
            Apply(DisplayedValue);
            isAnimating = false;
        }

        /// <summary>
        /// Resets the controller to its default state, clearing references and values.
        /// </summary>
        public void Reset()
        {
            apply = null;
            value = 0f;
            displayedValue = 0f;
            wasValueSet = false;
            changesLevel = 0;
            needRefresh = false;
            isAnimating = false;
            animation = null;
            events = null;
        }
        
        /// <summary>
        /// Starts a batch of changes, pausing visual updates and event triggers until changes are committed.
        /// </summary>
        public void StartChanges()
        {
            ++changesLevel;
        }

        /// <summary>
        /// Commits a batch of changes, applying them and refreshing the controller if necessary.
        /// </summary>
        public void CommitChanges()
        {
            --changesLevel;
            if (changesLevel < 0)
            {
                Debug.LogError("[Progressio] Commit changes was invoked without StartChanges()");
            }
            if (changesLevel <= 0 && needRefresh) Refresh();
        }
        
        /// <summary>
        /// Sets the progress value directly, bypassing any animation, and applies it immediately.
        /// </summary>
        /// <param name="newValue">The target value between 0 and 1.</param>
        public void SetValueWithoutAnimation(float newValue)
        {
            var clampedValue = Mathf.Clamp01(newValue);
            
            var changed = !Mathf.Approximately(value, clampedValue);
            value = clampedValue;
            
            if (isAnimating)
            {
                isAnimating = false;
                if(events != null) events.OnAnimationFinished?.Invoke();
            }

            DisplayedValue = value;
            Refresh();
            
            if (changed)
            {
                if(events != null)events.OnValueChanged?.Invoke(value);
            }
            
            wasValueSet = true;
        }
        
        /// <summary>
        /// Evaluates the current state, starts animations if needed, and applies the display value.
        /// Should be called after modifying settings or changing states.
        /// </summary>
        public void Refresh()
        {
            if (changesLevel > 0)
            {
                needRefresh = true;
                return;
            }
            needRefresh = false;

            if (animation == null || !animation.Animated || !Application.isPlaying)
            {
                if (isAnimating)
                {
                    isAnimating = false;
                    if(events != null) events.OnAnimationFinished?.Invoke();
                }
                DisplayedValue = value;
            }
            else if (Application.isPlaying && !Mathf.Approximately(DisplayedValue, value))
            {
                if (!isAnimating)
                {
                    isAnimating = true;
                    if(events != null)events.OnAnimationStarted?.Invoke();
                }
            }

            Apply(DisplayedValue);
        }
        
        /// <summary>
        /// Updates the controller, driving animations if enabled. Usually called every frame.
        /// </summary>
        public void Update(float dt)
        {
            if (animation == null || !animation.Animated || !Application.isPlaying || changesLevel > 0) return;
            if (Mathf.Approximately(DisplayedValue, value)) return;
            
            DisplayedValue = Mathf.Lerp(DisplayedValue, value, dt * animation.Speed);
            
            if (Mathf.Abs(DisplayedValue - value) < 0.001f)
            {
                DisplayedValue = value;
                if (isAnimating)
                {
                    isAnimating = false;
                    if(events != null) events.OnAnimationFinished?.Invoke();
                }
            }
            Apply(DisplayedValue);
        }

        private void Apply(float value)
        {
            apply?.Invoke(value);
        }
        
    }
}