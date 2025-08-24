using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AbstractCard", menuName = "Scriptable Objects/AbstractCard")]
public abstract class Card : ScriptableObject
{
    public string cardId;
    public string cardName;
    public int manaCost;

    public List<CardEffect> effects;

    public abstract void OnActivated(IHittable target);
}
