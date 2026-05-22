using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using System;

public class ConveyorManager : MonoBehaviour
{
    [SerializeField] GameObject converyorBlock;
    [SerializeField] NodeLimitsData nodeLimitsData;
    [SerializeField] int spawnDirection;
    NodeManager nodeManager;
    
    float elapsed = 0f;
    Dictionary<Vector2, List<Vector2>> conveyors = new Dictionary<Vector2, List<Vector2>>();

    class MovingObjects
    {
        public Vector3 dir;
        public GameObject thing;

        public MovingObjects(Vector2 dir2, GameObject thing)
        {
            dir = new Vector3(dir2.x, dir2.y, -2);
            this.thing = thing;
        }
    }

    List<MovingObjects> movingObjects = new List<MovingObjects>();

    void Start()
    {
        nodeManager = FindAnyObjectByType<NodeManager>();
        StartCoroutine(MoveCoroutine());
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed <= 1)
        {
            foreach(var block in movingObjects)
            {
                block.thing.transform.Translate(block.dir * Time.deltaTime);
            }
        }
        else
        {
            foreach(var block in movingObjects)
            {
                block.thing.transform.position = new Vector3((float)Math.Floor(block.thing.transform.position.x) + 0.5f, (float)Math.Floor(block.thing.transform.position.y) + 0.5f, -1);
            }
            elapsed = 0;
            movingObjects.Clear();
        }
    }

    public void NewConveyor(Vector2 gridId)
    {
        List<Vector2>[] status;
        nodeManager.CheckConnections(gridId, out status);
        bool incoming = true;
        bool outgoing = true;
        if (!status[0].Any())
        {
            incoming = false;
        }
        if (!status[1].Any())
        {
            outgoing = false;
        }

        if  (!incoming && !outgoing)
        {
            NewLine(gridId);
        }
        else if(incoming && !outgoing)
        {
            Vector2 owner = nodeManager.CheckOwner(gridId);
            NewConveyor(gridId, owner);
        }
        
    }

    void NewLine(Vector2 gridId)
    {
        Vector2 outgoingDirection = OutGoingDirection(spawnDirection);
        if (!nodeManager.UpdateNode(gridId, outgoingDirection, gridId))
        {
            Debug.LogWarning($"Cannot place conveyor at {gridId}: next node is outside the grid.");
            return;
        }

        conveyors.Add(gridId, new List<Vector2>());
        conveyors[gridId].Add(gridId);
        Instantiate(converyorBlock, new Vector3(gridId.x + 0.5f - nodeLimitsData.width / 4, gridId.y + 0.5f - nodeLimitsData.height / 4, -1), Quaternion.Euler(0, 0, spawnDirection), transform);
    }

    void NewConveyor(Vector2 gridId, Vector2 owner)
    {
        Vector2 outgoingDirection = OutGoingDirection(spawnDirection);
        if (!nodeManager.UpdateNode(gridId, outgoingDirection, owner))
        {
            Debug.LogWarning($"Cannot extend conveyor at {gridId}: next node is outside the grid.");
            return;
        }

        conveyors[owner].Add(gridId);
        Instantiate(converyorBlock, new Vector3(gridId.x + 0.5f - nodeLimitsData.width / 4, gridId.y + 0.5f - nodeLimitsData.height / 4, -1), Quaternion.Euler(0, 0, spawnDirection), transform);
    }

    Vector2 OutGoingDirection(int dir)
    {
        switch(dir)
        {
            case 0:
                return new Vector2(1,0);
            case 90: 
                return new Vector2(0,1);
            case 180:
                return new Vector2(-1,0);
            case 270:
                return new Vector2(0,-1);
            default:
                Debug.Log("invalid direction return up");
                return new Vector2(0,1);
        }
    }

    public void Details(Vector2 gridId)
    {
        List<Vector2>[] status;
        nodeManager.CheckConnections(gridId, out status);
        Debug.Log("gridId: " + gridId);
        Debug.Log("incoming: ");
        for (int i = 0; i < status[0].Count; i++)
        {
            Debug.Log(status[0][i]);
        }
        Debug.Log("outgoing: ");
        for (int i = 0; i < status[1].Count; i++)
        {
            Debug.Log(status[1][i]);
        }
        Debug.Log("owner");
        Debug.Log(nodeManager.CheckOwner(gridId));
        Debug.Log("Object");
        GameObject thing;
        if (nodeManager.CheckObject(gridId, out thing))
        {
            Debug.Log(thing.name);
        }
        else
        {
            Debug.Log("null");
        }

        Debug.Log("            ");
    }

    IEnumerator MoveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            MoveObjects();
        }
    }

    void MoveObjects()
    {
        foreach(var conveyor in conveyors)
        {
            //Debug.Log(conveyor.Value.Count);
            //foreach(var gridId in conveyor.Value)
            for (int i = conveyor.Value.Count - 1; i > -1; i--)
            {
                Vector2 gridId = conveyor.Value[i];
                // Debug.Log(gridId);
                GameObject currentObject;
                if (nodeManager.CheckObject(gridId, out currentObject))
                {
                    //Debug.Log(currentObject);
                    List<Vector2>[] connections;
                    nodeManager.CheckConnections(gridId, out connections);
                    if (connections[1].Count > 0)
                    {
                        //Debug.Log(connections[1][0]);
                        GameObject other;
                        if (nodeManager.CheckObject(connections[1][0], out other) == false)
                        {
                            Debug.Log("test");
                            Vector2 dir = connections[1][0] - gridId;
                            Debug.Log(dir);
                            
                            movingObjects.Add(new MovingObjects(dir, currentObject));
                            nodeManager.UpdateNodePostion(gridId, dir);

                        }
                    }
                }
            }
        }
    }
}
