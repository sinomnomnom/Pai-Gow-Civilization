using System;
using System.Collections.Generic;
using UnityEngine;

public class Trigger
{
    private Card source;
    TriggerData data;
    
    public Trigger(TriggerData triggerData, Card card)
    {
        this.source = card;
        this.data = triggerData;
    }

    public void AddTrigger(TriggerEventSystem triggerEventSystem)
    {
        triggerEventSystem.AddTrigger(data.eventName, this);
    }

    public void RemoveTrigger(TriggerEventSystem triggerEventSystem)
    {
        triggerEventSystem.RemoveTrigger(data.eventName, this);
    }

    public void Activate()
    {
        data.ability.Activate(source);
    }
}

public abstract class CardSelector
{
    abstract public List<Card> Resolve(Card card);
}

public class ThisCard : CardSelector
{
    public override List<Card> Resolve(Card card)
    {
        return new List<Card>() { card };
    }
}

public class AnyCardInPlay : CardSelector
{
    public override List<Card> Resolve(Card card)
    {
        return new List<Card>() { card.owner.ChooseCardInPlay() };
    }
}

public abstract class PlayerSelector
{
    abstract public List<CardPlayer> Resolve(Card source);
}

public class Opponent : PlayerSelector
{
    public override List<CardPlayer> Resolve(Card source)
    {
        return new List<CardPlayer>() {source.owner.opponent};
    }
}

public class Owner : PlayerSelector
{
    public override List<CardPlayer> Resolve(Card source)
    {
        return new List<CardPlayer>(){source.owner};
    }
}

public abstract class Ability
{
    abstract public void Activate(Card source);
}

public abstract class Condition
{
    abstract public bool Activate(Card source);
}

public class Choice : Ability
{
    private String question;
    private List<String> options;
    private List<Ability> results;

    public Choice(String question, List<String> options, List<Ability> results, Card source)
    {
        this.question = question;
        this.options = options;
        this.results = results;
    }

    public override void Activate(Card card)
    {
        card.owner.Choose(question, options, results, card).Activate(card);
    }
}

public class ChangeLife : Ability
{
    int delta;
    PlayerSelector selector;

    public ChangeLife(PlayerSelector selector, int delta)
    {
        this.delta = delta;
        this.selector = selector;
    }

    public override void Activate(Card source)
    {
        foreach (CardPlayer player in selector.Resolve(source))
        {
            player.ChangeLife(delta);
        }
    }
}

public class Sequence : Ability
{
    List<Ability> abilities;
    public Sequence(List<Ability> abilities)
    {
        this.abilities = abilities;
    }

    public override void Activate(Card source)
    {
        foreach (Ability ability in abilities)
        {
            ability.Activate(source);
        }
    }
}

public class Tap : Ability
{
    CardSelector selector;

    public Tap(CardSelector selector)
    {
        this.selector = selector;
    }
    public override void Activate(Card source)
    {
        foreach (Card card in selector.Resolve(source))
        {
            card.Tap();
        }
    }
}

public class Not : Condition
{
    Condition test;
    public Not(Condition test)
    {this.test = test;}

    public override bool Activate(Card source)
    {
        return !test.Activate(source);
    }
}

public class InZone : Condition
{
    CardSelector cardSelector;
    CardZone.ZoneType zoneType;

    public InZone(CardSelector cardSelector, CardZone.ZoneType zoneType)
    {
        this.cardSelector = cardSelector;
        this.zoneType = zoneType;
    }

    public override bool Activate(Card source)
    {
        foreach (Card card in cardSelector.Resolve(source))
        {
            if (card.zone.zoneType == this.zoneType)
            {
                return true;
            }
        }
        return false;
    }
}

public class Conditional : Ability
{
    Condition test;
    Ability ifTrue;

    public Conditional(Condition test, Ability ifTrue)
    {
        this.test = test;
        this.ifTrue = ifTrue;
    }
    
    public override void Activate(Card card)
    {
        if (test.Activate(card))
        {
            ifTrue.Activate(card);
        }
    }
}

public class TriggerData
{
    public readonly string eventName;
    public readonly Ability ability;
    public TriggerData(string eventName, Ability ability)
    {
        this.eventName = eventName;
        this.ability = ability;
    }
}

public class CardData
{
    public readonly String name;
    public readonly String description;
    public readonly String flavorText;
    public readonly int power;
    public readonly int toughness;
    
    public readonly List<TriggerData> abilities;
    
    public CardData(String name, String description, String flavorText, int power, int toughness, List<TriggerData> abilities)
    {
        this.name = name;
        this.description = description;
        this.flavorText = flavorText;
        this.power = power;
        this.toughness = toughness;
        this.abilities = abilities;
    }
}

public class Card
{
    public CardPlayer owner;
    public CardZone zone;
    CardData cardData;
    
    //base state
    String name;
    String description;
    String flavorText;
    int power;
    int toughness;
    
    List<Trigger> abilities;
    
    //state
    bool tapped = false;
    bool canAttack  = false;
    int damage = 0;

    public Card(CardData cardData, CardPlayer owner)
    {
        this.owner = owner;
        
        this.cardData = cardData;
        
        name = cardData.name;
        description = cardData.description;
        flavorText = cardData.flavorText;
        power = cardData.power;
        toughness = cardData.toughness;
        
        abilities = new List<Trigger>();
        foreach(TriggerData data in cardData.abilities)
        {
            abilities.Add(new Trigger(data, this));
        }
    }
    
    public void Tap()
    {
        tapped = true;
    }

    public void Untap()
    {
        tapped = false;
    }

    public void RegisterTriggers(TriggerEventSystem triggerEventSystem)
    {
        foreach(Trigger trigger in abilities) trigger.AddTrigger(triggerEventSystem);
    }

    public void UnregisterTriggers(TriggerEventSystem triggerEventSystem)
    {
        foreach(Trigger trigger in abilities) trigger.RemoveTrigger(triggerEventSystem);
    }
}
