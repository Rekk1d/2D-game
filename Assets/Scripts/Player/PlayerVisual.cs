using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerVisual: MonoBehaviour {
    private Animator _animator;
    private SpriteRenderer spriteRenderer;
    private FlashBlink _flashBlink;
    
    private const string IS_RUNNING = "isRunning";
    private const string IS_DIE = "IsDie";

    private void Awake() {
        _animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _flashBlink = GetComponent<FlashBlink>();
    }

    private void Start() {
        Player.Instance.OnPlayerDeath += Player_OnPlayerDeath;
    }

    private void Player_OnPlayerDeath(object sender, EventArgs e) {
        _animator.SetBool(IS_DIE, true);
        _flashBlink.StopBlinking();
    }

    private void Update() {
        _animator.SetBool(IS_RUNNING, Player.Instance.IsRunning());
        if (Player.Instance.IsAlive()) {
            ChangePlayerFacingDirection();
        }
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

    private void OnDestroy() {
        Player.Instance.OnPlayerDeath -= Player_OnPlayerDeath;
    }
}
