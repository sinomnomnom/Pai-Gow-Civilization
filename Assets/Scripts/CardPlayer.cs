using System.Collections.Generic;
using UnityEngine;

public class Player
{
    int lifeTotal;
    List<Card> hand;
    public Player opponent;
    
    public static void SetOpponents(Player a, Player b)
    {
        a.opponent = b;
        b.opponent = a;
    }
    
    public void ChangeLife(int delta)
    {
        lifeTotal += delta;
    }

    public bool IsCardInHand(Card card)
    {
        return hand.Contains(card);
    }
    
    public void AddToHand(Card card)
    {
        hand.Add(card);
    }

    public void AddToHand(List<Card> cards)
    {
        hand.AddRange(cards);
    }

    public Card ChooseCardInPlay()
    {
        
    }
}
