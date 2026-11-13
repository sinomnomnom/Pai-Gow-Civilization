using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;

public class CardPlayer
{
    //player stats
    public readonly String name;
    private int startingLifeTotal = 5;
    List<CardData> collection = new List<CardData>();
    
    int lifeTotal;
    
    public CardZone hand;
    public CardZone inPlay;
    public CardZone graveyard;
    public CardPlayer opponent;
    
    public TriggerEventSystem triggerEventSystem;

    public CardPlayer(CardPlayerData data)
    {
        collection = data.collection;
        name = data.name;
        startingLifeTotal = data.startingLifeTotal;
    }

    public Queue<List<Card>> CreateRandomDecks(int numDecks, int cardsPerDeck)
    {
        if (numDecks * cardsPerDeck > collection.Count) Debug.Log("not enough cards to make decks!");
        
        List<CardData> shuffled = new List<CardData>(collection);
        shuffled = shuffled.OrderBy(i => Guid.NewGuid()).ToList();
        
        Queue<List<Card>> decks = new Queue<List<Card>>();
        for (int i = 0; i < numDecks; i++)
        {
            List<Card> deck = new List<Card>(cardsPerDeck);
            for (int j = 0; j < cardsPerDeck; j++)
            {
                deck.Add(new Card(shuffled[i * cardsPerDeck + j], this));
            }
            decks.Enqueue(deck);
        }
        return decks;
    }

    public Queue<List<Card>> CreateDecks(int numDecks, int cardsPerDeck)
    {
        //TODO: make real choices
        return CreateRandomDecks(numDecks, cardsPerDeck);
    }
    
    public static void SetOpponents(CardPlayer a, CardPlayer b)
    {
        a.opponent = b;
        b.opponent = a;
    }

    public void StartNewRound(TriggerEventSystem triggerEventSystem)
    {
        lifeTotal = startingLifeTotal;
        hand = new CardZone(CardZone.ZoneType.HAND, triggerEventSystem);
        inPlay = new CardZone(CardZone.ZoneType.INPLAY, triggerEventSystem);
        graveyard = new CardZone(CardZone.ZoneType.GRAVEYARD, triggerEventSystem);
        this.triggerEventSystem = triggerEventSystem;
    }

    public void Untap()
    {
        foreach (Card card in inPlay.cards)
        {
            card.Untap();
        }
    }
    
    public void ChangeLife(int delta)
    {
        lifeTotal += delta;
        if(lifeTotal <= 0)  triggerEventSystem.TriggerEvent("PlayerDeath", new EffectSource(this));
        else
        {
            if (delta > 0) triggerEventSystem.TriggerEvent("GainLife", new EffectSource(this));
            if (delta < 0) triggerEventSystem.TriggerEvent("LoseLife", new EffectSource(this));
        }
        Debug.Log("new lifeTotal: "  + lifeTotal);
    }

    public void PlayCard(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            triggerEventSystem.TriggerEvent("PlayCard", new EffectSource(this));
            CardZone.MoveCard(card,inPlay);
        }
    }

    public Ability Choose(String question, List<String> options, List<Ability> results, EffectSource source)
    {
        //TODO : let players actually choose
        return results[0];
    }

    public Card ChooseCardInPlay()
    {
        //todo: real choice
        List<Card> options = inPlay.cards.Concat(opponent.inPlay.cards).ToList();
        if (options.Count<1) return null;
        return options[0];
    }
    
    public Card ChooseCardInHand()
    {
        //todo: real choice
        List<Card> options = hand.cards;
        if (options.Count<1) return null;
        return options[0];
    }

    public List<Card> ChooseCardsFromList(List<Card> cards)
    {
        //todo: real choice
        return cards;
    }
}

public class CardPlayerData
{
    public List<CardData> collection =  new List<CardData>();
    public String name;
    public int startingLifeTotal;

    public CardPlayerData(List<CardData> collection, String name = "Player",  int startingLifeTotal = 5)
    {
        this.collection = collection;
        this.name = name;
        this.startingLifeTotal = startingLifeTotal;
    }
}
