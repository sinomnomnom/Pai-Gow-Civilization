using System;
using System.Collections.Generic;
using UnityEngine;

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
            Debug.Log(eventName+ " triggered with 0 listeners");
        }
    }
    
    public void TriggerEvent(String eventName, EffectSource source)
    {
        if (triggers.TryGetValue(eventName, out var list))
        {
            Debug.Log(eventName+ " triggered");
            foreach (Trigger trigger in new List<Trigger>(list))
            {
                if(trigger!=null) trigger.Activate(source);
            }
        }
        else
        {
            Debug.Log(eventName+ " triggered with 0 listeners");
        }
    }
}

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
