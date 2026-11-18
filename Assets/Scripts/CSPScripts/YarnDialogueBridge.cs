using UnityEngine;
using Yarn;
using Yarn.Unity;

public class YarnDialogueBridge : MonoBehaviour
{
    [SerializeField] private DialogueRunner dialogueRunner;

    private void OnEnable()
    {
        //listens for external requests for conversation start
        DialogueEvents.OnDialogueRequested += StartConversation;

        //forwards yarnspinners built in events to C# stuff
        dialogueRunner.onDialogueStart.AddListener(DialogueEvents.NotifyDialogueStart);
        dialogueRunner.onDialogueComplete.AddListener(DialogueEvents.NotifyDialogueEnd);
    }

    private void StartConversation(string nodeName)
    {
        Debug.Log(nodeName);
        if (!dialogueRunner.IsDialogueRunning) dialogueRunner.StartDialogue(nodeName);
    }

    [YarnCommand("start_quest")]
    public static void CommandStartQuest(string questID)
    {
        DialogueEvents.TriggerQuest(questID);
    }

    [YarnCommand("give_item")]
    public static void CommandGiveItem(string itemID, int amount)
    {
        DialogueEvents.GiveItem(itemID, amount);
    }
}
