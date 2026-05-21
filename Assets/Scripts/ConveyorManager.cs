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
    ResourceSpawnerManager resourceSpawnerManager;
    
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
        resourceSpawnerManager = FindAnyObjectByType<ResourceSpawnerManager>();
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

    // Deletes the given gridId conveyor
    public void DeleteConveyor(Vector2 gridId)
    {
        List<Vector2>[] status;
        nodeManager.CheckConnections(gridId, out status);
        Vector2 next = status[1][0];
        if (!nodeManager.CheckEmpty(next))
        {
            SetOwner(gridId);
        }
        nodeManager.ResetNode(gridId);
    }
    // builds a conveyor with a new line
    public void NewConveyor(Vector2 gridId)
    {
        List<Vector2>[] status;
        bool[] playstates = new bool[2];

        
        // checks if the node is empty or not
        if (!(playstates[0] == false && playstates[1] == false)) return;

        //gets information about the connections
        playstates = nodeManager.CheckStatus(gridId);
        nodeManager.CheckConnections(gridId, out status);

        //establises which operation to do
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
    
    // makes a new line called when a new conveyor without owner is built
    void NewLine(Vector2 gridId)
    {
        conveyors.Add(gridId, new List<Vector2>());
        conveyors[gridId].Add(gridId);
        GameObject temp = Instantiate(converyorBlock, new Vector3(gridId.x + 0.5f - nodeLimitsData.width / 4, gridId.y + 0.5f - nodeLimitsData.height / 4, -1), Quaternion.Euler(0, 0, spawnDirection), transform);
        Vector2 outgoingDirection = OutGoingDirection(spawnDirection);
        nodeManager.UpdateNode(gridId, outgoingDirection, gridId, temp);
    }
    
    // builds a new conveyor on an existing line
    void NewConveyor(Vector2 gridId, Vector2 owner)
    {
        conveyors[owner].Add(gridId);
        GameObject temp = Instantiate(converyorBlock, new Vector3(gridId.x + 0.5f - nodeLimitsData.width / 4, gridId.y + 0.5f - nodeLimitsData.height / 4, -1), Quaternion.Euler(0, 0, spawnDirection), transform);
        Vector2 outgoingDirection = OutGoingDirection(spawnDirection);
        nodeManager.UpdateNode(gridId, outgoingDirection, owner, temp);
    }
    
    // pass a gridID which on the conveyor, all conveyor after passed gridId have their owner set as the next gridId
    void SetOwner(Vector2 gridId)
    {
        Vector2 owner = nodeManager.CheckOwner(gridId);
        var conveyor = conveyors[owner];
        int count = conveyor.Count();
        int i = 0;
        for (; i < count; i++)
        {
            if (conveyor[i] == gridId)
            {
                break;
            }
        }
        i++;
        int id = i;
        var newOwner = conveyor[i];
        conveyors.Add(newOwner, new List<Vector2>());
        Debug.Log(newOwner);
        for (; i < count; i++)
        {
            nodeManager.UpdateOwner(conveyor[i], newOwner);
            conveyors[newOwner].Add(conveyor[i]);
            //conveyor.Remove(conveyor[id]);
        }
        
        for (int j = id; j < i; i--)
        {
            conveyor.Remove(conveyor[j]);
        }
        Debug.Log(i);
    }
    
    // converts Vector2 to quaternion
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
    
    // sets direction using user input
    public void SetOutGoingDirection(Vector2 dir)
    {
        if (dir.x == 0)
        {
            if (dir.y == 1)
            {
                spawnDirection = 90;
            }
            else if (dir.y == -1)
            {
                spawnDirection = 270;
            }
        }
        else if (dir.y == 0)
        {
            if (dir.x == 1)
            {
                spawnDirection = 0;
            }
            else if (dir.x == -1)
            {
                spawnDirection = 180;
            }
        }
    }
    
    // Prints details of given conveyor in the console
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
    
    // Calls To Refresh Moving Obejcts
    IEnumerator MoveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            MoveObjects();
        }
    }

    // Checks which objects to Move and adds them to a list also applies to spwners
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
                        if (!nodeManager.CheckObject(connections[1][0], out other))
                        {
                            Vector2 dir = connections[1][0] - gridId;
                            movingObjects.Add(new MovingObjects(dir, currentObject));
                            nodeManager.UpdateNodePostion(gridId, dir);

                        }
                    }
                }
            }
        }
        resourceSpawnerManager.NewResource();
    }
}
