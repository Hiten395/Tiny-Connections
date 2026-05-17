using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorManager : MonoBehaviour
{
    NodeManager nodeManager;
    class Conveyor
    {
        public List<Vector2> coveyorLine = new List<Vector2>();

        public Conveyor(Vector2 start)
        {
            coveyorLine.Add(start);
        }
    }

    void Start()
    {
        nodeManager = FindAnyObjectByType<NodeManager>();
    }

    public void NewConveyor(Vector2 gridId)
    {
        List<Vector2>[] status;
        nodeManager.CheckConnections(gridId, out status);
        bool incoming = true;
        bool outgoing = true;
        if (!status[0].Any())
        {
            Debug.Log("0 incoming");
            incoming = false;
        }
        if (!status[1].Any())
        {
            Debug.Log("0 outgoing");
            outgoing = false;
        }

        if  (!incoming && !outgoing)
        {
            
        }
        
    }
}
