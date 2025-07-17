using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput: MonoBehaviour {
    
    private PlayerInputActions playerInputActions;
    
    public static GameInput Instance { get; private set; }

    public event EventHandler OnPlayerAttack;
    private void Awake() {
        Instance = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Combat.Attack.started += PlayerAttack_started;
    }
    
    public Vector2 GetMovementVector() {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        
        return inputVector;
    }

    public Vector3 GetMousePosition() {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        return mousePos;
    }

    public void DisableMovement() {
        playerInputActions.Disable();
    }
    
    private void PlayerAttack_started(InputAction.CallbackContext obj) {
        OnPlayerAttack?.Invoke(this, EventArgs.Empty);
    }
    
}

