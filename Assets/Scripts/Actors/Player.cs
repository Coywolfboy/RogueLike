using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Actor))]
public class Player : MonoBehaviour, Controls.IPlayerActions
{
    private Controls controls;
    private Inventory inventory;
    private bool inventoryIsOpen = false;
    private bool droppingItem = false;
    private bool usingItem = false;

    private void Awake()
    {
        controls = new Controls();
    }

    private void Start()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
        GameManager.Get.Player = GetComponent<Actor>();
        inventory = GetComponent<Inventory>();
    }

    private void OnEnable()
    {
        controls.Player.SetCallbacks(this);
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Player.SetCallbacks(null);
        controls.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Move();
        }
    }
    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var item = GameManager.Get.GetItemAtLocation(transform.position);
            if (item == null)
            {
                Debug.Log("No item at this location.");
            }
            else if (!inventory.AddItem(item))
            {
                Debug.Log("Inventory is full.");
            }
            else
            {
                item.gameObject.SetActive(false);
                GameManager.Get.RemoveItem(item);
                Debug.Log("Item added to inventory.");
            }
        }
    }
    public void OnDrop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!inventoryIsOpen)
            {
                UIManager.Get.InventoryUI.Show(inventory.Items);
                inventoryIsOpen = true;
                droppingItem = true;
            }
            else
            {
                var selectedItem = inventory.Items[UIManager.Get.InventoryUI.Selected];
                inventory.DropItem(selectedItem);
                selectedItem.transform.position = transform.position;
                GameManager.Get.AddItem(selectedItem);
                selectedItem.gameObject.SetActive(true);

                UIManager.Get.InventoryUI.Hide();
                inventoryIsOpen = false;
                droppingItem = false;
            }
        }
    }
    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!inventoryIsOpen)
            {
                UIManager.Get.InventoryUI.Show(inventory.Items);
                inventoryIsOpen = true;
                usingItem = true;
            }
            else
            {
                var selectedItem = inventory.Items[UIManager.Get.InventoryUI.Selected];
                UseItem(selectedItem);
                Destroy(selectedItem.gameObject);

                UIManager.Get.InventoryUI.Hide();
                inventoryIsOpen = false;
                usingItem = false;
            }
        }
    }
    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.performed && inventoryIsOpen)
        {
            var selectedItem = inventory.Items[UIManager.Get.InventoryUI.Selected];
            if (droppingItem)
            {
                inventory.DropItem(selectedItem);
                selectedItem.transform.position = transform.position;
                GameManager.Get.AddItem(selectedItem);
                selectedItem.gameObject.SetActive(true);
            }
            else if (usingItem)
            {
                UseItem(selectedItem);
                Destroy(selectedItem.gameObject);
            }

            UIManager.Get.InventoryUI.Hide();
            inventoryIsOpen = false;
            droppingItem = false;
            usingItem = false;
        }
    }
    public void OnExit(InputAction.CallbackContext context)
    {
        
    }

    private void Move()
    {
        Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
        Vector2 roundedDirection = new Vector2(Mathf.Round(direction.x), Mathf.Round(direction.y));
        Action.MoveOrHit(GetComponent<Actor>(), roundedDirection);
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
    }
    private void UseItem(Consumable item)
    {
        // Implement item usage based on the item type
        switch (item.Type)
        {
            case Consumable.ItemType.HealthPotion:
                Debug.Log("Used Health Potion");
                break;
            case Consumable.ItemType.Fireball:
                Debug.Log("Used Fireball");
                break;
            case Consumable.ItemType.ScrollOfConfusion:
                Debug.Log("Used Scroll of Confusion");
                break;
        }
    }
}
