using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

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
        Tween.Rotation(transform, //the target
            new Vector3(0f, 180f, 0f), //180 degree in y axis
            0.2f); // in 2 seconds

        Tween.Delay(0.1f, () => iconImage.sprite = iconSprite);
        isSelected = true;
    }


    public void Hide()
    {
        Tween.Rotation(transform,
            new Vector3(0f, 0f, 0f),
            0.2f);

        Tween.Delay(0.1f, () =>
        {
            iconImage.sprite = hiddenIconSprite;
            isSelected = false;
        });
        
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
