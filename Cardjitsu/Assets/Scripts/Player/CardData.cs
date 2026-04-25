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

    public string instanceId;
    public Element element;
    public int value;

    public CardData(string instanceId, Element element, int value)
    {
        this.instanceId = instanceId;
        this.element = element;
        this.value = value;
    }
}