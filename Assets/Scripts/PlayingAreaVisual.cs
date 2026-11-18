using UnityEngine;

public class PlayingAreaVisual : MonoBehaviour
{
    public CardZoneVisual player1Hand;
    public CardZoneVisual player2Hand;

    public CardZoneVisual player1Battlefield;
    public CardZoneVisual player2Battlefield;

    public CardZoneVisual player1Graveyard;
    public CardZoneVisual player2Graveyard;
    
    public GameObject CardVisualPrefab;

    public void SpawnCard(Card card)
    {
        GameObject cardVisual = Instantiate(CardVisualPrefab);
        cardVisual.GetComponent<CardVisual>().SetCard(card);
        
    }
}
