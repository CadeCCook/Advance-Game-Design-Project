using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public float health;
    public float maxHealth = 300f;

    public Image healthFill;
    public GameObject winPanel;

    private Animator animator;
    private Boolean isDead = false;

    void Start()
    {
        Time.timeScale = 1f;

        health = maxHealth;
        animator = GetComponent<Animator>();

        if (healthFill != null)
            healthFill.fillAmount = 1f;

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    void Update()
    {
        if (isDead && Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1f;

            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        Debug.Log("Boss taking damage: " + amount + ". Health: " + health);

        health -= amount;

        Vector2 current = healthFill.rectTransform.sizeDelta;

        if (health < 0) health = 0;

        Vector2 newSize = new Vector2(health/3, current.y);

        healthFill.rectTransform.sizeDelta = newSize;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        Debug.Log("Boss defeated!");

        GetComponent<Collider>().enabled = false;

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    public Boolean getIsDead()
    {
        return isDead;
    }
}