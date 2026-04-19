using UnityEngine;
using TMPro;

public class HouseTutorialTrigger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject knockPrompt;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text tutorialText;

    [Header("Input")]
    [SerializeField] private KeyCode interactKey = KeyCode.A;

    [TextArea(3, 6)]
    [SerializeField] private string tutorialMessage = "Press Q to charge up your magic to protect us from those goblins.";

    private bool playerInRange;
    private bool tutorialOpen;

    private void Start()
    {
        if (knockPrompt != null)
            knockPrompt.SetActive(false);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            if (!tutorialOpen)
            {
                tutorialOpen = true;

                if (knockPrompt != null)
                    knockPrompt.SetActive(false);

                if (tutorialPanel != null)
                    tutorialPanel.SetActive(true);

                if (tutorialText != null)
                    tutorialText.text = tutorialMessage;
            }
            else
            {
                tutorialOpen = false;

                if (tutorialPanel != null)
                    tutorialPanel.SetActive(false);

                if (knockPrompt != null)
                    knockPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (!tutorialOpen && knockPrompt != null)
            knockPrompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;
        tutorialOpen = false;

        if (knockPrompt != null)
            knockPrompt.SetActive(false);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }
}