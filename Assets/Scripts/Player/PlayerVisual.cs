using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerVisual: MonoBehaviour {
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    private const string IS_RUNNING = "isRunning";

    private void Awake() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        animator.SetBool(IS_RUNNING, Player.Instance.IsRunning());
        ChangePlayerFacingDirection();
    }

    private void ChangePlayerFacingDirection() {
        Vector3 playerPos = Player.Instance.GetPlayerPosition();
        Vector3 mousePos = GameInput.Instance.GetMousePosition();
        Vector2 rightStick = Vector2.zero;
        if (Gamepad.current != null) {
            rightStick = Gamepad.current.rightStick.ReadValue();
        }
        spriteRenderer.flipX = mousePos.x < playerPos.x || rightStick.x < 0f;
    }
}
