using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

public class AnyCardInMyHand : CardSelector
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