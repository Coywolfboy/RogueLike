using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public List<Actor> Enemies { get; private set; } = new List<Actor>();
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
    public void AddEnemy(Actor enemy)
    {
        Enemies.Add(enemy);
    }
    public static GameManager Get { get => instance; }

    public Actor GetActorAtLocation(Vector3 location)
    {
        return null;
    }
    public void CreateActor(string actorType, Vector2 position)
    {
        GameObject actorPrefab = Resources.Load<GameObject>(actorType);
        if (actorPrefab != null)
        {
            Instantiate(actorPrefab, position, Quaternion.identity);
        }
    }
}
