using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AbstractCard", menuName = "Scriptable Objects/Cards/AbstractCard")]
public abstract class Card : ScriptableObject
{
    public string cardId;
    public string cardName;
    public int manaCost;
    public int damage;
    public int healPoints;
    public bool areaEffect;
    public Sprite cardSprite;

    public List<CardEffect> effects;
    public EnemyType targetType;

    public virtual void OnActivated(IHittable target)
    {
        if (this.effects != null)
            foreach (CardEffect effect in effects)
                effect.OnEffectActivated(target, this.damage);
    }
}
