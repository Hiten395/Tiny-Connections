using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayHUDUI : MonoBehaviour
{
    [SerializeField] LoseManager loseManager;
    [SerializeField] TMP_Text citySupplyText;
    [SerializeField] Image demandBarFill;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] Color stableColor = new Color32(110, 224, 106, 255);
    [SerializeField] Color lowColor = new Color32(229, 154, 82, 255);
    [SerializeField] Color criticalColor = new Color32(220, 72, 72, 255);

    RectTransform demandBarFillRect;
    
    int currentResource = 1;
    void Awake()
    {
        if (loseManager == null)
        {
            loseManager = FindAnyObjectByType<LoseManager>();
        }

        if (demandBarFill != null)
        {
            demandBarFillRect = demandBarFill.rectTransform;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (loseManager == null || citySupplyText == null || demandBarFill == null || demandBarFillRect == null)
        {
            return;
        }

        float supplyPercent = loseManager.supplyPercent[0];
        UpdateText(supplyPercent);
        UpdateBar(supplyPercent);
    }

    void UpdateText(float supplyPercent)
    {
        string status = "Stable";
        if (supplyPercent <= 0.3f)
        {
            status = "Critical";
        }
        else if (supplyPercent <= 0.6f)
        {
            status = "Low";
        }

        citySupplyText.text = $"Home Supply: {status}";
    }

    void UpdateBar(float supplyPercent)
    {
        if (supplyPercent > 1f)
        {
            supplyPercent = 1f;
        }
        demandBarFillRect.anchorMin = new Vector2(0f, 0f);
        demandBarFillRect.anchorMax = new Vector2(supplyPercent, 1f);
        demandBarFillRect.offsetMin = new Vector2(3f, 3f);
        demandBarFillRect.offsetMax = new Vector2(-3f, -3f);

        if (supplyPercent <= 0.3f)
        {
            demandBarFill.color = criticalColor;
        }
        else if (supplyPercent <= 0.6f)
        {
            demandBarFill.color = lowColor;
        }
        else
        {
            demandBarFill.color = stableColor;
        }
    }

    public void NewResource()
    {
        currentResource++;
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }
}
