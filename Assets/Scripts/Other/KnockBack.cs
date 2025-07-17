using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockBack : MonoBehaviour
{
    [SerializeField] private float _knockBackForce = 1f;
    [SerializeField] private float _knockBackMovingTimerMax = 0.3f;

    private float _knockBackMovingTimer;
    public bool IsGetKnockedBack { get; private set;  }
    private Rigidbody2D rb;
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        _knockBackMovingTimer -= Time.deltaTime;

        if (_knockBackMovingTimer < 0) {
            StopKnockBackMovement();
        }
    }

    public void GetKnockedBack(Transform damageSource) {
        IsGetKnockedBack = true;
        _knockBackMovingTimer = _knockBackMovingTimerMax;
        Vector2 difference = (transform.position - damageSource.position).normalized * _knockBackForce / rb.mass;
        rb.AddForce(difference, ForceMode2D.Impulse);
    }

    public void StopKnockBackMovement() {
        rb.linearVelocity = Vector2.zero;
        IsGetKnockedBack = false;
    }
}
