using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class CardSystemTest : MonoBehaviour
{
    public CardGameManager cardGameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TriggerData UpkeepGain3life = new TriggerData("Upkeep",
            new Conditional(new InZone(new ThisCard(), CardZone.ZoneType.INPLAY),new Conditional(new OwnerIsActivePlayer(new ThisCard()),new ChangeLife(new Owner(), 3))));
        CardData chuddyCard = new CardData("chuddy","this card is a basic 1/1 chud","oops, I am a chud - chud", 1,1,new List<TriggerData>(){UpkeepGain3life});
        List<CardData> collection = new List<CardData>();
        for (int i = 0; i < 12; i++)
        {
            collection.Add(chuddyCard);
        }
        CardPlayerData playerData = new CardPlayerData(collection, "chuddy1");
        CardPlayerData playerData2 = new CardPlayerData(collection, "chuddy2");
        CardPlayer player1 = new CardPlayer(playerData);
        CardPlayer player2 = new CardPlayer(playerData2);
        cardGameManager = new CardGameManager(player1, player2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
