using System.Collections.Generic;
using UnityEngine;

public class CardZoneVisual : MonoBehaviour
{
    public Vector3 offset = Vector3.up;
    public List<CardVisual> cards = new List<CardVisual>();

    public void AddCard(CardVisual card)
    {
        cards.Add(card);
        CalculateBasePositions();
    }

    public void RemoveCard(CardVisual card)
    {
        cards.Remove(card);
        CalculateBasePositions();
    }

    public void CalculateBasePositions()
    {
        for(int i = 0; i< cards.Count; i++)
        {
            cards[i].basePos = transform.TransformPoint((i - ((cards.Count-1) / 2f))/cards.Count * Vector3.right+ offset);
            cards[i].baseRot = transform.rotation;
        }
        
    }
}
