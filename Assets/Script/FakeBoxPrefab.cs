using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class FakeBoxPrefab : MonoBehaviour
{
    [SerializeField] GameObject child;
    [SerializeField] SpriteRenderer goSprite;
    TMP_Text text;
    public List<Sprite> sprites;
    public string letter;
    
    void Awake()
    {
        text = child.GetComponent<TMP_Text>();
        ChooseLetter(letter);
    }

    void OnEnable()
    {
        ChooseSprite();
    }

    public void ChooseLetter(string letter)
    {
        text.SetText(letter);
    }

    void ChooseSprite()
    {
        System.Random rand = new System.Random();
        int num = rand.Next(0, 4);

        goSprite.sprite = sprites[num];
    }
}
