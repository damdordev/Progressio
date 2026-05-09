# Progressio

Progressio is a lightweight and customizable library for creating and managing progress bars in Unity. It provides a simple API for common operations, built-in animations, event handling, and supports both traditional MonoBehaviours and the newer UI Toolkit.

# Usage

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

## Use progress bars for toolkit
Progressio integrates with Unity's UI Toolkit through the `UiToolkitProgressBar` wrapper. You can retrieve instances from a pool and bind them to your UI elements. Remember to call `Update` manually if animations are enabled.

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
        
        // Wrap the UI element and add animations/events if needed
        progressBarWrapper = ProgressioSettings.GetUiToolkitProgressBar(uiElement);
        progressBarWrapper.Value = 0.75f;
    }

    void Update()
    {
        // Must be called manually to drive animations for UI Toolkit progress bars
        progressBarWrapper?.Update();
    }

    void OnDestroy()
    {
        ProgressioSettings.ReleaseUiToolkitProgressBar(progressBarWrapper);
    }
}
```

## ProgressBarController
`ProgressBarController` is the core logic class handling clamping, animations, and events. It's independent of Unity components and can be used in pure C# environments or customized setups. It is used internally by both `ProgressBar` and `UiToolkitProgressBar`.

## Batching changes
If you need to make multiple property changes without triggering intermediate visual updates or animations, use `StartChanges()` and `CommitChanges()`. The progress bar will only refresh and animate once `CommitChanges()` is called.

```csharp
progressBar.StartChanges();
progressBar.Animation.Speed = 5f;
progressBar.Value = 1f;
progressBar.CommitChanges(); // Updates happen here
```

## Animations
The `ProgressBarAnimation` class allows you to smoothly interpolate the progress value. You can enable animations, set the speed, and choose whether to ignore `Time.timeScale` (useful for animations during paused games).

```csharp
progressBar.Animation.Animated = true;
progressBar.Animation.Speed = 2f;
progressBar.Animation.IgnoreTimescale = false;
```

## Events
The `ProgressBarEvents` class exposes UnityEvents that you can hook into via the Inspector or through code:
- `OnValueChanged`: Called when the target value is changed.
- `OnDisplayedValueChanged`: Called as the progress visually changes (during animations).
- `OnAnimationStarted`: Called when an animation begins.
- `OnAnimationFinished`: Called when an animation concludes.

```csharp
progressBar.Events.OnAnimationFinished.AddListener(() => Debug.Log("Animation done!"));
```

## List of built-in progress bars
The package comes with several pre-made progress bar components for common use cases:
- **CanvasAlphaProgressBar**: Fades a CanvasGroup's alpha value.
- **ColorProgressBar**: Interpolates between colors on a graphic element.
- **CombinedProgressBar**: Groups multiple progress bars to update them simultaneously.
- **ImageFillProgressBar**: Updates the `fillAmount` of a standard Unity UI Image.
- **PositionProgressBar**: Moves an object's local position between start and end points.
- **RotationProgressBar**: Rotates an object's local rotation between start and end points.
- **ScaleProgressBar**: Scales an object's local scale between start and end points.
- **UiToolkitProgressBar**: A wrapper for UI Toolkit's abstract progress bar element.
