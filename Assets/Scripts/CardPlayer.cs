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

    public void StartNewRound()
    {
        lifeTotal = startingLifeTotal;
        hand = new CardZone(CardZone.ZoneType.HAND);
        inPlay = new CardZone(CardZone.ZoneType.INPLAY);
        graveyard = new CardZone(CardZone.ZoneType.GRAVEYARD);
    }
    
    public void ChangeLife(int delta)
    {
        lifeTotal += delta;
    }

    public Ability Choose(String question, List<String> options, List<Ability> results, Card card)
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
