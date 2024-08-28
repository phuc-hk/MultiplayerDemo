using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    // Reference to the Win and Lose UI Panels
    [SerializeField] private GameObject winUIPanel;
    [SerializeField] private GameObject loseUIPanel;

    // Reference to a TextMeshProUGUI for the countdown number
    [SerializeField] private TextMeshProUGUI countdownText;

    // Start with the UI panels deactivated
    void Start()
    {
        winUIPanel.SetActive(false);
        loseUIPanel.SetActive(false);
    }

    // Call this method when the player wins
    public void ShowWinUI()
    {
        winUIPanel.SetActive(true);
        StartCoroutine(BackToRoom());
    }

    // Call this method when the player loses
    public void ShowLoseUI()
    {
        loseUIPanel.SetActive(true);
        StartCoroutine(BackToRoom());
    }

    // Call this method to reset or hide both panels if needed
    public void ResetUI()
    {
        winUIPanel.SetActive(false);
        loseUIPanel.SetActive(false);
    }

    private IEnumerator BackToRoom()
    {
        countdownText.gameObject.SetActive(true); // Show countdown

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString(); // Update the countdown text
            yield return new WaitForSeconds(1); // Wait for 1 second
        }

        countdownText.gameObject.SetActive(false); // Hide countdown after finishing

        PhotonManager.Instance.NavigateBackToRoom(); // Go back to room
    }

    public void ToggleUI(GameObject UIElement)
    {
        UIElement.SetActive(!UIElement.activeSelf);
    }
}
