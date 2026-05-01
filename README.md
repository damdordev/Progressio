# Progressio

Progressio is a Unity package that provides an abstraction for different types of progress bars.
It works in both Edit Mode (to visualize changes instantly when modifying the value in the inspector) and Play Mode.

## Features

- **Base `ProgressBar` class:** All progress bars inherit from this class and can be controlled uniformly by changing the `Value` property.
- **`ImageFillProgressBar`:** Modifies the `fillAmount` of a target `Image`.
- **`PositionProgressBar`:** Modifies the position (local or world) of a target `Transform`.
- **`RotationProgressBar`:** Modifies the rotation (Euler angles, local or world) of a target `Transform`.
- **`ScaleProgressBar`:** Modifies the local scale of a target `Transform`.
- **`ColorProgressBar`:** Modifies the color of a target `Graphic` (like `Image` or `Text`).
- **`CanvasAlphaProgressBar`:** Modifies the `alpha` property of a target `CanvasGroup`.
- **`CombinedProgressBar`:** Takes a list of other progress bars and sets their `Value` to its own value, synchronizing them.

## Usage

### Example 1: Creating a simple Image Fill Progress Bar
1. Add an `Image` to your Canvas and set its `Image Type` to `Filled`.
2. Add the `ImageFillProgressBar` component to a GameObject.
3. Assign the `Image` to the `Target` field in the inspector.
4. Set `Start Value` and `End Value` (e.g., 0 to 1).
5. Modify the `Value` slider in the inspector to see the image fill instantly!

### Example 2: Synchronizing multiple visual effects
If you want a progress bar that scales up an object while also fading it in:
1. Create a `ScaleProgressBar` and a `CanvasAlphaProgressBar` on your target GameObject (ensure it has a CanvasGroup).
2. Create a `CombinedProgressBar` component.
3. Add the `ScaleProgressBar` and `CanvasAlphaProgressBar` to the `ProgressBars` list in the `CombinedProgressBar` inspector.
4. Changing the `Value` of the `CombinedProgressBar` will now update both scale and alpha simultaneously.

### Scripting Example
You can easily control progress bars from your own scripts:

```csharp
using UnityEngine;
using Damdor.Progressio;

public class ProgressController : MonoBehaviour
{
    public ProgressBar myProgressBar;

    private void Update()
    {
        // Example: Oscillating progress between 0 and 1 over time
        float progress = (Mathf.Sin(Time.time) + 1f) / 2f;
        myProgressBar.Value = progress;
    }
}
```
