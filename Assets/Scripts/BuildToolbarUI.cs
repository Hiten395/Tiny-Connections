using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildToolbarUI : MonoBehaviour
{
    [SerializeField] MouseManager mouseManager;
    [SerializeField] TMP_Text selectedModeText;
    [SerializeField] Image conveyorButtonImage;
    [SerializeField] Image recipeButtonImage;
    [SerializeField] TMP_Text recipeButtonText;
    [SerializeField] Image machineButtonImage;
    [SerializeField] Image deleteButtonImage;
    [SerializeField] Color normalColor = new Color32(47, 65, 88, 255);
    [SerializeField] Color selectedColor = new Color32(229, 154, 82, 255);
    [SerializeField] Color deleteSelectedColor = new Color32(190, 70, 75, 255);

    string currentMode = "Conveyor";

    void Awake()
    {
        if (mouseManager == null)
        {
            mouseManager = FindAnyObjectByType<MouseManager>();
        }
    }

    void Start()
    {
        SetConveyorMode();
    }

    public void SetConveyorMode()
    {
        if (mouseManager != null)
        {
            mouseManager.SetConveyorMode();
        }

        SetMode("Conveyor", conveyorButtonImage);
    }

    public void SetRecipeMode()
    {
        string description;
        if (mouseManager != null)
        {
            description = mouseManager.SetRecipeMode();
            recipeButtonText.text = description;
        }

        SetMode("Spawner", recipeButtonImage);
    }

    public void SetMachineMode()
    {
        if (mouseManager != null)
        {
            mouseManager.SetMachineMode();
        }

        SetMode("Machine", machineButtonImage);
    }

    public void SetDeleteMode()
    {
        if (mouseManager != null)
        {
            mouseManager.SetDeleteMode();
        }

        SetMode("Delete", deleteButtonImage);
    }

    void SetMode(string mode, Image selectedImage)
    {
        currentMode = mode;

        if (selectedModeText != null)
        {
            selectedModeText.text = $"Mode: {currentMode}";
        }

        ResetButton(conveyorButtonImage);
        ResetButton(recipeButtonImage);
        ResetButton(machineButtonImage);
        ResetButton(deleteButtonImage);

        if (selectedImage != null)
        {
            selectedImage.color = mode == "Delete" ? deleteSelectedColor : selectedColor;
        }
    }

    void ResetButton(Image buttonImage)
    {
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }
}
