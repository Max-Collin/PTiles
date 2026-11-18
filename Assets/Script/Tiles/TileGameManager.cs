using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGameManager : MonoBehaviour
{
    public static TileGameManager Instance {get; private set;}
    public List<Transform> tileSpawnLocations = new List<Transform>();
    public List<TileData> tileDataList;
    [SerializeField] private GameObject tilePrefab;
    
    // sets the tile game manager as a singleton
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Game Started!");
        GameStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameStart()
    {
        SpawnTiles();
    }

    // spawns each tile after a delay 
    void SpawnTiles()
    {
        if (tileDataList.Count > 0)
        {
           TileScript NewTile = Instantiate(tilePrefab).GetComponent<TileScript>();
           NewTile.transform.position = tileSpawnLocations[tileDataList[0].lane-1].position;
           NewTile.SetTileData(tileDataList[0]);
           float delay = tileDataList[0].timeUntilNextTile;
           tileDataList.RemoveAt(0);
           if (tileDataList.Count > 0)
           {
               Invoke(nameof(SpawnTiles), delay);
           }
        }
    }
}
