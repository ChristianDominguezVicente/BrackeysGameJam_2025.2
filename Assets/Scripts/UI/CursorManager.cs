using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    private string controlUsed;

    private void OnEnable()
    {
        InputSystem.onEvent += OnInputEvent;
    }

    private void OnDisable()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            return;

        if (device is Gamepad && controlUsed != "Gamepad")
        {
            controlUsed = "Gamepad";
            UpdateUIForGamepad();
        }
        else if ((device is Keyboard || device is Mouse) && controlUsed != "Keyboard")
        {
            controlUsed = "Keyboard";
            UpdateUIForKeyboard();
        }
    }

    private void UpdateUIForKeyboard()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void UpdateUIForGamepad()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
