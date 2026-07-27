# Progressio

Progressio is a lightweight and customizable library for creating and managing progress bars in Unity. It provides a simple API for common operations, built-in animations, event handling, and supports both traditional MonoBehaviours and the newer UI Toolkit.

## Table of Contents
- [Usage Canvas](#usage-canvas)
  - [Use mono-based progress bars](#use-mono-based-progress-bars)
  - [Create own mono-based progress bar (deriving from ProgressBar)](#create-own-mono-based-progress-bar-deriving-from-progressbar)
- [Usage UiElement](#usage-uielement)
  - [Use progress bars for toolkit](#use-progress-bars-for-toolkit)
  - [Pooling](#pooling)
- [Animations](#animations)
- [Events](#events)
- [Advanced usage](#advanced-usage)
  - [Batching changes](#batching-changes)
  - [ProgressBarController](#progressbarcontroller)
- [Extensions](#extensions)
- [List of built-in progress bars](#list-of-built-in-progress-bars)

---

# Usage Canvas

## Use mono-based progress bars
To use a built-in mono-based progress bar, simply add one of the provided components (like `ImageFillProgressBar` or `ColorProgressBar`) to a GameObject in your scene. Configure its properties, animation settings, and events through the Unity Inspector. You can then update the progress programmatically:

```csharp
public class MyScript : MonoBehaviour 
{
    public ProgressBar myProgressBar;

    void Start() 
    {
        myProgressBar.Value = 0.5f; // Sets progress to 50%
    }
}
```

## Create own mono-based progress bar (deriving from ProgressBar)
To create a custom mono-based progress bar, inherit from the `Damdor.Progressio.ProgressBar` abstract class and implement the `Apply` method to define how the visual representation updates based on the current progress value (between 0 and 1).

```csharp
using UnityEngine;
using Damdor.Progressio;

public class MyCustomProgressBar : ProgressBar
{
    public Transform indicator;

    protected override void Apply(float newValue)
    {
        // Update the visual representation
        indicator.localScale = new Vector3(newValue, 1f, 1f);
    }
}
```

# Usage UiElement

## Use progress bars for toolkit
Progressio integrates with Unity's UI Toolkit through the `UiToolkitProgressBar` wrapper. You can create instances and bind them to your UI elements. Animations are updated automatically.

There are several ways to create a `UiToolkitProgressBar`:

**1. Using `UiToolkitProgressBar.Create` with animation and event objects:**

```csharp
using UnityEngine;
using UnityEngine.UIElements;
using Damdor.Progressio;

public class UIToolkitExample : MonoBehaviour
{
    public UIDocument uiDocument;
    private UiToolkitProgressBar progressBarWrapper;

    void Start()
    {
        var root = uiDocument.rootVisualElement;
        var uiElement = root.Q<UnityEngine.UIElements.ProgressBar>("MyProgressBar");
        
        var animation = new ProgressBarAnimation { Animated = true, Speed = 5f };
        var events = new ProgressBarEvents();
        events.OnAnimationFinished.AddListener(() => Debug.Log("Animation Finished!"));

        // Wrap the UI element and add animations/events
        progressBarWrapper = UiToolkitProgressBar.Create(uiElement, animation, events);
        progressBarWrapper.Value = 0.75f;
    }

    void OnDestroy()
    {
        // Release the wrapper to the pool
        progressBarWrapper.Release();
    }
}
```

**2. Using `UiToolkitProgressBarData`:**

This is useful for configuring progress bars in the inspector without creating `ScriptableObject` assets.

```csharp
[System.Serializable]
public class MyUI
{
    public UiToolkitProgressBarData progressBarData;
}
```

Then in your code:

```csharp
var progressBar = UiToolkitProgressBar.Create(uiElement, myUI.progressBarData);
```

**3. Using `UiToolkitProgressBarAsset`:**

Create a `ScriptableObject` asset in your project to reuse progress bar configurations.

```csharp
// In your script
public UiToolkitProgressBarAsset progressBarAsset;

// ...

var progressBar = UiToolkitProgressBar.Create(uiElement, progressBarAsset);
```

## Pooling
The library uses pooling to prevent allocations for `UiToolkitProgressBar`, `ProgressBarController`, `ProgressBarAnimation`, and `ProgressBarEvents`. You can configure the pool sizes via `ProgressioSettings`.

```csharp
// Configure the maximum pool size (default is 20)
ProgressioSettings.MaxUiToolkitProgressBarPoolSize = 30;

// Acquiring a wrapper uses a pooled instance if available
var wrapper = UiToolkitProgressBar.Create(uiElement, null, null);

// Releasing a wrapper resets its state and returns it to the pool
wrapper.Release();
```

# Animations
The `ProgressBarAnimation` class allows you to smoothly interpolate the progress value. You can enable animations, set the speed, and ignore `Time.timeScale` (useful for paused games).

```csharp
progressBar.Animation.Animated = true;
progressBar.Animation.Speed = 5f;
progressBar.Animation.IgnoreTimescale = true;

// If you have UniTask integrated via DAMDOR_PROGRESSIO_UNITASK, you can await animations:
await progressBar.AnimateTo(1f, cancellationToken);
```

# Events
The `ProgressBarEvents` class exposes UnityEvents that you can hook into via the Inspector or through code:

```csharp
void Start()
{
    // Called when the target value is changed
    progressBar.Events.OnValueChanged.AddListener(val => Debug.Log($"Target: {val}"));
    
    // Called as the progress visually changes (during animations)
    progressBar.Events.OnDisplayedValueChanged.AddListener(val => Debug.Log($"Visual: {val}"));
    
    // Called when an animation begins
    progressBar.Events.OnAnimationStarted.AddListener(() => Debug.Log("Animation started!"));
    
    // Called when an animation concludes
    progressBar.Events.OnAnimationFinished.AddListener(() => Debug.Log("Animation finished!"));
}
```

# Advanced usage

## Batching changes
If you need to make multiple property changes without triggering intermediate visual updates or animations, use `StartChanges()` and `CommitChanges()`. 

```csharp
progressBar.StartChanges(); // Pauses visual updates and events

progressBar.Animation.Speed = 5f;
progressBar.Value = 1f;

progressBar.CommitChanges(); // Refreshes state and triggers animations/events
```

## ProgressBarController
`ProgressBarController` is the core logic class handling clamping, animations, and events. It is independent of Unity components and can be used in pure C# environments. It is used internally by both `ProgressBar` and `UiToolkitProgressBar`. You can retrieve instances via the internal pool.

```csharp
var controller = ProgressioPooling.GetController(
    apply: val => Debug.Log($"Current visual progress is {val}"),
    animation: new ProgressBarAnimation { Animated = true, Speed = 2f },
    events: new ProgressBarEvents()
);

controller.Value = 0.5f;

// The controller must be updated manually every frame
controller.Update(Time.deltaTime);

// Return it to the pool when it is no longer needed
ProgressioPooling.ReleaseController(controller);
```

# List of built-in progress bars
The package comes with several pre-made progress bar components for common use cases:
- **CanvasAlphaProgressBar**: Fades a CanvasGroup's alpha value.
- **ColorProgressBar**: Interpolates between colors on a graphic element.
- **CombinedProgressBar**: Groups multiple progress bars to update them simultaneously.
- **ImageFillProgressBar**: Updates the `fillAmount` of a standard Unity UI Image.
- **PositionProgressBar**: Moves an object's local position between start and end points.
- **RotationProgressBar**: Rotates an object's local rotation between start and end points.
- **ScaleProgressBar**: Scales an object's local scale between start and end points.
- **VisualStateProgressBar**: Setting a proper state for visual state based on progress
- **SegmentedProgressBar** A progress bar that divides its progress across multiple child progress bar segments.
- **UiToolkitProgressBar**: A wrapper for UI Toolkit's abstract progress bar element.


Example of modifying a built-in progress bar dynamically:
```csharp
var colorBar = GetComponent<ColorProgressBar>();
colorBar.StartColor = Color.red;
colorBar.EndColor = Color.green;
colorBar.Value = 0.8f;
```