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

    public Vector3 basePos;
    public Quaternion baseRot;
    
    public CardZone currentZone;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetCard(Card card)
    {
        this.card = card;
        SetStats();
        UpdateZone();
    }

    void SetStats()
    {
        cardName.text = card.name;
        cardDescription.text = card.description;
        stats.text = card.power + "/" + card.toughness + "";
        art.sprite = card.art;
    }
    
    void UpdateZone()
    {
        CardZone newZone = card?.state.zone;
        if (newZone == currentZone) return;

        if (currentZone != null && currentZone.visual != null)
            currentZone.visual.RemoveCard(this);

        currentZone = newZone;

        if (currentZone != null && currentZone.visual != null)
            currentZone.visual.AddCard(this);
    }


    // Update is called once per frame
    void Update()
    {
        transform.position += (basePos - transform.position) * Time.deltaTime;
        transform.rotation = baseRot;
        if (card != null && card.state != null && card.state.tapped)
        {
            
            transform.rotation*= Quaternion.Euler(0,0,90);
        }
        UpdateZone();
    }
}
