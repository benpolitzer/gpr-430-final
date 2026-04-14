using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private Image cardBack;
    [SerializeField] private Image art;
    [SerializeField] private Image elementIcon;
    [SerializeField] private TMP_Text valueText;

    public void SetCard(Sprite back, Sprite artSprite, Sprite elementSprite, int value)
    {
        if (cardBack != null)
            cardBack.sprite = back;

        if (art != null)
            art.sprite = artSprite;

        if (elementIcon != null)
            elementIcon.sprite = elementSprite;

        if (valueText != null)
            valueText.text = value.ToString();
    }
}