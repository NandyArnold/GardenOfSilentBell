using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    [SerializeField] private GameObject tooltipRoot;
    [SerializeField] private TMP_Text tooltipText;

    private RectTransform rectTransform;

    private void Awake()
    {
        Instance = this;
        rectTransform = tooltipRoot.GetComponent<RectTransform>();
        Hide();
    }

    public void Show(string text, Vector3 position)
    {
        tooltipText.text = text;
        tooltipRoot.SetActive(true);
        rectTransform.position = position + new Vector3(10, -10); // slight offset
    }

    public void Hide()
    {
        tooltipRoot.SetActive(false);
    }
}
