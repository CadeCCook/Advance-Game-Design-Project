using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    float health;     // Initialize health

    Animator anim;  // Initialize animator

    Boolean isDead = false;

    public Image healthFill;
    public float maxHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (maxHealth == 0) // If health variable is not assigned, defaults to 100
            maxHealth = 100;

        health = maxHealth;

        anim = GetComponent<Animator>();    // Get the animator in order to play animations
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {

        }
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("Goblin taking damage: " + amount + ". Health: " + health);
        if (isDead) return;

        health -= amount;

        // Update healthbar
        Vector2 current = healthFill.rectTransform.sizeDelta;

        if (health < 0) health = 0;

        Vector2 newSize = new Vector2(health, current.y);

        healthFill.rectTransform.sizeDelta = newSize;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        anim.SetTrigger("Die");

        GetComponent<Collider>().enabled = false;

        StartCoroutine(DestroyAfterDeath());
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(5); // match your animation length
        Destroy(gameObject);
    }
}
