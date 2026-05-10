using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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
#if DAMDOR_PROGRESSIO_UNITASK
                FinishCurrentCompletionSource(false);
#endif
                if (!wasValueSet)
                {
                    SetValueWithoutAnimation(value);
                    return;
                }
                var clampedValue = Mathf.Clamp01(value);
                if (Mathf.Approximately(this.value, clampedValue)) return;
                this.value = clampedValue;
                Refresh();
                Events.OnValueChanged?.Invoke(this.value);
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
                Events.OnDisplayedValueChanged?.Invoke(displayedValue);
            }
        }
        
        /// <summary>
        /// Animation settings used by the controller.
        /// </summary>
        public ProgressBarAnimation Animation { get; private set; } = new();

        /// <summary>
        /// Events associated with the controller's lifecycle and value changes.
        /// </summary>
        public ProgressBarEvents Events { get; private set; } = new();

        private float value;

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
            this.Animation = animation;
            this.Events = events;
        }
        
        /// <summary>
        /// Initializes the controller state, triggers initial events, and applies the initial value.
        /// </summary>
        public void Start()
        {
            if (Events != null)
            {
                Events.OnValueChanged?.Invoke(value);
                Events.OnDisplayedValueChanged?.Invoke(value);
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
            Animation = null;
            Events = null;
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
#if DAMDOR_PROGRESSIO_UNITASK
            FinishCurrentCompletionSource(false);
#endif
            var clampedValue = Mathf.Clamp01(newValue);
            
            var changed = !Mathf.Approximately(value, clampedValue);
            value = clampedValue;
            
            if (isAnimating)
            {
                isAnimating = false;
                Events?.OnAnimationFinished?.Invoke();
            }

            DisplayedValue = value;
            Refresh();
            
            if (changed)
            {
                Events?.OnValueChanged?.Invoke(value);
            }
            
            wasValueSet = true;
        }

        #if DAMDOR_PROGRESSIO_UNITASK

        private UniTaskCompletionSource<bool> completionSource;
        private CancellationToken cancellationToken;
        
        /// <summary>
        /// Animates the progress bar to the specified target value.
        /// </summary>
        /// <param name="value">The target value between 0 and 1.</param>
        /// <param name="cancellationToken">Token to cancel the animation.</param>
        /// <returns>A task representing the animation process. True if completed successfully, false if value was changed during animation.</returns>
        public UniTask<bool> AnimateTo(float value, CancellationToken cancellationToken = default)
        {
            Value = value;
            completionSource = new UniTaskCompletionSource<bool>();
            this.cancellationToken = cancellationToken;
            return completionSource.Task;
        }

        private void FinishCurrentCompletionSource(bool result)
        {
            if (completionSource == null) return;
            var tmpSource = completionSource;
            var tmpCancellationToken = cancellationToken;
            completionSource = null;
            cancellationToken = CancellationToken.None;
            if(!tmpCancellationToken.IsCancellationRequested) tmpSource.TrySetResult(result);
        }
        
        #endif
        
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

            if (Animation == null || !Animation.Animated || !Application.isPlaying)
            {
                if (isAnimating)
                {
                    isAnimating = false;
                    Events?.OnAnimationFinished?.Invoke();
                }
                DisplayedValue = value;
#if DAMDOR_PROGRESSIO_UNITASK
                FinishCurrentCompletionSource(true);
#endif
            }
            else if (Application.isPlaying && !Mathf.Approximately(DisplayedValue, value))
            {
                if (!isAnimating)
                {
                    isAnimating = true;
                    Events?.OnAnimationStarted?.Invoke();
                }
            }

            Apply(DisplayedValue);
        }
        
        /// <summary>
        /// Updates the controller, driving animations if enabled. Usually called every frame.
        /// </summary>
        /// <param name="dt">The time delta for the update.</param>
        public void Update(float dt)
        {
            if (Animation == null || !Animation.Animated || !Application.isPlaying || changesLevel > 0) return;
            if (Mathf.Approximately(DisplayedValue, value)) return;
            
            DisplayedValue = Mathf.Lerp(DisplayedValue, value, dt * Animation.Speed);
            
            if (Mathf.Abs(DisplayedValue - value) < 0.001f)
            {
                DisplayedValue = value;
                if (isAnimating)
                {
                    isAnimating = false;
                    Events?.OnAnimationFinished?.Invoke();
                }
#if DAMDOR_PROGRESSIO_UNITASK
                FinishCurrentCompletionSource(true);
#endif
            }
            Apply(DisplayedValue);
        }

        private void Apply(float value)
        {
            apply?.Invoke(value);
        }
        
    }
}