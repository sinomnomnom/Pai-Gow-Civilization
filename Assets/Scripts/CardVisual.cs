using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    public Card card;

    public Image art;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDescription;
    public TextMeshProUGUI stats;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CardPlayer player = new CardPlayer(new CardPlayerData(new List<CardData>(),"chud_player"));
        TriggerData UpkeepGain3life = new TriggerData("Upkeep",
            new Conditional(new InZone(new ThisCard(), CardZone.ZoneType.INPLAY), new ChangeLife(new Owner(), 3)));
        CardData chuddyCard = new CardData("chuddy","this card is a basic 1/1 chud","oops, I am a chud - chud", 1,1,new List<TriggerData>(){UpkeepGain3life});
        card = new Card(chuddyCard,player);
        SetStats();
    }

    void SetCard(Card card)
    {
        this.card = card;
    }

    public void SetStats()
    {
        cardName.text = card.name;
        cardDescription.text = card.description;
        stats.text = card.power + "/" + card.toughness + "";
        art.sprite = card.art;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
