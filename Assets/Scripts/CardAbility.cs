using System;
using System.Collections.Generic;
using UnityEngine;

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