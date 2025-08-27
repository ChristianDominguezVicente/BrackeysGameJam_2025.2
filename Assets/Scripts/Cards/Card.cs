using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AbstractCard", menuName = "Scriptable Objects/Cards/AbstractCard")]
public abstract class Card : ScriptableObject
{
    public string cardId;
    public string cardName;
    public int manaCost;
    public int damage;
    public bool areaEffect;

    public List<CardEffect> effects;

    public abstract void OnActivated(IHittable target);
}
