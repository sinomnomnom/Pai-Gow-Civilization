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
    abstract public List<Player> Resolve(Card card);
}

public class Opponent : PlayerSelector
{
    public override List<Player> Resolve(Card card)
    {
        return new List<Player>() {card.owner.opponent};
    }
}

public class Owner : PlayerSelector
{
    public override List<Player> Resolve(Card card)
    {
        return new List<Player>(){card.owner};
    }
}

public abstract class Ability
{
    abstract public void Activate(Card card);
}

public abstract class Condition
{
    abstract public bool Activate(Card card);
}

public class Choice : Ability
{
    private String question;
    private String option1;
    private String option2;

    private Ability ability1;
    Ability ability2;

    public Choice(String question, String option1, String option2, Ability ability1, Ability ability2)
    {
        this.question = question;
        this.option1 = option1;
        this.option2 = option2;
        this.ability1 = ability1;
        this.ability2 = ability2;
    }

    public override void Activate(Card card)
    {
        card.owner.Choose(question, option1, option2, ability1, ability2, card).Activate(card);
    }
}

public class ChangeLife : Ability
{
    int delta;
    Player player;

    public ChangeLife(Player player, int delta)
    {
        this.player = player;
        this.delta = delta;
    }

    public override void Activate(Card card)
    {
        player.ChangeLife(delta);
    }
}

public class Tap : Ability
{
    public Card cardToTap;

    public Tap(Card cardToTap)
    {
        this.cardToTap = cardToTap;
    }
    public override void Activate(Card card)
    {
        cardToTap.Tap();
    }
}

public class Not : Condition
{
    Condition test;
    public Not(Condition test)
    {this.test = test;}

    public override bool Activate(Card card)
    {
        return !test.Activate(card);
    }
}

public class InHand : Condition
{
    Card card;
    Player player;

    public InHand(Card card, Player player)
    {
        this.card = card;
        this.player = player;
    }

    public override bool Activate(Card card)
    {
        return player.IsCardInHand(card);
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
    public Player owner;
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

    public Card(CardData cardData, Player owner)
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
