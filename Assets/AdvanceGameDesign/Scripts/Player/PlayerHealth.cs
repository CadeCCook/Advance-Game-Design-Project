using System; 
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    float health;     // Initialize health

    Boolean isDead = false;

    private Animator animator;

    public Image healthFill;
    public float maxHealth;

    //Game over ui
    public GameObject gameOverPanel;

    void Start()
    {
        Time.timeScale = 1f; //Reset time when scene starts

        if (maxHealth == 0) // If health variable is not assigned, defaults to 100
            maxHealth = 100;

        health = maxHealth;

        animator = GetComponentInChildren<Animator>();

        //Hides game over at start
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        else
            Debug.LogWarning("Game Over Panel is not assigned on PlayerHealth.");
    }

    void Update()
    {
        //Restarts game
        if (isDead && Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1f;

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        Debug.Log("Player taking damage: " + amount + ". Health: " + health);

        health -= amount;

        // Update healthbar
        Vector2 current = healthFill.rectTransform.sizeDelta;

        if (health < 0) health = 0;

        Vector2 newSize = new Vector2(health * 3, current.y);

        healthFill.rectTransform.sizeDelta = newSize;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        animator.SetTrigger("Die");

        GetComponent<Collider>().enabled = false;

        //Shows game over
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        //Pauses the game
        Time.timeScale = 0f;

        //StartCoroutine(DestroyAfterDeath());
    }

    //IEnumerator DestroyAfterDeath()
    //{
        //yield return new WaitForSeconds(5); // match your animation length
        //Destroy(gameObject);
    //}

    public Boolean getIsDead()
    {
        return isDead;
    }
}