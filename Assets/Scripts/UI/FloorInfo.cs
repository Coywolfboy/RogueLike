using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FloorInfo : MonoBehaviour
{
    private VisualElement root;
    private Label floorLabel;
    private Label enemiesLabel;
    // Start is called before the first frame update
    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        floorLabel = root.Q<Label>("Floor");
        enemiesLabel = root.Q<Label>("Enemies");
    }

    public void UpdateFloor(int floorCount)
    {
        floorLabel.text = $"Floor {floorCount}";
    }

    public void UpdateEnemies(int enemiesLeft)
    {
        enemiesLabel.text = $"{enemiesLeft} enemies left";
    }
}
