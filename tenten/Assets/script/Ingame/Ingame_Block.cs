using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class Ingame_Block : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [SerializeField]
    private int Block_Offset = 100;
    private Transform Base_Parent;

    [SerializeField]
    private Transform Base_Block;

    [SerializeField]
    private Transform Block_BG;

    public Block_Type Block_Type;
    public List<Transform> Blocks = new List<Transform>();
    public Color Block_Color;

    Action<Ingame_Block> Drag_CallBack;

    public void Init(Action<Ingame_Block> callback, Transform parent, Block_Type type = Block_Type.ADD)
    {
        this.transform.parent = parent;
        this.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        this.transform.localPosition = Vector3.zero;

        Blocks.AddRange(Base_Block.GetComponentsInChildren<Transform>());
        Blocks.RemoveAt(0);
        for (int i = 0; i < Blocks.Count; ++i)
        {
            Blocks[i].GetComponent<Image>().color = Block_Color;
        }
        Drag_CallBack = callback;
        Base_Parent = parent;
        Block_Type = type;
    }

    public void OnBeginDrag(PointerEventData eventdata)
    {
        float position_x = eventdata.position.x - (Screen.width / 2);
        float position_y = eventdata.position.y - (Screen.height / 2);


        this.transform.parent = Base_Parent.parent.parent;
        this.transform.localScale = Vector3.one;
    }
    public void OnDrag(PointerEventData eventdata)
    {
        this.transform.position = new Vector3(eventdata.position.x, eventdata.position.y + Block_Offset, 0);
    }
    public void OnEndDrag(PointerEventData eventdata)
    {
        Block_BG.gameObject.SetActive(false);
        Drag_CallBack(this);
    }

    public void Reset_Position()
    {
        Block_BG.gameObject.SetActive(true);
        this.transform.parent = Base_Parent;
        this.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        this.transform.localPosition = Vector3.zero;
    }


    public bool Check_Block_Position()
    {
        for (int i = 0; i < Blocks.Count; ++i)
        {
            Ray2D ray = new Ray2D(Blocks[i].position, Vector2.zero);
            RaycastHit2D[] ray_obj = Physics2D.RaycastAll(ray.origin, ray.direction);

            if (ray_obj.Length == 0)
                return false;
        }
        return true;
    }
}
