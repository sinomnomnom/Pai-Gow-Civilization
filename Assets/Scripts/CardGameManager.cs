using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    private Player player1;
    private Player player2;
    
    Player firstPlayer;
    
    public TriggerEventSystem triggerEventSystem =  new TriggerEventSystem();
    
    int round;
    private int totalRounds;

    public GameManager(Player player1, Player player2)
    {
        this.player1 = player1;
        this.player2 = player2;
        
        Player.SetOpponents(player1, player2);
    }

    void StartGame()
    {
        Debug.Log("Starting game");
        //TODO: randomly choose starting player
        
        firstPlayer = player1;
        Debug.Log("First player chosen");
        
        //TODO: make decks
        Queue<List<Card>> player1Decks =  new Queue<List<Card>>();
        Queue<List<Card>> player2Decks =  new Queue<List<Card>>();
        
        for (int i = 0; i < totalRounds; i++)
        {
            //TODO: choose active decks
            List<Card> player1Deck = player1Decks.Dequeue();
            List<Card> player2Deck = player2Decks.Dequeue();
            
            Debug.Log("Starting Round " + i+1);
            RoundManager roundManager = new RoundManager(player1, player2, firstPlayer, player1Deck, player2Deck, triggerEventSystem);
            Player winner = roundManager.StartRound();
            Debug.Log("Round "+i+1+" Winner " + winner);
            firstPlayer = winner.opponent;
        }
    }
}

public class RoundManager
{
    Player player1;
    Player player2;
    Player activePlayer;
    
    List<Card> player1Deck;
    List<Card> player2Deck;
    
    TriggerEventSystem triggerEventSystem;

    public RoundManager(Player player1, Player player2, Player activePlayer, List<Card> player1Deck, List <Card> player2Deck, TriggerEventSystem triggerEventSystem)
    {
        this.player1 = player1;
        this.player2 = player2;
        this.activePlayer = activePlayer;
        this.player1Deck = player1Deck;
        this.player2Deck = player2Deck;
        this.triggerEventSystem = triggerEventSystem;
    }

    public Player StartRound()
    {
        foreach(Card card in player1Deck) card.RegisterTriggers(triggerEventSystem);
        foreach(Card card in player2Deck) card.RegisterTriggers(triggerEventSystem);
        player1.AddToHand(player1Deck);
        player2.AddToHand(player2Deck);
        triggerEventSystem.TriggerEvent("RoundStart");
        while (true)
        {
            triggerEventSystem.TriggerEvent("TurnStart");
            //activePlayer.Start turn :)
            //activePlayer = activePlayer.opponent
            //TODO: game logic
            break;
        }
        Player winner = player1;
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
            foreach (Trigger trigger in new List<Trigger>(list))
            {
                if(trigger!=null) trigger.Activate();
            }
        }
    }
}
