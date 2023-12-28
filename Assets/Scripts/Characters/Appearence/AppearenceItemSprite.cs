using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearenceItemSprite : MonoBehaviour
{
    public void SetSprite(Sprite spr)
    {
        GetComponent<SpriteRenderer>().sprite = spr;
    }
}
