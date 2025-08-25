using UnityEngine;
using UnityEngine.EventSystems;

public class HighlghtSelect : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
