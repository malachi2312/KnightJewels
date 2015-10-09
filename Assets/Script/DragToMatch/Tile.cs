using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;
using UnityEngine.UI;

/// <summary>
/// Tile cell effect and touch/click event.
/// </summary>
public class Tile : MonoBehaviour {
    public Grid gridManager;

    // tile destory effect
    public GameObject effectPrefab;

    // tile order
    public int idx = 0;

    public float height = 1f;

    // tile type
    int _type = 0;

    Transform tf;

    public Sprite[] sprites;
    // for changing tile color & shape
    SpriteRenderer shapeRenderer;
    // for choice display
    Renderer choiceRenderer;
    // Near tile list
    public List<Tile> nearTiles;

    // Save color
    //Color color = Color.white;

    // Tile Line Component
    public Line lineScript;

    // Condition when tile is moving
    public bool isMove = false;

    void Awake()
    {
        tf = transform;
        choiceRenderer = tf.FindChild("Choice").GetComponent<Renderer>();
        shapeRenderer = tf.FindChild("Shape").GetComponent<SpriteRenderer>();
        UnSetChoice();
    }

    // Setup Tile Type.
    public void SetTileType(int type)
    {
        _type = type;
        shapeRenderer.sprite = sprites[type];
    }

    // Get Tile Type.
    public int GetTileType()
    {
        return _type;
    }

    // Click Down Event
    public void OnClickDown()
    {
        gridManager.ChoiceCell(this, lineScript.idx, idx, _type, isMove);
    }

    //Rotate
    //void OnMouseOver()
    //{
    //    this.transform.Rotate(new Vector3(0, 0, 20) * Time.deltaTime * 30);
    //}
    //void OnMouseExit()
    //{
    //    this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    //}

    // Move To Order Position
    public void MoveTo(int seq)
    {
        if (effectPrefab)
        {
            Instantiate(effectPrefab, tf.position, Quaternion.identity);
        }
        tf.localPosition = Vector3.forward * (seq * height);
    }

    // Set Choice Tile
    public void SetChoice()
    {
        choiceRenderer.enabled = true;
    }
    // Unset Choice Tile
    public void UnSetChoice()
    {
        choiceRenderer.enabled = false;
    }


    // Move with Tweening Animation
    public void TweenMoveTo(int seq)
    {
        TweenMove(tf, tf.localPosition, Vector3.forward * (seq * height));
    }

    // Move with Tweening Animation
    void TweenMove(Transform tr, Vector3 pos1, Vector3 pos2)
    {
        tr.localPosition = pos1;
        choiceRenderer.enabled = false;
        if (isMove)
        {
            shapeRenderer.enabled = false;
        }
        StartCoroutine(DelayAction(0.3f, () =>
        {
            if (isMove)
            {
                shapeRenderer.enabled = true;
                isMove = false;
            }
            TweenParms parms = new TweenParms().Prop("localPosition", pos2).Ease(EaseType.EaseOutBounce);
            HOTween.To(tr, 0.3f, parms);
        }));
    }

    // Coroutine Delay Method
    IEnumerator DelayAction(float dTime, System.Action callback)
    {
        yield return new WaitForSeconds(dTime);
        callback();
    }
}
