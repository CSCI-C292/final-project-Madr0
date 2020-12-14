using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] RuntimeData _runtimeData;

    public float health = 40f;

    public void TakeDamage(float amount) {
        health -= amount;
        if(health <= 0f) {
            Die();
        }
    }

    void Die() {
        _runtimeData.currentScore += 250;
        Destroy(gameObject);
    }
}
