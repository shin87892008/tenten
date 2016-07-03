using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ingame_main : MonoBehaviour {

    [SerializeField]
    private Camera ingame_camera;
    [SerializeField]
    private Transform parent_tile;
    [SerializeField]
    private List<Transform> block_positions = new List<Transform>();

    private List<Ingame_tile> list_map = new List<Ingame_tile>();

    private int Regist_Count = 0;

    void Start()
    {
        list_map.AddRange(parent_tile.GetComponentsInChildren<Ingame_tile>());

        for(int i = 0; i < list_map.Count; ++i)
            list_map[i].index = i;

        Create_Block();
    }

    private void Create_Block()
    {
        for(int i = 0; i < block_positions.Count; ++i)
        {
            GameObject block =(GameObject)Instantiate(
                Resources.Load(string.Format("prefab/block/item_block_{0}", Random.Range(1, 5))));

            block.GetComponent<Ingame_Block>().Init(End_Drag, block_positions[i]);
        }
    }

    private void End_Drag(Ingame_Block block )
    {
        if (Check_Regist_Block(block))
        {
            Remove_BingoLine(Regist_Block(block));

            Regist_Count++;
            if( Regist_Count == block_positions.Count)
            {
                Regist_Count = 0;
                Create_Block();
            }
        }
        else
            block.Reset_Position();
    }

    private void Remove_BingoLine(Dictionary<Bingo_Axis, List<int>> bingo)
    {
        foreach(KeyValuePair<Bingo_Axis, List<int>> pair in bingo)
        {
            int offset = 1;
            if (pair.Key != Bingo_Axis.Horizontal)
                offset = 10;

            for(int i = 0; i < pair.Value.Count; ++i)
            {
                for(int j = 0; j < 10; ++j)
                {
                    list_map[pair.Value[i] + (offset * j)].Refresh();
                }
            }
        }
    }

    private bool Check_Regist_Block(Ingame_Block block)
    {
        if (!block.Check_Block_Position())
            return false;

        for (int i = 0; i < block.Blocks.Count; ++i)
        {
            Ray2D ray = new Ray2D(block.Blocks[i].position, Vector2.zero);
            RaycastHit2D[] ray_obj = Physics2D.RaycastAll(ray.origin, ray.direction);

            int tile_index = ray_obj[0].collider.gameObject.GetComponent<Ingame_tile>().index;

            if (list_map[tile_index].inblock)
                return false;
        }    
        return true;
    }

    private Dictionary<Bingo_Axis, List<int>> Regist_Block(Ingame_Block block)
    {
        Dictionary<Bingo_Axis, List<int>> Dic_Bingo = new Dictionary<Bingo_Axis, List<int>>();

        int[] tile_indexs = new int[block.Blocks.Count];

        for (int i = 0; i < block.Blocks.Count; ++i)
        {
            Ray2D ray = new Ray2D(block.Blocks[i].position, Vector2.zero);
            RaycastHit2D[] ray_obj = Physics2D.RaycastAll(ray.origin, ray.direction);

            tile_indexs[i] = ray_obj[0].collider.gameObject.GetComponent<Ingame_tile>().index;

            list_map[tile_indexs[i]].inblock = true;
            list_map[tile_indexs[i]].block.color = block.Block_Color;
        }

        DestroyImmediate(block.gameObject);

        for (int i = 0; i < tile_indexs.Length; ++i)
        {
            List<Bingo> bingo = Check_bingo(tile_indexs[i]);

            for(int j = 0; j < bingo.Count; ++j)
            {
                if(!Dic_Bingo.ContainsKey(bingo[j].Axis))
                    Dic_Bingo.Add(bingo[j].Axis, new List<int>());

                if(!Dic_Bingo[bingo[j].Axis].Contains(bingo[j].index))
                {
                    Dic_Bingo[bingo[j].Axis].Add(bingo[j].index);
                }
            }
        }
        
        return Dic_Bingo;
    }

    private List<Bingo> Check_bingo(int index)
    {
        List<Bingo> list_bingo = new List<Bingo>();

        bool horizontal = true;
        int start_index = index - (index % 10);
        for (int i = 0; i < 10; ++i)
        {
            if (!list_map[start_index + i].inblock)
            {
                horizontal = false;
                break;
            }
        }

        if( horizontal)
        {
            Bingo hori_bingo = new Bingo();
            hori_bingo.index = start_index;
            hori_bingo.Axis = Bingo_Axis.Horizontal;
            list_bingo.Add(hori_bingo);
        }

        bool vertical = true;
        start_index = index % 10;
        for (int i = 0; i < 10; ++i)
        {
            if (!list_map[start_index + (i * 10)].inblock)
            {
                vertical = false;
                break;
            }
        }

        if (vertical)
        {
            Bingo vert_bingo = new Bingo();
            vert_bingo.index = start_index;
            vert_bingo.Axis = Bingo_Axis.Vertical;
            list_bingo.Add(vert_bingo);
        }

        return list_bingo;
    }
}
