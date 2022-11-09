using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public Button interactButton;
    public Label interactText;

    private void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        interactButton = root.Q<Button>("interact-button");
    }

    private void InteractButtonPressed()
    {
        
    }
}
