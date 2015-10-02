using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Line : MonoBehaviour {
    public int idx = 0;
    public Tile[] items;
  
    void Start()
    {
    }

    public void SortCells()
    {
        List<Tile> tlist = new List<Tile>();
        int y = 0;
        int t = 11;
        for (int i = 0; i < GameInit.totalY ; i++)
            if (!items[i].isMove)
            {
                tlist.Add(items[i]);
                items[i].idx = y++;
            }
        for (int i = 0; i < GameInit.totalY; i++)
            if (items[i].isMove)
            {
                tlist.Add(items[i]);
                items[i].idx = y++;
                items[i].MoveTo(t++);
                items[i].SetTileType(Random.Range(0, 6));
            }
        items = tlist.ToArray();
        for (int i = 0; i < GameInit.totalY; i++) items[i].TweenMoveTo(i);
    }

    void Update()
    {
    }
}
