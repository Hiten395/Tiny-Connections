using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawnerManager : MonoBehaviour
{
    [SerializeField] GameObject spawner;
    class Spanwer
    {
        public Vector2 outgoingDirection;
        public List<Vector2> occupyingNodes = new List<Vector2>();
    }

    void Start()
    {
        List<Spanwer> spanwers = new List<Spanwer>();
    }

    public void NewSpawner(Vector2 gridId)
    {
        
    }
}
