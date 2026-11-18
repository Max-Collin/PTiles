using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// tile data class used for instantiation
[System.Serializable]
public class TileData
{
    public float speed = 5.0f;
    public int lane = 1;
    public bool isLong;
    public float length = 1.0f;
    
    public float timeUntilNextTile = 1f;
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

    public void Interact()
    {
        Debug.Log("Tile interacted");
        interacted = true;
    }
}
