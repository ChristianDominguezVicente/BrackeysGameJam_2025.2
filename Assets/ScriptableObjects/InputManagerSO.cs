using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "InputManager")]
public class InputManagerSO : ScriptableObject
{
    Controls controls;
    public event Action OnAction;
    public event Action OnCancel;
    public event Action OnPause;
    public event Action<Vector2> OnMove;

    private void OnEnable()
    {
        controls = new Controls();
        controls.Gameplay.Enable();
        controls.Gameplay.Action.started += Action;
        controls.Gameplay.Cancel.started += Cancel;
        controls.Gameplay.Pause.started += Pause;
        controls.Gameplay.Move.performed += Move;
        controls.Gameplay.Move.canceled += Move;
    }

    private void Action(InputAction.CallbackContext ctx)
    {
        OnAction?.Invoke();
    }

    private void Cancel(InputAction.CallbackContext ctx)
    {
        OnCancel?.Invoke();
    }

    private void Pause(InputAction.CallbackContext ctx)
    {
        OnPause?.Invoke();
    }

    private void Move(InputAction.CallbackContext ctx)
    {
        OnMove?.Invoke(ctx.ReadValue<Vector2>());
    }
}
