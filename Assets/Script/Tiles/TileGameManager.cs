using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TileGameManager : MonoBehaviour
{
    public static TileGameManager Instance {get; private set;}
    
    
    [Header("Tile Spawn")]
    public List<Transform> tileSpawnLocations = new List<Transform>(); // Spawn points for lanes 1–4
    public List<TileData> tileDataList; // Queue of upcoming tile definitions to spawn
    [SerializeField] private GameObject tilePrefab;
    
    [Header("Score & UI")]
    [SerializeField] private int score; 
    [SerializeField] private TMP_Text scoreText;
    public Transform scoreLine; // Reference Y position used to judge timing accuracy
    
    
    
    [Header("Tile Tracking")]
    [SerializeField] private List<TileScript> spawnedTiles = new List<TileScript>(); // All tiles spawned so far (will have null reference)
    [SerializeField] private List<TileScript> activeTiles = new List<TileScript>(4); // Currently active tiles on screen
    [SerializeField] public List<TileScript> heldTiles = new List<TileScript>(); // Tiles currently being held down
    [SerializeField] private int completedTileIndex = 0; // Index of the next tile in spawnedTiles to activate
    
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
    
    void Start()
    {
        Debug.Log("Game Started!");
        GameStart();
    }

    
    void Update()
    {
        
        // Check initial key presses for lanes 1 (Left) through 4 (Right)
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            print("lane 1");
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
        // Update active hold tiles while keys remain held or are released
        if(heldTiles.Count>0)
        {
            // While held, advance hold timer and award bonus points if TileHeld returns true
            if (Input.GetKey(KeyCode.LeftArrow) && heldTiles[0])
            {
                if (heldTiles[0].TileHeld(Time.deltaTime))
                {
                    ChangeScore(200);
                }
            }
            if (Input.GetKey(KeyCode.UpArrow)&& heldTiles[1])
            {
                if (heldTiles[1].TileHeld(Time.deltaTime))
                {
                    ChangeScore(200);
                }
            }
            if (Input.GetKey(KeyCode.DownArrow)&& heldTiles[2])
            {
                if (heldTiles[2].TileHeld(Time.deltaTime))
                {
                    ChangeScore(200);
                }
            }
            if (Input.GetKey(KeyCode.RightArrow)&& heldTiles[3])
            {
                if (heldTiles[3].TileHeld(Time.deltaTime))
                {
                    ChangeScore(200);
                }
            }
            
            // On key release, finish the hold and clear the slot
            if (Input.GetKeyUp(KeyCode.LeftArrow)&& heldTiles[0])
            {
                heldTiles[0].TileReleased();
                heldTiles[0] = null;
                
            }

            if (Input.GetKeyUp(KeyCode.UpArrow)&& heldTiles[1])
            {
                heldTiles[1].TileReleased();
                heldTiles[1] = null;
                
            }

            if (Input.GetKeyUp(KeyCode.DownArrow)&& heldTiles[2])
            {
                heldTiles[2].TileReleased();
                heldTiles[2] = null;
                
            }

            if (Input.GetKeyUp(KeyCode.RightArrow)&& heldTiles[3])
            {
                heldTiles[3].TileReleased();
                heldTiles[3] = null;
                
            }
        }

        
    }
    // Begins tile spawning for the game session
    public void GameStart()
    {
        SpawnTiles();
    }

    // Spawns the next tile from tileDataList and schedules the following spawn after delay
    void SpawnTiles()
    {
        if (tileDataList.Count > 0)
        {
           TileScript newTile = Instantiate(tilePrefab).GetComponent<TileScript>();
           // Lanes are 1-indexed (1–4), so subtract 1 for 0-based list indexing
           newTile.transform.position = tileSpawnLocations[tileDataList[0].lane-1].position;
           newTile.SetTileData(tileDataList[0]);
           float delay = tileDataList[0].timeUntilNextTile;
           tileDataList.RemoveAt(0);
           spawnedTiles.Add(newTile);
           // Ensure there is at least one active tile if none are currently active
           if (activeTiles.Count == 0)
           {
               activeTiles.Add(newTile);
           }

           // If delay is 0, immediately spawn a simultaneous tile on the next entry
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
           // Schedule the next SpawnTiles call if more tiles remain in the chart
           if (tileDataList.Count > 0)
           {
               Invoke(nameof(SpawnTiles), delay);
           }
        }
    }
    // Called when a tile passes without being hit; advances activeTiles if it was the only one
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
    // Evaluates a player key press for the given lane (1–4) and scores matching active tiles
    
    void CheckTile(int lane)
    {   
        // Penalize pressing a key when no active tile is available
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
                    // Check if the single active tile matches the pressed lane
                    TileScript tile = activeTiles[0];
                    if(!scoreLine) return;
                    // Distance above ScoreLine (smaller = closer to the hit line)
                    float scorePositon = tile.transform.position.y -scoreLine.position.y ;
                    
                    int score = tile.GetTileData().tileScore;
                    // Apply timing accuracy multipliers based on proximity to ScoreLine
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
                    
                    // Queue the next spawned tile into activeTiles if available
                    if (spawnedTiles.Count>completedTileIndex)
                    {
                        activeTiles.Add(spawnedTiles[completedTileIndex]);
                    }
                    

                }
                else
                { 
                    // Wrong lane pressed for the current active tile
                    ChangeScore(-50);
                }
            }
            else
            {
                // Multiple active tiles on screen: check each for a matching lane
                for (int i = 0; i < activeTiles.Count; i++)
                {
                    if (activeTiles[i].GetTileData().lane == lane)
                    {
                        TileScript tile = activeTiles[i];
                        if(!scoreLine) return;
                        float scorePositon = tile.transform.position.y -scoreLine.position.y ;
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
                        return; // Exit early so the miss penalty below does not run
                    }
                }
                // No active tile matched the pressed lane
                ChangeScore(-50);
            }
        }
    }
    // Returns the current total score
    public int GetScore()
    {
        return score;
    }
    // Adds newScore to the total Score and refreshes the TextMeshPro UI label
    public void ChangeScore(int newScore)
    {
        score += newScore;
        if (scoreText)
        {
            scoreText.text = score.ToString();
        }
    }
    
    
    
}
