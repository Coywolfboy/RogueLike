using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Actor : MonoBehaviour
{
    private AdamMilVisibility algorithm;

    [Header("FieldOfView")]
    public List<Vector3Int> FieldOfView = new List<Vector3Int>();
    public int FieldOfViewRange = 8;

    [Header("Powers")]
    [SerializeField] private int maxHitPoints;
    [SerializeField] private int hitPoints;
    [SerializeField] private int defense;
    [SerializeField] private int power;

    [Header("HealthBar")]
    [SerializeField] public int level = 1;
    [SerializeField] public int xp = 0;
    [SerializeField] public int xpToNextLevel = 50;
    public int MaxHitPoints { get => maxHitPoints; }
    public int HitPoints { get => hitPoints; }
    public int Defense { get => defense; }
    public int Power { get => power; }
    

    public int Level { get => level; }
    public int XP { get => xp; }
    public int XPToNextLevel { get => xpToNextLevel; }
    public void SetMaxHitPoints(int value) => maxHitPoints = value;
    public void SetHitPoints(int value) => hitPoints = value;
    public void SetDefense(int value) => defense = value;
    public void SetPower(int value) => power = value;
    public void SetLevel(int value) => level = value;
    public void SetXP(int value) => xp = value;
    public void SetXPToNextLevel(int value) => xpToNextLevel = value;
    private void Start()
    {
        algorithm = new AdamMilVisibility();
        UpdateFieldOfView();

        if (GetComponent<Player>())
        {
            UIManager.Get.UpdateHealth(HitPoints, MaxHitPoints);
            UIManager.Get.UpdateLevel(Level);
            UIManager.Get.UpdateXP(XP);
        }
    }

    public void Move(Vector3 direction)
    {
        if (MapManager.Get.IsWalkable(transform.position + direction))
        {
            transform.position += direction;
        }
    }

    public void UpdateFieldOfView()
    {
        var pos = MapManager.Get.FloorMap.WorldToCell(transform.position);

        FieldOfView.Clear();
        algorithm.Compute(pos, FieldOfViewRange, FieldOfView);

        if (GetComponent<Player>())
        {
            MapManager.Get.UpdateFogMap(FieldOfView);
        }
    }

    public void DoDamage(int hp, Actor attacker)
    {
        hitPoints -= hp;

        if (hitPoints < 0) hitPoints = 0;

        if (GetComponent<Player>())
        {
            UIManager.Get.UpdateHealth(hitPoints, MaxHitPoints);
        }

        if (hitPoints == 0)
        {
            Die(attacker);
        }
    }

    public void Heal(int hp)
    {
        int maxHealing = maxHitPoints - hitPoints;
        if (hp > maxHealing) hp = maxHealing;

        hitPoints += hp;

        if (GetComponent<Player>())
        {
            UIManager.Get.UpdateHealth(hitPoints, MaxHitPoints);
            UIManager.Get.AddMessage($"You are healed for {hp} hit points.", Color.green);
        }
    }

    private void Die(Actor attacker)
    {
        if (GetComponent<Player>())
        {
            UIManager.Get.AddMessage("You died!", Color.red); //Red
            GameManager.Get.ClearSave();
        }
        else
        {
            UIManager.Get.AddMessage($"{name} is dead!", Color.green); //Light Orange
            if (attacker.GetComponent<Player>())
            {
                attacker.AddXP(xp);
            }
        }
        GameManager.Get.CreateGameObject("Dead", transform.position).name = $"Remains of {name}";
        GameManager.Get.RemoveEnemy(this);
        Destroy(gameObject);
    }
    public void AddXP(int amount)
    {
        xp += amount;

        while (xp >= xpToNextLevel)
        {
            xp -= xpToNextLevel;
            LevelUp();
        }

        if (GetComponent<Player>())
        {
            UIManager.Get.UpdateXP(XP);
        }
    }
    private void LevelUp()
    {
        level++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);

        // Verhoog stats bij level up
        maxHitPoints += 10;
        hitPoints = maxHitPoints;
        power += 2;
        defense += 5;

        if (GetComponent<Player>())
        {
            UIManager.Get.UpdateLevel(Level);
            UIManager.Get.UpdateHealth(HitPoints, MaxHitPoints);
            UIManager.Get.AddMessage("Level Up!", Color.yellow);
        }
    }
}
