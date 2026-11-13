using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum Keywords { FLASH, HASTE, FLYING}

public class CardData
{
    public readonly Sprite art;
    public readonly String name;
    public readonly String description;
    public readonly String flavorText;
    public readonly int power;
    public readonly int toughness;
    
    public readonly List<Keywords> keywords;
    
    public readonly List<TriggerData> abilities;
    
    public CardData(String name, String description, String flavorText, int power, int toughness, List<TriggerData> abilities = null, List<Keywords>  keywords = null, Sprite art = null)
    {
        this.art = art;
        this.name = name;
        this.description = description;
        this.flavorText = flavorText;
        this.power = power;
        this.toughness = toughness;
        this.abilities = abilities;
        this.keywords = keywords;
    }
}

public class Card
{
    public CardPlayer owner;
    public CardZone zone;
    public CardData cardData;
    TriggerEventSystem triggerEventSystem;
    
    //base state
    public readonly Sprite art;
    public readonly String name;
    public readonly String description;
    public readonly String flavorText;
    public readonly int power;
    public readonly int toughness;
    public readonly List<Keywords> keywords;
    public readonly List<Trigger> abilities;
    
    //state
    bool tapped = false;
    public bool canAttack  = false;
    int damage = 0;
    bool declaredAttacker = false;

    public Card(CardData cardData, CardPlayer owner)
    {
        this.owner = owner;
        
        this.cardData = cardData;
        
        name = cardData.name;
        description = cardData.description;
        flavorText = cardData.flavorText;
        power = cardData.power;
        toughness = cardData.toughness;
        keywords = cardData.keywords;
        this.art = cardData.art;
        
        abilities = new List<Trigger>();
        foreach(TriggerData data in cardData.abilities)
        {
            abilities.Add(new Trigger(data, new EffectSource(this)));
        }
    }
    
    public void Tap()
    {
        tapped = true;
        triggerEventSystem?.TriggerEvent("CardTap", new EffectSource(this));
    }

    public void Untap()
    {
        tapped = false;
        triggerEventSystem?.TriggerEvent("CardUntap", new EffectSource(this));
    }

    public void Endstep()
    {
        Heal();
        canAttack = true;
    }

    public void Heal()
    {
        damage = 0;
    }

    public void TakeDamage(int damage)
    {
        this.damage += damage;
        if (this.damage > this.toughness)
        {
            Die();
        }
    }

    public void Die()
    {
        CardZone.MoveCard(this, owner.graveyard);
        triggerEventSystem.TriggerEvent("CardDie", new EffectSource(this));
    }

    public void DeclareAttacker()
    {
        declaredAttacker = true;
        triggerEventSystem?.TriggerEvent("DeclaredAttacker", new EffectSource(this));
        Tap();
    }

    public void RegisterTriggers(TriggerEventSystem triggerEventSystem)
    {
        this.triggerEventSystem =  triggerEventSystem;
        foreach(Trigger trigger in abilities) trigger.AddTrigger(triggerEventSystem);
    }

    public void UnregisterTriggers(TriggerEventSystem triggerEventSystem)
    {
        this.triggerEventSystem = null;
        foreach(Trigger trigger in abilities) trigger.RemoveTrigger(triggerEventSystem);
    }
}
