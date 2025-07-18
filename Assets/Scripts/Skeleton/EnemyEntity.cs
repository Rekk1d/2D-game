using System;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class EnemyEntity : MonoBehaviour {

    public event EventHandler OnTakeHit;
    public event EventHandler OnDie;
    [SerializeField] private Skeleton _skeleton;
    private int _currentHealth;
    private bool _isAttacking = false;
    
    private PolygonCollider2D _polygonCollider2D;
    private BoxCollider2D _boxCollider2D;
    private EnemyAI _enemyAI;

    private void Awake() {
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
        _boxCollider2D =  GetComponent<BoxCollider2D>();
        _enemyAI = GetComponent<EnemyAI>();
    }
    
    private void Start() {
        _currentHealth = _skeleton.enemyHealth;
    }

    public void TakeDamage(int damage) {
        _currentHealth -= damage;
        OnTakeHit?.Invoke(this, EventArgs.Empty);
        DetectDeath();
    }
    
    public void AttackColliderTurnOff() {
        _polygonCollider2D.enabled = false;
        _isAttacking = false;
    }

    public void AttackColliderTurnOn() {
        _polygonCollider2D.enabled = true;
        _isAttacking = true;
    }

    private void DetectDeath() {
        if (_currentHealth <= 0) {
            OnDie?.Invoke(this, EventArgs.Empty);
            _boxCollider2D.enabled = false;
            _polygonCollider2D.enabled = false;
            _enemyAI.SetDeathState();
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (!_isAttacking) {
            return;
        }
        if (collision.transform.TryGetComponent(out Player player)) {
            player.TakeDamage(transform, _skeleton.enemyDamageAmount);
            _isAttacking = false;
        }
    }
}
