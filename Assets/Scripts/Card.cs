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

public class CardState
{
    public bool tapped { get; private set; } = false;
    public bool summoningSick { get; private set; } = true;
    public bool declaredAttacker { get; private set; } = false;
    public int damage { get; private set; } = 0;
    public Card blockingTarget { get; private set; } = null;
    public CardZone zone { get; private set; }
    public CardPlayer owner { get; private set; }

    public CardState(CardPlayer owner, CardZone zone)
    {
        this.owner = owner;
        this.zone = zone;
    }
    
    public void Tap()
    {
        tapped = true;
    }

    public void Untap()
    {
        tapped = false;
    }

    public void Endstep()
    {
        damage = 0;
        summoningSick = false;
    }

    public void Heal()
    {
        damage = 0;
    }

    public void DeclareAttacker()
    {
        declaredAttacker = true;
    }

    public void DeclareBlocker(Card card)
    {
        blockingTarget = card;
    }
    
    public void EndCombat()
    {
        blockingTarget = null;
        declaredAttacker = false;
    }

    public void ChangeZone(CardZone newZone)
    {
        zone = newZone;
    }

    public void TakeDamage(int damage)
    {
        this.damage += damage;
    }
}

public class Card
{
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
    public CardState state;
    
    public Card(CardData cardData, CardPlayer owner, CardZone zone)
    {
        state = new CardState(owner, zone);
        
        this.cardData = cardData;
        
        name = cardData.name;
        description = cardData.description;
        flavorText = cardData.flavorText;
        power = cardData.power;
        toughness = cardData.toughness;
        keywords = cardData.keywords;
        art = cardData.art;
        
        abilities = new List<Trigger>();
        foreach(TriggerData data in cardData.abilities)
        {
            abilities.Add(new Trigger(data, new EffectSource(this)));
        }
    }
    
    public void Tap()
    {
        state.Tap();
        triggerEventSystem?.TriggerEvent("CardTap", new EffectSource(this));
    }

    public void Untap()
    {
        state.Untap();
        triggerEventSystem?.TriggerEvent("CardUntap", new EffectSource(this));
    }

    public void Endstep()
    {
        state.Endstep();
    }

    public void Heal()
    {
        state.Heal();
    }

    public void TakeDamage(int damage)
    {
        state.TakeDamage(damage);
    }

    public void DeclareAttacker()
    {
        state.DeclareAttacker();
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
