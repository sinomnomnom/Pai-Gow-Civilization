using System.Collections.Generic;
using UnityEngine;

public class CardZone
{
    public enum ZoneType {HAND, GRAVEYARD, INPLAY}
    public CardZoneVisual visual;
    public ZoneType zoneType;
    public readonly List<Card> cards = new List<Card>();
    private TriggerEventSystem  triggerEventSystem;

    public CardZone(ZoneType zoneType, TriggerEventSystem triggerEventSystem)
    {
        this.zoneType =  zoneType;
        this.triggerEventSystem = triggerEventSystem;
    }

    public CardZone(ZoneType zoneType, TriggerEventSystem triggerEventSystem, CardZoneVisual visual)
    {
        this.zoneType =  zoneType;
        this.triggerEventSystem = triggerEventSystem;
        this.visual = visual;
    }

    public static void MoveCard(Card card, CardZone to)
    {
        card.state.zone.RemoveCard(card);
        to.AddCard(card);
    }

    public bool HasCard(Card card)
    {
        return cards.Contains(card);
    }

    public void AddCard(Card card)
    {
        card.state.ChangeZone(this);
        cards.Add(card);
        triggerEventSystem.TriggerEvent("CardEntered", new EffectSource(card, to: this));
    }

    public void AddCard(List<Card> card)
    {
        this.cards.AddRange(card);
        foreach (Card newCard in card)
        {
            newCard.state.ChangeZone(this);
        }
    }

    public void RemoveCard(Card card)
    {
        cards.Remove(card);
        triggerEventSystem.TriggerEvent("CardExited", new EffectSource(from: this, card));
    }
}
