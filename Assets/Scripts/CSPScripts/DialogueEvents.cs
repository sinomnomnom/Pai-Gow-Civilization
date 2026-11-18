using System;
using System.Xml.Serialization;
using UnityEngine;

public static class DialogueEvents
{

    public static bool IsDialogueActive {get; private set;}

    //lifecycle delegates
    public static event Action<string> OnDialogueRequested;
    public static event Action OnDialogueStart;
    public static event Action OnDialogueEnd;

    //gameplay command delegates (triggered from yarn)
    public static event Action<string> OnQuestStart;
    public static event Action<string, int> OnItemGiven;

    //Invokers
    public static void RequestDialogue(string nodeName) => OnDialogueRequested?.Invoke(nodeName);
    public static void NotifyDialogueStart() 
    {
        IsDialogueActive = true;
        OnDialogueStart?.Invoke();
    }
    public static void NotifyDialogueEnd() 
    {
        IsDialogueActive = false;
        OnDialogueEnd?.Invoke();
    }
    public static void TriggerQuest(string questID) => OnQuestStart?.Invoke(questID);
    public static void GiveItem(string itemID, int amount) => OnItemGiven?.Invoke(itemID, amount);
}
