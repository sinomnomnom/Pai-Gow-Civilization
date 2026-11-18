using System;
using UnityEngine;

public static class DialogueEvents
{
    //lifecycle delegates
    public static event Action<string> OnDialogueRequested;
    public static event Action OnDialogueStart;
    public static event Action OnDialogueEnd;

    //gameplay command delegates (triggered from yarn)
    public static event Action<string> OnQuestStart;
    public static event Action<string, int> OnItemGiven;

    //Invokers
    public static void RequestDialogue(string nodeName) => OnDialogueRequested?.Invoke(nodeName);
    public static void NotifyDialogueStart() => OnDialogueStart?.Invoke();
    public static void NotifyDialogueEnd() => OnDialogueEnd?.Invoke();
    public static void TriggerQuest(string questID) => OnQuestStart?.Invoke(questID);
    public static void GiveItem(string itemID, int amount) => OnItemGiven?.Invoke(itemID, amount);
}
