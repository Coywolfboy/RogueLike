using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor), typeof(AStar))]
public class Enemy : MonoBehaviour
{
    public Actor Target { get; set; }
    public bool IsFighting { get; private set; } = false;
    private AStar algorithm;

    void Start()
    {
        algorithm = GetComponent<AStar>();
        GameManager.Get.AddEnemy(GetComponent<Actor>());
    }

    void Update()
    {
        RunAI();
    }

    public void MoveAlongPath(Vector3Int targetPosition)
    {
        Vector3Int gridPosition = MapManager.Get.FloorMap.WorldToCell(transform.position);
        Vector2 direction = algorithm.Compute((Vector2Int)gridPosition, (Vector2Int)targetPosition);
        Action.Move(GetComponent<Actor>(), direction);
    }

    public void RunAI()
    {
        if (Target == null)
        {
            Target = GameManager.Get.Player;
        }

        Vector3Int gridPosition = MapManager.Get.FloorMap.WorldToCell(Target.transform.position);

        if (IsFighting || GetComponent<Actor>().FieldOfView.Contains(gridPosition))
        {
            IsFighting = true;

            float distance = Vector3.Distance(transform.position, Target.transform.position);
            if (distance < 1.5f)
            {
                Action.Hit(GetComponent<Actor>(), Target);
            }
            else
            {
                MoveAlongPath(gridPosition);
            }
        }
    }
}
