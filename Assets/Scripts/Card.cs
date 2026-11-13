using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EffectSource
{
    public Card card;
    public CardPlayer player;
    public CardZone from, to;
    public RoundManager roundManager;
    public Card triggeringOn;

    public EffectSource(Card card)
    {
        this.card = card;
    }

    public EffectSource(Card card, CardZone to)
    {
        this.card = card;
        this.to = to;
    }
    public EffectSource(CardZone from, Card card)
    {
        this.card = card;
        this.from = from;
    }

    public EffectSource(CardPlayer player)
    {
        this.player = player;
    }

    public EffectSource(RoundManager roundManager)
    {
        this.roundManager = roundManager;
    }

    public CardPlayer GetPlayer()
    {
        return player ?? card?.owner ?? roundManager?.activePlayer;
    }

}

public class Trigger
{
    private EffectSource source;
    TriggerData data;
    
    public Trigger(TriggerData triggerData, EffectSource source)
    {
        this.source = source;
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

    public void Activate(EffectSource source)
    {
        source.triggeringOn = this.source.card;
        data.ability.Activate(source);
    }
}

public abstract class CardSelector
{
    abstract public List<Card> Resolve(EffectSource source);
}

public class ThisCard : CardSelector
{
    public override List<Card> Resolve(EffectSource source)
    {
        if (source.triggeringOn != null)
        {
            return new List<Card>() { source.triggeringOn };
        }
        if (source.card !=null)
        {
            return new List<Card>() { source.card };
        }
        else
        {
            Debug.Log("ThisCard selector effect did not come from a card");
            return new List<Card>();
        }
        
    }
}

public class AnyCardInPlay : CardSelector
{
    public override List<Card> Resolve(EffectSource source)
    {
        CardPlayer player = source.GetPlayer();

        if (player == null)
        {
            Debug.Log("no valid player");
            return new List<Card>();
        }

        return new List<Card> { player.ChooseCardInPlay() };
    }
}

public class AllCardsInPlay : CardSelector
{
    public override List<Card> Resolve(EffectSource source)
    {
        CardPlayer player = source.GetPlayer();

        if (player == null)
        {
            Debug.Log("no valid player");
            return new List<Card>();
        }

        return player.inPlay.cards.Concat(player.opponent.inPlay.cards).ToList();
    }
}

public class AllMyCardsInPlay : CardSelector
{
    public override List<Card> Resolve(EffectSource source)
    {
        CardPlayer player = source.GetPlayer();

        if (player == null)
        {
            Debug.Log("no valid player");
            return new List<Card>();
        }

        return player.inPlay.cards;
    }
}

public class AnyCardInHand : CardSelector
{
    public override List<Card> Resolve(EffectSource source)
    {
        CardPlayer player = source.GetPlayer();

        if (player == null)
        {
            Debug.Log("no valid player");
            return new List<Card>();
        }

        return new List<Card> { player.ChooseCardInHand() };
    }
}

public class ChooseAttackers : CardSelector
{
    public override List<Card> Resolve(EffectSource source)
    {
        CardPlayer player = source.GetPlayer();

        if (player == null)
        {
            Debug.Log("Selector effect has no valid player");
            return new List<Card>();
        }
        List<Card> options = new List<Card>();
        foreach (Card card in player.inPlay.cards)
        {
            if (card.canAttack) options.Add(card);
        }
        return player.ChooseCardsFromList(options);
    }
}

public abstract class PlayerSelector
{
    abstract public List<CardPlayer> Resolve(EffectSource source);
}

public class Opponent : PlayerSelector
{
    public override List<CardPlayer> Resolve(EffectSource source)
    {
        return new List<CardPlayer>() {source.GetPlayer().opponent};
    }
}

public class Owner : PlayerSelector
{
    public override List<CardPlayer> Resolve(EffectSource source)
    {
        return new List<CardPlayer>(){source.GetPlayer()};
    }
}

public abstract class Ability
{
    abstract public void Activate(EffectSource source);
}

public abstract class Condition
{
    abstract public bool Activate(EffectSource source);
}

public class Choice : Ability
{
    private String question;
    private List<String> options;
    private List<Ability> results;

    public Choice(String question, List<String> options, List<Ability> results)
    {
        this.question = question;
        this.options = options;
        this.results = results;
    }

    public override void Activate(EffectSource source)
    {
        source.GetPlayer().Choose(question, options, results, source).Activate(source);
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

    public override void Activate(EffectSource source)
    {
        foreach (CardPlayer player in selector.Resolve(source))
        {
            player.ChangeLife(delta);
        }
    }
}

public class PlayCard : Ability
{
    CardSelector selector;
    public PlayCard(CardSelector selector)
    {
        this.selector = selector;
    }

    public override void Activate(EffectSource source)
    {
        source.GetPlayer().PlayCard(selector.Resolve(source));
    }
}

public class Sequence : Ability
{
    List<Ability> abilities;
    public Sequence(List<Ability> abilities)
    {
        this.abilities = abilities;
    }

    public override void Activate(EffectSource source)
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
    public override void Activate(EffectSource source)
    {
        foreach (Card card in selector.Resolve(source))
        {
            card.Tap();
        }
    }
}

public class DeclareAttackers : Ability
{
    CardSelector selector;
    public DeclareAttackers(CardSelector selector)
    {
        this.selector = selector;
    }

    public override void Activate(EffectSource source)
    {
        foreach (Card card in selector.Resolve(source))
        {
            card.DeclareAttacker();
        }
    }
}

public class EndstepReset : Ability
{
    CardSelector selector;
    public EndstepReset(CardSelector selector)
    {
        this.selector = selector;
    }

    public override void Activate(EffectSource source)
    {
        foreach (Card card in selector.Resolve(source))
        {
            card.Endstep();
        }
    }
}

public class Heal : Ability
{
    CardSelector selector;
    public Heal(CardSelector selector)
    {
        this.selector = selector;
    }

    public override void Activate(EffectSource source)
    {
        foreach (Card card in selector.Resolve(source))
        {
            card.Heal();
        }
    }
}

public class Untap : Ability
{
    CardSelector selector;

    public Untap(CardSelector selector)
    {
        this.selector = selector;
    }
    public override void Activate(EffectSource source)
    {
        foreach (Card card in selector.Resolve(source))
        {
            card.Untap();
        }
    }
}

public class Not : Condition
{
    Condition test;
    public Not(Condition test)
    {this.test = test;}

    public override bool Activate(EffectSource source)
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

    public override bool Activate(EffectSource source)
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

public class OwnerIsActivePlayer : Condition
{
    CardSelector cardSelector;

    public OwnerIsActivePlayer(CardSelector cardSelector)
    {
        this.cardSelector = cardSelector;
    }

    public override bool Activate(EffectSource source)
    {
        foreach (Card card in cardSelector.Resolve(source))
        {
            if (card.owner == source.GetPlayer())
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
    
    public override void Activate(EffectSource card)
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
