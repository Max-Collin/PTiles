using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Serializable data container defining a single tile's behavior and timing
[System.Serializable]
public class TileData
{
    public float speed = 5.0f;// Downward movement speed in units per second
    public int lane = 1;// Target lane (1–4)
    public bool isHeld = false;// True if this tile requires holding rather than a single tap
    public float length = 1.0f;// Vertical scale / hold length of the tile
    public float timeUntilNextTile = 1f;// Delay before spawning the next tile in the chart
    public int tileScore = 100;// Base point value awarded for hitting this tile
}
public class TileScript : MonoBehaviour
{
    
    [SerializeField] private TileData tileData;// Runtime data assigned when the tile spawns
    public bool interacted = false;// Tracks whether the player has already triggered this tile
    public bool hasCompletedHold = false;// Tracks whether a held tile has reached its full hold target
    
    // Hold-progress tracking fields
    float elapsed = 0f;
    float holdDuration; 
    
    void Update()
    {
        // Move the tile downward along the Y axis each frame
        transform.Translate(Vector3.down * (tileData.speed * Time.deltaTime));
    }
    
    // sets the tile data class called in the game manager
    public void SetTileData(TileData data)
    {
        tileData = data;
        // Resize the tile to match its configured length
        transform.localScale = new Vector3(1f, tileData.length, 1f);
        holdDuration = Mathf.Max(0.01f, tileData.length/tileData.speed);
    }
    // Returns the TileData assigned to this tile
    public TileData GetTileData()
    {
        return tileData;
    }
    // Handles initial player interaction (tapping a normal tile or starting a hold tile)   
    public void Interact()
    {
        if (!tileData.isHeld)
        {
           
            interacted = true;
            SpriteInteracted();
        }
        else
        {
            // Hold tile: mark interacted and register this tile in the lane's heldTiles slot in the gamemanager
            interacted = true;
            TileGameManager.Instance.heldTiles[tileData.lane - 1] = this;
            
        }
    }
    
    // Advances hold progress over delta seconds; returns true on the frame the hold finishes
    public bool TileHeld(float delta)
    {
        // Already finished holding—do not award completion again
        if(hasCompletedHold) return false;
        elapsed += delta;
        float t = Mathf.Clamp01(elapsed / holdDuration);// Normalized progress from 0 to 1
        float value = Mathf.Lerp(0f, 1f, t); // 0.0 -> 1.0 over holdDuration seconds
        
        // Once full hold progress is reached, mark complete and dim the sprite
        if (value >= 1f)
        {
            hasCompletedHold = true;
            SpriteInteracted();
        }

        return hasCompletedHold;
    }
    // Returns the interpolated hold value reached when the key is released
    public float TileReleased()
    {
        
        float t = Mathf.Clamp01(elapsed / tileData.length);
        float value = Mathf.Lerp(0f, tileData.length, t);
        hasCompletedHold = true;
        SpriteInteracted();
        return value;
    }
   
    private void SpriteInteracted()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = sr.color;
        color.a = 0.5f; // 0f = invisible, 1f = fully opaque
        sr.color = color;
    }
}
