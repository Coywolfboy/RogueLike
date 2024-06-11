using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.EventSystems.EventTrigger;

public class MapManager : MonoBehaviour
{
    private static MapManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            currentFloor = 1; // Initialize currentFloor here

        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static MapManager Get { get => instance; }

    [Header("TileMaps")]
    public Tilemap FloorMap;
    public Tilemap ObstacleMap;
    public Tilemap FogMap;

    [Header("Tiles")]
    public TileBase FloorTile;
    public TileBase WallTile;
    public TileBase FogTile;

    [Header("Features")]
    public Dictionary<Vector2Int, Node> Nodes = new Dictionary<Vector2Int, Node>();
    public List<Vector3Int> VisibleTiles;
    public Dictionary<Vector3Int, TileData> Tiles;

    [Header("Map Settings")]
    public int width = 80;
    public int height = 45;
    public int roomMaxSize = 10;
    public int roomMinSize = 6;
    public int maxRooms = 30;
    public int maxEnemies = 2;
    public int maxItems = 2;
    public int currentFloor;
    private void Start()
    {
        SetCurrentFloor(currentFloor); // Ensure UI shows the correct floor initially
        GenerateDungeon();
        GameManager.Get.LoadGame();
    }

    public void SetCurrentFloor(int floor)
    {
        currentFloor = floor;
        UIManager.Get.SetFloor(currentFloor); // Update UI manager to display the correct floor
    }
    private void GenerateDungeon()
    {
        FloorMap.ClearAllTiles();
        ObstacleMap.ClearAllTiles();
        FogMap.ClearAllTiles();

        Tiles = new Dictionary<Vector3Int, TileData>();
        VisibleTiles = new List<Vector3Int>();

        var generator = new DungeonGenerator();
        generator.SetSize(width, height);
        generator.SetRoomSize(roomMinSize, roomMaxSize);
        generator.SetMaxRooms(maxRooms);
        generator.SetMaxEnemies(maxEnemies);
        generator.SetMaxItems(maxItems);
        generator.SetCurrentFloor(currentFloor);
        generator.Generate();

        AddTileMapToDictionary(FloorMap);
        AddTileMapToDictionary(ObstacleMap);
        SetupFogMap();
    }

    public bool InBounds(int x, int y) => 0 <= x && x < width && 0 <= y && y < height;

    public bool IsWalkable(Vector3 position)
    {
        Vector3Int gridPosition = FloorMap.WorldToCell(position);
        if (!InBounds(gridPosition.x, gridPosition.y) || ObstacleMap.HasTile(gridPosition))
        {
            return false;
        }
        return true;
    }

    private void AddTileMapToDictionary(Tilemap tilemap)
    {
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos))
            {
                continue;
            }
            TileData tile = new TileData(
                name: tilemap.GetTile(pos).name,
                isExplored: false,
                isVisible: false
            );

            Tiles.Add(pos, tile);
        }
    }

    private void SetupFogMap()
    {
        foreach (Vector3Int pos in Tiles.Keys)
        {
            if (!FogMap.HasTile(pos))
            {
                FogMap.SetTile(pos, FogTile);
                FogMap.SetTileFlags(pos, TileFlags.None);
            }

            if (Tiles[pos].IsExplored)
            {
                FogMap.SetColor(pos, new Color(1.0f, 1.0f, 1.0f, 0.5f));
            }
            else
            {
                FogMap.SetColor(pos, Color.white);
            }
        }
    }

    public void UpdateFogMap(List<Vector3Int> playerFOV)
    {
        foreach (var pos in VisibleTiles)
        {
            if (!Tiles[pos].IsExplored)
            {
                Tiles[pos].IsExplored = true;
            }

            Tiles[pos].IsVisible = false;
            FogMap.SetColor(pos, new Color(1.0f, 1.0f, 1.0f, 0.5f));
        }

        VisibleTiles.Clear();

        foreach (var pos in playerFOV)
        {
            Tiles[pos].IsVisible = true;
            FogMap.SetColor(pos, Color.clear);
            VisibleTiles.Add(pos);
        }
    }

    public void MoveUp()
    {
        GameManager.Get.ClearFloor();
        SetCurrentFloor(currentFloor + 1); // Update UI and current floor
        GenerateDungeon();
    }

    public void MoveDown()
    {
        if (currentFloor > 1)  // Zorg ervoor dat de verdieping niet lager wordt dan 1
        {
            GameManager.Get.ClearFloor();
            SetCurrentFloor(currentFloor - 1); // Update de UI en de huidige verdieping
            GenerateDungeon();
        }
    }

}
