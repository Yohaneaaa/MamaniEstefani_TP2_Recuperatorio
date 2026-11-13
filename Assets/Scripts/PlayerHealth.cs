using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private int maxHealth;
    private int currentHealth;
    private float invulnerableTime;  
    private float lastHitTime;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0;

    public event Action<int> OnHealthChanged;  
    public event Action OnDied;

    private void Awake()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
        invulnerableTime = 1f;
        lastHitTime = -999f;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        if (Time.time - lastHitTime < invulnerableTime) return;
        lastHitTime = Time.time;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth == 0)
        {
            OnDied?.Invoke();
        }
    }

    public int Heal(int amount)
    {
        if (IsDead || amount <= 0) return 0;

        int before = currentHealth;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth);
        return currentHealth - before;
    }
}