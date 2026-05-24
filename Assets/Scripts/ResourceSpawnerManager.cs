using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using Mono.Cecil;
using Unity.VisualScripting;

public class ResourceSpawnerManager : MonoBehaviour
{
    [SerializeField] GameObject spawner;
    [SerializeField] NodeLimitsData nodeLimitsData;
    [SerializeField] int ticksBetweenSpawns;
    int ticks;
    List<Spanwer> spanwers = new List<Spanwer>();
    List<MovingObject> movingObjects = new List<MovingObject>();
    NodeManager nodeManager;
    float elapsed = 0;

    int spawnDirection = 0;

    class MovingObject
    {
        public GameObject thing;
        public Vector2 dir;
        public bool toDestroy;
        public MovingObject(Vector2 dir2, GameObject thing, bool toDestroy)
        {
            dir = new Vector3(dir2.x, dir2.y, -2);
            this.thing = thing;
            
            this.toDestroy = toDestroy;
        }
    }

    class Spanwer
    {
        public Vector2 outGoingDirection;
        public Vector2 nodeId = new Vector2(-1, -1);
        public GameObject resource;
        public GameObject currentResource;
        public GameObject self;

        public Spanwer(Vector2 nodeId, Vector2 outGoingDirection, GameObject resource, GameObject self)
        {
            this.outGoingDirection = outGoingDirection;
            this.nodeId = nodeId;
            this.resource = resource;
            this.self = self;
        }
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed <= 1)
        {
            foreach (var movingObject in movingObjects)
            {
                if (movingObject.thing == null)
                {
                    continue;
                }

                movingObject.thing.transform.Translate(new Vector3(movingObject.dir.x, movingObject.dir.y, 0)* Time.deltaTime);
            }
        }
        else
        {
            foreach(var block in movingObjects)
            {
                if (block.thing == null)
                {
                    continue;
                }

                if (block.toDestroy)
                {
                    Destroy(block.thing);
                }
                else
                {
                    block.thing.transform.position = new Vector3((float)Math.Floor(block.thing.transform.position.x) + 0.5f, (float)Math.Floor(block.thing.transform.position.y) + 0.5f, -1.5f);
                }
            }
            elapsed = 0;
            movingObjects.Clear();
        }
    }


    void Start()
    {
        ticks = ticksBetweenSpawns - 1;
        nodeManager = FindAnyObjectByType<NodeManager>();
    }

    public void NewResource()
    {
        if (nodeManager == null || nodeLimitsData == null)
        {
            return;
        }

        if (!(ticks == ticksBetweenSpawns))
        {
            ticks++;
            return;
        }

        ticks = 0;
        
        foreach (var spawner in spanwers)
        {

            GameObject item;
            if (!nodeManager.CheckObject(spawner.nodeId, out item))
            {
                GameObject newItem = Instantiate(spawner.resource, new Vector3(spawner.nodeId.x + 0.5f - nodeLimitsData.width / 4, spawner.nodeId.y + 0.5f - nodeLimitsData.height / 4, -2f), Quaternion.identity, transform);
                spawner.currentResource = newItem;
                nodeManager.Additem(spawner.nodeId, newItem);                
            }
            else if(!nodeManager.CheckObject(spawner.outGoingDirection, out item))
            {
                nodeManager.CheckObject(spawner.nodeId, out spawner.currentResource);
                Vector2 dir = spawner.outGoingDirection - spawner.nodeId;
                if (nodeManager.CheckEmpty(spawner.outGoingDirection))
                {
                    if (spawner.currentResource != null)
                    {
                        movingObjects.Add(new MovingObject(dir, spawner.currentResource, true));
                    }
                    nodeManager.Removeitem(spawner.nodeId);
                }
                else
                {
                    if (spawner.currentResource != null && nodeManager.UpdateNodePostion(spawner.nodeId, dir))
                    {
                        movingObjects.Add(new MovingObject(dir, spawner.currentResource, false));
                    }
                }
            }
        }
    }

    public void NewSpawner(Vector2 gridId, GameObject resource)
    {
        if (nodeManager == null || spawner == null || nodeLimitsData == null)
        {
            return;
        }

        Vector2 outgoingDirection = OutGoingDirection(spawnDirection);

        if (!nodeManager.CheckEmpty(gridId) || !nodeManager.IsWithinBounds(gridId + outgoingDirection))
        {
            return;
        }

        GameObject self = Instantiate(spawner, new Vector3(gridId.x + 0.5f - nodeLimitsData.width / 4, gridId.y + 0.5f - nodeLimitsData.height / 4, -1.5f), Quaternion.Euler(0, 0, spawnDirection), transform);
        if (!nodeManager.UpdateNode(gridId, outgoingDirection, gridId, self))
        {
            Destroy(self);
            return;
        }

        spanwers.Add(new Spanwer(gridId, gridId + outgoingDirection, resource, self));
    }

    public void SpawnDirection(Vector2 dir)
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
    
    
}
