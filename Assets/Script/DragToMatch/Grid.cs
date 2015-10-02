using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {
    public GameObject cellItemPrefab;
    Transform[] lines;
    public Tile[] items;

    public Vector3 cellSize = new Vector3(1f, 0.96f, 1f);
    public Vector3 cellScale = new Vector3(1f, 1f, 1f);

    List<Tile> choiceList;

    Line[] lineList;


    Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
        choiceList = new List<Tile>();
        IniteArena();
        isInput = true;
    }

    void IniteArena()
    {
        lines = new Transform[GameInit.totalX];
        lineList = new Line[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            GameObject pgo = new GameObject();
            pgo.name = "Base" + (i+1).ToString("000");
            pgo.transform.parent = transform;
            GameObject go = new GameObject();
            go.transform.parent = pgo.transform;
  
            Line script = go.AddComponent<Line>();
            script.idx = i;
            Transform tf = go.transform;
            lines[i] = tf;
            tf.parent = pgo.transform;
     
            tf.localPosition = (i - 2.5f) * Vector3.right;
            tf.localScale = Vector3.one;
            go.name = "Line" + (i+1).ToString("000");

            script.items = new Tile[GameInit.totalY];
            for (int j = 0; j < script.items.Length; j++)
            {
                GameObject g = Instantiate(cellItemPrefab) as GameObject;
                g.name = "Tile" + (j+1).ToString("000");
                Transform t = g.transform;
                Tile c = g.GetComponent<Tile>();
                c.height = cellSize.y;
                c.gridManager = this;
                c.SetTileType(Random.Range(0, 6));
                c.lineScript = script;
                script.items[j] = c;
                c.idx = j;
                t.parent = tf;
                t.localPosition = Vector3.forward * j * cellSize.y;
                t.localScale = cellScale;
                t.localRotation = Quaternion.identity;
            }
            script.idx = i;
            lineList[i] = script;
        }

        items = GetComponentsInChildren<Tile>();
    }
    public void ChoiceCell(Tile cell, int x, int y, int type, bool isMove)
    {
        if (oldX == x && oldY == y) return;
        if ((oldType != -1 && oldType != type) || choiceList.Contains(cell))
        {
            DoneDrag();
            cell.UnSetChoice();
            return;
        }
        if (isMove) return;
        choiceList.Add(cell);
        cell.isMove = true;
        cell.SetChoice();
        oldX = x;
        oldY = y;
        oldType = type;
    }
 
    public static bool isDrag = false, isInput = false;
    int oldX = -1, oldY = -1, oldType = -1;

    void DoneDrag()
    {
        choiceList.Clear();
        isDrag = false;
        oldX = -1;
        oldY = -1;
        oldType = -1;
        foreach (Transform item in lines)
            item.SendMessage("SortCells", SendMessageOptions.DontRequireReceiver);

        isInput = false;
        StartCoroutine(DelayAction(0.3f, () =>
        {
            isInput = true;
        }));
    }

    IEnumerator DelayAction(float dTime, System.Action callback)
    {
        yield return new WaitForSeconds(dTime);
        callback();
    }


    
    void Update()
    {
        if (!isInput) return;
        if (Input.GetMouseButtonDown(0)) isDrag = true;
        if (Input.GetMouseButtonUp(0)) DoneDrag();

        if (isDrag)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                Tile cb = hit.collider.GetComponent<Tile>();
                if (cb)
                {
                    cb.OnClickDown();
                }
            }
        }
    }
}
