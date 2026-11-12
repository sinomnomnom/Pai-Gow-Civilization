using System;
using System.Collections.Generic;
using UnityEngine;

public class CardGameManager
{
    private CardPlayer player1;
    private CardPlayer player2;
    
    int player1Score = 0;
    int player2Score = 0;
    
    CardPlayer firstPlayer;
    
    public TriggerEventSystem triggerEventSystem =  new TriggerEventSystem();
    
    private int totalRounds = 3;
    private int numDecks = 3;
    private int cardsPerDeck = 4;

    public CardGameManager(CardPlayer player1, CardPlayer player2)
    {
        this.player1 = player1;
        this.player2 = player2;
        
        CardPlayer.SetOpponents(player1, player2);
        StartGame();
    }

    void StartGame()
    {
        Debug.Log("Starting game");
        
        //TODO: randomly choose starting player
        firstPlayer = player1;
        Debug.Log("First player chosen");
        
        Queue<List<Card>> player1Decks =  player1.CreateDecks(numDecks, cardsPerDeck);
        Queue<List<Card>> player2Decks =  player2.CreateDecks(numDecks, cardsPerDeck);
        
        for (int i = 0; i < totalRounds; i++)
        {
            //TODO: choose active decks
            List<Card> player1Deck = player1Decks.Dequeue();
            List<Card> player2Deck = player2Decks.Dequeue();
            
            Debug.Log("Starting Round " + (i+1));
            RoundManager roundManager = new RoundManager(player1, player2, firstPlayer, player1Deck, player2Deck, triggerEventSystem);
            CardPlayer winner = roundManager.StartRound();
            if (winner == player1)
            {
                player1Score += 1;
            }
            if (winner == player2)
            {
                player2Score += 1;
            }
            Debug.Log("Round "+(i+1)+" Winner " + winner.name);
            firstPlayer = winner.opponent;
            if (player1Score > totalRounds / 2)
            {
                break;
            }
            if (player2Score > totalRounds / 2)
            {
                break;
            }
        }
        Debug.Log("Game over");
        Debug.Log(player1.name+" Score: " + player1Score);
        Debug.Log(player2.name+" Score: " + player2Score);
    }
}

public class RoundManager
{
    CardPlayer player1;
    CardPlayer player2;
    CardPlayer activePlayer;
    
    List<Card> player1Deck;
    List<Card> player2Deck;
    
    TriggerEventSystem triggerEventSystem;

    public RoundManager(CardPlayer player1, CardPlayer player2, CardPlayer activePlayer, List<Card> player1Deck, List <Card> player2Deck, TriggerEventSystem triggerEventSystem)
    {
        this.player1 = player1;
        this.player2 = player2;
        this.activePlayer = activePlayer;
        this.player1Deck = player1Deck;
        this.player2Deck = player2Deck;
        this.triggerEventSystem = triggerEventSystem;
    }

    public CardPlayer StartRound()
    {
        foreach(Card card in player1Deck) card.RegisterTriggers(triggerEventSystem);
        foreach(Card card in player2Deck) card.RegisterTriggers(triggerEventSystem);
        player1.StartNewRound();
        player2.StartNewRound();
        player1.hand.AddCard(player1Deck);
        player2.hand.AddCard(player2Deck);
        triggerEventSystem.TriggerEvent("RoundStart");
        while (true)
        {
            triggerEventSystem.TriggerEvent("TurnStart");
            /*
            Untap();
            Upkeep();
            PreCombat();
            DeclareAttackers();
            DeclareBlockers();
            Damage();
            PostCombat();
            Endstep();
            
            activePlayer = activePlayer.opponent;
            */
            triggerEventSystem.TriggerEvent("TurnEnd");
            break;
        }
        CardPlayer winner = activePlayer;
        foreach(Card card in player1Deck) card.UnregisterTriggers(triggerEventSystem);
        foreach(Card card in player2Deck) card.UnregisterTriggers(triggerEventSystem);
        return winner;
    }
}

public class TriggerEventSystem
{
    Dictionary<String, List<Trigger>> triggers;
    
    public TriggerEventSystem()
    {
        triggers = new Dictionary<string, List<Trigger>>();
    }

    public void AddTrigger(String eventName, Trigger trigger)
    {
        if (!triggers.ContainsKey(eventName))
        {
            triggers.Add(eventName, new List<Trigger>());
        }
        triggers[eventName].Add(trigger);
    }

    public void RemoveTrigger(String eventName, Trigger trigger)
    {
        if (triggers.ContainsKey(eventName))
        {
            triggers[eventName].Remove(trigger);
        }
    }

    public void TriggerEvent(String eventName)
    {
        if (triggers.TryGetValue(eventName, out var list))
        {
            Debug.Log(eventName+ " triggered");
            foreach (Trigger trigger in new List<Trigger>(list))
            {
                if(trigger!=null) trigger.Activate();
            }
        }
        else
        {
            Debug.Log(eventName+ " triggered with no listeners");
        }
    }
}
