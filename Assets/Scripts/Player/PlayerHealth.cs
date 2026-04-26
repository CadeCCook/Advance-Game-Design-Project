using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    float health;     // Initialize health

    Boolean isDead = false;

    //public Image healthFill;
    public float maxHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (maxHealth == 0) // If health variable is not assigned, defaults to 100
            maxHealth = 100;

        health = maxHealth;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        Debug.Log("Player taking damage: " + amount + ". Health: " + health);

        health -= amount;

        // Update healthbar
        //Vector2 current = healthFill.rectTransform.sizeDelta;

        //if (health < 0) health = 0;

        //Vector2 newSize = new Vector2(health, current.y);

        //healthFill.rectTransform.sizeDelta = newSize;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        //anim.SetTrigger("Die");


        GetComponent<Collider>().enabled = false;

        StartCoroutine(DestroyAfterDeath());
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(5); // match your animation length
        Destroy(gameObject);
    }

    public Boolean getIsDead()
    {
        return isDead;
    }
}
