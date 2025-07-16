using System;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]

public class Player: MonoBehaviour {

    public static Player Instance { get; private set; }
    
    private Rigidbody2D rb;

    [SerializeField] private float moveSpeed = 5f;
    private float minMovingSpeed = 0.1f;
    private Vector2 inputVector;
    private bool isRunning = false;
    
    private Camera mainCamera;
    
    private void Awake() {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        mainCamera = Camera.main;
    }

    private void Start() {
        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
    }

    private void GameInput_OnPlayerAttack(object sender, System.EventArgs e) {
        ActiveWeapon.Instance.GetActiveWeapon().Attack();
    }
    
    private void Update() {
        inputVector = GameInput.Instance.GetMovementVector();
    }
    
    private void FixedUpdate() {
        HandeMovement();
    }

    private void HandeMovement() {
        rb.MovePosition(rb.position + inputVector * (moveSpeed * Time.fixedDeltaTime));

        if (Math.Abs(inputVector.x) > minMovingSpeed || Math.Abs(inputVector.y) > minMovingSpeed) {
            isRunning = true;
        } else {
            isRunning = false;
        }
        
    }

    public bool IsRunning() {
        return isRunning;
    }
    
    public Vector3 GetPlayerPosition() {
        Vector3 playerPos = mainCamera.WorldToScreenPoint(transform.position);
        return playerPos;
    }
    
}