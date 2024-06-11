using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static GameManager Get { get => instance; }

    public Actor Player;
    public List<Actor> Enemies = new List<Actor>();
    public List<Consumable> Items = new List<Consumable>();
    public List<Ladder> Ladders = new List<Ladder>();
    public List<TombStone> TombStones = new List<TombStone>();

    public GameObject CreateGameObject(string name, Vector2 position)
    {
        GameObject actor = Instantiate(Resources.Load<GameObject>($"Prefabs/{name}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
        actor.name = name;
        return actor;
    }

    public void AddItem(Consumable item)
    {
        Items.Add(item);
    }

    public void RemoveItem(Consumable item)
    {
        if (Items.Contains(item))
        {
            Items.Remove(item);
        }
    }

    public void AddEnemy(Actor enemy)
    {
        Enemies.Add(enemy);
        UIManager.Get.SetEnemiesLeft(Enemies.Count);
    }

    public void RemoveEnemy(Actor enemy)
    {
        if (Enemies.Contains(enemy))
        {
            Enemies.Remove(enemy);
            UIManager.Get.SetEnemiesLeft(Enemies.Count);

        }
    }

    public void StartEnemyTurn()
    {
        foreach (var enemy in Enemies)
        {
            enemy.GetComponent<Enemy>().RunAI();
        }
    }

    public Actor GetActorAtLocation(Vector3 location)
    {
        if (Player.transform.position == location)
        {
            return Player;
        }
        else
        {
            foreach (Actor enemy in Enemies)
            {
                if (enemy.transform.position == location)
                {
                    return enemy;
                }
            }
        }
        return null;
    }

    public List<Actor> GetNearbyEnemies(Vector3 location)
    {
        var result = new List<Actor>();
        foreach (Actor enemy in Enemies)
        {
            if (Vector3.Distance(enemy.transform.position, location) < 5)
            {
                result.Add(enemy);
            }
        }
        return result;
    }

    public Consumable GetItemAtLocation(Vector3 location)
    {
        foreach (var item in Items)
        {
            if (item.transform.position == location)
            {
                return item;
            }
        }
        return null;
    }

    public void AddLadder(Ladder ladder)
    {
        Ladders.Add(ladder);
    }

    public Ladder GetLadderAtLocation(Vector3 location)
    {
        foreach (var ladder in Ladders)
        {
            if (ladder.transform.position == location)
            {
                return ladder;
            }
        }
        return null;
    }
    public void ClearFloor()
    {
        foreach (var enemy in Enemies)
        {
            Destroy(enemy.gameObject);
        }
        Enemies.Clear();

        foreach (var item in Items)
        {
            Destroy(item.gameObject);
        }
        Items.Clear();

        foreach (var ladder in Ladders)
        {
            Destroy(ladder.gameObject);
        }
        Ladders.Clear();

        foreach (var tombStone in TombStones)
        {
            Destroy(tombStone.gameObject);
        }
        TombStones.Clear();
    }
    public void AddTombStone(TombStone stone)
    {
        TombStones.Add(stone);
    }
    public void SaveGame()
    {
        var player = Get.Player.GetComponent<Actor>();
        PlayerPrefs.SetInt("MaxHitPoints", player.MaxHitPoints);
        PlayerPrefs.SetInt("HitPoints", player.HitPoints);
        PlayerPrefs.SetInt("Defense", player.Defense);
        PlayerPrefs.SetInt("Power", player.Power);
        PlayerPrefs.SetInt("Level", player.Level);
        PlayerPrefs.SetInt("XP", player.XP);
        PlayerPrefs.SetInt("XPToNextLevel", player.XPToNextLevel);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("MaxHitPoints"))
        {
            var player = Get.Player.GetComponent<Actor>();
            player.SetMaxHitPoints(PlayerPrefs.GetInt("MaxHitPoints"));
            player.SetHitPoints(PlayerPrefs.GetInt("HitPoints"));
            player.SetDefense(PlayerPrefs.GetInt("Defense"));
            player.SetPower(PlayerPrefs.GetInt("Power"));
            player.SetLevel(PlayerPrefs.GetInt("Level"));
            player.SetXP(PlayerPrefs.GetInt("XP"));
            player.SetXPToNextLevel(PlayerPrefs.GetInt("XPToNextLevel"));

            UIManager.Get.UpdateLevel(player.Level);
            UIManager.Get.UpdateXP(player.XP);
        }
        
    }

    public void ClearSave()
    {
        PlayerPrefs.DeleteAll();
    }
    
    private void Start()
    {
        // Bestaande start code...

        // Laad spelerdata als die beschikbaar is

        LoadGame();
    }

}
