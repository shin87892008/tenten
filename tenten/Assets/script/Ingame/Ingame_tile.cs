using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Ingame_tile : MonoBehaviour {

    [SerializeField]
    private Color Block_Color;
    public Image block;

    public int index = 0;
    public bool inblock;

    public void Refresh()
    {
        block.color = Block_Color;
        inblock = false;
    }
}
