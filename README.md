# Progressio

Progressio is a lightweight, modular, and extensible progress bar library for Unity. It abstracts visual progress representation, allowing you to easily link progress values (between 0 and 1) to various Transform or UI properties. With built-in editor support, the changes update automatically as you adjust them in the Unity Editor.

## Usage

Progressio components can be attached to GameObjects in your scene. Simply modify the `Value` property (ranging from 0.0 to 1.0) on any `ProgressBar` component to update its visual state.

```csharp
using Damdor.Progressio;
using UnityEngine;

public class ProgressExample : MonoBehaviour
{
    public ProgressBar healthBar;

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        // Automatically updates the visual representation
        healthBar.Value = currentHealth / maxHealth;
    }
}
```

### Smooth Animations

Progressio supports smooth, animated transitions. To enable this, simply check the `Animated` box in the Inspector and adjust the `Speed`.

- **Animated**: If true, the progress bar will animate towards the target value.
- **Speed**: Determines how fast the transition occurs.
- **Ignore Timescale**: If true, the animation will not be affected by `Time.timeScale`, which is useful for UI that needs to remain responsive even when the game is paused.

You can also set the value instantly, bypassing the animation, by calling `SetValueWithoutAnimation()`:

```csharp
// Instantly sets the health bar to full, e.g., on respawn
healthBar.SetValueWithoutAnimation(1.0f);
```

### Batch Changes

If you need to change multiple configuration properties of a progress bar via code (e.g., changing colors or transforms) without triggering redundant visual refreshes each time, use `StartChanges()` and `CommitChanges()`:

```csharp
myColorBar.StartChanges();
myColorBar.StartColor = Color.red;
myColorBar.EndColor = Color.green;
myColorBar.CommitChanges(); // Visuals are updated only once here
```

## Built-in Progress Bars

Progressio comes with several built-in implementations ready to use:

* **ScaleProgressBar**: Interpolates the `localScale` of a target `Transform` between a start and end scale.
* **PositionProgressBar**: Interpolates the `localPosition` of a target `Transform`.
* **RotationProgressBar**: Interpolates the `localRotation` of a target `Transform`.
* **ColorProgressBar**: Interpolates the `color` of a Unity UI `Graphic` (like an `Image` or `Text`) between a start and end color.
* **ImageFillProgressBar**: Adjusts the `fillAmount` of a Unity UI `Image`.
* **CanvasAlphaProgressBar**: Interpolates the `alpha` of a `CanvasGroup`.
* **CombinedProgressBar**: Groups multiple progress bars together. Updating the `Value` on this component automatically updates all progress bars in its list.

## Creating Custom Progress Bars

You can easily create your own custom progress bar by inheriting from the `ProgressBar` base class and implementing the `Apply(float newValue)` method.

```csharp
using Damdor.Progressio;
using UnityEngine;
using TMPro;

public class TextProgressBar : ProgressBar
{
    public TextMeshProUGUI text;

    protected override void Apply(float newValue)
    {
        if (text != null)
        {
            // Convert the 0-1 progress value to a percentage
            text.text = $"{(newValue * 100f):0}%";
        }
    }
}
```