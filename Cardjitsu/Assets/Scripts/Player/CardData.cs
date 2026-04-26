using System;

[Serializable]
public class CardData
{
    public enum Element
    {
        Fire,
        Water,
        Ice
    }

    public enum CardColor
    {
        Red,
        Blue,
        Yellow,
        Green,
        Orange,
        Purple
    }

    public string instanceId;
    public Element element;
    public CardColor color;
    public int value;
    public int handIndex;

    public CardData(string instanceId, Element element, CardColor color, int value, int handIndex = -1)
    {
        this.instanceId = instanceId;
        this.element = element;
        this.color = color;
        this.value = value;
        this.handIndex = handIndex;
    }
}