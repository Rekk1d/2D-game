using System;
using UnityEngine;
using UnityEngine.AI;
using Adventure.Utils;

public class EnemyAI: MonoBehaviour {
    [SerializeField] private State _startingState;
    [SerializeField] private float _minDistance = 3f;
    [SerializeField] private float _maxDistance = 7f;
    [SerializeField] private float _roamingTimerMax = 2f;
    [SerializeField] private bool _isChasingEnemy = false;
    [SerializeField] private bool _isAttackingEnemy = false;
    
    private NavMeshAgent _navMeshAgent;
    private State _currentState;
    
    private float _roamingTimer;
    private float _roamingSpeed;
    
    private Vector3 _routePosition;
    private Vector3 _startingPosition;
    
    [SerializeField] private float _chasingDistance = 4f;
    [SerializeField] private float _chasingSpeedMultiplier = 2f;
    private float _chasingSpeed;

    [SerializeField] private float _attackRate = 2f;
    private float _nextAttackTime = 0f;
    [SerializeField] private float _attackingDistance = 2f;
    public event EventHandler OnEnemyAttack;

    private float _nexCheckDirectionTime = 0f;
    private float _checkDirectionDuration = 0.1f;
    private Vector3 _lastPosition;

    public bool IsRunning  {
        get {
            if (_navMeshAgent.velocity == Vector3.zero) {
                return false;
            }

            return true;
        }
    }
    
    public float GetChasingAnimationSpeed() {
        return _navMeshAgent.speed / _roamingSpeed;
    }
    
    
    private enum State {
        Idle,
        Roaming,
        Chasing,
        Attacking,
        Death
    }

    private void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _currentState = _startingState;
        _roamingSpeed = _navMeshAgent.speed;
        _chasingSpeed = _navMeshAgent.speed * _chasingSpeedMultiplier;
    }

    private void Update() {
        StateHandler();
        MovementDirectionHandler();
    }

    public void SetDeathState() {
        _navMeshAgent.ResetPath();
        _currentState = State.Death;
    }

    private void StateHandler() {
        switch (_currentState) {
            case State.Roaming:
                _roamingTimer -= Time.deltaTime;
                if (_roamingTimer < 0) {
                    Roaming();
                    _roamingTimer = _roamingTimerMax;
                }
                CheckCurrentState();
                break;
            case State.Chasing:
                ChasingTarget();
                CheckCurrentState();
                break;
            case State.Attacking:
                AttackingTarget();
                CheckCurrentState();
                break;
            case State.Death:
                break;
            default:
            case State.Idle:
                break;
        }
    }

    private void CheckCurrentState() {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);
        State newState = State.Roaming;

        if (_isChasingEnemy && distanceToPlayer <= _minDistance) {
            newState = State.Chasing;
        }

        if (_isAttackingEnemy && distanceToPlayer <= _attackingDistance) {
            if (Player.Instance.IsAlive()) {
                newState = State.Attacking;
            }
            else {
                newState = State.Roaming;
            }
        }

        if (newState != _currentState) {
            if (newState == State.Chasing) {
                _navMeshAgent.ResetPath();
                _navMeshAgent.speed = _chasingSpeed;
            } else if (newState == State.Roaming) {
                _roamingTimer = 0f;
                _navMeshAgent.speed = _roamingSpeed;
            } else if (newState == State.Attacking) {
                _navMeshAgent.ResetPath();
            }
            _currentState = newState;
        }
    }
    
    private void Roaming() {
        _startingPosition = transform.position;
        _routePosition = GetRoamingPosition();
        _navMeshAgent.SetDestination(_routePosition);
    }

    private Vector3 GetRoamingPosition() {
        return _startingPosition + Utils.GetRandom() * UnityEngine.Random.Range(_minDistance, _maxDistance);
    }

    private void MovementDirectionHandler() {
        if (Time.time > _nexCheckDirectionTime) {
            if (IsRunning) {
                ChangeFaceDirection(_lastPosition, transform.position);
            } else if (_currentState == State.Attacking) {
                ChangeFaceDirection(transform.position, Player.Instance.transform.position);
            }
            
            _lastPosition = transform.position;
            _nexCheckDirectionTime = Time.time + _checkDirectionDuration;
        }
    }

    private void ChangeFaceDirection(Vector3 currentPosition, Vector3 targetPosition) {
        if (currentPosition.x > targetPosition.x) {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void ChasingTarget() {
        _navMeshAgent.SetDestination(Player.Instance.transform.position);
    }

    private void AttackingTarget() {
        if (Time.time > _nextAttackTime) {
            OnEnemyAttack?.Invoke(this, EventArgs.Empty);
            _nextAttackTime = Time.time + _attackRate;
        }
    }
    
}
