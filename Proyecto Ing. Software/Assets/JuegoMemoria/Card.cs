using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image iconImage;


    public Sprite hiddenIconSprite;
    public Sprite iconSprite;


    public bool isSelected;


    public CardsController controller;


    public void OnCardClick()
    {
        controller.SetSelected(this);
    }


    public void SetIconSprite(Sprite sp)
    {
        iconSprite = sp;
    }


    public void Show()
    {
        iconImage.sprite = iconSprite;
        isSelected = true;
    }


    public void Hide()
    {
        iconImage.sprite = hiddenIconSprite;
        isSelected = false;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
