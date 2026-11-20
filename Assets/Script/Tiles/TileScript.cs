using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// tile data class used for instantiation
[System.Serializable]
public class TileData
{
    public float speed = 5.0f;
    public int lane = 1;
    public bool isHeld = false;
    public float length = 1.0f;
    
    public float timeUntilNextTile = 1f;

    public int tileScore = 100;
}
public class TileScript : MonoBehaviour
{
    [SerializeField] private TileGameManager tileGameManager;
    [SerializeField] private TileData tileData;
    public bool interacted = false;
    // Start is called before the first frame update
    void Start()
    {
        // accesses game manager singleton
        tileGameManager = TileGameManager.Instance;
       
    }

    // Update is called once per frame
    void Update()
    {
        //move the tile down the screen 
        transform.Translate(Vector3.down * (tileData.speed * Time.deltaTime));
    }
    
    // sets the tile data class called in the game manager
    public void SetTileData(TileData data)
    {
        tileData = data;
        // resizes the tiles to correct size
        transform.localScale = new Vector3(1f, tileData.length, 1f);
    }

    public TileData GetTileData()
    {
        return tileData;
    }

    public void Interact()
    {
        if (!tileData.isHeld)
        {
            interacted = true;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            Color color = sr.color;
            color.a = 0.5f; // 0f = invisible, 1f = fully opaque
            sr.color = color;
        }
       
    }
}
