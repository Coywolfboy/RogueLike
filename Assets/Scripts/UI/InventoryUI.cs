using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{
    public Label[] labels = new Label[8];
    private VisualElement root;
    private int selected;
    private int numItems;

    public int Selected => selected;
    // Start is called before the first frame update
    private void Start()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        for (int i = 0; i < labels.Length; i++)
        {
            labels[i] = root.Q<Label>($"#Item{i + 1}");
        }

        // Ensure all labels are initialized properly
        for (int i = 0; i < labels.Length; i++)
        {
            if (labels[i] == null)
            {
                Debug.LogWarning($"Label {i + 1} could not be found in the UI document.");
            }
        }

        Clear();
        root.style.display = DisplayStyle.None;
    }
    public void Clear()
    {
        foreach (var label in labels)
        {
            if (label != null)
            {
                label.text = "";
            }
        }
    }
    private void UpdateSelected()
    {
        for (int i = 0; i < labels.Length; i++)
        {
            if (i == selected)
            {
                labels[i].style.backgroundColor = new StyleColor(Color.green);
            }
            else
            {
                labels[i].style.backgroundColor = new StyleColor(Color.clear);
            }
        }
    }
    public void SelectNextItem()
    {
        if (selected < numItems - 1)
        {
            selected++;
            UpdateSelected();
        }
    }

    public void SelectPreviousItem()
    {
        if (selected > 0)
        {
            selected--;
            UpdateSelected();
        }
    }
    public void Show(List<Consumable> list)
    {
        selected = 0;
        numItems = list.Count;
        Clear();

        for (int i = 0; i < numItems; i++)
        {
            labels[i].text = list[i].name;
        }

        UpdateSelected();
        root.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        root.style.display = DisplayStyle.None;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
