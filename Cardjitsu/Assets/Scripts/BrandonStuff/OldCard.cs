using NUnit.Framework.Internal;
using UnityEngine;
using VG.Tweener;

public class OldCard : MonoBehaviour
{
    public enum Element
    {
        Fire = 1,
        Water = 2,
        Ice = 3
    };

    public enum Color
    {
        Red = 1,
        Blue = 2,
        Yellow = 3,
        Green = 4,
        Orange = 5,
        Purple = 6
    };

    public enum ModifierType
    {
        ValueChange = 1, // TODO: power card types
    }

    private Color mColor = Color.Red;
    private Element mElement = Element.Fire;
    private int mValue = 1;

    private int playerWhoOwns = 1; // implement me with card checking on server

    public Color color { get { return mColor; } }
    public Element element { get { return mElement; } }
    public int value { get { return mValue; } }

    // used for power cards
    public void ApplyModifier()
    {

    }
    
    public void Randomize()
    {
        mValue = Random.Range(1, 13);
        mElement = (Element)Random.Range(1, 3);
        mColor = (Color)Random.Range(1, 6);
    }

    public void OnMouseOver()
    {
        Tweener.LocalScale(gameObject, Vector3.one * 1.2f, 0.3f, EasingStyle.Quintic);
    }

    public void OnMouseOut() 
    {
        Tweener.LocalScale(gameObject, Vector3.one, 0.3f, EasingStyle.Quintic);
    }
}
