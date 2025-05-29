using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    [SerializeField] private string content;

    public void SetText(string newText)
    {
        content = newText;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.Instance?.Show(content, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance?.Hide();
    }
}
