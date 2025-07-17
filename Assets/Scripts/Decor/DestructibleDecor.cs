using System;
using UnityEngine;

public class DestructibleDecor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<Sword>()) {
            Destroy(gameObject);
        }
    }
}
