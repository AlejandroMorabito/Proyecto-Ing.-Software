using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsController : MonoBehaviour
{
    [SerializeField] Card cardPrefab;
    [SerializeField] Transform gridTransform;
    [SerializeField] Sprite[] sprites;


    private List<Sprite> spritePairs;


    Card firstSelected;
    Card secondSelected;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        PrepareSprites();
        CreateCards();
    }
   
    private void PrepareSprites()
    {
        spritePairs = new List<Sprite>();
        for(int i=0; i < sprites.Length; i++)
        {
            // adding sprite 2 times to make it pair
            spritePairs.Add(sprites[i]);
            spritePairs.Add(sprites[i]);
        }


        ShuffleSprites(spritePairs);


    }


    void CreateCards()
    {
        for(int i = 0; i < spritePairs.Count; i++)
        {
            Card card = Instantiate(cardPrefab, gridTransform);
            card.SetIconSprite(spritePairs[i]);
            card.controller = this;
        }
    }


    public void SetSelected(Card card)
    {
        if(card.isSelected == false)
        {
            card.Show();


            if (firstSelected == null)
            {
                firstSelected = card;
                return;
            }


            if (secondSelected  == null)
            {
                secondSelected = card;
                StartCoroutine(CheckMatching(firstSelected, secondSelected));
                firstSelected = null;
                secondSelected = null;
            }
        }


    }


    IEnumerator CheckMatching (Card a, Card b)
    {
        yield return new WaitForSeconds(0.3f);
        if(a.iconSprite == b.iconSprite)
        {
            // Matched
        }
        else
        {
            // flip them back
            a.Hide();
            b.Hide();
        }
    }


    // Method to shuffle a list of sprites


    void ShuffleSprites(List<Sprite> spriteList)
    {
        for (int i = spriteList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);


            // Swap the elements at i and randomIndex
            Sprite temp = spriteList[i];
            spriteList[i] = spriteList[randomIndex];
            spriteList[randomIndex] = temp;
        }


    }


    // Update is called once per frame
    void Update()
    {
       
    }
}
