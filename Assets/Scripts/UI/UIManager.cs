using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        floorInfo = FindObjectOfType<FloorInfo>();
    }
    public static UIManager Get { get => instance; }
    //public HealthBar healthbar;
    [Header("UI GameObject's")]
    public GameObject HealthBar;
    public GameObject Messages;
    [SerializeField] private GameObject InventoryUI;
    public FloorInfo floorInfo;
    public InventoryUI Inventory { get => InventoryUI.GetComponent<InventoryUI>(); }
    public void UpdateHealth(int current, int max)
    {
        HealthBar.GetComponent<HealthBar>().SetValues(current, max);
    }

    public void AddMessage(string message, Color color)
    {
        Messages.GetComponent<Messages>().AddMessage(message, color);
    }
    public void UpdateLevel(int level)
    {
        HealthBar.GetComponent<HealthBar>().SetLevel(level);
    }
    public void UpdateXP(int xp)
    {
        HealthBar.GetComponent<HealthBar>().SetXP(xp);
    }
    public void SetFloor(int floorCount)
    {
        floorInfo.UpdateFloor(floorCount);
    }
    public void SetEnemiesLeft(int enemiesLeft)
    {
        floorInfo.UpdateEnemies(enemiesLeft);
    }
}