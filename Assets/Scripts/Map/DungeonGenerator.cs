using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private int width, height;
    private int maxRoomSize, minRoomSize;
    private int maxRooms;
    private int maxEnemies;
    private int maxItems;
    private int currentFloor;

    List<Room> rooms = new List<Room>();
    private List<string> enemyNames = new List<string>
    {
        "Pig", "Snake", "Spin", "Bij", "Tijger",
        "Draak", "Paard", "Spook", "Hond", "Insect"
    };

    public void SetSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public void SetRoomSize(int min, int max)
    {
        minRoomSize = min;
        maxRoomSize = max;
    }

    public void SetMaxRooms(int max)
    {
        maxRooms = max;
    }

    public void SetMaxEnemies(int max)
    {
        maxEnemies = max;
    }

    public void SetMaxItems(int max)
    {
        maxItems = max;
    }

    public void SetCurrentFloor(int floor)
    {
        currentFloor = floor;
    }

    public void Generate()
    {
        rooms.Clear();

        for (int roomNum = 0; roomNum < maxRooms; roomNum++)
        {
            int roomWidth = Random.Range(minRoomSize, maxRoomSize);
            int roomHeight = Random.Range(minRoomSize, maxRoomSize);

            int roomX = Random.Range(0, width - roomWidth - 1);
            int roomY = Random.Range(0, height - roomHeight - 1);

            var room = new Room(roomX, roomY, roomWidth, roomHeight);

            if (room.Overlaps(rooms))
            {
                continue;
            }
            CreateRoom(room);
            for (int x = roomX; x < roomX + roomWidth; x++)
            {
                for (int y = roomY; y < roomY + roomHeight; y++)
                {
                    if (x == roomX || x == roomX + roomWidth - 1 || y == roomY || y == roomY + roomHeight - 1)
                    {
                        if (!TrySetWallTile(new Vector3Int(x, y)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        SetFloorTile(new Vector3Int(x, y, 0));
                    }
                }
            }

            if (rooms.Count != 0)
            {
                TunnelBetween(rooms[rooms.Count - 1], room);
            }

            PlaceEnemies(room, maxEnemies);
            PlaceItems(room, maxItems);
            rooms.Add(room);
        }

        if (currentFloor == 1)
        {
            GameManager.Get.CreateGameObject("LadderDown", rooms[rooms.Count - 1].Center()).GetComponent<Ladder>().Up = true;
        }
        else if (currentFloor < 1)
        {
            GameManager.Get.CreateGameObject("LadderUp", rooms[0].Center()).GetComponent<Ladder>().Up = false;
            GameManager.Get.CreateGameObject("LadderDown", rooms[rooms.Count - 1].Center()).GetComponent<Ladder>().Up = true;
        }

        if (GameManager.Get.Player != null)
        {
            GameManager.Get.Player.transform.position = new Vector3(rooms[0].Center().x + 0.5f, rooms[0].Center().y + 0.5f, 0);
        }
        else
        {
            GameManager.Get.CreateGameObject("Player", rooms[0].Center());
        }

    }


    private void CreateRoom(Room room)
    {
        for (int x = room.X; x < room.X + room.Width; x++)
        {
            for (int y = room.Y; y < room.Y + room.Height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (x == room.X || x == room.X + room.Width - 1 || y == room.Y || y == room.Y + room.Height - 1)
                {
                    TrySetWallTile(pos);
                }
                else
                {
                    SetFloorTile(pos);
                }
            }
        }
    }

    private bool TrySetWallTile(Vector3Int pos)
    {
        // if this is a floor, it should not be a wall
        if (MapManager.Get.FloorMap.GetTile(pos))
        {
            return false;
        }
        else
        {
            // if not, it can be a wall
            MapManager.Get.ObstacleMap.SetTile(pos, MapManager.Get.WallTile);
            return true;
        }
    }

    private void SetFloorTile(Vector3Int pos)
    {
        // this tile should be walkable, so remove every obstacle
        if (MapManager.Get.ObstacleMap.GetTile(pos))
        {
            MapManager.Get.ObstacleMap.SetTile(pos, null);
        }
        // set the floor tile
        MapManager.Get.FloorMap.SetTile(pos, MapManager.Get.FloorTile);
    }

    private void TunnelBetween(Room oldRoom, Room newRoom)
    {
        Vector2Int oldRoomCenter = oldRoom.Center();
        Vector2Int newRoomCenter = newRoom.Center();
        Vector2Int tunnelCorner;

        if (Random.value < 0.5f)
        {
            // move horizontally, then vertically
            tunnelCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y);
        }
        else
        {
            // move vertically, then horizontally
            tunnelCorner = new Vector2Int(oldRoomCenter.x, newRoomCenter.y);
        }

        // Generate the coordinates for this tunnel
        List<Vector2Int> tunnelCoords = new List<Vector2Int>();
        BresenhamLine.Compute(oldRoomCenter, tunnelCorner, tunnelCoords);
        BresenhamLine.Compute(tunnelCorner, newRoomCenter, tunnelCoords);

        // Set the tiles for this tunnel
        foreach (var coord in tunnelCoords)
        {
            SetFloorTile(new Vector3Int(coord.x, coord.y, 0));
            for (int x = coord.x - 1; x <= coord.x + 1; x++)
            {
                for (int y = coord.y - 1; y <= coord.y + 1; y++)
                {
                    TrySetWallTile(new Vector3Int(x, y, 0));
                }
            }
        }
    }

    private void PlaceEnemies(Room room, int maxEnemies)
    {
        int num = Random.Range(0, maxEnemies + 1);

        for (int counter = 0; counter < num; counter++)
        {
            int x = Random.Range(room.X + 1, room.X + room.Width - 1);
            int y = Random.Range(room.Y + 1, room.Y + room.Height - 1);

            List<string> weightedEnemies = new List<string>();

            for (int i = 0; i < enemyNames.Count; i++)
            {
                int weight = Mathf.Max(1, currentFloor - (enemyNames.Count - i - 1));
                for (int j = 0; j < weight; j++)
                {
                    weightedEnemies.Add(enemyNames[i]);
                }
            }

            string selectedEnemy = weightedEnemies[Random.Range(0, weightedEnemies.Count)];

            GameManager.Get.CreateGameObject(selectedEnemy, new Vector2(x, y));
        }
    }

    private void PlaceItems(Room room, int maxItems)
    {
        int num = Random.Range(0, maxItems + 1);

        for (int counter = 0; counter < num; counter++)
        {
            int x = Random.Range(room.X + 1, room.X + room.Width - 1);
            int y = Random.Range(room.Y + 1, room.Y + room.Height - 1);

            float value = Random.value;
            if (value > 0.8f)
            {
                GameManager.Get.CreateGameObject("ScrollOfConfusion", new Vector2(x, y));
            }
            else if (value > 0.5f)
            {
                GameManager.Get.CreateGameObject("Fireball", new Vector2(x, y));
            }
            else
            {
                GameManager.Get.CreateGameObject("HealthPotion", new Vector2(x, y));
            }
        }
    }
}
