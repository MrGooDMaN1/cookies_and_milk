using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
   [SerializeField] private int health;
    public Action<int, GameObject> OnTakeHit;
   public int CurrentHealth
    {
        get { return health; }
    }
    private void Start()
    {
        GameManager.Instance.healthContainer.Add(gameObject, this);
    }
    public void Takehit(int damage, GameObject attacker)
    {
        health -= damage;
        Debug.Log(health);

        if (OnTakeHit != null)
            OnTakeHit(damage, attacker);

        if (health <= 0)
            Destroy(gameObject);
    }
     public void SetHealth(int bonusHealth)
     {
        health += bonusHealth;
     }
}
