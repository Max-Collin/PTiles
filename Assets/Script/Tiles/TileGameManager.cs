using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileGameManager : MonoBehaviour
{
    public static TileGameManager Instance {get; private set;}
    public List<Transform> tileSpawnLocations = new List<Transform>();
    public Transform ScoreLine;
    public List<TileData> tileDataList;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int Score;
    [SerializeField] private List<TileScript> spawnedTiles = new List<TileScript>();
    [SerializeField] private List<TileScript> activeTiles = new List<TileScript>();
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private int completedTileIndex = 0;
    
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
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CheckTile(1);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            CheckTile(2);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CheckTile(3);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CheckTile(4);
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            
        }
        
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
           TileScript newTile = Instantiate(tilePrefab).GetComponent<TileScript>();
           newTile.transform.position = tileSpawnLocations[tileDataList[0].lane-1].position;
           newTile.SetTileData(tileDataList[0]);
           float delay = tileDataList[0].timeUntilNextTile;
           tileDataList.RemoveAt(0);
           spawnedTiles.Add(newTile);
           if (activeTiles.Count == 0)
           {
               activeTiles.Add(newTile);
           }

           
           if (delay == 0 && tileDataList.Count > 0 )
           {
               newTile = Instantiate(tilePrefab).GetComponent<TileScript>();
               newTile.transform.position = tileSpawnLocations[tileDataList[0].lane-1].position;
               newTile.SetTileData(tileDataList[0]);
               delay = tileDataList[0].timeUntilNextTile;
               tileDataList.RemoveAt(0);
               spawnedTiles.Add(newTile);
               
               activeTiles.Add(newTile);
               
           }
           if (tileDataList.Count > 0)
           {
               Invoke(nameof(SpawnTiles), delay);
           }
        }
    }

    public void MissedTile( TileScript tile)
    {
        if (activeTiles.Count == 1)
        {
            activeTiles.Remove(tile);
            completedTileIndex++;

            if (spawnedTiles.Count>completedTileIndex)
            {
                activeTiles.Add(spawnedTiles[completedTileIndex]);
            }
        }
        
        
    }

    void CheckTile(int lane)
    {
        if (activeTiles.Count == 0)
        {
            ChangeScore(-50);
        }
        else
        {
            if (activeTiles.Count == 1)
            {
                if (lane == activeTiles[0].GetTileData().lane)
                {
                    TileScript tile = activeTiles[0];
                    if(!ScoreLine) return;
                    float scorePositon = tile.transform.position.y -ScoreLine.position.y ;
                    //print(scorePositon);
                    int score = tile.GetTileData().tileScore;
                    
                    if (scorePositon < 0.25)
                    {
                        score *= 3;
                    }
                    else if (scorePositon < 2)
                    {
                        score *= 2;
                    }
                    ChangeScore(score);
                    tile.Interact();
                    activeTiles.RemoveAt(0);
                    completedTileIndex++;

                    if (spawnedTiles.Count>completedTileIndex)
                    {
                        activeTiles.Add(spawnedTiles[completedTileIndex]);
                    }
                    

                }
                else
                {
                    ChangeScore(-50);
                }
            }
            else
            {
                for (int i = 0; i < activeTiles.Count; i++)
                {
                    if (activeTiles[i].GetTileData().lane == lane)
                    {
                        TileScript tile = activeTiles[i];
                        if(!ScoreLine) return;
                        float scorePositon = tile.transform.position.y -ScoreLine.position.y ;
                        //print(scorePositon);
                        int score = tile.GetTileData().tileScore;
                    
                        if (scorePositon < 0.25)
                        {
                            score *= 3;
                        }
                        else if (scorePositon < 2)
                        {
                            score *= 2;
                        }
                        ChangeScore(score);
                        tile.Interact();
                        activeTiles.RemoveAt(i);
                        completedTileIndex++;
                        break;
                    }
                }
                ChangeScore(-50);
            }
        }
    }
   
    public int GetScore()
    {
        return Score;
    }

    public void ChangeScore(int newScore)
    {
        Score += newScore;
        if (scoreText)
        {
            scoreText.text = Score.ToString();
        }
    }
    
    
    
}
