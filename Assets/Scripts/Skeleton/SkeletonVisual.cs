using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class SkeletonVisual : MonoBehaviour {
    [SerializeField] private EnemyAI _enemyAI;
    [SerializeField] private EnemyEntity _enemyEntity;
    [SerializeField] private GameObject _enemyShadow;
    private Animator _animator;

    private const string IS_RUNNING = "IsRunning";
    private const string CHASING_SPEED_MULTIPLIER = "ChasingSpeedMultiplier";
    private const string ATTACK = "Attack";
    private const string TAKE_HIT = "TakeHit";
    private const string IS_DIE = "IsDie";
    
    SpriteRenderer _spriteRenderer;
    private void Awake() {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        _enemyAI.OnEnemyAttack += _enemyAI_OnEnemyAttack;
        _enemyEntity.OnTakeHit += _enemyEntity_OnTakeHit;
        _enemyEntity.OnDie += _enemyEntity_OnDie;
    }

    private void _enemyEntity_OnTakeHit(object sender, EventArgs e) {
        _animator.SetTrigger(TAKE_HIT);
    }

    private void _enemyEntity_OnDie(object sender, EventArgs e) {
        _animator.SetBool(IS_DIE, true);
        _spriteRenderer.sortingOrder = -1;
        _enemyShadow.SetActive(false);
    }
    private void OnDestroy() {
        _enemyAI.OnEnemyAttack -= _enemyAI_OnEnemyAttack;
    }
    
    private void Update() {
        _animator.SetBool(IS_RUNNING, _enemyAI.IsRunning);
        _animator.SetFloat(CHASING_SPEED_MULTIPLIER, _enemyAI.GetChasingAnimationSpeed());
    }

    public void AttackColliderTurnOff() {
        _enemyEntity.AttackColliderTurnOff();
    }

    public void AttackColliderTurnOn() {
        _enemyEntity.AttackColliderTurnOn();
    }
    
    private void _enemyAI_OnEnemyAttack(object sender, EventArgs e) {
        _animator.SetTrigger(ATTACK);
    }
}
