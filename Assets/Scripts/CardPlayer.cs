using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public async Task<Queue<List<CardData>>> CreateRandomDecks(int numDecks, int cardsPerDeck)
    {
        if (numDecks * cardsPerDeck > collection.Count) Debug.Log("not enough cards to make decks!");
        
        List<CardData> shuffled = new List<CardData>(collection);
        shuffled = shuffled.OrderBy(i => Guid.NewGuid()).ToList();
        
        Queue<List<CardData>> decks = new Queue<List<CardData>>();
        for (int i = 0; i < numDecks; i++)
        {
            List<CardData> deck = new List<CardData>(cardsPerDeck);
            for (int j = 0; j < cardsPerDeck; j++)
            {
                deck.Add(shuffled[i * cardsPerDeck + j]);
            }
            decks.Enqueue(deck);
        }
        await Task.Delay(3000);
        return decks;
    }

    public async Task<Queue<List<CardData>>> CreateDecks(int numDecks, int cardsPerDeck)
    {
        //TODO: make real choices
        return await CreateRandomDecks(numDecks, cardsPerDeck);
    }
    
    public static void SetOpponents(CardPlayer a, CardPlayer b)
    {
        a.opponent = b;
        b.opponent = a;
    }

    public void StartNewRound(TriggerEventSystem triggerEventSystem)
    {
        lifeTotal = startingLifeTotal;
        this.triggerEventSystem = triggerEventSystem;
    }

    public void Untap()
    {
        foreach (Card card in inPlay.cards)
        {
            card.Untap();
        }
    }
    
    public async Task ChangeLife(int delta)
    {
        lifeTotal += delta;
        if(lifeTotal <= 0)  await triggerEventSystem.TriggerEvent("PlayerDeath", new EffectSource(this));
        else
        {
            if (delta > 0) await triggerEventSystem.TriggerEvent("GainLife", new EffectSource(this));
            if (delta < 0) await triggerEventSystem.TriggerEvent("LoseLife", new EffectSource(this));
        }
        Debug.Log("new lifeTotal: "  + lifeTotal);
    }

    public async Task PlayCard(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            await triggerEventSystem.TriggerEvent("PlayCard", new EffectSource(this));
            CardZone.MoveCard(card,inPlay);
        }
    }

    public async Task<Ability> Choose(String question, List<String> options, List<Ability> results, EffectSource source)
    {
        //TODO : let players actually choose
        await Task.Delay(1000);
        return results[0];
    }

    public async Task<Card> ChooseCardInPlay()
    {
        //todo: real choice
        List<Card> options = inPlay.cards.Concat(opponent.inPlay.cards).ToList();
        await Task.Delay(1000);
        if (options.Count<1) return null;
        return options[0];
    }
    
    public async Task<Card> ChooseCardInHand()
    {
        //todo: real choice
        List<Card> options = hand.cards;
        await Task.Delay(1000);
        if (options.Count<1) return null;
        return options[0];
    }

    public async Task<List<Card>> ChooseCardsFromList(List<Card> cards)
    {
        //todo: real choice
        await Task.Delay(1000);
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
