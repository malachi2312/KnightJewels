using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameGrid : MonoBehaviour {

    public static int minItemForMatch = 3;
    public float delayBetweenMatches = 0.5f;
    public int xSize, ySize;
    public float jewelsHeight = 1f;
    private GameObject[] _jewels;
    private GridItem[,] _item;
    private GridItem _currentItem;
    bool canPlay;
    void Start()
    {
        canPlay = true;
        GetJewels();
        FillGrid();
        ClearGrid();
        GridItem.OnMouseOverItemEventHandler += OnMouseOverItem;
        //ist<GridItem> matchesForItem = SearchHorizontally(_item[3, 3]);
    }

    void OnDisable()
    {
        GridItem.OnMouseOverItemEventHandler -= OnMouseOverItem;
    }

    void FillGrid()
    {
        _item = new GridItem[xSize, ySize];
        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
               _item[x,y] =  InstantiateJewels(x, y);
            }
        }
    }

    void ClearGrid()
    {
        for(int x = 0; x < xSize; x++)
        {
            for(int y =0; y< ySize ; y++)
            {
                MatchInfo matchInfo = GetMatchInformation(_item[x,y]);
                if(matchInfo.validMatch)
                {
                    Destroy(_item[x, y].gameObject);
                    _item[x,y] = InstantiateJewels(x, y);
                    y--;
                    GridItem.OnMouseOverItemEventHandler -= OnMouseOverItem;
                }
            }
        }
    }

    void Update()
    {
       // ClearGrid();
    }
    GridItem InstantiateJewels(int x, int y)
    {
        GameObject randomJewels = _jewels[Random.Range(0, _jewels.Length)];
        GridItem newJewels = (Instantiate(randomJewels, new Vector3(x, y *jewelsHeight), Quaternion.identity) as GameObject).GetComponent<GridItem>();
        newJewels.OnItemPositionChanged(x, y);
       // newJewels.transform.SetParent(this.transform);
        return newJewels;
    }

    void GetJewels()
    {
        _jewels = Resources.LoadAll<GameObject>("PrefabsSprite");
        for(int i = 0; i < _jewels.Length; i++)
        {
            _jewels[i].GetComponent<GridItem>().id = i;
        }
    }

    void OnMouseOverItem(GridItem item)
    {
        if (_currentItem == item || !canPlay)
            return;

        if(_currentItem == null)
        {
            _currentItem = item;
        }
        else
        {
            float xDiff = Mathf.Abs(item.x - _currentItem.x);
            float yDiff = Mathf.Abs(item.y - _currentItem.y);
            if(xDiff + yDiff == 1)
            {
                //Accept swap
                StartCoroutine(TryMatch(_currentItem, item));   
            }
            else
            {
                Debug.LogError("aaa");
            }
            _currentItem = null;
        }
    }

    IEnumerator TryMatch(GridItem a, GridItem b)
    {
        canPlay = false;
        yield return StartCoroutine(Swap(a, b));
        MatchInfo matchA = GetMatchInformation(a);
        MatchInfo matchB = GetMatchInformation(b);
        
        if(!matchA.validMatch && !matchB.validMatch)
        {
            yield return StartCoroutine(Swap(a, b));
           // GridItem.OnMouseOverItemEventHandler -= OnMouseOverItem;
          //  yield break;
        }
        if(matchA.validMatch)
        {
            yield return StartCoroutine(DestroyItem(matchA.match));
            yield return new WaitForSeconds(delayBetweenMatches);
            yield return StartCoroutine(UpdateGridAfterMatch(matchA));
        }
        else if(matchB.validMatch)
        {
            yield return StartCoroutine(DestroyItem(matchB.match));
            yield return new WaitForSeconds(delayBetweenMatches);
            yield return StartCoroutine(UpdateGridAfterMatch(matchB));
        }
        canPlay = true;
    }

    IEnumerator UpdateGridAfterMatch(MatchInfo match)
    {
        if(match.matchStartingY == match.matchEndingY)
        {
            //match Horizontal
            for(int x = match.matchStartingX; x <= match.matchEndingX; x++)
            {
                for(int y = match.matchStartingY; y < ySize - 1; y++)
                {
                    GridItem upperIndex = _item[x, y + 1];
                    GridItem current = _item[x, y];
                    _item[x, y] = upperIndex;
                    _item[x, y + 1] = current;
                    _item[x, y].OnItemPositionChanged(_item[x, y].x, _item[x, y].y - 1);
                }
                _item[x, ySize - 1] = InstantiateJewels(x, ySize - 1);
            }
        }
        else if( match.matchEndingX == match.matchStartingX)
        {
            int matchHeight = 1 + (match.matchEndingY - match.matchStartingY);
            for(int y = match.matchStartingY + matchHeight; y <= ySize - 1; y++)
            {
                GridItem lowerIndex = _item[match.matchStartingX, y - matchHeight];
                GridItem current = _item[match.matchStartingX, y];
                _item[match.matchStartingX, y - matchHeight] = current;
                _item[match.matchStartingX, y] = lowerIndex;
            }
            for(int y = 0; y < ySize - matchHeight; y++)
            {
                _item[match.matchStartingX, y].OnItemPositionChanged(match.matchStartingX, y);
            }
            for(int i = 0; i< match.match.Count; i++)
            {
                _item[match.matchStartingX,(ySize-1)-i] = InstantiateJewels(match.matchStartingX,(ySize-1)-i);
            }

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    MatchInfo matchInfo = GetMatchInformation(_item[x, y]);
                    if (matchInfo.validMatch)
                    {
                       // yield return new WaitForSeconds(delayBetweenMatches);
                        yield return StartCoroutine(DestroyItem(matchInfo.match));
                        yield return new WaitForSeconds(delayBetweenMatches);
                        yield return StartCoroutine(UpdateGridAfterMatch(matchInfo));
                    }
                }
            }

        }
    }

    IEnumerator DestroyItem(List<GridItem> item)
    {
       foreach(GridItem g in item)
       {
           yield return StartCoroutine(g.transform.Scale(Vector3.zero, 0.1f));
           Destroy(g.gameObject);
       }
    }

    IEnumerator Swap(GridItem a, GridItem b)
    {
        ChangeRigidbodyStatus(false); //Disable rigidbody
        float moveDuration = 0.1f; 
        Vector3 aPosition = a.transform.position;
        StartCoroutine(a.transform.Move(b.transform.position, moveDuration));
        StartCoroutine(b.transform.Move(aPosition, moveDuration));
        yield return new WaitForSeconds(moveDuration);
        SwapIndices(a, b);
        ChangeRigidbodyStatus(true);
    }

    void SwapIndices(GridItem a, GridItem b)
    {
        GridItem tempA = _item[a.x, a.y];
        _item[a.x, a.y] = b;
        _item[b.x, b.y] = tempA;
        int boldX = b.x;
        int boldY = b.y;
        b.OnItemPositionChanged(a.x, a.y);
        a.OnItemPositionChanged(boldX, boldY);
    }

    List<GridItem>SearchHorizontally(GridItem item)
    {
        List<GridItem> hItem = new List<GridItem> { item };
        int left = item.x - 1;
        int right = item.x + 1;
       while(left >= 0 && _item[left,item.y].id == item.id && _item[left,item.y]!= null)
       {
           hItem.Add(_item[left, item.y]);
           left--;
       }
        while(right < xSize && _item[right,item.y].id == item.id && _item[right,item.y]!=null)
        {
            hItem.Add(_item[right,item.y]);
            right++;
        }
        return hItem;
    }

    List<GridItem>SearchVertically(GridItem item)
    {
        List<GridItem> vItem = new List<GridItem> { item };
        int lower = item.y - 1;
        int upper = item.y + 1;
        while(lower >=0 && _item [item.x, lower].id == item.id && _item[item.x, lower]!=null)
        {
            vItem.Add(_item[item.x, lower]);
            lower--;
        }
        while (upper < ySize && _item[item.x, upper].id == item.id && _item[item.x, upper] != null) 
        {
            vItem.Add(_item[item.x, upper]);
            upper++;
        }
        return vItem;
    }

    MatchInfo GetMatchInformation(GridItem item)
    {
        MatchInfo m = new MatchInfo();
        m.match = null;
        List<GridItem> hMatch = SearchHorizontally(item);
        List<GridItem> vMatch = SearchVertically(item);
        if(hMatch.Count >= minItemForMatch && hMatch.Count > vMatch.Count)
        {
            m.matchStartingX = GetMinimumX(hMatch);
            m.matchEndingX = GetMaximumX(hMatch);
            m.matchStartingY = m.matchEndingY = hMatch[0].y;
            m.match = hMatch;
        }
        else if(vMatch.Count >= minItemForMatch)
        {
            m.matchStartingY = GetMinimumY(vMatch);
            m.matchEndingY = GetMaximumY(vMatch);
            m.matchStartingX = m.matchEndingX = vMatch[0].x;
            m.match = vMatch;
        }
        return m;
    }

    int GetMinimumX(List<GridItem>item)
    {
        float[] indices = new float[item.Count];
        for(int i = 0; i < indices.Length; i++)
        {
            indices[i] = item[i].x;
        }
        return (int)Mathf.Min(indices); 
    }

    int GetMaximumX(List<GridItem> item)
    {
        float[] indices = new float[item.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = item[i].x;
        }
        return (int)Mathf.Max(indices);
    }

    int GetMinimumY(List<GridItem> item)
    {
        float[] indices = new float[item.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = item[i].y;
        }
        return (int)Mathf.Min(indices);
    }

    int GetMaximumY(List<GridItem> item)
    {
        float[] indices = new float[item.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = item[i].y;
        }
        return (int)Mathf.Max(indices);
    }

    void ChangeRigidbodyStatus(bool status)
    {
        foreach(GridItem item in _item)
        {
            if (item != null)
            {
                item.GetComponent<Rigidbody2D>().isKinematic = !status;
            }
        }
    }
}
