using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
[RequireComponent(typeof(Rigidbody2D))]
public class Player: MonoBehaviour {

    public static Player Instance { get; private set; }
    public event EventHandler OnPlayerDeath;
    public event EventHandler OnFlashBlink;
    private bool _isAlive;
    public bool IsAlive() => _isAlive;

    
    private Rigidbody2D rb;

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private int _maxHealth = 10;
    [SerializeField] private float _damageRecoveryTime = 0.5f;
    private float _minMovingSpeed = 0.1f;
    private Vector2 _inputVector;
    private bool _isRunning = false;
    private int _currentHealth;
    private bool _canTakeDamage;
    
    private Camera _mainCamera;
    private KnockBack _knockBack;
    
    private void Awake() {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        _mainCamera = Camera.main;
        _knockBack = GetComponent<KnockBack>();
    }

    private void Start() {
        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
        _currentHealth = _maxHealth;
        _canTakeDamage = true;
        _isAlive = true;
    }
    
    private void Update() {
        _inputVector = GameInput.Instance.GetMovementVector();
    }
    
    public bool IsRunning() {
        return _isRunning;
    }
    
    public Vector3 GetPlayerPosition() {
        Vector3 playerPos = _mainCamera.WorldToScreenPoint(transform.position);
        return playerPos;
    }

    public void TakeDamage(Transform damageSource, int damage) {
        if (_canTakeDamage && _isAlive) {
            _canTakeDamage = false;
            _knockBack.GetKnockedBack(damageSource);
            _currentHealth = Math.Max(0, _currentHealth -= damage);
            StartCoroutine(DamageRecoveryTimer());
            OnFlashBlink?.Invoke(this, EventArgs.Empty);
        }
        
        DetectDeath();
    }

    private IEnumerator DamageRecoveryTimer() {
        yield return new WaitForSeconds(_damageRecoveryTime);
        _canTakeDamage = true;
    }
    
    private void GameInput_OnPlayerAttack(object sender, System.EventArgs e) {
        ActiveWeapon.Instance.GetActiveWeapon().Attack();
    }
    
    private void FixedUpdate() {
        if (_knockBack.IsGetKnockedBack) {
            return;
        }
        HandleMovement();
    }

    private void HandleMovement() {
        rb.MovePosition(rb.position + _inputVector * (_moveSpeed * Time.fixedDeltaTime));

        if (Math.Abs(_inputVector.x) > _minMovingSpeed || Math.Abs(_inputVector.y) > _minMovingSpeed) {
            _isRunning = true;
        } else {
            _isRunning = false;
        }
        
    }

    private void DetectDeath() {
        if (_currentHealth == 0 && _isAlive) {
            _isAlive = false;
            _canTakeDamage = false;
            _knockBack.StopKnockBackMovement();
            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
            GameInput.Instance.DisableMovement();
        }
    }
    
}